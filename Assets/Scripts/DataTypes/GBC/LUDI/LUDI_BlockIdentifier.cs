﻿namespace R1Engine
{
    public class LUDI_BlockIdentifier : R1Serializable
    {
        public ushort BlockID { get; set; }
        public ushort BlockType { get; set; } // 0 = normal (OffsetTable), 1 = special (DataInfo)

        public override void SerializeImpl(SerializerObject s) {
            BlockID = s.Serialize<ushort>(BlockID, name: nameof(BlockID));
            BlockType = s.Serialize<ushort>(BlockType, name: nameof(BlockType));
        }
    }
}