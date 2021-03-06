﻿namespace R1Engine
{
    public class GBAIsometric_Spyro_ByrdRescueInfo : R1Serializable
    {
        public ushort ID { get; set; }
        public GameMode Game { get; set; }
        public ushort LevelDataID { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex ObjectTableIndex { get; set; }

        // Parsed
        public GBAIsometric_Spyro_ObjectTable ObjectTable { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ID = s.Serialize<ushort>(ID, name: nameof(ID));
            Game = s.Serialize<GameMode>(Game, name: nameof(Game));
            LevelDataID = s.Serialize<ushort>(LevelDataID, name: nameof(LevelDataID));
            ObjectTableIndex = s.SerializeObject<GBAIsometric_Spyro_DataBlockIndex>(ObjectTableIndex, name: nameof(ObjectTableIndex));

            ObjectTable = ObjectTableIndex.DoAtBlock(size => s.SerializeObject<GBAIsometric_Spyro_ObjectTable>(ObjectTable, name: nameof(ObjectTable)));
        }

        public enum GameMode : ushort
        {
            HeadToHead,
            Cooperative,
            SinglePlayer
        }
    }
}