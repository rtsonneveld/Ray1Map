﻿using UnityEngine;

namespace R1Engine
{
    public class PC_LocFile : R1Serializable
    {
        // NombreLangues
        public byte NumberOfLanguages { get; set; }

        // LangueUtilisee
        public byte LanguageUtilized { get; set; }

        public KeyboardTypes KeyboardType { get; set; }

        public string[] LanguageNames { get; set; }

        public ushort Unk0 { get; set; }

        public uint TextDefineCount { get; set; }
        
        // Different for each language
        public ushort Unk1 { get; set; }

        public ushort Unk2 { get; set; }

        public PC_LocFileString[] TextDefine { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            NumberOfLanguages = s.Serialize<byte>(NumberOfLanguages, name: nameof(NumberOfLanguages));
            LanguageUtilized = s.Serialize<byte>(LanguageUtilized, name: nameof(LanguageUtilized));
            KeyboardType = s.Serialize<KeyboardTypes>(KeyboardType, name: nameof(KeyboardType));

            LanguageNames = s.SerializeStringArray(LanguageNames,
                // Most versions have 3 languages, but sometimes the NumberOfLanguages is set to 1 because only 1 is available. Other versions may have up to 5.
                Mathf.Clamp(NumberOfLanguages, 3, 5), 11, name: nameof(LanguageNames));

            // Hack since the Scandinavian release has an extra value here and it has 5 languages
            if (NumberOfLanguages == 5)
                Unk0 = s.Serialize<ushort>(Unk0, name: nameof(Unk0));

            TextDefineCount = s.Serialize<uint>(TextDefineCount, name: nameof(TextDefineCount));
            Unk1 = s.Serialize<ushort>(Unk1, name: nameof(Unk1));
            Unk2 = s.Serialize<ushort>(Unk2, name: nameof(Unk2));

            TextDefine = s.SerializeObjectArray<PC_LocFileString>(TextDefine, TextDefineCount, name: nameof(TextDefine));
        }

        public enum KeyboardTypes : byte
        {
            QWERTY,
            AZERTY
        }
    }
}