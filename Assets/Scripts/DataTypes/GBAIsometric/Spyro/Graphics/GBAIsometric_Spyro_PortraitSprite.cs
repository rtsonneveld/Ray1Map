﻿using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBAIsometric_Spyro_PortraitSprite : R1Serializable
    {
        public uint ID { get; set; }
        public Pointer Pointer_04 { get; set; } // Uint array
        public GBAIsometric_Spyro_DataBlockIndex MapIndex { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex TileSetIndex { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex PaletteIndex { get; set; }

        // Parsed
        public GBAIsometric_Spyro_SpriteMap Map { get; set; }
        public byte[] TileSet { get; set; }
        public ARGB1555Color[] Palette { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ID = s.Serialize<uint>(ID, name: nameof(ID));
            Pointer_04 = s.SerializePointer(Pointer_04, name: nameof(Pointer_04));
            MapIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(MapIndex, name: nameof(MapIndex));
            TileSetIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(TileSetIndex, name: nameof(TileSetIndex));
            PaletteIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(PaletteIndex, name: nameof(PaletteIndex));
            s.Serialize<ushort>(default, name: "Padding");

            Map = MapIndex.DoAtBlock(size => s.SerializeObject<GBAIsometric_Spyro_SpriteMap>(Map, name: nameof(Map)));
            TileSet = TileSetIndex.DoAtBlock(size => s.SerializeArray<byte>(TileSet, size, name: nameof(TileSet)));
            Palette = PaletteIndex.DoAtBlock(size => s.SerializeObjectArray<ARGB1555Color>(Palette, 16, name: nameof(Palette)));
        }

        public Texture2D ToTexture2D()
        {
            const int cellSize = 8;
            const int tileSize = (cellSize * cellSize) / 2;

            var tex = TextureHelpers.CreateTexture2D(Map.Width * cellSize, Map.Height * cellSize);
            var pal = Palette.Select((x, i) =>
            {
                if (i != 0)
                    x.Alpha = 255;
                return x.GetColor();
            }).ToArray();

            for (int y = 0; y < Map.Height; y++)
            {
                for (int x = 0; x < Map.Width; x++)
                {
                    var tile = Map.MapData[y * Map.Width + x];
                    tex.FillInTile(TileSet, tile.TileMapY * tileSize, pal, false, cellSize, true, x * cellSize, y * cellSize);
                }
            }

            tex.Apply();

            return tex;
        }
    }
}