﻿using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace R1Engine
{
    public class GBA_BatmanVengeance_Manager : GBA_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 37)
        };

        public override int[] MenuLevels => new int[0];
        public override int DLCLevelCount => 0;
        public override int[] AdditionalSprites4bpp => new int[0];
        public override int[] AdditionalSprites8bpp => new int[0];

        public override UniTask ExtractVignetteAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();


        public override Unity_ObjGraphics GetCommonDesign(GBA_ActorModel model, GBA_Data data) => GetCommonDesign(model?.Puppet_BatmanVengeance, data);
        public Unity_ObjGraphics GetCommonDesign(GBA_BatmanVengeance_Puppet puppet, GBA_Data data) {
            // Create the design
            var des = new Unity_ObjGraphics {
                Sprites = new List<Sprite>(),
                Animations = new List<Unity_ObjAnimation>(),
            };

            if (puppet == null) 
                return des;

            var tileSet = puppet.TileSet;
            var pal = Util.ConvertGBAPalette((RGBA5551Color[])GetSpritePalette(puppet, data).Palette);
            const int tileWidth = 8;
            const int tileSize = (tileWidth * tileWidth) / 2;
            var numPalettes = pal.Length / 16;

            // Add sprites for each palette
            if (tileSet.Is8Bit)
            {
                var tileSetTex = Util.ToTileSetTexture(tileSet.TileSet, pal, Util.TileEncoding.Linear_8bpp, CellSize, false);

                // Extract every sprite
                for (int y = 0; y < tileSetTex.height; y += CellSize)
                {
                    for (int x = 0; x < tileSetTex.width; x += CellSize)
                    {
                        des.Sprites.Add(tileSetTex.CreateSprite(rect: new Rect(x, y, CellSize, CellSize)));
                    }
                }
            }
            else
            {
                for (int palIndex = 0; palIndex < numPalettes; palIndex++)
                {
                    for (int i = 0; i < tileSet.TileSetLength; i++)
                    {
                        var tex = TextureHelpers.CreateTexture2D(CellSize, CellSize);

                        for (int y = 0; y < tileWidth; y++)
                        {
                            for (int x = 0; x < tileWidth; x++)
                            {
                                int index = (i * tileSize) + ((y * tileWidth + x) / 2);

                                var b = tileSet.TileSet[index];
                                var v = BitHelpers.ExtractBits(b, 4, x % 2 == 0 ? 0 : 4);

                                Color c = pal[palIndex * 16 + v];

                                if (v != 0)
                                    c = new Color(c.r, c.g, c.b, 1f);

                                tex.SetPixel(x, (tileWidth - 1 - y), c);
                            }
                        }

                        tex.Apply();
                        des.Sprites.Add(tex.CreateSprite());
                    }
                }
            }

            Unity_ObjAnimationPart[] GetPartsForLayer(GBA_BatmanVengeance_Puppet s, GBA_BatmanVengeance_Animation a, int frame, GBA_BatmanVengeance_AnimationChannel l) {
                /*if (l.TransformMode == GBA_AnimationLayer.AffineObjectMode.Hide
                    || l.RenderMode == GBA_AnimationLayer.GfxMode.Window
                    || l.RenderMode == GBA_AnimationLayer.GfxMode.Regular
                    || l.Mosaic) return new Unity_ObjAnimationPart[0];
                if (l.Color == GBA_AnimationLayer.ColorMode.Color8bpp) {
                    Debug.LogWarning("Animation Layer @ " + l.Offset + " has 8bpp color mode, which is currently not supported.");
                    return new Unity_ObjAnimationPart[0];
                }*/
                Unity_ObjAnimationPart[] parts = new Unity_ObjAnimationPart[l.XSize * l.YSize];
                if (l.ImageIndex > puppet.TileSet.TileSetLength) {
                    Controller.print("Image index too high: " + puppet.Offset + " - " + l.Offset + $"Index: {l.ImageIndex} - Max: {puppet.TileSet.TileSetLength - 1}");
                }
                if (l.PaletteIndex > pal.Length / 16) {
                    Controller.print("Palette index too high: " + puppet.Offset + " - " + l.Offset + " - " + l.PaletteIndex + " - " + (pal.Length / 16));
                }
                float rot = 0;// l.GetRotation(a, s, frame);
                Vector2? scl = null;// l.GetScale(a, s, frame);
                for (int y = 0; y < l.YSize; y++) {
                    for (int x = 0; x < l.XSize; x++) {
                        parts[y * l.XSize + x] = new Unity_ObjAnimationPart {
                            ImageIndex = tileSet.TileSetLength * (tileSet.Is8Bit ? 0 : l.PaletteIndex) + (l.ImageIndex + y * l.XSize + x),
                            IsFlippedHorizontally = l.IsFlippedHorizontally,
                            IsFlippedVertically = l.IsFlippedVertically,
                            XPosition = (l.XPosition + (l.IsFlippedHorizontally ? (l.XSize - 1 - x) : x) * CellSize),
                            YPosition = (l.YPosition + (l.IsFlippedVertically ? (l.YSize - 1 - y) : y) * CellSize),
                            Rotation = rot,
                            Scale = scl,
                            TransformOriginX = (l.XPosition + l.XSize * CellSize / 2f),
                            TransformOriginY = (l.YPosition + l.YSize * CellSize / 2f)
                        };
                    }
                }
                return parts;
            }

            // Add first animation for now
            foreach (var a in puppet.Animations) {
                var unityAnim = new Unity_ObjAnimation();
                var frames = new List<Unity_ObjAnimationFrame>();
                for (int i = 0; i < a.FrameCount; i++) {
                    frames.Add(new Unity_ObjAnimationFrame(a.Frames[i].Layers/*.OrderByDescending(l => l.Priority)*/.SelectMany(l => GetPartsForLayer(puppet, a, i, l)).Reverse().ToArray()));
                }
                unityAnim.Frames = frames.ToArray();
                unityAnim.AnimSpeed = 1;
                des.Animations.Add(unityAnim);
            }

            return des;
        }

        protected virtual GBA_SpritePalette GetSpritePalette(GBA_BatmanVengeance_Puppet puppet, GBA_Data data) => puppet.Palette;
    }
}