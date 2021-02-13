﻿using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace R1Engine
{
    public class MouseOverUI : MonoBehaviour {
        public Text textGraphic, textCollision;
        public GameObject panel;

        void Update() 
        {
            // Make sure the editor has finished loading
            if (Controller.LoadState != Controller.State.Finished) 
                return;

            // Set position to mouse position
            transform.position = Input.mousePosition;

            var selector = Controller.obj.levelController.editor.objectHighlight;
            var cam = Controller.obj.levelController.editor.cam;
            bool mouselook = cam.MouseLookEnabled || cam.MouseLookRMBEnabled;
            if (mouselook) {
                if(panel.activeSelf) 
                    panel.SetActive(false);
            } else {
                if (!panel.activeSelf) 
                    panel.SetActive(true);
            }

            var e = selector?.highlightedObject;
            var t = selector?.highlightedTile;
            var c = selector?.highlightedCollision;
            var c3d = selector?.highlightedCollision3D;

            // Mouse over object
            if (e != null) {
                textGraphic.enabled = true;

                textCollision.text = $"{e.ObjData.PrimaryName}{(string.IsNullOrEmpty(e.ObjData.SecondaryName) ? "" : $" | {e.ObjData.SecondaryName}")}";
                if (LevelEditorData.Level.IsometricData != null && e.ObjData is Unity_Object_3D) {
                    textGraphic.text = $"Pos: ({((Unity_Object_3D)e.ObjData).Position}){Environment.NewLine}" +
                                       $"Pivot: ({e.ObjData.Pivot.x}, {e.ObjData.Pivot.y})";
                } else {
                    textGraphic.text = $"Pos: ({e.ObjData.XPosition}, {e.ObjData.YPosition}){Environment.NewLine}" +
                                       $"Pivot: ({e.ObjData.Pivot.x}, {e.ObjData.Pivot.y})";
                }

                if (e.ObjData is Unity_Object_R1 r1Data) {

                    var flags = r1Data.ObjManager?.EventFlags?.ElementAtOrDefault((ushort) r1Data.EventData.Type);

                    if (flags.HasValue) {
                        textGraphic.text += $"{Environment.NewLine}Flags: " + flags.Value.ToString();
                    }
                }

                // Set debug text
                Controller.obj.tempDebugText.text = Settings.ShowDebugInfo 
                    ? $"{e.ObjData.DebugText}{Environment.NewLine}" +
                      $"{e.ObjData.CurrentAnimation?.Frames?.ElementAtOrDefault(e.ObjData.AnimationFrame)?.DebugInfo}{Environment.NewLine}" +
                      $"{nameof(e.ObjData.AnimationIndex)}: {e.ObjData.AnimationIndex}{Environment.NewLine}" +
                      $"{nameof(e.ObjData.AnimationFrame)}: {e.ObjData.AnimationFrame}{Environment.NewLine}" +
                      $"{nameof(e.ObjData.AnimSpeed)}: {e.ObjData.AnimSpeed}{Environment.NewLine}" +
                      $"Frames: {e.ObjData.CurrentAnimation?.Frames?.GetLength(0)}{Environment.NewLine}" +
                      $"{Environment.NewLine}" +
                      $"{nameof(e.ObjData.IsAlways)}: {e.ObjData.IsAlways}{Environment.NewLine}" +
                      $"{nameof(e.ObjData.IsEditor)}: {e.ObjData.IsEditor}{Environment.NewLine}" +
                      $"{nameof(e.ObjData.IsActive)}: {e.ObjData.IsActive}{Environment.NewLine}" +
                      $"{nameof(e.ObjData.IsDisabled)}: {e.ObjData.IsDisabled}{Environment.NewLine}" +
                      $"{nameof(e.ObjData.IsVisible)}: {e.ObjData.IsVisible}{Environment.NewLine}" +
                      $"{nameof(e.ObjData.MapLayer)}: {e.ObjData.MapLayer}{Environment.NewLine}" +
                      $"{nameof(e.ObjData.Scale)}: {e.ObjData.Scale}{Environment.NewLine}" +
                      $"{nameof(e.ObjData.FlipHorizontally)}: {e.ObjData.FlipHorizontally}{Environment.NewLine}" +
                      $"{nameof(e.ObjData.FlipVertically)}: {e.ObjData.FlipVertically}{Environment.NewLine}" +
                      $"{Environment.NewLine}" +
                      $"{nameof(e.Index)}: {e.Index}{Environment.NewLine}" +
                      $"{nameof(e.Layer)}: {e.Layer}{Environment.NewLine}" +
                      $"{nameof(e.ObjData.EditorLinkGroup)}: {e.ObjData.EditorLinkGroup}{Environment.NewLine}" 
                    : String.Empty;
            }
            // Else Mouse over type
            else {
                Controller.obj.tempDebugText.text = String.Empty;
                if (cam.FreeCameraMode) {
                    textCollision.text = "";
                    textGraphic.enabled = false;
                } else {
                    textGraphic.enabled = true;
                    if (t != null && c != null) {
                        //Debug.Log("Tile here x:" + t.XPosition + " y:" + t.YPosition + " col:" + t.CollisionType);
                        textCollision.text = $"Collision: {String.Join(", ", c.Select(x => $"{LevelEditorData.Level.GetCollisionTypeNameFunc(x?.Data?.CollisionType ?? 0)}({x?.Data?.CollisionType}){(x?.Data?.UsesCollisionShape == true ? $" - Shape: {x.Data.GBAVV_CollisionShape}({(byte?)x.Data.GBAVV_CollisionShape})" : null)}"))}";
                        textGraphic.text = $"Graphic tile: {String.Join(", ", t.Select(x => $"({x?.Data?.TileMapX}, {x?.Data?.TileMapY})"))}";

                        // Set debug text
                        Controller.obj.tempDebugText.text = Settings.ShowDebugInfo
                            ? $"{String.Join(Environment.NewLine, t.Select(x => x?.DebugText))}{Environment.NewLine}" +
                              $"PC_TransparencyMode: {String.Join(", ", t.Select(x => x?.Data?.PC_TransparencyMode))}{Environment.NewLine}" +
                              $"PC_Unk1: {String.Join(", ", t.Select(x => x?.Data?.PC_Unk1))}{Environment.NewLine}" +
                              $"PC_Unk2: {String.Join(", ", t.Select(x => x?.Data?.PC_Unk2))}{Environment.NewLine}" +
                              $"HorizontalFlip: {String.Join(", ", t.Select(x => x?.Data?.HorizontalFlip))}{Environment.NewLine}" +
                              $"VerticalFlip: {String.Join(", ", t.Select(x => x?.Data?.VerticalFlip))}{Environment.NewLine}" +
                              $"PaletteIndex: {String.Join(", ", t.Select(x => x?.Data?.PaletteIndex))}{Environment.NewLine}"
                            : String.Empty;
                    }
                }
                if (LevelEditorData.Level.IsometricData != null && c3d != null) {
                    textCollision.text = $"Collision: {c3d.Type} | Add: {c3d.AddType} | Shape: {c3d.Shape}";

                    textGraphic.enabled = true;
                    // Set debug text
                    Controller.obj.tempDebugText.text = Settings.ShowDebugInfo
                        ? c3d.DebugText
                        : String.Empty;
                }
            }
        }
    }
}