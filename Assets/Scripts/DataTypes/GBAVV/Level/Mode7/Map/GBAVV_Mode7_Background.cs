﻿namespace R1Engine
{
    public class GBAVV_Mode7_Background : R1Serializable
    {
        public RGBA5551Color[] Palette { get; set; }

        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public uint TileSetCount { get; set; }

        public MapTile[] TileMap { get; set; }
        public byte[] TileSet { get; set; }
        public byte[] PaletteIndices { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, 256, name: nameof(Palette));

            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            TileSetCount = s.Serialize<uint>(TileSetCount, name: nameof(TileSetCount));

            TileMap = s.SerializeObjectArray<MapTile>(TileMap, Width * Height, name: nameof(TileMap));
            TileSet = s.SerializeArray<byte>(TileSet, TileSetCount * 0x20, name: nameof(TileSet));
            PaletteIndices = s.SerializeArray<byte>(PaletteIndices, (Width * Height) / 2, name: nameof(PaletteIndices));
        }
    }
}