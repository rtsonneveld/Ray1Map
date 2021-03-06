﻿using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace R1Engine
{
    public static class Util {
        public static bool ByteArrayToFile(string fileName, byte[] byteArray) {
			if (byteArray == null) return false;
            if (FileSystem.mode == FileSystem.Mode.Web) return false;
            try {
				Directory.CreateDirectory(new System.IO.FileInfo(fileName).Directory.FullName);
                using (var fs = new FileStream(fileName, System.IO.FileMode.Create, FileAccess.Write)) {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            } catch (Exception ex) {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }

		private static readonly string[] SizeSuffixes =
				  { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
		public static string SizeSuffix(Int64 value, int decimalPlaces = 1) {
			if (value < 0) { return "-" + SizeSuffix(-value); }

			int i = 0;
			decimal dValue = value;
			while (Math.Round(dValue, decimalPlaces) >= 1000) {
				dValue /= 1024;
				i++;
			}

			return string.Format("{0:n" + decimalPlaces + "} {1}", dValue, SizeSuffixes[i]);
		}
        public static BaseColor[] CreateDummyPalette(int length, bool firstTransparent = true, int? wrap = null) {
            BaseColor[] pal = new BaseColor[length];
            if(wrap == null) wrap = length;
            if (firstTransparent) {
                pal[0] = BaseColor.clear;
            }
            for (int i = firstTransparent ? 1 : 0; i < length; i++) {
                float val = (float)(i % wrap.Value) / (wrap.Value - 1);
                float bv = val;
                pal[i] = new CustomColor(bv,bv,bv);
            }
            return pal;
        }

        public static uint NextPowerOfTwo(uint v) {
            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v++;
            return v;
        }

		public static string NormalizePath(string path, bool isFolder) {
			string newPath = path.Replace("\\", "/");
			if (isFolder && !newPath.EndsWith("/")) newPath += "/";
			return newPath;
		}

		// For debugging
		public static void ExportPointerArray(SerializerObject s, string path, IEnumerable<Pointer> pointers)
        {
            var p1 = pointers.Where(x => x != null).Distinct().OrderBy(x => x.AbsoluteOffset).ToArray();
            var output = new List<string>();

            for (int i = 0; i < p1.Length - 1; i++)
            {
                var length = p1[i + 1] - p1[i];

                s.DoAt(p1[i], () => output.Add($"{p1[i]}: byte[{length:000}] {String.Join(" ", s.SerializeArray<byte>(null, length).Select(x => x.ToString("X2")))}"));
            }

            File.WriteAllLines(path, output);
        }

		/// <summary>
		/// Convert a byte array to a hex string
		/// </summary>
		/// <param name="Bytes">The byte array to convert</param>
		/// <param name="Align">Should the byte array be split in different lines, this defines the length of one line</param>
		/// <param name="NewLinePrefix">The prefix to add to each new line</param>
		/// <returns></returns>
		public static string ByteArrayToHexString(byte[] Bytes, int? Align = null, string NewLinePrefix = null, int? MaxLines = null) {
			StringBuilder Result = new StringBuilder(Bytes.Length * 2);
			string HexAlphabet = "0123456789ABCDEF";
			int curLine = 0;
			for(int i = 0; i < Bytes.Length; i++) {
				if (i > 0 && Align.HasValue && i % Align == 0) {
					curLine++;
					if (MaxLines.HasValue && curLine >= MaxLines.Value) {
						Result.Append("...");
						return Result.ToString();
					}
					Result.Append("\n" + NewLinePrefix ?? "");
				}
				byte B = Bytes[i];
				Result.Append(HexAlphabet[(int)(B >> 4)]);
				Result.Append(HexAlphabet[(int)(B & 0xF)]);
				if(i < Bytes.Length-1) Result.Append(' ');
			}

			return Result.ToString();
		}

        public static void OutputJSONForWeb(string outputDir)
        {
            foreach (var mode in EnumHelpers.GetValues<GameModeSelection>().Where(x => Settings.GameDirectories.ContainsKey(x) && Directory.Exists(Settings.GameDirectories[x])))
            {
                var s = new GameSettings(mode, Settings.GameDirectories[mode], 0, 0);
                var m = (IGameManager)Activator.CreateInstance(mode.GetAttribute<GameModeAttribute>().ManagerType);

                foreach (var vol in m.GetLevels(s))
                {
                    s.EduVolume = vol.Name;
                    OutputJSONForWeb(Path.Combine(outputDir, $"{mode}{vol.Name}.json"), s);
                }
            }
        }

        public static void OutputJSONForWeb(string outputPath, GameSettings s)
        {
            var manager = s.GetGameManager;
            var attr = s.GameModeSelection.GetAttribute<GameModeAttribute>();
            var settings = s;
            var worlds = manager.GetLevels(settings).First(x => x.Name == null || x.Name == s.EduVolume).Worlds.ToArray();
            var names = MapNames.GetMapNames(attr.Game);

            var lvlWorldIndex = 0;

            var jsonObj = new
            {
                name = attr.DisplayName,
                mode = s.GameModeSelection.ToString(),
                folder = (string)null,
                icons = worlds.Select(x =>
                {
                    var icon = new
                    {
                        image = (string)null,
                        level = lvlWorldIndex
                    };

                    lvlWorldIndex += x.Maps.Length;

                    return icon;
                }),
                levels = worlds.Select(w => w.Maps.OrderBy(x => x).Select(lvl => new
                {
                    world = w.Index,
                    level = lvl,
                    nameInternal = s.MajorEngineVersion == MajorEngineVersion.GBA ? lvl.ToString() : (string)null,
                    name = names?.TryGetItem(w.Index)?.TryGetItem(lvl) ?? (s.MajorEngineVersion == MajorEngineVersion.GBA ? $"Map {lvl}" : $"Map {w.Index}-{lvl}")
                })).SelectMany(x => x)
            };

            JsonHelpers.SerializeToFile(jsonObj, outputPath);
        }

        public static void OutputEDUJSONForWeb(string dir, GameModeSelection mode, bool isPC)
        {
            var modeName = mode == GameModeSelection.RaymanQuizPC || mode == GameModeSelection.RaymanQuizPS1 ? "quiz" : "edu";
            var platformName = isPC ? "PC" : "PS1";
            var m = isPC ? new R1_PCEdu_Manager() : new R1_PS1Edu_Manager();

            foreach (var subDir in Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly))
            {
                var settings = new GameSettings(mode, subDir, 1, 1);

                using (var context = new Context(settings))
                {
                    foreach (var v in m.GetLevels(settings))
                    {
                        var vol = v.Name;
                        settings.EduVolume = vol;
                        var specialPath = m.GetSpecialArchiveFilePath(vol);

                        context.AddFile(new LinearSerializedFile(context)
                        {
                            filePath = specialPath
                        });

                        var wldMap = m.LoadArchiveFile<R1_PC_WorldMap>(context, specialPath, R1_PCBaseManager.R1_PC_ArchiveFileName.WLDMAP01);
                        var text = m.LoadArchiveFile<R1_PC_LocFile>(context, specialPath, R1_PCBaseManager.R1_PC_ArchiveFileName.TEXT);

                        var worlds = v.Worlds;

                        var lvlWorldIndex = 0;

                        var jsonObj = new
                        {
                            name = $"NAME ({platformName} - {vol})",
                            mode = mode.ToString(),
                            folder = $"r1/{modeName}/{Path.GetFileName(subDir)}",
                            volume = vol,
                            icons = worlds.Select(x =>
                            {
                                var icon = new
                                {
                                    image = $"./img/icon/R1/R1-W{x.Index}.png",
                                    level = lvlWorldIndex
                                };

                                lvlWorldIndex += x.Maps.Length;

                                return icon;
                            }),
                            levels = worlds.Select(w => w.Maps.OrderBy(x => x).Select(lvl => new
                            {
                                world = w.Index,
                                level = lvl,
                                nameInternal = $"{m.GetShortWorldName((R1_World)w.Index)}{lvl:00}",
                                name = getLevelName(w.Index, lvl)
                            })).SelectMany(x => x)
                        };

                        JsonHelpers.SerializeToFile(jsonObj, Path.Combine(dir, $"{platformName.ToLower()}_{vol.ToLower()}.json"));

                        string getLevelName(int world, int level)
                        {
                            foreach (var lvl in wldMap.Levels.Take(wldMap.LevelsCount))
                            {
                                sbyte currentWorld = -1;
                                var levelIndex = 0;
                                var groupIndex = 1;

                                for (int i = 0; i < lvl.MapEntries.Length; i++)
                                {
                                    var entry = lvl.MapEntries[i];

                                    if (entry.Level == -1)
                                    {
                                        levelIndex = 0;
                                        groupIndex++;
                                        continue;
                                    }
                                    else
                                    {
                                        levelIndex++;
                                    }

                                    if (entry.World != -1)
                                        currentWorld = entry.World;

                                    if (currentWorld == world && entry.Level == level)
                                        return $"{text.TextDefine[lvl.LevelName].Value.Trim('/')} {groupIndex}-{levelIndex}";
                                }
                            }

                            return $"{(R1_World)world} {level}";
                        }
                    }
                }
            }
        }

        public static void OutputGBAJSONForWeb(string dir, GameModeSelection mode)
        {
            var dirName = Path.GetFileName(dir);
            var name = dirName.Substring(0, dirName.LastIndexOf('_'));
            var settings = new GameSettings(mode, dir, 1, 1);

            var m = settings.GetGameManager;

            var worlds = m.GetLevels(settings).First().Worlds;

            var jsonObj = new
            {
                name = $"{mode.GetAttribute<GameModeAttribute>().DisplayName.Replace(" - EU", "").Replace(" - US", "").Replace(" - EU 1", "").Replace(" - EU 2", "")}",
                mode = mode.ToString(),
                folder = $"gba/{dirName}",
                levels = worlds.Select(w => w.Maps.OrderBy(x => x).Select(lvl => new
                {
                    world = w.Index,
                    level = lvl,
                    nameInternal = $"{lvl}",
                    name = $"Map {lvl}"
                })).SelectMany(x => x)
            };

            var outDir = Path.Combine(Path.GetDirectoryName(dir), "JSON", name);

            Directory.CreateDirectory(outDir);

            JsonHelpers.SerializeToFile(jsonObj, Path.Combine(outDir, $"{dirName.Substring(name.Length + 1)}.json"));
        }

        public static void OutputGBAVVJSONLevelListForWeb(GameModeSelection mode)
        {
            // Helper for getting a line
            string getLine(int world, int level, string nameInternal, string name) => $"    {{ \"world\": {world}, \"level\": {level}, \"nameInternal\": \"{nameInternal}\", \"name\": \"{name}\" }},";

            StringBuilder str = new StringBuilder();

            var manager = (GBAVV_BaseManager)new GameSettings(mode, "", 0, 0).GetGameManager;

            for (var i = 0; i < manager.LevInfos.Length; i++)
            {
                var lev = manager.LevInfos[i];
                str.AppendLine(getLine(0, i, lev.LevelIndex != -1 ? lev.LevelIndex.ToString() : null, lev.DisplayName));
            }

            str.ToString().CopyToClipboard();
        }

        public static void RenameFilesToUpper(string inputDir)
        {
            foreach (var file in Directory.GetFiles(inputDir, "*", SearchOption.AllDirectories))
            {
                var dir = Path.GetDirectoryName(file);
                var fileName = Path.GetFileName(file);

                // Move to temp name
                var tempPath = Path.Combine(dir, $"TEMP_{fileName}");
                File.Move(file, tempPath);

                // Move to upper-case name
                File.Move(tempPath, Path.Combine(dir, fileName.ToUpper()));
            }
        }

        public static async UniTask EnumerateLevelsAsync(Func<GameSettings, UniTask> action)
        {
            var manager = Settings.GetGameManager;
            var settings = Settings.GetGameSettings;

            foreach (var vol in manager.GetLevels(settings))
            {
                settings.EduVolume = vol.Name;

                foreach (var world in vol.Worlds)
                {
                    settings.World = world.Index;

                    foreach (var map in world.Maps)
                    {
                        settings.Level = map;
                        await action(settings);
                    }
                }
            }
        }

        public static void FindMatchingEncoding(params KeyValuePair<string, byte[]>[] input)
        {
            if (input.Length < 2)
                throw new Exception("Too few strings to check!");

            // Get all possible encodings
            var encodings = Encoding.GetEncodings().Select(x => Encoding.GetEncoding(x.CodePage)).ToArray();

            // Keep a list of all matching ones
            var matches = new List<Encoding>();

            // Helper method for getting all matching encodings
            IEnumerable<Encoding> GetMatches(KeyValuePair<string, byte[]> str)
            {
                var m = encodings.Where(enc => enc.GetString(str.Value).Equals(str.Key, StringComparison.InvariantCultureIgnoreCase)).ToArray();
                Debug.Log($"Matching encodings for {str.Key}: {String.Join(", ", m.Select(x => $"{x.EncodingName} ({x.CodePage})"))}");
                return m;
            }

            // Add matches for the first one
            matches.AddRange(GetMatches(input.First()));

            // Check remaining ones, removing any which don't match
            foreach (var str in input.Skip(1))
            {
                var ma = GetMatches(str);
                matches.RemoveAll(x => !ma.Contains(x));
            }

            // Log the result
            Debug.Log($"Matching encodings for all: {String.Join(", ", matches.Select(x => $"{x.EncodingName} ({x.CodePage})"))}");
        }

        public static void ExportWAVChunks(Context context, WAVChunk[] chunks, string outputDir)
        {
            var index = 0;

            foreach (var chunk in chunks)
            {
                if (chunk.ChunkHeader == "LIST")
                {
                    var list = chunk.SerializeTo<WAVListChunk>(context);
                    ExportWAVChunks(context, list.Chunks, Path.Combine(outputDir, $"{index}_{list.ListHeader}"));
                }
                else
                {
                    Util.ByteArrayToFile(Path.Combine(outputDir, $"{index}_{chunk.ChunkHeader}"), chunk.Data);
                }

                index++;
            }
        }

        public static void ExportAllCompressedData(Context context, Pointer offset, IStreamEncoder encoder, byte[] header, string outputDir, int alignment = 4, int minDecompSize = 33)
        {
            var s = context.Deserializer;

            s.Goto(offset);

            // Keep track of blocks
            var blocks = new List<Tuple<long, long, int>>();

            // Enumerate every byte
            for (int i = 0; i < s.CurrentLength; i += alignment)
            {
                // Go to the offset
                s.Goto(offset + i);

                // Check for compression header
                if (s.SerializeArray<byte>(default, header.Length).SequenceEqual(header))
                {
                    if (encoder is GBA_LZSSEncoder)
                    {
                        // Get the decompressed size
                        var decompressedSizeValue = s.SerializeArray<byte>(default, 3);
                        Array.Resize(ref decompressedSizeValue, 4);
                        var decompressedSize = BitConverter.ToUInt32(decompressedSizeValue, 0);

                        // Skip if the decompressed size is too low
                        if (decompressedSize < minDecompSize)
                            continue;
                    }

                    // Go back to the offset
                    s.Goto(offset + i);

                    // Attempt to decompress
                    try
                    {
                        byte[] data = null;

                        s.DoEncoded(encoder, () => data = s.SerializeArray<byte>(default, s.CurrentLength));

                        // Make sure we got some data
                        if (data != null && data.Length >= minDecompSize)
                        {
                            ByteArrayToFile(Path.Combine(outputDir, $"Block_0x{(offset + i).AbsoluteOffset:X8}.dat"), data);

                            blocks.Add(new Tuple<long, long, int>((offset + i).AbsoluteOffset, s.CurrentPointer - (offset + i), data.Length));
                        }
                    }
                    catch
                    {
                        // Ignore exceptions...
                    }
                }
            }

            var log = new List<string>();

            for (int i = 0; i < blocks.Count; i++)
            {
                var (off, compressedSize, size) = blocks[i];

                var end = off + compressedSize;

                log.Add($"0x{off:X8} - 0x{end:X8} (0x{compressedSize:X8} - 0x{size:X8}) - ");

                if (i != blocks.Count - 1)
                {
                    var dif = blocks[i + 1].Item1 - end;

                    if (dif >= 4)
                        log.Add($"0x{end:X8} - 0x{end + dif:X8} (0x{dif:X8})              - ");
                }
            }

            File.WriteAllLines(Path.Combine(outputDir, "blocks_log.txt"), log);
        }

        public static Texture2D ToTileSetTexture(byte[] imgData, Color[] pal, TileEncoding encoding, int tileWidth, bool flipY, int wrap = 32, Func<int, Color[]> getPalFunc = null, bool flipTileX = false, bool flipTileY = false, bool flipX = false)
        {
            int bpp;

            switch (encoding)
            {
                case TileEncoding.Planar_2bpp:
                    bpp = 2; break;
                case TileEncoding.Planar_4bpp:
                case TileEncoding.Linear_4bpp:
                case TileEncoding.Linear_4bpp_ReverseOrder:
                    bpp = 4; break;
                case TileEncoding.Linear_8bpp: 
                    bpp = 8; break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null);
            }

            int tileSize = tileWidth * tileWidth * bpp / 8;
            int tilesetLength = imgData.Length / tileSize;

            int tilesX = Math.Min(tilesetLength, wrap);
            int tilesY = Mathf.CeilToInt(tilesetLength / (float)wrap);

            var tex = TextureHelpers.CreateTexture2D(tilesX * tileWidth, tilesY * tileWidth);

            for (int i = 0; i < tilesetLength; i++)
            {
                int tileY = ((i / wrap)) * tileWidth;
                int tileX = (i % wrap) * tileWidth;

                tex.FillInTile(
                    imgData: imgData, 
                    imgDataOffset: i * tileSize, 
                    pal: getPalFunc?.Invoke(i) ?? pal, 
                    encoding: encoding, 
                    tileWidth: tileWidth, 
                    flipTextureY: flipY, 
                    flipTextureX: flipX, 
                    tileX: tileX, 
                    tileY: tileY, 
                    flipTileX: flipTileX, 
                    flipTileY: flipTileY);
            }

            tex.Apply();

            return tex;
        }

        public static void FillInTile(this Texture2D tex, byte[] imgData, int imgDataOffset, Color[] pal, TileEncoding encoding, int tileWidth, bool flipTextureY, int tileX, int tileY, bool flipTileX = false, bool flipTileY = false, bool ignoreTransparent = false, bool flipTextureX = false)
        {
            bool reverseOrder = (encoding == TileEncoding.Linear_4bpp_ReverseOrder);

            // Fill in tile pixels
            for (int y = 0; y < tileWidth; y++) {

                var yy = tileY + y;

                if (flipTextureY)
                    yy = tex.height - yy - 1;
                if (yy < 0 || yy >= tex.height) continue;

                for (int x = 0; x < tileWidth; x++) {
                    var xx = tileX + x;

                    if (flipTextureX)
                        xx = tex.width - xx - 1;

                    if (xx < 0 || xx >= tex.width) continue;
                    Color c;

                    if (encoding == TileEncoding.Linear_8bpp) 
                    {
                        int index = imgDataOffset + (((flipTileY ? (tileWidth - y - 1) : y) * tileWidth + (flipTileX ? (tileWidth - x - 1) : x)));

                        var b = imgData[index];
                        c = pal[b];
                    } 
                    else if (encoding == TileEncoding.Linear_4bpp || encoding == TileEncoding.Linear_4bpp_ReverseOrder)
                    {
                        int index = imgDataOffset + (((flipTileY ? (tileWidth - y - 1) : y) * tileWidth + (flipTileX ? (tileWidth - x - 1) : x)) / 2);

                        var b = imgData[index];
                        var v = (flipTileX ^ reverseOrder) ? 
                            BitHelpers.ExtractBits(b, 4, x % 2 == 1 ? 0 : 4) : 
                            BitHelpers.ExtractBits(b, 4, x % 2 == 0 ? 0 : 4);
                        c = pal[v];
                    } 
                    else if (encoding == TileEncoding.Planar_4bpp)
                    {
                        int off = (flipTileY ? (tileWidth - y - 1) : y) * tileWidth + (flipTileX ? (tileWidth - x - 1) : x);

                        var offset1 = imgDataOffset;
                        var offset2 = imgDataOffset + 16;

                        var bit0 = BitHelpers.ExtractBits(imgData[offset1 + ((off / 8) * 2)], 1, off % 8);
                        var bit1 = BitHelpers.ExtractBits(imgData[offset1 + ((off / 8) * 2 + 1)], 1, off % 8);
                        var bit2 = BitHelpers.ExtractBits(imgData[offset2 + ((off / 8) * 2)], 1, off % 8);
                        var bit3 = BitHelpers.ExtractBits(imgData[offset2 + ((off / 8) * 2 + 1)], 1, off % 8);

                        int b = 0;

                        b = BitHelpers.SetBits(b, bit0, 1, 0);
                        b = BitHelpers.SetBits(b, bit1, 1, 1);
                        b = BitHelpers.SetBits(b, bit2, 1, 2);
                        b = BitHelpers.SetBits(b, bit3, 1, 3);

                        c = pal[b];
                    }
                    else if (encoding == TileEncoding.Planar_2bpp) 
                    {
                        int index = imgDataOffset + (((flipTileY ? (tileWidth - y - 1) : y) * tileWidth + (flipTileX ? (tileWidth - x - 1) : x)) / 8) * 2;
                        var b0 = imgData[index];
                        var b1 = imgData[index + 1];
                        int actualX = flipTileX ? x : 7 - x;
                        var v = (BitHelpers.ExtractBits(b1, 1, actualX) << 1) | BitHelpers.ExtractBits(b0, 1, actualX);
                        c = pal[v];
                    } 
                    else 
                    {
                        c = Color.clear;
                    }

                    if (!ignoreTransparent || c.a > 0) {
                        tex.SetPixel(xx, yy, c);
                    }
                }
            }
        }


        public static Texture2D ToTileSetTexture(BaseColor[] imgData, int tileWidth, bool flipY, int wrap = 32) {
            int tileSize = tileWidth * tileWidth;
            int tilesetLength = imgData.Length / tileSize;

            int tilesX = Math.Min(tilesetLength, wrap);
            int tilesY = Mathf.CeilToInt(tilesetLength / (float)wrap);

            var tex = TextureHelpers.CreateTexture2D(tilesX * tileWidth, tilesY * tileWidth);

            for (int i = 0; i < tilesetLength; i++) {
                int tileY = ((i / wrap)) * tileWidth;
                int tileX = (i % wrap) * tileWidth;

                tex.FillInTile(imgData, i * tileSize, tileWidth, flipY, tileX, tileY);
            }

            tex.Apply();

            return tex;
        }

        public static void FillInTile(this Texture2D tex, BaseColor[] imgData, int imgDataOffset, int tileWidth, bool flipTextureY, int tileX, int tileY, bool flipTileX = false, bool flipTileY = false, bool ignoreTransparent = false) {
            // Fill in tile pixels
            for (int y = 0; y < tileWidth; y++) {

                var yy = tileY + y;

                if (flipTextureY)
                    yy = tex.height - yy - 1;
                if (yy < 0 || yy >= tex.height) continue;

                for (int x = 0; x < tileWidth; x++) {
                    var xx = tileX + x;
                    if (xx < 0 || xx >= tex.width) continue;
                    Color c;

                    int index = imgDataOffset + (((flipTileY ? (tileWidth - y - 1) : y) * tileWidth + (flipTileX ? (tileWidth - x - 1) : x)));
                    c = imgData[index].GetColor();

                    if (!ignoreTransparent || c.a > 0) {
                        tex.SetPixel(xx, yy, c);
                    }
                }
            }
        }

        public static Texture2D GetGridTex(int cellSize)
        {
            var tex = TextureHelpers.CreateTexture2D(cellSize, cellSize);

            for (int y = 0; y < cellSize; y++)
            {
                for (int x = 0; x < cellSize; x++)
                {
                    if (y == cellSize - 1 || x == cellSize - 1)
                        tex.SetPixel(x, y, new Color(1,1,1,0.25f));
                    else
                        tex.SetPixel(x, y, Color.clear);
                }
            }

            tex.Apply();

            return tex;
        }


        public static int GCF(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        public static int LCM(int a, int b)
        {
            return (a / GCF(a, b)) * b;
        }

        public static int LCM(IList<int> numbers, int i = 0)
        {
            if (i + 2 == numbers.Count)
                return LCM(numbers[i], numbers[i + 1]);
            else
                return LCM(numbers[i], LCM(numbers, i + 1));
        }

        public static Color[] ConvertGBAPalette(IEnumerable<BaseColor> palette, int? transparentIndex = 0) => palette.Select((x, i) => {
            Color c = x.GetColor();
            if (!transparentIndex.HasValue || i != transparentIndex.Value) {
                c.a = 1f;
            } else {
                c.a = 0f;
            }
            return c;
        }).ToArray();
        public static Color[][] ConvertAndSplitGBAPalette(BaseColor[] palette, bool firstTransparent = true)
            => palette
            .Split(palette.Length / 16, 16)
            .Select(p => ConvertGBAPalette(p, transparentIndex: firstTransparent ? (int?)0 : null))
            .ToArray();
        public static Color[][] ConvertAndSplitGBCPalette(RGBA5551Color[] palette, int? transparentIndex = 0)
            => palette
            .Split(palette.Length / 4, 4)
            .Select(p => ConvertGBAPalette(p, transparentIndex: transparentIndex))
            .ToArray();


        public static IEnumerable<T[]> Split<T>(this T[] array, int length, int size) => Enumerable.Range(0, length).Select(x => array.Skip(size * x).Take(size).ToArray());

        public static void GetGBASize(byte shape, byte size, out int width, out int height)
        {
            width = 1;
            height = 1;

            switch (shape)
            {
                case 0: // Square
                    width = 1 << size;
                    height = width;
                    break;

                case 1: // Wide
                    switch (size)
                    {
                        case 0: width = 2; height = 1; break;
                        case 1: width = 4; height = 1; break;
                        case 2: width = 4; height = 2; break;
                        case 3: width = 8; height = 4; break;
                    }
                    break;

                case 2: // Tall
                    switch (size)
                    {
                        case 0: width = 1; height = 2; break;
                        case 1: width = 1; height = 4; break;
                        case 2: width = 2; height = 4; break;
                        case 3: width = 4; height = 8; break;
                    }
                    break;
            }
        }

        public enum TileEncoding
        {
            Planar_2bpp,
            Planar_4bpp,
            Linear_4bpp,
            Linear_4bpp_ReverseOrder,
            Linear_8bpp,
        }

        public static void CopyToClipboard(this string str)
        {
            TextEditor te = new TextEditor
            {
                text = str
            };
            te.SelectAll();
            te.Copy();
        }
    }
}