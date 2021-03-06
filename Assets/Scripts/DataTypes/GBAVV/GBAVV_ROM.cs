﻿using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class GBAVV_ROM : GBA_ROMBase
    {
        public GBAVV_BaseManager.LevInfo CurrentLevInfo { get; set; } // Set before serializing

        // Helpers
        public GBAVV_MapInfo CurrentMapInfo
        {
            get
            {
                GBAVV_MapInfo map;

                if (CurrentLevInfo.LevelIndex == -1)
                    map = new GBAVV_MapInfo
                    {
                        MapType = CurrentLevInfo.SpecialMapType,
                        Index3D = CurrentLevInfo.Index3D
                    };
                else if (CurrentLevInfo.MapType == GBAVV_BaseManager.LevInfo.Type.Normal)
                    map = LevelInfos[CurrentLevInfo.LevelIndex].LevelData.Maps[CurrentLevInfo.MapIndex];
                else if (CurrentLevInfo.MapType == GBAVV_BaseManager.LevInfo.Type.Bonus)
                    map = LevelInfos[CurrentLevInfo.LevelIndex].LevelData.BonusMap;
                else
                    map = LevelInfos[CurrentLevInfo.LevelIndex].LevelData.ChallengeMap;

                return map;
            }
        }
        public GBAVV_Mode7_LevelInfo CurrentMode7LevelInfo => Mode7_LevelInfos[CurrentMapInfo.Index3D];
        public GBAVV_Isometric_MapData CurrentIsometricMapData => Isometric_MapDatas[CurrentIsometricIndex];
        public GBAVV_Isometric_ObjectData CurrentIsometricObjData => Isometric_ObjectDatas[CurrentIsometricIndex];
        public int CurrentIsometricIndex => CurrentMapInfo.Index3D + 4;

        // Common
        public Pointer[] LocTablePointers { get; set; }
        public GBAVV_LocTable[] LocTables { get; set; }
        public GBAVV_LevelInfo[] LevelInfos { get; set; }

        // 2D
        public GBAVV_Map2D_Graphics Map2D_Graphics { get; set; }

        // Mode7
        public GBAVV_Mode7_LevelInfo[] Mode7_LevelInfos { get; set; }
        public RGBA5551Color[] Mode7_TilePalette { get; set; }
        public RGBA5551Color[] Mode7_Crash1_Type0_TilePalette_0F { get; set; }
        public byte[] Mode7_Crash2_Type0_BG1 { get; set; }
        public Pointer[] Mode7_Crash2_Type1_FlamesTileMapsPointers { get; set; }
        public MapTile[][] Mode7_Crash2_Type1_FlamesTileMaps { get; set; }
        public uint[] Mode7_Crash2_Type1_FlamesTileSetLengths { get; set; }
        public Pointer[] Mode7_Crash2_Type1_FlamesTileSetsPointers { get; set; }
        public byte[][] Mode7_Crash2_Type1_FlamesTileSets { get; set; }
        public RGBA5551Color[] Mode7_GetTilePal(GBAVV_Mode7_LevelInfo levInfo)
        {
            var tilePal = Context.Settings.EngineVersion == EngineVersion.GBAVV_Crash1 ? levInfo.TileSetFrames.Palette : Mode7_TilePalette;

            if (Context.Settings.EngineVersion == EngineVersion.GBAVV_Crash1 && levInfo.LevelType == 0)
                tilePal = tilePal.Take(256 - 16).Concat(Mode7_Crash1_Type0_TilePalette_0F).ToArray(); // Over last palette

            return tilePal;
        }

        // Isometric
        public GBAVV_Isometric_MapData[] Isometric_MapDatas { get; set; }
        public GBAVV_Isometric_ObjectData[] Isometric_ObjectDatas { get; set; }
        public GBAVV_Isometric_CharacterInfo[] Isometric_CharacterInfos { get; set; }
        public GBAVV_Isometric_CharacterIcon[] Isometric_CharacterIcons { get; set; }
        public GBAVV_Isometric_Animation[] Isometric_ObjAnimations { get; set; }
        public RGBA5551Color[] Isometric_ObjPalette_0 { get; set; }
        public RGBA5551Color[] Isometric_ObjPalette_1 { get; set; }
        public RGBA5551Color[] Isometric_ObjPalette_2 { get; set; }
        public RGBA5551Color[] Isometric_ObjPalette_4 { get; set; }
        public RGBA5551Color[] Isometric_ObjPalette_11 { get; set; }
        public RGBA5551Color[] Isometric_ObjPalette_12 { get; set; }
        public RGBA5551Color[] Isometric_ObjPalette_13 { get; set; }
        public RGBA5551Color[] Isometric_GetObjPalette =>
            Isometric_ObjPalette_0.
                Concat(Isometric_ObjPalette_1).
                Concat(Isometric_ObjPalette_2).
                Concat(Enumerable.Repeat(new RGBA5551Color(), 16* 1)).
                Concat(Isometric_ObjPalette_4).
                Concat(Enumerable.Repeat(new RGBA5551Color(), 16 * 6)).
                Concat(Isometric_ObjPalette_11).
                Concat(Isometric_ObjPalette_12).
                Concat(Isometric_ObjPalette_13).
                Concat(Enumerable.Repeat(new RGBA5551Color(), 16 * 2)).
                ToArray();
        public GBAVV_Isometric_Animation[] Isometric_AdditionalAnimations { get; set; }
        public IEnumerable<GBAVV_Isometric_Animation> Isometric_GetAnimations => Isometric_ObjAnimations.Concat(Isometric_AdditionalAnimations);

        // WorldMap
        public GBAVV_WorldMap_Data WorldMap { get; set; }
        public GBAVV_WorldMap_Crash1_LevelIcon[] WorldMap_Crash1_LevelIcons { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // Get the pointer table
            var pointerTable = PointerTables.GBAVV_PointerTable(s.GameSettings.GameModeSelection, Offset.file);

            // Get the current lev info
            var manager = (GBAVV_BaseManager)s.GameSettings.GetGameManager;

            if (pointerTable.ContainsKey(GBAVV_Pointer.Localization))
            {
                var multipleLanguages = s.GameSettings.GameModeSelection == GameModeSelection.Crash1GBAEU || s.GameSettings.GameModeSelection == GameModeSelection.Crash2GBAEU;

                if (multipleLanguages)
                    LocTablePointers = s.DoAt(pointerTable[GBAVV_Pointer.Localization], () => s.SerializePointerArray(LocTablePointers, 6, name: nameof(LocTablePointers)));
                else
                    LocTablePointers = new Pointer[]
                    {
                        pointerTable[GBAVV_Pointer.Localization]
                    };

                if (LocTables == null)
                    LocTables = new GBAVV_LocTable[LocTablePointers.Length];

                for (int i = 0; i < LocTables.Length; i++)
                    LocTables[i] = s.DoAt(LocTablePointers[i], () => s.SerializeObject<GBAVV_LocTable>(LocTables[i], name: $"{nameof(LocTables)}[{i}]"));
            }

            s.Context.StoreObject(GBAVV_BaseManager.LocTableID, LocTables?.FirstOrDefault());

            s.DoAt(pointerTable[GBAVV_Pointer.LevelInfo], () =>
            {
                if (LevelInfos == null)
                    LevelInfos = new GBAVV_LevelInfo[manager.LevInfos.Max(x => x.LevelIndex) + 1];

                for (int i = 0; i < LevelInfos.Length; i++)
                    LevelInfos[i] = s.SerializeObject<GBAVV_LevelInfo>(LevelInfos[i], x => x.LevInfo = i == CurrentLevInfo.LevelIndex ? CurrentLevInfo : null, name: $"{nameof(LevelInfos)}[{i}]");
            });

            if (CurrentMapInfo.MapType == GBAVV_MapInfo.GBAVV_MapType.Normal ||
                CurrentMapInfo.MapType == GBAVV_MapInfo.GBAVV_MapType.Normal_Vehicle_0 ||
                CurrentMapInfo.MapType == GBAVV_MapInfo.GBAVV_MapType.Normal_Vehicle_1 ||
                CurrentMapInfo.MapType == GBAVV_MapInfo.GBAVV_MapType.WorldMap)
            {
                Map2D_Graphics = s.DoAt(pointerTable[GBAVV_Pointer.Map2D_Graphics], () => s.SerializeObject<GBAVV_Map2D_Graphics>(Map2D_Graphics, name: nameof(Map2D_Graphics)));
            }

            if (CurrentMapInfo.MapType == GBAVV_MapInfo.GBAVV_MapType.Mode7)
            {
                s.DoAt(pointerTable[GBAVV_Pointer.Mode7_LevelInfo], () =>
                {
                    if (Mode7_LevelInfos == null)
                        Mode7_LevelInfos = new GBAVV_Mode7_LevelInfo[7];

                    var index3D = CurrentMapInfo.Index3D;

                    for (int i = 0; i < Mode7_LevelInfos.Length; i++)
                        Mode7_LevelInfos[i] = s.SerializeObject<GBAVV_Mode7_LevelInfo>(Mode7_LevelInfos[i], x => x.SerializeData = i == index3D, name: $"{nameof(Mode7_LevelInfos)}[{i}]");
                });

                if (s.GameSettings.EngineVersion == EngineVersion.GBAVV_Crash2)
                {
                    GBAVV_Pointer palPointer = CurrentMode7LevelInfo.LevelType == 0 ? GBAVV_Pointer.Mode7_TilePalette_Type0 : GBAVV_Pointer.Mode7_TilePalette_Type1_Flames;

                    Mode7_TilePalette = s.DoAt(pointerTable[palPointer], () => s.SerializeObjectArray<RGBA5551Color>(Mode7_TilePalette, CurrentMode7LevelInfo.LevelType == 0 ? 256 : 16, name: nameof(Mode7_TilePalette)));
                }
                else if (s.GameSettings.EngineVersion == EngineVersion.GBAVV_Crash1 && CurrentMode7LevelInfo.LevelType == 0)
                {
                    Mode7_Crash1_Type0_TilePalette_0F = s.DoAt(pointerTable[GBAVV_Pointer.Mode7_Crash1_Type0_TilePalette_0F], () => s.SerializeObjectArray<RGBA5551Color>(Mode7_Crash1_Type0_TilePalette_0F, 16, name: nameof(Mode7_Crash1_Type0_TilePalette_0F)));
                }

                if (s.GameSettings.EngineVersion == EngineVersion.GBAVV_Crash2 && CurrentMode7LevelInfo.LevelType == 0)
                {
                    Mode7_Crash2_Type0_BG1 = s.DoAt(pointerTable[GBAVV_Pointer.Mode7_Crash2_Type0_BG1], () => s.SerializeArray<byte>(Mode7_Crash2_Type0_BG1, 38 * 9 * 32, name: nameof(Mode7_Crash2_Type0_BG1)));
                }
                else if (s.GameSettings.EngineVersion == EngineVersion.GBAVV_Crash2 && CurrentMode7LevelInfo.LevelType == 1)
                {
                    Mode7_Crash2_Type1_FlamesTileMapsPointers = s.DoAt(pointerTable[GBAVV_Pointer.Mode7_Crash2_Type1_FlamesTileMaps], () => s.SerializePointerArray(Mode7_Crash2_Type1_FlamesTileMapsPointers, 20, name: nameof(Mode7_Crash2_Type1_FlamesTileMapsPointers)));

                    if (Mode7_Crash2_Type1_FlamesTileMaps == null)
                        Mode7_Crash2_Type1_FlamesTileMaps = new MapTile[20][];

                    for (int i = 0; i < Mode7_Crash2_Type1_FlamesTileMaps.Length; i++)
                        Mode7_Crash2_Type1_FlamesTileMaps[i] = s.DoAt(Mode7_Crash2_Type1_FlamesTileMapsPointers[i], () => s.SerializeObjectArray<MapTile>(Mode7_Crash2_Type1_FlamesTileMaps[i], 0x1E * 0x14, name: $"{nameof(Mode7_Crash2_Type1_FlamesTileMaps)}[{i}]"));

                    Mode7_Crash2_Type1_FlamesTileSetLengths = s.DoAt(pointerTable[GBAVV_Pointer.Mode7_Crash2_Type1_FlamesTileSetLengths], () => s.SerializeArray<uint>(Mode7_Crash2_Type1_FlamesTileSetLengths, 20, name: nameof(Mode7_Crash2_Type1_FlamesTileSetLengths)));

                    Mode7_Crash2_Type1_FlamesTileSetsPointers = s.DoAt(pointerTable[GBAVV_Pointer.Mode7_Crash2_Type1_FlamesTileSets], () => s.SerializePointerArray(Mode7_Crash2_Type1_FlamesTileSetsPointers, 20, name: nameof(Mode7_Crash2_Type1_FlamesTileSetsPointers)));

                    if (Mode7_Crash2_Type1_FlamesTileSets == null)
                        Mode7_Crash2_Type1_FlamesTileSets = new byte[20][];

                    for (int i = 0; i < Mode7_Crash2_Type1_FlamesTileSets.Length; i++)
                        Mode7_Crash2_Type1_FlamesTileSets[i] = s.DoAt(Mode7_Crash2_Type1_FlamesTileSetsPointers[i], () => s.SerializeArray<byte>(Mode7_Crash2_Type1_FlamesTileSets[i], Mode7_Crash2_Type1_FlamesTileSetLengths[i], name: $"{nameof(Mode7_Crash2_Type1_FlamesTileSets)}[{i}]"));
                }
            }

            if (CurrentMapInfo.MapType == GBAVV_MapInfo.GBAVV_MapType.Isometric)
            {
                var index3D = CurrentMapInfo.Index3D;

                s.DoAt(pointerTable[GBAVV_Pointer.Isometric_MapDatas], () =>
                {
                    if (Isometric_MapDatas == null)
                        Isometric_MapDatas = new GBAVV_Isometric_MapData[7];


                    for (int i = 0; i < Isometric_MapDatas.Length; i++)
                        Isometric_MapDatas[i] = s.SerializeObject<GBAVV_Isometric_MapData>(Isometric_MapDatas[i], x => x.SerializeData = i == index3D + 4, name: $"{nameof(Isometric_MapDatas)}[{i}]");
                });

                s.DoAt(pointerTable[GBAVV_Pointer.Isometric_ObjectDatas], () =>
                {
                    if (Isometric_ObjectDatas == null)
                        Isometric_ObjectDatas = new GBAVV_Isometric_ObjectData[7];

                    for (int i = 0; i < Isometric_ObjectDatas.Length; i++)
                        Isometric_ObjectDatas[i] = s.SerializeObject<GBAVV_Isometric_ObjectData>(Isometric_ObjectDatas[i], x =>
                        {
                            x.SerializeData = i == index3D + 4;
                            x.IsMultiplayer = i < 4;
                        }, name: $"{nameof(Isometric_ObjectDatas)}[{i}]");
                });

                Isometric_CharacterInfos = s.DoAt(pointerTable[GBAVV_Pointer.Isometric_Characters], () => s.SerializeObjectArray<GBAVV_Isometric_CharacterInfo>(Isometric_CharacterInfos, 12, name: nameof(Isometric_CharacterInfos)));
                Isometric_CharacterIcons = s.DoAt(pointerTable[GBAVV_Pointer.Isometric_CharacterIcons], () => s.SerializeObjectArray<GBAVV_Isometric_CharacterIcon>(Isometric_CharacterIcons, 11, name: nameof(Isometric_CharacterIcons)));
                Isometric_ObjAnimations = s.DoAt(pointerTable[GBAVV_Pointer.Isometric_ObjAnimations], () => s.SerializeObjectArray<GBAVV_Isometric_Animation>(Isometric_ObjAnimations, 22, name: nameof(Isometric_ObjAnimations)));

                Isometric_ObjPalette_0 = s.DoAt(pointerTable[GBAVV_Pointer.Isometric_ObjPalette_0], () => s.SerializeObjectArray<RGBA5551Color>(Isometric_ObjPalette_0, 16, name: nameof(Isometric_ObjPalette_0)));
                Isometric_ObjPalette_1 = s.DoAt(pointerTable[GBAVV_Pointer.Isometric_ObjPalette_1], () => s.SerializeObjectArray<RGBA5551Color>(Isometric_ObjPalette_1, 16, name: nameof(Isometric_ObjPalette_1)));
                Isometric_ObjPalette_2 = s.DoAt(pointerTable[GBAVV_Pointer.Isometric_ObjPalette_2], () => s.SerializeObjectArray<RGBA5551Color>(Isometric_ObjPalette_2, 16, name: nameof(Isometric_ObjPalette_2)));
                Isometric_ObjPalette_4 = s.DoAt(pointerTable[GBAVV_Pointer.Isometric_ObjPalette_4], () => s.SerializeObjectArray<RGBA5551Color>(Isometric_ObjPalette_4, 16, name: nameof(Isometric_ObjPalette_4)));
                Isometric_ObjPalette_11 = s.DoAt(pointerTable[GBAVV_Pointer.Isometric_ObjPalette_11], () => s.SerializeObjectArray<RGBA5551Color>(Isometric_ObjPalette_11, 16, name: nameof(Isometric_ObjPalette_11)));
                Isometric_ObjPalette_12 = s.DoAt(pointerTable[GBAVV_Pointer.Isometric_ObjPalette_12], () => s.SerializeObjectArray<RGBA5551Color>(Isometric_ObjPalette_12, 16, name: nameof(Isometric_ObjPalette_12)));
                Isometric_ObjPalette_13 = s.DoAt(pointerTable[GBAVV_Pointer.Isometric_ObjPalette_13], () => s.SerializeObjectArray<RGBA5551Color>(Isometric_ObjPalette_13, 16, name: nameof(Isometric_ObjPalette_13)));

                // These animations are all hard-coded from functions:
                Isometric_AdditionalAnimations = new GBAVV_Isometric_Animation[]
                {
                    GBAVV_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim0_Frames], 0x03, 4, 4, 2), // Green barrel
                    GBAVV_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim1_Frames], 0x03, 4, 4, 2), // Laser beam
                    GBAVV_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim2_Frames], 0x06, 4, 4, 1), // Crate breaks
                    GBAVV_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim3_Frames], 0x07, 4, 4, 1), // Checkpoint breaks
                    GBAVV_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim4_Frames], 0x18, 8, 4, 0), // Checkpoint text
                    GBAVV_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim5_Frames], 0x08, 4, 4, 2), // Nitro explosion
                    GBAVV_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim6_Frames], 0x08, 4, 4, 2), // Nitro switch
                    GBAVV_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim7_Frames], 0x0E, 4, 4, 0), // Wumpa HUD
                    GBAVV_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim8_Frames], 0x0A, 8, 8, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim8_Palette]), // Crystal collected
                    GBAVV_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim9_Frames], 0x03, 4, 4, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim9_Palette]), // Multiplayer base
                    GBAVV_Isometric_Animation.CrateAndSerialize(s, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim10_Frames], 0x0A, 2, 2, pointerTable[GBAVV_Pointer.Isometric_AdditionalAnim10_Palette]), // Multiplayer item
                };
            }

            if (CurrentMapInfo.MapType == GBAVV_MapInfo.GBAVV_MapType.WorldMap)
            {
                if (pointerTable.ContainsKey(GBAVV_Pointer.WorldMap))
                    WorldMap = s.DoAt(pointerTable[GBAVV_Pointer.WorldMap], () => s.SerializeObject(WorldMap, name: nameof(WorldMap)));

                if (s.GameSettings.EngineVersion == EngineVersion.GBAVV_Crash1)
                    WorldMap_Crash1_LevelIcons = s.DoAt(pointerTable[GBAVV_Pointer.WorldMap_Crash1_LevelIcons], () => s.SerializeObjectArray<GBAVV_WorldMap_Crash1_LevelIcon>(WorldMap_Crash1_LevelIcons, 10, name: nameof(WorldMap_Crash1_LevelIcons)));
            }
        }
    }
}