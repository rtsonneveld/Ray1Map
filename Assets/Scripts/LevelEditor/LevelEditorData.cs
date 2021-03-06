﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    public static class LevelEditorData
    {
        public static GeneralEventInfoData[] EventInfoData { get; private set; } = new GeneralEventInfoData[0];
        public static string[][] NameTable_R1PCDES { get; private set; }
        public static string[][] NameTable_R1PCETA { get; private set; }
        public static string[][] NameTable_EDUDES { get; private set; }
        public static string[][] NameTable_EDUETA { get; private set; }
        public static Dictionary<string, Dictionary<string, R1_DESPointers>> NameTable_R1PS1DES { get; private set; }
        public static Dictionary<string, Dictionary<string, uint>> NameTable_R1PS1ETA { get; private set; }

        public static Context MainContext { get; set; }
        public static GameSettings CurrentSettings => MainContext?.Settings;

        public static Unity_Level Level { get; set; }
        public static Unity_ObjectManager ObjManager => Level.ObjManager;

        public static int MaxWidth => Level.MaxWidth;
        public static int MaxHeight => Level.MaxHeight;

        public static int CurrentMap { get; set; }
        public static int CurrentCollisionMap { get; set; }
        public static bool[] ShowEventsForMaps { get; set; }
        public static bool ShowOnlyActiveSector { get; set; }
        public static int ActiveSector { get; set; }

        public static async UniTask InitAsync(GameSettings settings, bool loadAll = false)
        {
            async UniTask loadFile(string filePath, Action<Stream> func)
            {
                await FileSystem.PrepareFile(filePath);

                if (FileSystem.FileExists(filePath))
                {
                    using (var stream = FileSystem.GetFileReadStream(filePath))
                        func(stream);
                }
                else
                {
                    Debug.Log($"{filePath} has not been loaded");
                }
            }

            // Only load event manifests for Rayman 1
            if (loadAll || (settings.MajorEngineVersion == MajorEngineVersion.Rayman1 && settings.EngineVersion != EngineVersion.R2_PS1))
            {
                const string dir = "EventManifests/";

                // Load global event manifest
                await loadFile(dir + "Events.csv", 
                    file => EventInfoData = GeneralEventInfoData.ReadCSV(file));

                // Load version specific mapping tables
                if (loadAll || settings.EngineVersion == EngineVersion.R1_PC || settings.EngineVersion == EngineVersion.R1_PocketPC)
                {
                    await loadFile(dir + "r1_des.json",
                        file => NameTable_R1PCDES = JsonHelpers.DeserializeFromFile<string[][]>(file));
                    await loadFile(dir + "r1_eta.json",
                        file => NameTable_R1PCETA = JsonHelpers.DeserializeFromFile<string[][]>(file));
                }

                if (loadAll || settings.EngineVersion == EngineVersion.R1_PC_Edu || settings.EngineVersion == EngineVersion.R1_PS1_Edu)
                {
                    await loadFile(dir + "edu_des.json",
                        file => NameTable_EDUDES = JsonHelpers.DeserializeFromFile<string[][]>(file));
                    await loadFile(dir + "edu_eta.json",
                        file => NameTable_EDUETA = JsonHelpers.DeserializeFromFile<string[][]>(file));
                }
                
                if (loadAll || settings.EngineVersion == EngineVersion.R1_PC_Kit)
                {
                    // Do nothing - Kit has its own mapping table
                }

                if (loadAll || settings.GameModeSelection == GameModeSelection.RaymanPS1US || settings.GameModeSelection == GameModeSelection.RaymanPS1USDemo)
                {
                    await loadFile(dir + "r1_ps1_des.json",
                        file => NameTable_R1PS1DES = JsonHelpers.DeserializeFromFile<Dictionary<string, Dictionary<string, R1_DESPointers>>>(file));
                    await loadFile(dir + "r1_ps1_eta.json",
                        file => NameTable_R1PS1ETA = JsonHelpers.DeserializeFromFile<Dictionary<string, Dictionary<string, uint>>>(file));
                }
            }
        }

        public class R1_DESPointers
        {
            public uint? ImageDescriptors { get; set; }
            public ushort ImageDescriptorsCount { get; set; }
            public uint? AnimationDescriptors { get; set; }
            public byte AnimationDescriptorsCount { get; set; }
            public uint? ImageBuffer { get; set; }
            public uint ImageBufferLength { get; set; }
        }
    }
}