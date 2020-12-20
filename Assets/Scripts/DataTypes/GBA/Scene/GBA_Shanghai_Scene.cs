﻿using System.Collections.Generic;

namespace R1Engine
{
    public class GBA_Shanghai_Scene : GBA_BaseBlock
    {
        public byte ActorModelsCount { get; set; }
        public byte GameObjectsCount { get; set; } // Doesn't seem to match - maybe always objects count?
        public byte Byte_02 { get; set; }

        public GBC_GameObject[] GameObjects { get; set; }

        // Parsed from offsets
        public GBA_Shanghai_ActorModel[] ActorModels { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            ActorModelsCount = s.Serialize<byte>(ActorModelsCount, name: nameof(ActorModelsCount));
            GameObjectsCount = s.Serialize<byte>(GameObjectsCount, name: nameof(GameObjectsCount));
            Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));

            if (GameObjects == null)
            {
                var obj = new List<GBC_GameObject>();

                var index = 0;
                while (s.CurrentPointer.AbsoluteOffset < (Offset + BlockSize).AbsoluteOffset)
                    obj.Add(s.SerializeObject<GBC_GameObject>(default, name: $"{nameof(GameObjects)}[{index++}]"));

                GameObjects = obj.ToArray();
            }
            else
            {
                GameObjects = s.SerializeObjectArray<GBC_GameObject>(GameObjects, GameObjectsCount, name: nameof(GameObjects));
            }
        }

        public override void SerializeOffsetData(SerializerObject s)
        {
            if (ActorModels == null)
                ActorModels = new GBA_Shanghai_ActorModel[ActorModelsCount];

            for (int i = 0; i < ActorModels.Length; i++)
                ActorModels[i] = s.DoAt(OffsetTable.GetPointer(i), () => s.SerializeObject<GBA_Shanghai_ActorModel>(ActorModels[i], name: $"{nameof(ActorModels)}[{i}]"));
        }
    }
}