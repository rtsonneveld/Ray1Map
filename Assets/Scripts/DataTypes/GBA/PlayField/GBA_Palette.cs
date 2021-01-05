﻿using System;
using System.Collections.Generic;

namespace R1Engine {
    /// <summary>
    /// Palette block for GBA
    /// </summary>
    public class GBA_Palette : GBA_BaseBlock {
        public uint Length { get; set; }
        public BaseColor[] Palette { get; set; }

        public ushort Milan_Ushort_00 { get; set; }
        public ushort Milan_Ushort_02 { get; set; }
        public Dictionary<int, RGBA5551Color[]> Milan_Palettes { get; set; }

        public override void SerializeBlock(SerializerObject s) 
        {
            if (s.GameSettings.GBA_IsMilan)
            {
                // Copied from function at 0x080087e4 in The Mummy (US)

                // Serialize header
                s.DoAt(ShanghaiOffsetTable.GetPointer(0), () =>
                {
                    Milan_Ushort_00 = s.Serialize<ushort>(Milan_Ushort_00, name: nameof(Milan_Ushort_00));
                    Milan_Ushort_02 = s.Serialize<ushort>(Milan_Ushort_02, name: nameof(Milan_Ushort_02));
                });

                // Go to palette data
                s.Goto(ShanghaiOffsetTable.GetPointer(1));

                if (Milan_Palettes == null)
                {
                    Milan_Palettes = new Dictionary<int, RGBA5551Color[]>();

                    if (Milan_Ushort_00 == 0)
                    {
                        var palIndex = 0;

                        do
                        {
                            if (BitHelpers.ExtractBits(Milan_Ushort_02, 1, palIndex) == 1)
                            {
                                var pal = s.SerializeObjectArray<RGBA5551Color>(default, 0x10, name: $"Palette[{palIndex}]");
                                Milan_Palettes.Add(palIndex, pal);
                            }
                            palIndex += 1;
                        } while (palIndex < 0x10);
                    }
                    else
                    {
                        var palIndex = Milan_Ushort_02 >> 4;
                        var index = Milan_Ushort_00 >> 4;
                        while (0 < index)
                        {
                            var pal = s.SerializeObjectArray<RGBA5551Color>(default, 0x10, name: $"Palette[{palIndex}]");
                            Milan_Palettes.Add(palIndex, pal);

                            palIndex += 1;
                            index -= 1;
                        }
                    }

                    // Create a palette
                    if (Palette == null)
                        Palette = new RGBA5551Color[16 * 16];

                    // Fill the palette
                    foreach (var pal in Milan_Palettes)
                        for (int i = 0; i < 0x10; i++)
                            Palette[pal.Key * 0x10 + i] = pal.Value[i];
                }
                else
                {
                    throw new NotImplementedException("Writing is not supported");
                }

            }
            else
            {
                if (s.GameSettings.EngineVersion <= EngineVersion.GBA_R3_MadTrax)
                    s.Goto(ShanghaiOffsetTable.GetPointer(0));

                Length = s.Serialize<uint>(Length, name: nameof(Length));

                if (s.GameSettings.EngineVersion <= EngineVersion.GBA_R3_MadTrax)
                    s.Goto(ShanghaiOffsetTable.GetPointer(1));

                if (s.GameSettings.EngineVersion == EngineVersion.GBA_SplinterCell_NGage)
                {
                    Palette = s.SerializeObjectArray<BGRA4441Color>((BGRA4441Color[])Palette, Length, name: nameof(Palette));
                }
                else
                {
                    Palette = s.SerializeObjectArray<RGBA5551Color>((RGBA5551Color[])Palette, Length, name: nameof(Palette));
                }
            }
        }

        public override long GetShanghaiOffsetTableLength => 2;
    }
}