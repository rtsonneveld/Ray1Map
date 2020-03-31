﻿using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 2 (PS1 - Demo)
    /// </summary>
    public class PS1_R2Demo_Manager : PS1_Manager
    {
        /// <summary>
        /// The width of the tile set in tiles
        /// </summary>
        public override int TileSetWidth => 16;

        /// <summary>
        /// The file info to use
        /// </summary>
        protected override Dictionary<string, PS1FileInfo> FileInfo => PS1FileInfo.fileInfoR2PS1;

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => EnumHelpers.GetValues<World>().Select(w => new KeyValuePair<World, int[]>(w, Enumerable.Range(1, w == World.Jungle ? 4 : 0).ToArray())).ToArray();

        /// <summary>
        /// Gets the name for the specified map
        /// </summary>
        /// <param name="map">The map</param>
        /// <returns>The map name</returns>
        public virtual string GetMapName(int map)
        {
            switch (map)
            {
                case 1:
                    return "PL1";

                case 2:
                    return "PL2";

                case 3:
                    return "FD1";

                case 4:
                    return "FD2";

                default:
                    throw new ArgumentOutOfRangeException(nameof(map));
            }
        }

        /// <summary>
        /// Gets the tile set to use
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The tile set to use</returns>
        public override IList<ARGBColor> GetTileSet(Context context) {
            var tileSetPath = $"JUNGLE/{GetMapName(context.Settings.Level)}.RAW";
            var palettePath = $"JUNGLE/{GetMapName(context.Settings.Level)}.PAL";
            var tileSet = FileFactory.Read<Array<byte>>(tileSetPath, context, (s, x) => x.Length = s.CurrentLength);
            var palette = FileFactory.Read<ObjectArray<ARGB1555Color>>(palettePath, context, (s, x) => x.Length = s.CurrentLength / 2);

            return tileSet.Value.Select(ind => palette.Value[ind]).ToArray();
        }

        public async Task<uint> LoadFile(Context context, string path, uint baseAddress) {
            await FileSystem.PrepareFile(context.BasePath + path);

            Dictionary<string, PS1FileInfo> fileInfo = FileInfo;
            if (baseAddress != 0) {
                PS1MemoryMappedFile file = new PS1MemoryMappedFile(context, baseAddress) {
                    filePath = path,
                    Length = fileInfo[path].Size
                };
                context.AddFile(file);

                return fileInfo[path].Size;
            } else {
                LinearSerializedFile file = new LinearSerializedFile(context) {
                    filePath = path,
                    length = fileInfo.ContainsKey(path) ? fileInfo[path].Size : 0
                };
                context.AddFile(file);
                return 0;
            }
        }

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <returns>The editor manager</returns>
        public override async Task<BaseEditorManager> LoadAsync(Context context)
        {
            uint baseAddress = 0x80018000;

            // TODO: Move these to methods to avoid hard-coding
            var fixDTAPath = $"RAY.DTA";
            var fixGRPPath = $"RAY.GRP";
            var sprPLSPath = $"SPR.PLS";
            var levelDTAPath = $"JUNGLE/{GetWorldName(context.Settings.World)}01.DTA";
            var levelSPRPath = $"JUNGLE/{GetWorldName(context.Settings.World)}01.SPR"; // SPRites?
            var levelGRPPath = $"JUNGLE/{GetWorldName(context.Settings.World)}01.GRP"; // GRaPhics/graphismes
            // TODO: Load submaps based on levelDTA file
            var tileSetPath = $"JUNGLE/{GetMapName(context.Settings.Level)}.RAW";
            var palettePath = $"JUNGLE/{GetMapName(context.Settings.Level)}.PAL";
            var mapPath = $"JUNGLE/{GetMapName(context.Settings.Level)}.MPU";


            baseAddress += await LoadFile(context, fixDTAPath, baseAddress);
            baseAddress -= 0x5E; // FIX.DTA header size
            Pointer fixDTAHeader = new Pointer(baseAddress, context.FilePointer(fixDTAPath).file);
            context.Deserializer.DoAt(fixDTAHeader, () => {
                // TODO: Read header here (0x5E bytes). Should be done now because these bytes will be overwritten

            });
            await LoadFile(context, fixGRPPath, 0);
            await LoadFile(context, sprPLSPath, 0);
            baseAddress += await LoadFile(context, levelSPRPath, baseAddress);
            baseAddress += await LoadFile(context, levelDTAPath, baseAddress);
            await LoadFile(context, levelGRPPath, 0);
            await LoadFile(context, tileSetPath, 0);
            await LoadFile(context, palettePath, 0);
            await LoadFile(context, mapPath, 0); // TODO: Load all maps for this level

            // Read the map block
            var map = FileFactory.Read<PS1_R1_MapBlock>(mapPath, context);

            // Load the level
            return await LoadAsync(context, null, null, map, null, null);
        }
    }
}