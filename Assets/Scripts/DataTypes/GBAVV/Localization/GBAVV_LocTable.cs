﻿namespace R1Engine
{
    public class GBAVV_LocTable : R1Serializable
    {
        public Pointer[] StringPointers { get; set; }
        public string[] Strings { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            StringPointers = s.SerializePointerArray(StringPointers, ((GBAVV_BaseManager)s.GameSettings.GetGameManager).LocTableCount, name: nameof(StringPointers));

            if (Strings == null)
                Strings = new string[StringPointers.Length];

            for (int i = 0; i < Strings.Length; i++)
                Strings[i] = s.DoAt(StringPointers[i], () => s.SerializeString(Strings[i], name: $"{nameof(Strings)}[{i}]"));
        }
    }
}