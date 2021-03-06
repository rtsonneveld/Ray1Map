﻿using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman 1 (PC)
    /// </summary>
    public class R1_PC_Manager : R1_PCBaseManager
    {
        #region Values and paths

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(WorldHelpers.GetR1Worlds().Where(x => Directory.Exists(settings.GameDirectory + GetWorldFolderPath(x))).Select(w => new GameInfo_World((int)w, Directory.EnumerateFiles(settings.GameDirectory + GetWorldFolderPath(w), $"RAY??.LEV", SearchOption.TopDirectoryOnly)
            .Select(FileSystem.GetFileNameWithoutExtensions)
            .Select(x => Int32.Parse(x.Substring(3)))
            .ToArray())).Append(new GameInfo_World(7, new []
        {
            0
        })).ToArray());

        /// <summary>
        /// Gets the folder path for the specified world
        /// </summary>
        /// <param name="world">The world</param>
        /// <returns>The world folder path</returns>
        public string GetWorldFolderPath(R1_World world) => GetDataPath() + GetWorldName(world) + "/";

        /// <summary>
        /// Gets the file path for the big ray file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The big ray file path</returns>
        public override string GetBigRayFilePath(GameSettings settings) => GetDataPath() + $"BRAY.DAT";

        /// <summary>
        /// Gets the file path for the vignette file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The vignette file path</returns>
        public override string GetVignetteFilePath(GameSettings settings) => $"VIGNET.DAT";

        /// <summary>
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public override string GetLevelFilePath(GameSettings settings) => GetWorldFolderPath(settings.R1_World) + $"RAY{settings.Level}.LEV";

        /// <summary>
        /// Gets the file path for the specified world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public override string GetWorldFilePath(GameSettings settings) => GetDataPath() + $"RAY{settings.World}.WLD";

        public virtual string GetLanguageFilePath() => "RAY.LNG";
        public virtual string GetSoundVignetteFilePath() => "SNDVIG.DAT";

        /// <summary>
        /// Gets the archive files which can be extracted
        /// </summary>
        public override Archive[] GetArchiveFiles(GameSettings settings) => new Archive[]
        {
            //GetLanguageFilePath(), // TODO: Serialize as an archive
            new Archive(GetVignetteFilePath(settings)),
            new Archive(GetSoundManifestFilePath()),
            new Archive(GetSoundFilePath()),
            new Archive(GetSoundVignetteFilePath())
        };

        /// <summary>
        /// Gets additional sound archives
        /// </summary>
        /// <param name="settings">The game settings</param>
        public override AdditionalSoundArchive[] GetAdditionalSoundArchives(GameSettings settings) => new AdditionalSoundArchive[]
        {
            new AdditionalSoundArchive("VIG", GetSoundVignetteFilePath(), 16),
        };

        /// <summary>
        /// Gets the available game actions
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The game actions</returns>
        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return base.GetGameActions(settings).Concat(new GameAction[]
            {
                new GameAction("Decrypt Save Files", false, false, (input, output) => DecryptSaveFiles(settings)),
                new GameAction("Read Save Files", false, false, (input, output) => ReadSaveFiles(settings)),
                new GameAction("Export Vignette (brute force)", false, true, (input, output) => ExtractEncryptedPCX(settings.GameDirectory + GetVignetteFilePath(settings), output)),
            }).ToArray();
        }

        #endregion

        #region Manager Methods

        public void DecryptSaveFiles(GameSettings settings)
        {
            using (var context = new Context(settings))
            {
                foreach (var save in Directory.GetFiles(settings.GameDirectory, "*.sav", SearchOption.TopDirectoryOnly).Select(Path.GetFileName))
                {
                    LinearSerializedFile f = new LinearSerializedFile(context)
                    {
                        filePath = save
                    };
                    context.AddFile(f);
                    SerializerObject s = context.Deserializer;
                    byte[] saveData = null;
                    s.DoAt(f.StartPointer, () => {
                        s.DoEncoded(new PC_R1_SaveEncoder(), () => {
                            saveData = s.SerializeArray<byte>(saveData, s.CurrentLength, name: "SaveData");
                            Util.ByteArrayToFile(context.BasePath + save + ".dec", saveData);
                        });
                    });
                    /*LinearSerializedFile f2 = new LinearSerializedFile(context) {
                        filePath = save + ".recompressed"
                    };
                    context.AddFile(f2);
                    s = context.Serializer;
                    s.DoAt(f2.StartPointer, () => {
                        s.DoEncoded(new R1PCSaveEncoder(), () => {
                            saveData = s.SerializeArray<byte>(saveData, saveData.Length, name: "SaveData");
                        });
                    });*/
                }
            }
        }

        public void ReadSaveFiles(GameSettings settings)
        {
            using (var context = new Context(settings))
            {
                foreach (var save in Directory.GetFiles(settings.GameDirectory, "*.sav", SearchOption.TopDirectoryOnly).Select(Path.GetFileName))
                {
                    LinearSerializedFile f = new LinearSerializedFile(context)
                    {
                        filePath = save
                    };
                    context.AddFile(f);
                    SerializerObject s = context.Deserializer;
                    s.DoAt(f.StartPointer, () => {
                        s.DoEncoded(new PC_R1_SaveEncoder(), () => s.SerializeObject<R1_PC_SaveFile>(default, name: "SaveFile"));
                    });
                }
            }
        }

        public override string[] GetDESNameTable(Context context) => LevelEditorData.NameTable_R1PCDES[context.Settings.R1_World == R1_World.Menu ? 0 : context.Settings.World - 1];
        public override string[] GetETANameTable(Context context) => LevelEditorData.NameTable_R1PCETA[context.Settings.R1_World == R1_World.Menu ? 0 : context.Settings.World - 1];

        public override byte[] GetTypeZDCBytes => R1_PC_ZDCTables.R1PC_Type_ZDC;
        public override byte[] GetZDCTableBytes => R1_PC_ZDCTables.R1PC_ZDCTable;
        public override byte[] GetEventFlagsBytes => R1_PC_EventFlagTables.R1PC_Flags;
        public override R1_WorldMapInfo[] GetWorldMapInfos(Context context) => context.Deserializer.SerializeFromBytes<ObjectArray<R1_WorldMapInfo>>(R1_PC_WorldInfoTables.R1PC_WorldInfo, "WorldInfo", x => x.Length = 24, name: "WorldInfos").Value;

        public override UniTask<Texture2D> LoadBackgroundVignetteAsync(Context context, R1_PC_WorldFile world, R1_PC_LevFile level, bool parallax)
        {
            // Return null if the parallax bg is the same as the normal one
            if (parallax && level.ParallaxBackgroundIndex == level.BackgroundIndex)
                return UniTask.FromResult<Texture2D>(null);

            var tex = LoadArchiveFile<PCX>(context, GetVignetteFilePath(context.Settings),
                world.Plan0NumPcx[parallax ? level.ParallaxBackgroundIndex : level.BackgroundIndex])?.ToTexture(true);

            return UniTask.FromResult(tex);
        }

        protected override async UniTask<IReadOnlyDictionary<string, string[]>> LoadLocalizationAsync(Context context)
        {
            var lngPath = GetLanguageFilePath();

            await FileSystem.PrepareFile(context.BasePath + lngPath);

            // Read the language file
            var lng = FileFactory.ReadText<R1_PC_LNGFile>(lngPath, context);

            var loc = new Dictionary<string, string[]>();

            // Set localization if available
            if (lng.Strings.Length > 0)
                loc.Add("English", lng.Strings[0]);
            if (lng.Strings.Length > 1)
                loc.Add("French", lng.Strings[1]);
            if (lng.Strings.Length > 2)
                loc.Add("German", lng.Strings[2]);
            if (lng.Strings.Length > 3)
                loc.Add("Japanese", lng.Strings[3]);
            if (lng.Strings.Length > 4)
                loc.Add("Chinese", lng.Strings[4]);

            return loc;
        }

        public override UniTask<PCX> GetWorldMapVigAsync(Context context)
        {
            int index;

            switch (context.Settings.GameModeSelection)
            {
                case GameModeSelection.RaymanPC_1_00:
                    index = 47;
                    break;

                case GameModeSelection.RaymanPC_1_10:
                case GameModeSelection.RaymanPC_1_12:
                case GameModeSelection.RaymanPC_Demo_2:
                case GameModeSelection.RaymanPC_1_20:
                case GameModeSelection.RaymanPC_1_21_JP:
                case GameModeSelection.RaymanClassicMobile:
                    index = 46;
                    break;

                case GameModeSelection.RaymanPC_1_21:
                    index = 48;
                    break;

                case GameModeSelection.RaymanPC_Demo_1:
                default:
                    throw new ArgumentOutOfRangeException(nameof(context.Settings.GameModeSelection), context.Settings.GameModeSelection, null);
            }

            return UniTask.FromResult(LoadArchiveFile<PCX>(context, GetVignetteFilePath(context.Settings), index));
        }

        #endregion
    }
}