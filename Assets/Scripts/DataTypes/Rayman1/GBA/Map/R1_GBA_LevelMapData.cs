﻿using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// Level data for Rayman Advance (GBA)
    /// </summary>
    public class R1_GBA_LevelMapData : R1Serializable
    {
        #region Level Data

        // Always 0
        public uint UnkDword_DSi_00 { get; set; }

        /// <summary>
        /// Pointer to the tiles
        /// </summary>
        public Pointer TileDataPointer { get; set; }

        // Always 0
        public uint UnkDword_DSi_08 { get; set; }

        /// <summary>
        /// Pointer to the compressed map data. Gets copied to 0x02002230 during runtime.
        /// </summary>
        public Pointer MapDataPointer { get; set; }

        /// <summary>
        /// Pointer to the compressed tile palette index table.
        /// </summary>
        public Pointer TilePaletteIndicesPointer { get; set; }

        // Always 0
        public uint UnkDword_DSi_10 { get; set; }

        /// <summary>
        /// Pointer to the tile header data (2 bytes per tile)
        /// </summary>
        public Pointer TileBlockIndicesPointer { get; set; }

        /// <summary>
        /// Pointer to the tile palettes
        /// </summary>
        public Pointer TilePalettePointer { get; set; }

        public uint UnkDword_DSi_1C { get; set; }

        public byte Unk_10 { get; set; }
        public byte Unk_11 { get; set; }
        public byte BackgroundIndex { get; set; }
        public byte ParallaxBackgroundIndex { get; set; }

        // 1 << 0: Compress map data
        // 1 << 1: Compress tile palette indices
        public uint CompressionFlags { get; set; }

        #endregion

        #region Parsed from Pointers

        public byte[] TileData { get; set; }

        /// <summary>
        /// The map data
        /// </summary>
        public MapData MapData { get; set; }

        /// <summary>
        /// The 10 available tile palettes (16 colors each)
        /// </summary>
        public RGBA5551Color[] TilePalettes { get; set; }

        public byte[] TilePaletteIndices { get; set; }

        public ushort[] TileBlockIndices { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.R1_GBA)
            {
                // Serialize values
                TileDataPointer = s.SerializePointer(TileDataPointer, name: nameof(TileDataPointer));
                MapDataPointer = s.SerializePointer(MapDataPointer, name: nameof(MapDataPointer));
                TilePaletteIndicesPointer = s.SerializePointer(TilePaletteIndicesPointer, name: nameof(TilePaletteIndicesPointer));
                TileBlockIndicesPointer = s.SerializePointer(TileBlockIndicesPointer, name: nameof(TileBlockIndicesPointer));
                TilePalettePointer = s.SerializePointer(TilePalettePointer, name: nameof(TilePalettePointer));
                Unk_10 = s.Serialize<byte>(Unk_10, name: nameof(Unk_10));
                Unk_11 = s.Serialize<byte>(Unk_11, name: nameof(Unk_11));
                BackgroundIndex = s.Serialize<byte>(BackgroundIndex, name: nameof(BackgroundIndex));
                ParallaxBackgroundIndex = s.Serialize<byte>(ParallaxBackgroundIndex, name: nameof(ParallaxBackgroundIndex));
                CompressionFlags = s.Serialize<uint>(CompressionFlags, name: nameof(CompressionFlags));
            }
            else if (s.GameSettings.EngineVersion == EngineVersion.R1_DSi)
            {
                // Serialize values
                UnkDword_DSi_00 = s.Serialize<uint>(UnkDword_DSi_00, name: nameof(UnkDword_DSi_00));
                TileDataPointer = s.SerializePointer(TileDataPointer, name: nameof(TileDataPointer));
                UnkDword_DSi_08 = s.Serialize<uint>(UnkDword_DSi_08, name: nameof(UnkDword_DSi_08));
                MapDataPointer = s.SerializePointer(MapDataPointer, name: nameof(MapDataPointer));
                UnkDword_DSi_10 = s.Serialize<uint>(UnkDword_DSi_10, name: nameof(UnkDword_DSi_10));
                TileBlockIndicesPointer = s.SerializePointer(TileBlockIndicesPointer, name: nameof(TileBlockIndicesPointer));
                TilePalettePointer = s.SerializePointer(TilePalettePointer, name: nameof(TilePalettePointer));
                UnkDword_DSi_1C = s.Serialize<uint>(UnkDword_DSi_1C, name: nameof(UnkDword_DSi_1C));
            }
        }

        public void SerializeLevelData(SerializerObject s) {
            if (s.GameSettings.EngineVersion == EngineVersion.R1_GBA)
            {
                s.DoAt(MapDataPointer, () => {
                    if ((CompressionFlags & 1) == 1)
                    {
                        s.DoEncoded(new GBA_LZSSEncoder(), () => {
                            MapData = s.SerializeObject<MapData>(MapData, name: nameof(MapData));
                        });
                    }
                    else
                    {
                        MapData = s.SerializeObject<MapData>(MapData, name: nameof(MapData));
                    }
                });
                s.DoAt(TilePaletteIndicesPointer, () => {
                    if ((CompressionFlags & 2) == 2)
                    {
                        s.DoEncoded(new GBA_LZSSEncoder(), () => TilePaletteIndices = s.SerializeArray<byte>(TilePaletteIndices, s.CurrentLength, name: nameof(TilePaletteIndices)));
                    }
                    else
                    {
                        uint numTileBlocks = (TilePaletteIndicesPointer.AbsoluteOffset - TileBlockIndicesPointer.AbsoluteOffset) / 2;
                        TilePaletteIndices = s.SerializeArray<byte>(TilePaletteIndices, numTileBlocks, name: nameof(TilePaletteIndices));
                    }
                });

                s.DoAt(TileBlockIndicesPointer, () => TileBlockIndices = s.SerializeArray<ushort>(TileBlockIndices, TilePaletteIndices.Length, name: nameof(TileBlockIndices)));
                s.DoAt(TilePalettePointer, () => TilePalettes = s.SerializeObjectArray<RGBA5551Color>(TilePalettes, 10 * 16, name: nameof(TilePalettes)));

                ushort maxBlockIndex = TileBlockIndices.Max();
                s.DoAt(TileDataPointer, () => TileData = s.SerializeArray<byte>(TileData, 0x20 * ((uint)maxBlockIndex + 1), name: nameof(TileData)));
            }
            else if (s.GameSettings.EngineVersion == EngineVersion.R1_DSi)
            {
                s.DoAt(MapDataPointer, () => s.DoEncoded(new GBA_LZSSEncoder(), () => MapData = s.SerializeObject<MapData>(MapData, name: nameof(MapData))));
                s.DoAt(TileDataPointer, () => {
                    s.DoEncoded(new GBA_LZSSEncoder(), () => TileData = s.SerializeArray<byte>(TileData, s.CurrentLength, name: nameof(TileData)));
                });
                s.DoAt(TilePalettePointer, () => TilePalettes = s.SerializeObjectArray<RGBA5551Color>(TilePalettes, 256, name: nameof(TilePalettes)));
                s.DoAt(TileBlockIndicesPointer, () => {
                    uint maxTileInd = MapData.Tiles.Max(t => t.TileMapY);
                    TileBlockIndices = s.SerializeArray<ushort>(TileBlockIndices, (maxTileInd + 1) * 4, name: nameof(TileBlockIndices));
                });
            }
        }

        #endregion
    }
}