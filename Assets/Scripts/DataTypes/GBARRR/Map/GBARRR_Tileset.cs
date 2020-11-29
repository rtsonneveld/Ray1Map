﻿namespace R1Engine
{
    public class GBARRR_Tileset : R1Serializable
    {
        public uint BlockSize { get; set; }

        public RGBA5551Color[] Palette { get; set; }
        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, 0x100, name: nameof(Palette));
            Data = s.SerializeArray<byte>(Data, BlockSize - (0x100 * 2), name: nameof(Data)); // Always 0x40040 for normal tilemaps
        }
    }
}