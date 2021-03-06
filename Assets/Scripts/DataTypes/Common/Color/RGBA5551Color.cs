﻿namespace R1Engine
{
    /// <summary>
    /// A standard ARGB color wrapper with serializing support for the encoding BGR-555
    /// </summary>
    public class RGBA5551Color : BaseColor
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public RGBA5551Color() : base() { }
        public RGBA5551Color(float r, float g, float b, float a = 1f) : base(r, g, b, a: a) { }

        /// <summary>
        /// The color as a raw 16-bit value
        /// </summary>
        public ushort Color5551 { get; set; }

        #region RGBA Values
        protected override float _R {
            get => BitHelpers.ExtractBits(Color5551, 5, 0) / 31f;
            set => Color5551 = (ushort)BitHelpers.SetBits(Color5551, (int)(value * 31), 5, 0);
        }
        protected override float _G {
            get => BitHelpers.ExtractBits(Color5551, 5, 5) / 31f;
            set => Color5551 = (ushort)BitHelpers.SetBits(Color5551, (int)(value * 31), 5, 5);
        }
        protected override float _B {
            get => BitHelpers.ExtractBits(Color5551, 5, 10) / 31f;
            set => Color5551 = (ushort)BitHelpers.SetBits(Color5551, (int)(value * 31), 5, 10);
        }
        protected override float _A {
            get => BitHelpers.ExtractBits(Color5551, 1, 15);
            set => Color5551 = (ushort)BitHelpers.SetBits(Color5551, (int)value, 1, 15);
        }
        #endregion
        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            Color5551 = s.Serialize<ushort>(Color5551, name: nameof(Color5551));
        }

        public static RGBA5551Color From5551(ushort rgba5551) {
            RGBA5551Color col = new RGBA5551Color();
            col.Color5551 = rgba5551;
            return col;
        }
    }
}