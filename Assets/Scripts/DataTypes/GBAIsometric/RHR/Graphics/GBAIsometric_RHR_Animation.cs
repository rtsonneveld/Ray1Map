﻿namespace R1Engine
{
    public class GBAIsometric_RHR_Animation : R1Serializable
    {
        public byte Speed { get; set; }
        public byte FrameCount { get; set; }
        public ushort StartFrameIndex { get; set; }
        public bool FlipX { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Speed = s.Serialize<byte>(Speed, name: nameof(Speed));
            FrameCount = s.Serialize<byte>(FrameCount, name: nameof(FrameCount));
            s.SerializeBitValues<ushort>(bitfunc => {
                StartFrameIndex = (ushort)bitfunc(StartFrameIndex, 15, name: nameof(StartFrameIndex));
                FlipX = bitfunc(FlipX ? 1 : 0, 1, name: nameof(FlipX)) == 1;
            });
        }
    }
}