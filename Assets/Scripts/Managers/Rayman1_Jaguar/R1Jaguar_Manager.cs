﻿using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Game manager for Jaguar
    /// </summary>
    public class R1Jaguar_Manager : IGameManager {
        #region Values and paths

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public virtual GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(GetNumLevels.OrderBy(x => x.Key).Select(x => new GameInfo_World((int)x.Key, Enumerable.Range(1, x.Value).ToArray())).ToArray());

        /// <summary>
        /// Gets the file path to the ROM file
        /// </summary>
        public virtual string GetROMFilePath => $"ROM.j64";

        /// <summary>
        /// Gets the base address for the ROM file
        /// </summary>
        protected virtual uint GetROMBaseAddress => 0x00800000;

        /// <summary>
        /// Gets the available levels ordered based on the global level array
        /// </summary>
        public virtual KeyValuePair<R1_World, int>[] GetNumLevels => new KeyValuePair<R1_World, int>[]
        {
            new KeyValuePair<R1_World, int>(R1_World.Jungle, 21),
            new KeyValuePair<R1_World, int>(R1_World.Mountain, 14),
            new KeyValuePair<R1_World, int>(R1_World.Cave, 13),
            new KeyValuePair<R1_World, int>(R1_World.Music, 19),
            new KeyValuePair<R1_World, int>(R1_World.Image, 14),
            new KeyValuePair<R1_World, int>(R1_World.Cake, 4)
        };

        public virtual int[] ExtraMapCommands => new int[] {
            0, 1, 3, 4, 5, 6, 7, 9
        };

        /// <summary>
        /// Gets the vignette addresses and widths
        /// </summary>
        public virtual KeyValuePair<uint, int>[] GetVignette => new KeyValuePair<uint, int>[]
        {
            // Vignette
            new KeyValuePair<uint, int>(GetROMBaseAddress + 43680, 384),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 127930, 160),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 140541, 136),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 150788, 160),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 162259, 80),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 169031, 320),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 246393, 320),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 300827, 320),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 329569, 320),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 351048, 320),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 372555, 320),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 391386, 320),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 409555, 320),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 423273, 320),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 429878, 320),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 450942, 320),

            // Background/foreground
            new KeyValuePair<uint, int>(GetROMBaseAddress + 1353130, 192),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 1395878, 384),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 1462294, 384),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 1553686, 320),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 1743668, 144),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 1750880, 48),

            new KeyValuePair<uint, int>(GetROMBaseAddress + 1809526, 192),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 1845684, 384),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 1928746, 192),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 1971368, 192),

            new KeyValuePair<uint, int>(GetROMBaseAddress + 2205640, 384),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 2269442, 384),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 2355852, 160),

            new KeyValuePair<uint, int>(GetROMBaseAddress + 2702140, 384),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 2803818, 192),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 2824590, 320),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 2916108, 192),

            new KeyValuePair<uint, int>(GetROMBaseAddress + 3078442, 192),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 3118496, 384),

            new KeyValuePair<uint, int>(GetROMBaseAddress + 3276778, 384),
            new KeyValuePair<uint, int>(GetROMBaseAddress + 3323878, 320),
        };

        #endregion

        #region Manager Methods

        /// <summary>
        /// Gets the available game actions
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The game actions</returns>
        public virtual GameAction[] GetGameActions(GameSettings settings)
        {
            return new GameAction[]
            {
                new GameAction("Export Sprites", false, true, (input, output) => ExportAllSpritesAsync(settings, output, false)),
                new GameAction("Export Animation Frames", false, true, (input, output) => ExportAllSpritesAsync(settings, output, true)),
                new GameAction("Export Vignette", false, true, (input, output) => ExtractVignetteAsync(settings, output)),
                new GameAction("Extract Compressed Data", false, true, (input, output) => ExtractCompressedDataAsync(settings, output, false)),
                new GameAction("Extract Compressed Data (888)", false, true, (input, output) => ExtractCompressedDataAsync(settings, output, true)),
                new GameAction("Convert Music to MIDI", false, true, (input, output) => ConvertMusicAsync(settings, output)),
                new GameAction("Fix memory dump byte swapping", false, false, (input, output) => FixMemoryDumpByteSwapping(settings)),
                new GameAction("Export Palettes", false, true, (input, output) => ExportPaletteImage(settings, output)),
            };
        }

        protected class ExportAnim {
            public Unity_ObjAnimation Anim;
            public byte AnimationSpeed;
            public Pointer Pointer;
            public R1_ImageDescriptor[] OverrideImageDescriptors;
        }

        public virtual uint EventCount => 0x1C4;

        /// <summary>
        /// Exports every sprite from the game
        /// </summary>
        /// <param name="baseGameSettings">The game settings</param>
        /// <param name="outputDir">The output directory</param>
        /// <param name="exportAnimFrames">True if animation frames should be exported, false if sprites should be exported</param>
        /// <returns>The task</returns>
        public virtual async UniTask ExportAllSpritesAsync(GameSettings baseGameSettings, string outputDir, bool exportAnimFrames)
        {
            var includeFinalSpritesForDemo = Settings.GameDirectories.ContainsKey(GameModeSelection.RaymanJaguar) && baseGameSettings.EngineVersion == EngineVersion.R1Jaguar_Demo && exportAnimFrames;

            Context mainRomContext = null;
            Pointer mainRomOffset = null;

            try
            {
                if (includeFinalSpritesForDemo)
                {
                    var mainRomSettings = new GameSettings(GameModeSelection.RaymanJaguar, Settings.GameDirectories[GameModeSelection.RaymanJaguar], 1, 1);

                    mainRomContext = new Context(mainRomSettings);

                    var mainManger = new R1Jaguar_Manager();

                    await mainManger.LoadFilesAsync(mainRomContext);

                    // Get the main Jaguar rom
                    mainRomOffset = mainRomContext.GetFile(mainManger.GetROMFilePath).StartPointer;
                }

                // Create the context
                using (var context = new Context(baseGameSettings))
                {
                    // Keep track of exported files
                    var exportedFiles = new List<string>();

                    // Load the game data
                    await LoadFilesAsync(context);

                    // Serialize the rom
                    var rom = FileFactory.Read<R1Jaguar_ROM>(GetROMFilePath, context);

                    // Get the level counts
                    var levels = GetNumLevels;

                    // Get the deserializer
                    var s = context.Deserializer;

                    // Get allfix sprite commands
                    var allfixCmds = rom.AllfixLoadCommands.Commands.Where(x => x.Type == R1Jaguar_LevelLoadCommand.LevelLoadCommandType.Sprites).ToArray();

                    // Export allfix
                    await ExportGroupAsync(allfixCmds, Enumerable.Repeat(rom.SpritePalette, allfixCmds.Length).ToArray(), "Allfix");

                    // Enumerate every world
                    foreach (var world in GetNumLevels)// GetLevels(baseGameSettings))
                                                       // Export world
                        await ExportWorldAsync(levels.FindItemIndex(x => x.Key == world.Key), world.Key.ToString());

                    // Export extra world
                    await ExportWorldAsync(6, "Extra");

                    // Helper method for exporting a world
                    async UniTask ExportWorldAsync(int worldIndex, string name)
                    {
                        // Get the level load commands
                        var lvlCmds = rom.LevelLoadCommands[worldIndex];

                        // Get palettes for the levels
                        var palettes = lvlCmds.
                            Select((x, i) => x?.Commands?.FirstOrDefault(c => c.Type == R1Jaguar_LevelLoadCommand.LevelLoadCommandType.Palette
                            || c.Type == R1Jaguar_LevelLoadCommand.LevelLoadCommandType.PaletteDemo)?.PalettePointer).
                            Select((x, i) => x == null ? rom.SpritePalette : s.DoAt<GBR655Color[]>(x, () => s.SerializeObjectArray<GBR655Color>(default, 256, name: $"SpritePalette[{i}]"))).
                            ToArray();

                        // Get the world and level sprite commands and palettes
                        var worldCmds = new List<R1Jaguar_LevelLoadCommand>();
                        var worldPal = new List<GBR655Color[]>();

                        if (worldIndex < 6)
                        {
                            // Add world data
                            foreach (var p in palettes)
                            {
                                var sprCommands = rom.WorldLoadCommands[worldIndex]?.Commands?.Where(x => x.Type == R1Jaguar_LevelLoadCommand.LevelLoadCommandType.Sprites) ?? new R1Jaguar_LevelLoadCommand[0];
                                worldCmds.AddRange(sprCommands);
                                worldPal.AddRange(Enumerable.Repeat(p, sprCommands.Count()));
                            }
                        }

                        // Enumerate every level
                        for (int lvl = 0; lvl < lvlCmds.Length; lvl++)
                        {
                            foreach (var c in lvlCmds[lvl]?.Commands?
                                .Where(x => x.Type == R1Jaguar_LevelLoadCommand.LevelLoadCommandType.Sprites)
                                .Where(x => worldCmds.All(y => y.ImageBufferPointer != x.ImageBufferPointer)) ?? new R1Jaguar_LevelLoadCommand[0])
                            {
                                worldCmds.Add(c);
                                worldPal.Add(palettes[lvl]);
                            }
                        }

                        // Export world
                        await ExportGroupAsync(worldCmds, worldPal, name);
                    }

                    // Helper method for exporting a collection of DES
                    async UniTask ExportGroupAsync(IReadOnlyList<R1Jaguar_LevelLoadCommand> cmds, IReadOnlyList<GBR655Color[]> palettes, string name)
                    {
                        // Enumerate every graphics
                        for (var desIndex = 0; desIndex < cmds.Count; desIndex++)
                        {
                            // Get values for current DES
                            var cmd = cmds[desIndex];
                            var pal = palettes[desIndex];

                            // Get the image buffer
                            byte[] imgBuffer = null;

                            // Get the event definition
                            var eventDefinitions = rom.EventDefinitions.Concat(rom.AdditionalEventDefinitions)
                                .Where(x => x.ImageBufferMemoryPointerPointer == cmd.ImageBufferMemoryPointerPointer).ToArray();
                            if (eventDefinitions.Length == 0)
                            {
                                Debug.LogWarning($"No EventDefinition found with ImageBufferMemoryPtrPtr == {cmd.ImageBufferMemoryPointerPointer:X8}!");
                                continue;
                            }

                            var imgBufferPointer = cmd.ImageBufferPointer;

                            // The demo does not include image buffers for certain animations, so we add them manually here from the final rom
                            if (includeFinalSpritesForDemo)
                            {
                                if (name == "Mountain")
                                {
                                    // Stoneman
                                    if (eventDefinitions.FirstOrDefault()?.Offset.AbsoluteOffset == 0x00919CC0)
                                        imgBufferPointer = mainRomOffset + 662779;

                                    // Spider
                                    if (eventDefinitions.FirstOrDefault()?.Offset.AbsoluteOffset == 0x0091A440)
                                        imgBufferPointer = mainRomOffset + 694186;

                                    // Stone dog
                                    if (eventDefinitions.FirstOrDefault()?.Offset.AbsoluteOffset == 0x00919D10)
                                        imgBufferPointer = mainRomOffset + 673800;

                                    // Mountain
                                    if (eventDefinitions.FirstOrDefault()?.Offset.AbsoluteOffset == 0x00919BA8)
                                        imgBufferPointer = mainRomOffset + 676759;
                                }
                            }

                            var eventDefIndex = 0;
                            try
                            {
                                // Set the deserializer
                                s = imgBufferPointer.Context.Deserializer;

                                s.DoAt(imgBufferPointer, () => s.DoEncoded(new RNCEncoder(), () => {
                                    imgBuffer = s.SerializeArray<byte>(default, s.CurrentLength, "ImageBuffer");
                                }));

                                // Set the deserializer
                                s = context.Deserializer;
                            }
                            catch (Exception ex)
                            {
                                Debug.LogWarning($"Failed to serialize image buffer at {cmd.ImageBufferMemoryPointerPointer:X8} with error {ex.Message}");
                                imgBuffer = new byte[0];
                                continue;
                            }

                            // Export every event DES
                            foreach (var ed in eventDefinitions)
                            {
                                var imageDescriptors = ed.ImageDescriptors ?? ed.ComplexData?.ImageDescriptors ?? new R1_ImageDescriptor[0];

                                if (exportAnimFrames)
                                {
                                    // Create template
                                    var animations = new List<ExportAnim>();
                                    HashSet<Pointer> complexDataSeen = new HashSet<Pointer>();

                                    var animSingle = ed.AnimationLayers != null ? new ExportAnim()
                                    {
                                        Anim = ed.ToCommonAnimation(),
                                        AnimationSpeed = 1,
                                        Pointer = ed.Offset
                                    } : null;
                                    if (animSingle != null) animations.Add(animSingle);
                                    var animNormal = ed.States?.Where(x => x.Animation?.Layers != null).Select(x => new ExportAnim()
                                    {
                                        Anim = x.Animation.ToCommonAnimation(ed),
                                        AnimationSpeed = (byte)(x.AnimationSpeed & 0b1111),
                                        Pointer = x.Animation.Offset
                                    });
                                    if (animNormal != null) animations.AddRange(animNormal);
                                    void AddComplexData(R1Jaguar_EventComplexData cd)
                                    {
                                        if (cd == null || complexDataSeen.Contains(cd.Offset)) return;
                                        complexDataSeen.Add(cd.Offset);
                                        var animComplex = cd.States?.Where(x => x.Layers != null).Select(x => new ExportAnim()
                                        {
                                            OverrideImageDescriptors = cd.ImageDescriptors,
                                            Anim = x.ToCommonAnimation(ed),
                                            AnimationSpeed = (byte)(x.UnkBytes[0] & 0b1111),
                                            Pointer = x.Offset
                                        });
                                        if (animComplex != null) animations.AddRange(animComplex);
                                        if (cd.Transitions != null)
                                        {
                                            foreach (R1Jaguar_EventComplexDataTransition t in cd.Transitions)
                                            {
                                                AddComplexData(t.ComplexData);
                                            }
                                        }
                                    }
                                    AddComplexData(ed.ComplexData);

                                    if (animations.Count == 0)
                                        continue;

                                    // Get every sprite
                                    var sprites = imageDescriptors.Select(x => GetSpriteTexture(x, pal, imgBuffer)).ToArray();

                                    var animIndex = 0;

                                    // Export every animation
                                    foreach (var anim in animations)
                                    {
                                        int flippingMethod = 0;
                                        if (((ed.UShort_12 & 5) == 5) || ed.StructType == 31)
                                        {
                                            flippingMethod = 1;
                                        }
                                        var animSprites = sprites;
                                        if (anim.OverrideImageDescriptors != null)
                                        {
                                            sprites = anim.OverrideImageDescriptors.Select(x => GetSpriteTexture(x, pal, imgBuffer)).ToArray();
                                        }
                                        var animKey = $"{anim.Pointer.StringAbsoluteOffset}-{pal.First().Offset.StringAbsoluteOffset}-{imageDescriptors.First().Offset.StringAbsoluteOffset}-{flippingMethod}";

                                        if (!anim.Anim.Frames.Any() || exportedFiles.Contains(animKey))
                                        {
                                            animIndex++;
                                            continue;
                                        }

                                        exportedFiles.Add(animKey);

                                        // Get the folder
                                        var animFolderPath = Path.Combine(outputDir, name, $"{desIndex}-{eventDefIndex}-{ed.Offset.StringAbsoluteOffset}", $"{animIndex}-{anim.AnimationSpeed}");

                                        int? frameWidth = null;
                                        int? frameHeight = null;

                                        var layersPerFrame = anim.Anim.Frames.First().SpriteLayers.Length;
                                        var frameCount = anim.Anim.Frames.Length;

                                        for (int dummyFrame = 0; dummyFrame < frameCount; dummyFrame++)
                                        {
                                            for (int dummyLayer = 0; dummyLayer < layersPerFrame; dummyLayer++)
                                            {
                                                var l = anim.Anim.Frames[dummyFrame].SpriteLayers[dummyLayer];

                                                if (l.ImageIndex < sprites.Length)
                                                {
                                                    var sprite = sprites[l.ImageIndex];

                                                    if (sprite != null)
                                                    {
                                                        var w = sprite.width + l.XPosition;
                                                        var h = sprite.height + l.YPosition;

                                                        if (frameWidth == null || frameWidth < w)
                                                            frameWidth = w;

                                                        if (frameHeight == null || frameHeight < h)
                                                            frameHeight = h;
                                                    }
                                                }
                                            }
                                        }

                                        // Create each animation frame
                                        for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
                                        {
                                            var tex = TextureHelpers.CreateTexture2D(frameWidth ?? 1, frameHeight ?? 1, clear: true);

                                            bool hasLayers = false;

                                            // Write each layer
                                            for (var layerIndex = 0; layerIndex < layersPerFrame; layerIndex++)
                                            {
                                                var animationLayer = anim.Anim.Frames[frameIndex].SpriteLayers[layerIndex];

                                                if (animationLayer.ImageIndex >= sprites.Length)
                                                    continue;

                                                // Get the sprite
                                                var sprite = sprites[animationLayer.ImageIndex];

                                                if (sprite == null)
                                                    continue;

                                                // Set every pixel
                                                for (int y = 0; y < sprite.height; y++)
                                                {
                                                    for (int x = 0; x < sprite.width; x++)
                                                    {
                                                        var c = sprite.GetPixel(x, sprite.height - y - 1);

                                                        var xPosition = (animationLayer.IsFlippedHorizontally ? (sprite.width - 1 - x) : x) + animationLayer.XPosition;
                                                        var yPosition = y + animationLayer.YPosition;

                                                        if (c.a != 0)
                                                            tex.SetPixel(xPosition, tex.height - 1 - yPosition, c);
                                                    }
                                                }

                                                hasLayers = true;
                                            }

                                            tex.Apply();

                                            if (!hasLayers)
                                                continue;

                                            // Save the file
                                            Util.ByteArrayToFile(Path.Combine(animFolderPath, $"{frameIndex}.png"), tex.EncodeToPNG());
                                        }

                                        animIndex++;
                                    }
                                }
                                else
                                {
                                    var imgIndex = 0;

                                    foreach (var d in imageDescriptors)
                                    {
                                        string filename = Path.Combine(outputDir, name, $"{cmd.ImageBufferPointer.StringAbsoluteOffset}_{pal.First().Offset.StringAbsoluteOffset}_{cmd.ImageBufferMemoryPointerPointer:X8}_{imageDescriptors.First().Offset.StringAbsoluteOffset} - {imgIndex}.png");

                                        if (!exportedFiles.Contains(filename))
                                        {
                                            // Get the texture
                                            var tex = GetSpriteTexture(d, pal, imgBuffer);

                                            // Export if not null
                                            if (tex != null)
                                            {
                                                Util.ByteArrayToFile(filename, tex.EncodeToPNG());
                                                exportedFiles.Add(filename);
                                            }
                                        }

                                        imgIndex++;
                                    }
                                }

                                eventDefIndex++;
                            }
                        }

                        // Unload textures
                        await Resources.UnloadUnusedAssets();
                    }
                }
            }
            finally
            {
                mainRomContext?.Dispose();
            }
        }

        /// <summary>
        /// Gets the texture for a sprite
        /// </summary>
        /// <param name="d">The image descriptor</param>
        /// <param name="pal">The palette</param>
        /// <param name="imgBuffer">The image buffer</param>
        /// <returns>The sprite texture</returns>
        public Texture2D GetSpriteTexture(R1_ImageDescriptor d, BaseColor[] pal, byte[] imgBuffer)
        {
            // Make sure the sprite is valid
            if (d.IsDummySprite())
                return null;

            bool is8Bit = BitHelpers.ExtractBits(d.Jag_Byte0E, 1, 4) != 0;

            // Make sure the index is not out of bounds
            if (d.ImageBufferOffset + ((d.Height * d.Width) / (is8Bit ? 1 : 2)) > imgBuffer.Length)
                return null;

            // Create a texture
            var tex = TextureHelpers.CreateTexture2D(d.Width, d.Height);

            var isFullyTransparent = true;

            // Set every pixel
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    var index = y * tex.width + x;

                    int palIndex;
                    if (is8Bit)
                    {
                        palIndex = imgBuffer[d.ImageBufferOffset + index];

                        if (palIndex != 0)
                            isFullyTransparent = false;

                        tex.SetPixel(x, tex.height - y - 1, palIndex == 0 ? new Color() : pal[palIndex].GetColor());
                    }
                    else
                    {
                        int indexInPal = BitHelpers.ExtractBits(d.Jag_Byte0A, 4, 1);
                        palIndex = imgBuffer[d.ImageBufferOffset + index / 2];
                        palIndex = BitHelpers.ExtractBits(palIndex, 4, index % 2 == 0 ? 4 : 0);

                        if (palIndex != 0)
                            isFullyTransparent = false;

                        tex.SetPixel(x, tex.height - y - 1, palIndex == 0 ? new Color() : pal[indexInPal * 16 + palIndex].GetColor());
                    }
                }
            }

            // Return null if fully transparent
            if (isFullyTransparent)
                return null;

            tex.Apply();

            return tex;
        }

        /// <summary>
        /// Extracts all vignette
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="outputPath">The path to extract to</param>
        /// <returns>The task</returns>
        public virtual async UniTask ExtractVignetteAsync(GameSettings settings, string outputPath)
        {
            // Create a context
            using (var context = new Context(settings))
            {
                // Get a deserializer
                var s = context.Deserializer;

                // Add the file
                var file = await LoadExtraFile(context, GetROMFilePath, GetROMBaseAddress);

                // Export every vignette
                foreach (var vig in GetVignette)
                {
                    s.DoAt(new Pointer(vig.Key, file), () =>
                    {
                        s.DoEncoded(new RNCEncoder(), () =>
                        {
                            var values = s.SerializeObjectArray<GBR655Color>(default, s.CurrentLength / 2);

                            var tex = TextureHelpers.CreateTexture2D(vig.Value, values.Length / vig.Value);

                            for (int y = 0; y < tex.height; y++)
                            {
                                for (int x = 0; x < tex.width; x++)
                                {
                                    tex.SetPixel(x, tex.height - y - 1, values[y * tex.width + x].GetColor());
                                }
                            }

                            tex.Apply();

                            Util.ByteArrayToFile(Path.Combine(outputPath, $"Vig_{vig.Key:X8}.png"), tex.EncodeToPNG());
                        });
                    });
                }
            }
        }

        /// <summary>
        /// Extracts all the compressed data from the rom
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <param name="outputPath">The path to extract to</param>
        /// <param name="as888">Indicates if the blocks should be converted to RGB-888</param>
        public async UniTask ExtractCompressedDataAsync(GameSettings settings, string outputPath, bool as888)
        {
            // Create a context
            using (var context = new Context(settings))
            {
                // Get a deserializer
                var s = context.Deserializer;

                // Add the file
                var file = await LoadExtraFile(context, GetROMFilePath, GetROMBaseAddress);

                s.DoAt(file.StartPointer, () =>
                {
                    // Enumerate every byte
                    while (s.CurrentPointer.FileOffset < file.Length - 4)
                    {
                        // Read the next 4 bytes and check if the header matches
                        var header = s.Serialize<uint>(default);

                        if (header == 0x524E4302)
                        {
                            // Go back four steps
                            s.Goto(s.CurrentPointer - 4);

                            // Get the current pointer
                            var p = s.CurrentPointer;

                            s.DoEncoded(new RNCEncoder(), () =>
                            {
                                if (as888)
                                {
                                    var values = s.SerializeObjectArray<GBR655Color>(default, s.CurrentLength / 2);

                                    var output = new byte[values.Length * 3];
                                        
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        var v = values[i];

                                        // Write RGB values
                                        output[i * 3 + 0] = (byte)(v.Red * 255f);
                                        output[i * 3 + 1] = (byte)(v.Green * 255f);
                                        output[i * 3 + 2] = (byte)(v.Blue * 255f);
                                    }

                                    Util.ByteArrayToFile(Path.Combine(outputPath, $"decompressedBlock_{p.FileOffset}_{p.FileOffset + 0x00800000:X8}"), output);
                                }
                                else
                                {
                                    Util.ByteArrayToFile(Path.Combine(outputPath, $"decompressedBlock_{p.FileOffset}_{p.FileOffset + 0x00800000:X8}"), s.SerializeArray<byte>(default, s.CurrentLength));
                                }
                            });
                        }
                        else
                        {
                            // Go back three steps
                            s.Goto(s.CurrentPointer - 3);
                        }
                    }
                });
            }
        }

        public async UniTask ConvertMusicAsync(GameSettings settings, string outputPath) {
            // Create a context
            using (var context = new Context(settings)) {
                // Get a deserializer
                var s = context.Deserializer;

                // Add the file
                var file = await LoadExtraFile(context, GetROMFilePath, GetROMBaseAddress);
                var pointerTable = PointerTables.JaguarR1_PointerTable(s.GameSettings.EngineVersion, file);
                s.DoAt(pointerTable[JaguarR1_Pointer.Music], () => {
                    // Read the music table
                    R1Jaguar_MusicDescriptor[] MusicTable = s.SerializeObjectArray<R1Jaguar_MusicDescriptor>(null, s.GameSettings.EngineVersion == EngineVersion.R1Jaguar ? 0x20 : 1, name: nameof(MusicTable));
                    // Immediately after this: pointer to sample buffer?

                    // For each entry
                    R1Jaguar_MidiWriter w = new R1Jaguar_MidiWriter();
                    for (int i = 0; i < MusicTable.Length; i++) {
                        w.Write(MusicTable[i],
                            Path.Combine(outputPath,
                            $"Track{i}_{MusicTable[i].MusicDataPointer.AbsoluteOffset:X8}.mid"));
                    }
                });
            }
        }

        public virtual async UniTask ExportPaletteImage(GameSettings settings, string outputPath)
        {
            using (var context = new Context(settings))
            {
                // Load the files
                await LoadFilesAsync(context);

                // Serialize the rom
                var rom = FileFactory.Read<R1Jaguar_ROM>(GetROMFilePath, context);

                // Get a deserializer
                var s = context.Deserializer;

                // Get every palette
                var pal = rom.LevelLoadCommands.
                    SelectMany(x => x).
                    Where(x => x?.Commands != null).
                    Select(x => x.Commands.First(y => y.Type == R1Jaguar_LevelLoadCommand.LevelLoadCommandType.Palette || y.Type == R1Jaguar_LevelLoadCommand.LevelLoadCommandType.PaletteDemo)).
                    Select(x => x.PalettePointer).
                    Distinct().
                    SelectMany(x => s.DoAt<GBR655Color[]>(x, () => s.SerializeObjectArray<GBR655Color>(default, 256, name: "SpritePalette"))).
                    ToArray();

                // Export
                PaletteHelpers.ExportPalette(Path.Combine(outputPath, $"{settings.GameModeSelection}.png"), pal, optionalWrap: 256);
            }
        }


        public void FixMemoryDumpByteSwapping(GameSettings settings) {
            // Create a context
            using (var context = new Context(settings)) {
                // Get a deserializer
                var s = context.Deserializer;
                string[] files = Directory.EnumerateFiles(context.BasePath, "*.jag", SearchOption.TopDirectoryOnly).ToArray();
                foreach (string filepath in files) {
                    // Add the file
                    string path = filepath.Substring(context.BasePath.Length);
                    var file = new LinearSerializedFile(context) {
                        filePath = path,
                        Endianness = BinaryFile.Endian.Little
                    };
                    context.AddFile(file);
                    ushort[] data = s.DoAt(file.StartPointer, () => s.SerializeArray<ushort>(null, s.CurrentLength / 2, name: nameof(data)));

                    using (MemoryStream ms = new MemoryStream()) {
                        Writer w = new Writer(ms, isLittleEndian: false);
                        foreach (ushort u in data)
                            w.Write(u);
                        ms.Position = 0;
                        Util.ByteArrayToFile(context.BasePath + path + ".fixed", ms.ToArray());
                    }
                }
            }
        }

        public Unity_Object_R1Jaguar CreateEventData(Context c, R1Jaguar_EventDefinition ed, List<Unity_ObjectManager_R1Jaguar.EventDefinition> eventDefinitions, bool loadTextures, Unity_ObjectManager_R1Jaguar objManager)
        {
            if (ed == null)
                return null;

            var rom = FileFactory.Read<R1Jaguar_ROM>(GetROMFilePath, c);

            var usedNames = new List<string>();

            // Helper method to get the name for a pointer
            string GetPointerName(Pointer p, int subEtat = 0, bool desEtaName = false)
            {
                string name;

                if (c.Settings.EngineVersion != EngineVersion.R1Jaguar_Proto)
                    name = null;
                // Need to do last here to get the correct event name for Rayman
                else if (desEtaName)
                    name = rom.References.LastOrDefault(x => x.DataPointer == p && !usedNames.Contains(x.String))
                        ?.String;
                // Need to save used names here to avoid getting the same name for animations
                else
                    name = rom.References.FirstOrDefault(x => x.DataPointer == p && !usedNames.Contains(x.String))
                               ?.String ?? $"{rom.References.FirstOrDefault(x => x.DataPointer == p)?.String} [{subEtat}]";

                if (name != null)
                    usedNames.Add(name);

                return name;
            }

            // Get the DES key (normally the offset of the event definition, but in the prototype we have strings we can use)
            var eventDefinitionPointer = ed.Offset;

            /* TODO: Process special event definitions.
             * - 0x001FB3C8[0x000023C8]: RAY POS
             * - 0x001FB760[0x00002760]: Mr Dark boss spawners
             * - 0x001F9CD0[0x00000CD0]: Gendoors. Spawns next event read by ReadEvent in Jaguar_R1_EventBlock
             */

            // Key for states
            bool usesComplexData = false;
            Pointer etatKeyOffset = null;

            if (ed.AnimationLayers != null)
                etatKeyOffset = ed.Offset;

            if (ed.States != null && ed.States.Length > 0)
            {
                etatKeyOffset = ed.States[0].Offset;
            }
            else if (ed.ComplexData != null)
            {
                usesComplexData = true;
                etatKeyOffset = ed.ComplexData.Transitions != null ? ed.ComplexData.Transitions[0].Offset : ed.ComplexData.Offset;
            }

            // Add if not found
            if (eventDefinitions.All(x => x.Pointer != eventDefinitionPointer))
            {
                Unity_ObjGraphics finalDesign = new Unity_ObjGraphics
                {
                    Sprites = new List<Sprite>(),
                    Animations = new List<Unity_ObjAnimation>()
                };

                // Get every sprite
                void AddImageDescriptors(R1_ImageDescriptor[] imgDesc, uint key)
                {
                    if (imgDesc == null) return;
                    foreach (R1_ImageDescriptor img in imgDesc)
                    {
                        // Get the texture for the sprite, or null if not loading textures
                        Texture2D tex = loadTextures && rom.ImageBuffers.ContainsKey(key) ? GetSpriteTexture(img, rom.SpritePalette, rom.ImageBuffers[key]) : null;

                        // Add it to the array
                        finalDesign.Sprites.Add(tex == null ? null : tex.CreateSprite());
                    }
                }

                if (c.Settings.EngineVersion == EngineVersion.R1Jaguar_Proto)
                {
                    var p = ed.ImageBufferMemoryPointerPointer >> 8;
                    if (rom.ImageBufferDescriptors.ContainsKey(p))
                        AddImageDescriptors(rom.ImageBufferDescriptors[p], p);
                }
                else
                {
                    if (ed.ImageDescriptors != null)
                        AddImageDescriptors(ed.ImageDescriptors, ed.ImageBufferMemoryPointerPointer);

                    if (ed.ComplexData != null)
                        AddImageDescriptors(ed.ComplexData?.ImageDescriptors, ed.ImageBufferMemoryPointerPointer);
                }

                // Add animations
                if (ed.AnimationLayers != null)
                {
                    finalDesign.Animations.Add(ed.ToCommonAnimation());
                }
                else if (ed.States != null)
                {
                    finalDesign.Animations.AddRange(ed.States.Where(x => x.Animation != null).Select(x => x.Animation.ToCommonAnimation(ed)));
                }
                else if (ed.ComplexData != null)
                {
                    if (ed.ComplexData.Transitions != null)
                    {
                        foreach (var transition in ed.ComplexData.Transitions)
                        {
                            if (transition.ComplexData?.States == null || (c.Settings.EngineVersion == EngineVersion.R1Jaguar_Proto && transition.ComplexData.ImageDescriptorsPointer != rom.ImageBufferDescriptors[ed.ImageBufferMemoryPointerPointer >> 8].First().Offset))
                                continue;
                            finalDesign.Animations.AddRange(transition.ComplexData.States.Where(x => x.Layers?.Length > 0).Select(x => x.ToCommonAnimation(ed)));
                        }
                    }
                    else
                    {
                        if (ed.ComplexData.States != null)
                        {
                            finalDesign.Animations.AddRange(ed.ComplexData.States.Where(x => x.Layers?.Length > 0).Select(x => x.ToCommonAnimation(ed)));
                        }
                    }
                }

                Unity_ObjectManager_R1Jaguar.State[][] finalStates;

                if (etatKeyOffset != null)
                {
                    if (!usesComplexData)
                    {
                        if (ed.AnimationLayers != null)
                        {
                            // Create a common state array
                            finalStates = new Unity_ObjectManager_R1Jaguar.State[][] 
                            {
                                new Unity_ObjectManager_R1Jaguar.State[] 
                                {
                                    new Unity_ObjectManager_R1Jaguar.State(
                                        animationIndex: 0, 
                                        animSpeed: 1, 
                                        //linkedStateIndex: 0, 
                                        //linkedComplexStateIndex: 0, 
                                        name: GetPointerName(ed.AnimationPointer - 4))
                                }
                            };
                        }
                        else
                        {
                            var validStates = ed.States.Where(x => x.Animation != null).ToArray();

                            // Create a common state array
                            finalStates = new Unity_ObjectManager_R1Jaguar.State[validStates.Length][];

                            // Add dummy states
                            for (byte s = 0; s < finalStates.Length; s++)
                            {
                                var stateLinkIndex = -1;
                                var fullStateIndex = ed.States.FindItemIndex(x => x == validStates[s]);

                                if (fullStateIndex + 1 < ed.States.Length && ed.States[fullStateIndex + 1].LinkedState != null)
                                    stateLinkIndex = validStates.FindItemIndex(x => x == ed.States[fullStateIndex + 1].LinkedState);

                                finalStates[s] = new Unity_ObjectManager_R1Jaguar.State[]
                                {
                                    new Unity_ObjectManager_R1Jaguar.State(
                                        animationIndex: s, 
                                        animSpeed: (byte)(validStates[s].AnimationSpeed & 0b1111),
                                        //linkedComplexStateIndex: 0,
                                        //linkedStateIndex: (byte)(stateLinkIndex == -1 ? s : stateLinkIndex), 
                                        name: GetPointerName(validStates[s].AnimationPointer, s))
                                };
                            }
                        }
                    }
                    else
                    {
                        List<R1Jaguar_EventComplexData> cds = new List<R1Jaguar_EventComplexData>();
                        List<Unity_ObjectManager_R1Jaguar.State[]> states = new List<Unity_ObjectManager_R1Jaguar.State[]>();
                        int curAnimIndex = 0;
                        void AddComplexData(R1Jaguar_EventComplexData cd)
                        {
                            if (cd == null || cds.Contains(cd)) return;

                            if ((c.Settings.EngineVersion == EngineVersion.R1Jaguar_Proto && cd.ImageDescriptorsPointer != rom.ImageBufferDescriptors[ed.ImageBufferMemoryPointerPointer >> 8].First().Offset))
                                return;

                            cds.Add(cd);
                            var validStates = cd.States.Where(x => x.Layers?.Length > 0).ToArray();
                            var substates = new Unity_ObjectManager_R1Jaguar.State[validStates.Length];
                            for (byte s = 0; s < substates.Length; s++)
                            {
                                var stateLinkIndex = -1;
                                if (validStates[s].LinkedStateIndex > 0 && validStates[s].LinkedStateIndex - 1 < cd.States.Length)
                                {
                                    var linkedState = cd.States[validStates[s].LinkedStateIndex - 1];
                                    stateLinkIndex = validStates.FindItemIndex(x => x == linkedState);
                                }

                                substates[s] = new Unity_ObjectManager_R1Jaguar.State(
                                    animationIndex: (byte)curAnimIndex++, 
                                    animSpeed: (byte)(validStates[s].UnkBytes[0] & 0b1111), 
                                    //linkedComplexStateIndex: (byte)states.Count, 
                                    //linkedStateIndex: (byte)(stateLinkIndex == -1 ? s : stateLinkIndex), 
                                    name: GetPointerName(validStates[s].AnimationPointer - 4, s));
                            }

                            states.Add(substates);

                            if (cd.Transitions == null) 
                                return;

                            foreach (R1Jaguar_EventComplexDataTransition t in cd.Transitions)
                                AddComplexData(t.ComplexData);
                        }
                        AddComplexData(ed.ComplexData);

                        finalStates = states.ToArray();
                    }
                }
                else
                {
                    finalStates = new Unity_ObjectManager_R1Jaguar.State[0][];
                }

                eventDefinitions.Add(new Unity_ObjectManager_R1Jaguar.EventDefinition(eventDefinitionPointer, finalDesign, finalStates, GetPointerName(eventDefinitionPointer, 0, true), ed));
            }

            // Get state index
            int stateIndex = 0;
            int substateIndex = 0;
            if (etatKeyOffset != null)
            {
                if (!usesComplexData)
                {
                    if (ed.States != null && ed.States.Length > 1)
                    {
                        var validStates = ed.States.Where(x => x.Animation != null).ToArray();

                        var ind = validStates.FindItemIndex(state => state.Offset == ed.CurrentStatePointer);

                        if (ind >= 0)
                            stateIndex = ind;
                    }
                }
                else
                {
                    stateIndex = 0;
                    substateIndex = 0;
                    if (ed.UInt_1C > 0 && ed.UInt_1C - 1 < ed.ComplexData?.States?.Length)
                    {
                        var validStates = ed.ComplexData.States.Where(x => x.Layers?.Length > 0).ToArray();
                        var linkedState = ed.ComplexData.States[ed.UInt_1C - 1];
                        var goodSubstate = validStates.FindItemIndex(x => x == linkedState);

                        if (goodSubstate != -1)
                            substateIndex = goodSubstate;
                    }
                }
            }

            var forceNoAnimation = new uint[]
            {
                // Pencils
                0x00003A98,
                0x00003AC0,
                0x00003AE8,
            };

            var forceFrame1 = new uint[]
            {
                // Pins
                0x000038E0,
                0x00003908,
                0x00003930,
                0x00003958,
                0x00003980,
                0x000039A8,
                0x000039D0,
                0x000039F8,
                0x00003A20,
                0x00003A48
            };
            var forceFrame2 = new uint[]
            {
                // Falling pencils
                0x00003E80,
                0x00003EA8,
                0x00003ED0,
                0x00003EF8,
                0x00003F20,
                0x00003F48,
                0x00003F70,
                0x00003F98,
            };

            // Add the event
            return new Unity_Object_R1Jaguar(objManager, eventDefinitionPointer)
            {
                RuntimeComplexStateIndex = (byte)stateIndex,
                ComplexStateIndex = (byte)stateIndex,
                RuntimeStateIndex = (byte)substateIndex,
                StateIndex = (byte)substateIndex,
                ForceNoAnimation = forceNoAnimation.Contains(ed.Offset.FileOffset) ||
                                   forceFrame1.Contains(ed.Offset.FileOffset) ||
                                   forceFrame2.Contains(ed.Offset.FileOffset),
                ForceFrame = forceFrame1.Contains(ed.Offset.FileOffset) ? (byte?)ed.Byte_25 : forceFrame2.Contains(ed.Offset.FileOffset) ? (byte?)ed.Byte_26 : null
            };
        }
        protected enum SpecialEventType {
            RayPos,
            Gendoor,
            Piranha,
            Piranha2,
            ScrollFast,
            ScrollSlow,
            RayOnBzzit,
            BzzitDemo,

            RaymanVisual,
            GendoorVisual,
            PiranhaVisual,
            ScrollVisual,
            RayOnBzzitVisual,
            BzzitDemoVisual,
        }
        protected virtual Dictionary<SpecialEventType, Pointer> GetSpecialEventPointers(Context context) {
            // Read the rom
            var rom = FileFactory.Read<R1Jaguar_ROM>(GetROMFilePath, context);
            Pointer baseOff = rom.EventDefinitions[0].Offset;
            return new Dictionary<SpecialEventType, Pointer>() {
                [SpecialEventType.RayPos] = baseOff + 0x000023C8,
                [SpecialEventType.Gendoor] = baseOff + 0xCD0,
                [SpecialEventType.Piranha] = baseOff + 0x000012C0,
                [SpecialEventType.Piranha2] = null,
                [SpecialEventType.ScrollFast] = baseOff + 0x00001450,
                [SpecialEventType.ScrollSlow] = baseOff + 0x00001478,
                [SpecialEventType.RayOnBzzit] = baseOff + 0x00002760,
                [SpecialEventType.BzzitDemo] = null,

                [SpecialEventType.RaymanVisual] = baseOff,
                [SpecialEventType.GendoorVisual] = baseOff + 0x00000A00,
                [SpecialEventType.PiranhaVisual] = baseOff + 0x000012E8,
                [SpecialEventType.ScrollVisual] = baseOff + 0x14A0,
                [SpecialEventType.RayOnBzzitVisual] = baseOff + 0x000000F0,
                [SpecialEventType.BzzitDemoVisual] = null,
            };
        }
        public virtual uint[] AdditionalEventDefinitionPointers => new uint[] {
            0x00BDBFDC,
            0x00B6018C,

            0x00B617EE,
            0x00B61816,
            0x00B6183E,
            0x00B61866,
            0x00B6188E,

            0x00B5DF54,
            0x00B5DF7C,

            0x00BF8B90,
            0x00BF8CA8,
            0x00BF8D20,
            0x00BF8E38,
            0x00BF8EB8,
            0x00BF8F9C,
            0x00BF9094,
            0x00BF90BC,
            0x00BF90FC,
            0x00BF9124,
            0x00BF9164,

        };

        /// <summary>
        /// Loads the specified level for the editor
        /// </summary>
        /// <param name="context">The serialization context</param>
        /// <param name="loadTextures">Indicates if textures should be loaded</param>
        /// <returns>The level</returns>
        public virtual async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures)
        {
            Controller.DetailedState = $"Loading data";
            await Controller.WaitIfNecessary();

            // Read the rom
            var rom = FileFactory.Read<R1Jaguar_ROM>(GetROMFilePath, context);

            // Get the map
            var map = rom.MapData;

            var maps = new Unity_Map[]
            {
                new Unity_Map()
                {
                    Type = Unity_Map.MapType.Graphics | Unity_Map.MapType.Collision,

                    // Set the dimensions
                    Width = map.Width,
                    Height = map.Height,

                    // Create the tile arrays
                    TileSet = new Unity_TileSet[1],
                    MapTiles = map.Tiles.Select(x => new Unity_Tile(x)).ToArray(),
                }
            };

            Controller.DetailedState = $"Loading tile set";
            await Controller.WaitIfNecessary();

            // Load tile set and treat black as transparent
            maps[0].TileSet[0] = new Unity_TileSet(rom.TileData.Select(x => x.Blue == 0 && x.Red == 0 && x.Green == 0 ? BaseColor.clear : x).ToArray(), 1, Settings.CellSize);

            var eventDefinitions = new List<Unity_ObjectManager_R1Jaguar.EventDefinition>();
            var objManager = new Unity_ObjectManager_R1Jaguar(context, eventDefinitions);

            var eventIndex = 0;

            // Set to true to change the event state to display them correctly, or false to use the original states
            var correctEventStates = context.Settings.EngineVersion != EngineVersion.R1Jaguar_Proto;

            Controller.DetailedState = $"Loading events & states";
            await Controller.WaitIfNecessary();

            // Load events
            Dictionary<int, Unity_Object> uniqueEvents = new Dictionary<int, Unity_Object>();

            // Get all event definitions
            var eventDefs = rom.EventDefinitions?.Concat(rom.AdditionalEventDefinitions ?? new R1Jaguar_EventDefinition[0]).ToArray() ?? new R1Jaguar_EventDefinition[0];

            // Helper method for loading an event definition
            Unity_Object_R1Jaguar loadEventDef(R1Jaguar_EventDefinition def) => CreateEventData(context, def, eventDefinitions, loadTextures, objManager);

            // Load special events so we can display them
            var specialPointers = GetSpecialEventPointers(context);

            var rayPos = correctEventStates ? loadEventDef(eventDefs.FirstOrDefault(x => x.Offset == specialPointers[SpecialEventType.RaymanVisual])) : null; // Rayman position
            var gendoor = correctEventStates ? loadEventDef(eventDefs.FirstOrDefault(x => x.Offset == specialPointers[SpecialEventType.GendoorVisual])) : null; // Gendoor
            var piranha = correctEventStates ? loadEventDef(eventDefs.FirstOrDefault(x => x.Offset == specialPointers[SpecialEventType.PiranhaVisual])) : null; // Piranha
            var scroll = correctEventStates ? loadEventDef(eventDefs.FirstOrDefault(x => x.Offset == specialPointers[SpecialEventType.ScrollVisual])) : null; // Scroll
            var rayBzzit = (correctEventStates && context.Settings.R1_World == R1_World.Jungle && context.Settings.Level == 7) ? loadEventDef(eventDefs.FirstOrDefault(x => x.Offset == specialPointers[SpecialEventType.RayOnBzzitVisual])) : null; // Rayman on Bzzit
            var bzzitDemo = correctEventStates ? CreateEventData(context, eventDefs.FirstOrDefault(x => x.Offset == specialPointers[SpecialEventType.BzzitDemoVisual]), eventDefinitions, loadTextures, objManager) : null; // Bzzit (demo)

            var eventDataList = new List<Unity_Object>();

            for (var i = 0; i < rom.EventData.EventData.Length; i++)
            {
                // Get the map base position, based on the event map
                var mapPos = rom.EventData.MapEvents.EventIndexMap.FindItemIndex(z => z == i + 1);

                // Get the x and y positions
                var mapY = (uint)Math.Floor(mapPos / (double)(rom.EventData.MapEvents.Width));
                var mapX = (uint)(mapPos - (mapY * rom.EventData.MapEvents.Width));

                // Calculate the actual position on the map
                mapX *= 4 * (uint)Settings.CellSize;
                mapY *= 4 * (uint)Settings.CellSize;

                bool IsGendoor(int index)
                {
                    switch (context.Settings.GameModeSelection)
                    {
                        default:
                            return rom.EventData.EventData[i][index].EventDefinitionPointer.AbsoluteOffset == 0x001F9CD0;
                    }
                }

                // Add every event on this tile
                int? linkBackIndex = null;
                for (int j = 0; j < rom.EventData.EventData[i].Length; j++)
                {
                    var e = rom.EventData.EventData[i][j];

                    if (uniqueEvents.ContainsKey(e.EventIndex))
                    {

                        if (uniqueEvents[e.EventIndex].XPosition != (uint)(mapX + e.OffsetX) || uniqueEvents[e.EventIndex].YPosition != (uint)(mapY + e.OffsetY))
                            Debug.LogWarning($"An event with an existing index (index {e.EventIndex} at EventData[{i}][{j}]) which was removed has a different map position");

                        continue; // Duplicate
                    }

                    var ed = e.EventDefinition;

                    /* TODO: Process special event definitions.
                     * - 0x001FB3C8[0x000023C8]: RAY POS
                     * - 0x001FB760[0x00002760]: Mr Dark boss spawners
                     * - 0x001F9CD0[0x00000CD0]: Gendoors. Spawns next event read by ReadEvent in Jaguar_R1_EventBlock
                     */

                    var linkIndex = eventIndex;
                    if (linkBackIndex.HasValue)
                    {
                        linkIndex++;
                        if (j == rom.EventData.EventData[i].Length - 1 || IsGendoor(j + 1) || rom.EventData.EventData[i][j + 1].Unk_00 != 2)
                        {
                            linkIndex = linkBackIndex.Value;
                            linkBackIndex = null;
                        }

                    }
                    else if (e.Unk_00 == 2)
                    {
                        // Duplicate
                        continue;
                    }
                    if (IsGendoor(j))
                    {
                        linkBackIndex = eventIndex;
                        linkIndex++;
                    }
                    /*if (ed.CodePointer?.FileOffset == 0x00101E32) {
                        var indEd = Array.IndexOf(rom.EventDefinitions,ed);
                        ed = rom.EventDefinitions[indEd + e.Unk_0C];
                    }*/
                    /*if (ed.CodePointer?.FileOffset == 0x00101E32) {
						var indEd = Array.IndexOf(rom.EventDefinitions, ed);
						ed = rom.EventDefinitions[indEd + 2];
					}*/
                    // Switch
                    /*if (ed.CodePointer?.AbsoluteOffset == 0x00B9C67C) {
						//var indEd = Array.IndexOf(rom.EventDefinitions, ed);
						ed = rom.EventDefinitions[388];
						predeterminedState = e.EventDefinition.UnkBytes[5];
					}*/
                    // Add the event
                    var eventData = loadEventDef(ed);
                    uniqueEvents[e.EventIndex] = eventData;
                    eventData.LinkIndex = linkIndex;
                    eventData.XPosition = (short)(mapX + e.OffsetX);
                    eventData.YPosition = (short)(mapY + e.OffsetY);
                    eventData.Instance = e;

                    // Hack change the DES and ETA if special event so it displays correctly
                    if (correctEventStates)
                    {
                        if (ed.Offset == specialPointers[SpecialEventType.RayPos]) // Rayman position
                        {
                            eventData.EventDefinitionIndex = rayPos.EventDefinitionIndex;
                            eventData.StateIndex = eventData.RuntimeStateIndex = 7;
                        }
                        else if (ed.Offset == specialPointers[SpecialEventType.Gendoor]) // Gendoor
                        {
                            eventData.EventDefinitionIndex = gendoor.EventDefinitionIndex;
                            eventData.StateIndex = eventData.RuntimeStateIndex = 2;
                        }
                        else if (ed.Offset == specialPointers[SpecialEventType.Piranha] || ed.Offset == specialPointers[SpecialEventType.Piranha2]) // Piranha
                        {
                            eventData.EventDefinitionIndex = piranha.EventDefinitionIndex;

                            if (context.Settings.EngineVersion == EngineVersion.R1Jaguar_Demo)
                                eventData.ComplexStateIndex = eventData.RuntimeComplexStateIndex = 1;
                        }
                        else if ((ed.Offset == specialPointers[SpecialEventType.ScrollFast] || ed.Offset == specialPointers[SpecialEventType.ScrollSlow]) && !Settings.ScreenshotEnumeration) // Scroll fast/slow
                        {
                            eventData.EventDefinitionIndex = scroll.EventDefinitionIndex;

                            if (context.Settings.EngineVersion == EngineVersion.R1Jaguar_Demo)
                                eventData.ComplexStateIndex = eventData.RuntimeComplexStateIndex = 6;
                            else
                                eventData.ComplexStateIndex = eventData.RuntimeComplexStateIndex = 2;
                        }
                        else if (ed.Offset == specialPointers[SpecialEventType.RayOnBzzit] && context.Settings.R1_World == R1_World.Jungle && context.Settings.Level == 7) // Rayman on Bzzit
                        {
                            if (rayBzzit != null)
                                eventData.EventDefinitionIndex = rayBzzit.EventDefinitionIndex;
                        }
                        else if (ed.Offset == specialPointers[SpecialEventType.BzzitDemo]) // Bzzit (demo)
                        {
                            if (bzzitDemo != null)
                            {
                                eventData.EventDefinitionIndex = bzzitDemo.EventDefinitionIndex;

                                eventData.StateIndex = eventData.RuntimeStateIndex = 0;
                            }
                        }
                    }

                    eventDataList.Add(uniqueEvents[e.EventIndex]);

                    eventIndex++;
                }
            }

            // Check if all events have been loaded
            foreach (var t in rom.EventData.EventData)
            {
                foreach (R1Jaguar_EventInstance inst in t)
                {
                    if (!uniqueEvents.ContainsKey(inst.EventIndex))
                        Debug.LogWarning($"Event with index {inst.EventIndex} wasn't loaded!");
                }
            }

            Unity_Object rayman = null;

            if (context.Settings.EngineVersion == EngineVersion.R1Jaguar_Proto)
            {
                var ray = eventDefs.FirstOrDefault(e => e.Offset == rom.GetProtoDataPointer(R1Jaguar_Proto_References.MS_rayman));
                if (ray != null)
                {
                    var eventData = loadEventDef(ray);
                    //uniqueEvents[e.EventIndex] = eventData;
                    eventData.XPosition = (short)rom.GetProtoDataReference(R1Jaguar_Proto_References.ray_center_x).DataValue;
                    eventData.YPosition = (short)rom.GetProtoDataReference(R1Jaguar_Proto_References.ray_center_y).DataValue;
                    eventData.StateIndex = eventData.RuntimeStateIndex = 7;
                    rayman = eventData;
                }

                // Load Mr Stone so the event def can be selected
                var bb1 = eventDefs.FirstOrDefault(e => e.Offset == rom.GetProtoDataPointer(R1Jaguar_Proto_References.MS_bb1));
                if (bb1 != null)
                    loadEventDef(bb1);

                /*foreach (var ed in eventDefs) {// Add the event
                    var eventData = CreateEventData(context, ed, eventDesigns, eventETA, eventETANames, loadTextures);
                    //uniqueEvents[e.EventIndex] = eventData;
                    eventData.Data.XPosition = xPos;
                    eventData.Data.YPosition = 5;
                    xPos += 5;
                    commonLev.EventData.Add(eventData);
                }*/
            }

            // Use this to load every single event

            /*commonLev.EventData.Clear();
            var ind = 0;
            foreach (var def in eventDefs)
            {
                try
                {
                    var eventData = CreateEventData(context, def, eventDesigns, eventETA, eventETANames, loadTextures);
                    eventData.LinkIndex = ind;
                    eventData.Data.XPosition = (short)(ind * 20);
                    eventData.DebugText = $"EventDefinitionPointer: {def.Offset}{Environment.NewLine}";
                    commonLev.EventData.Add(eventData);
                    ind++;
                }
                catch (Exception ex)
                {
                    // Some will crash cause they're from other worlds, just ignore for now...
                }
            }*/

            // Create background vignette if there is one
            Texture2D bg = null;
            
            if (rom.Background != null)
            {
                var width = context.Settings.EngineVersion == EngineVersion.R1Jaguar_Proto ? 192 : GetVignette.First(x => x.Key == rom.BackgroundPointer.AbsoluteOffset).Value;
                bg = TextureHelpers.CreateTexture2D(width, rom.Background.Length / width);

                for (int y = 0; y < bg.height; y++)
                {
                    for (int x = 0; x < bg.width; x++)
                    {
                        bg.SetPixel(x, bg.height - y - 1, rom.Background[y * bg.width + x].GetColor());
                    }
                }

                bg.Apply();
            }

            // Convert levelData to common level format
            return new Unity_Level(
                maps: maps, 
                objManager: objManager, 
                eventData: eventDataList, 
                rayman: rayman,
                getCollisionTypeNameFunc: x => ((R1Jaguar_TileCollisionType)x).ToString(),
                getCollisionTypeGraphicFunc: x => ((R1Jaguar_TileCollisionType)x).GetCollisionTypeGraphic(), 
                background: bg);
        }

        public UniTask SaveLevelAsync(Context context, Unity_Level level) => throw new NotImplementedException();

        /// <summary>
        /// Preloads all the necessary files into the context
        /// </summary>
        /// <param name="context">The serialization context</param>
        public async UniTask LoadFilesAsync(Context context) => await LoadExtraFile(context, GetROMFilePath, GetROMBaseAddress);

        public virtual async UniTask<MemoryMappedFile> LoadExtraFile(Context context, string path, uint baseAddress)
        {
            return await context.AddMemoryMappedFile(path, baseAddress, BinaryFile.Endian.Big);
        }

        #endregion
    }
}