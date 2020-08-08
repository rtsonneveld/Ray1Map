﻿using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace R1Engine
{
    public class SNES_Prototype_Manager : IGameManager
    {
        public KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => new KeyValuePair<World, int[]>[]
        {
            new KeyValuePair<World, int[]>(World.Jungle, new int[]
            {
                0
            }), 
        };

        public string[] GetEduVolumes(GameSettings settings) => new string[0];

        public virtual string GetROMFilePath => $"ROM.sfc";

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[0];

        public async Task<BaseEditorManager> LoadAsync(Context context, bool loadTextures)
        {
            Controller.status = $"Loading data";
            await Controller.WaitIfNecessary();

            // Read the rom
            var rom = FileFactory.Read<SNES_R1_ROM>(GetROMFilePath, context);

            // Get the map
            var map = rom.MapData;

            // Convert levelData to common level format
            Common_Lev commonLev = new Common_Lev
            {
                // Create the map
                Maps = new Common_LevelMap[]
                {
                    new Common_LevelMap()
                    {
                        // Set the dimensions
                        Width = map.Width,
                        Height = map.Height,

                        // Create the tile arrays
                        TileSet = new Common_Tileset[3],
                        MapTiles = map.Tiles.Select(x => new Editor_MapTile(x)).ToArray(),
                        TileSetWidth = 1
                    }
                },

                // Create the events list
                EventData = new List<Editor_EventData>(),
            };

            Controller.status = $"Loading tile set";
            await Controller.WaitIfNecessary();

            // Load tile set and treat black as transparent
            commonLev.Maps[0].TileSet[0] = GetTileSet(context, rom);

            return new Jaguar_EditorManager(commonLev, context, new Dictionary<string, Common_Design>(), new Dictionary<string, Common_EventState[][]>(), new Dictionary<string, string[][]>());
        }

        public virtual Common_Tileset GetTileSet(Context context, SNES_R1_ROM rom)
        {
            // Read the tiles
            const int block_size = 0x20;

            uint length = (uint)rom.TileDescriptors.Length * 8 * 8;

            // Get the tile-set texture
            var tex = new Texture2D(256, Mathf.CeilToInt(length / 256f / Settings.CellSize) * Settings.CellSize)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            for (int i = 0; i < rom.TileDescriptors.Length; i++)
            {
                ushort descriptor = rom.TileDescriptors[i];

                var tileIndex = BitHelpers.ExtractBits(descriptor, 10, 0);
                var pal = BitHelpers.ExtractBits(descriptor, 3, 10);
                
                // This is set to 1 for anything which should be drawn in front of Rayman, such as the pipe
                var isForeground = BitHelpers.ExtractBits(descriptor, 1, 13) == 1;
                
                var flipX = BitHelpers.ExtractBits(descriptor, 1, 14) == 1;
                var flipY = BitHelpers.ExtractBits(descriptor, 1, 15) == 1;

                var x = ((i / 4) * 2) % (256 / 8) + ((i % 2) == 0 ? 0 : 1);
                var y = (((i / 4) * 2) / (256 / 8)) * 2 + ((i % 4) < 2 ? 0 : 1);

                var curOff = block_size * tileIndex;

                FillTextureBlock(tex, 0, 0, x, y, rom.TileMap, curOff, rom.Palettes, pal, flipX, flipY);
            }

            tex.Apply();

            return new Common_Tileset(tex, Settings.CellSize);
        }

        public void FillTextureBlock(Texture2D tex, int blockX, int blockY, int relX, int relY, byte[] imageBuffer, int imageBufferOffset, IList<ARGB1555Color> pal, int paletteInd, bool flipX, bool flipY)
        {
            var offset1 = imageBufferOffset;
            var offset2 = imageBufferOffset + 16;

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    int actualX = blockX + relX * 8 + (flipX ? (8 - x - 1) : x);
                    int actualY = blockY + relY * 8 + (flipY ? (8 - y - 1) : y);

                    int off = y * 8 + (8 - x - 1);

                    var bit0 = BitHelpers.ExtractBits(imageBuffer[offset1 + ((off / 8) * 2)], 1, off % 8);
                    var bit1 = BitHelpers.ExtractBits(imageBuffer[offset1 + ((off / 8) * 2 + 1)], 1, off % 8);
                    var bit2 = BitHelpers.ExtractBits(imageBuffer[offset2 + ((off / 8) * 2)], 1, off % 8);
                    var bit3 = BitHelpers.ExtractBits(imageBuffer[offset2 + ((off / 8) * 2 + 1)], 1, off % 8);

                    int b = 0;

                    b = BitHelpers.SetBits(b, bit0, 1, 0);
                    b = BitHelpers.SetBits(b, bit1, 1, 1);
                    b = BitHelpers.SetBits(b, bit2, 1, 2);
                    b = BitHelpers.SetBits(b, bit3, 1, 3);
                    
                    Color c = pal[paletteInd * 0x10 + b].GetColor();

                    if (b != 0)
                        c = new Color(c.r, c.g, c.b, 1f);

                    tex.SetPixel(actualX, actualY, c);
                }
            }
        }

        public void SaveLevel(Context context, BaseEditorManager editorManager) => throw new NotImplementedException();

        public async Task LoadFilesAsync(Context context)
        {
            await FileSystem.PrepareFile(context.BasePath + GetROMFilePath);

            var file = new LinearSerializedFile(context)
            {
                filePath = GetROMFilePath,
            };
            context.AddFile(file);
        }
    }
}