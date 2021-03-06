﻿namespace R1Engine
{
    /// <summary>
    /// Palette Reference for Rayman 1 (DSi)
    /// </summary>
    public class R1_DSi_PaletteReference : R1Serializable
    {
        public Pointer PalettePointer;
        public uint UInt_04;
        public Pointer NamePointer;

        public RGBA5551Color[] Palette { get; set; }
        public string Name;

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            PalettePointer = s.SerializePointer(PalettePointer, name: nameof(PalettePointer));
            UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
            NamePointer = s.SerializePointer(NamePointer, name: nameof(NamePointer));

            s.DoAt(NamePointer, () => Name = s.SerializeString(Name, name: nameof(Name)));
            s.DoAt(PalettePointer, () => {
                Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, 256, name: nameof(Palette));
            });
        }
    }
}