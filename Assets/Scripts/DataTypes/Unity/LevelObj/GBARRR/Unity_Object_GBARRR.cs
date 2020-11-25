﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBARRR : Unity_Object
    {
        public Unity_Object_GBARRR(GBARRR_Actor actor, Unity_ObjectManager_GBARRR objManager)
        {
            Actor = actor;
            ObjManager = objManager;

            // P_FunctionPointer is filled in later, can't do this check here
            /*if (actor.ObjectType == GBARRR_ActorType.Special && Actor.P_GraphicsOffset == 0 && !Enum.IsDefined(typeof(SpecialType_Function), (SpecialType_Function)actor.P_FunctionPointer))
                Debug.LogWarning($"Special type with function pointer 0x{actor.P_FunctionPointer:X8} is not defined at ({Actor.XPosition}, {Actor.YPosition})");*/
        }

        public GBARRR_Actor Actor { get; }
        public Unity_ObjectManager_GBARRR ObjManager { get; }

        public override short XPosition
        {
            get => Actor.XPosition;
            set => Actor.XPosition = value;
        }
        public override short YPosition
        {
            get => Actor.YPosition;
            set => Actor.YPosition = value;
        }

        // We group animations so we can switch between them easier
        private int _AnimationGroupIndex { get; set; }
        public int AnimationGroupIndex {
            get => _AnimationGroupIndex;
            set {
                if (value != _AnimationGroupIndex) {
                    _AnimationGroupIndex = value;
                    AnimIndex = 0;
                }
            }
        }
        public int AnimIndex { get; set; }

        public override string DebugText =>
              $"UShort_0C: {Actor.Ushort_0C}{Environment.NewLine}" +
              $"P_GraphicsIndex: {Actor.P_GraphicsIndex}{Environment.NewLine}" +
              $"P_GraphicsOffset: 0x{Actor.P_GraphicsOffset:X8}{Environment.NewLine}" +
              $"P_FunctionPointer: 0x{Actor.P_FunctionPointer:X8}{Environment.NewLine}" +
              $"P_PaletteIndex: {Actor.P_PaletteIndex}{Environment.NewLine}" +
              $"P_SpriteSize: {Actor.P_SpriteSize}{Environment.NewLine}" +
              $"P_FrameCount: {Actor.P_FrameCount}{Environment.NewLine}" +
              $"Rotation: {Actor.Ushort_0E}{Environment.NewLine}";

        public override R1Serializable SerializableData => Actor;

        public override bool IsEditor => CurrentAnimation == null;

        public override bool FlipHorizontally => BitHelpers.ExtractBits(Actor.Data1[3], 1, 4) == 1;

        public override Vector2 Pivot
        {
            get
            {
                var sprite = Sprites?.ElementAtOrDefault(CurrentAnimation?.Frames.ElementAtOrDefault(AnimationFrame)?.SpriteLayers.FirstOrDefault()?.ImageIndex ?? 0);

                // Set the pivot to the center of the sprite
                if (sprite != null) {
                    return new Vector2(sprite.rect.width / 2 - sprite.pivot.x, sprite.rect.height / 2 - sprite.pivot.y);
                } else {
                    return Vector2.zero;
                }
            }
        }

        public override bool CanBeLinkedToGroup => true;

        public override string PrimaryName => $"Type_{(byte)Actor.ObjectType}";
        public override string SecondaryName {
            get {
                if (Actor.ObjectType == GBARRR_ActorType.Special) {
                    if (Enum.IsDefined(typeof(SpecialType_Function), (SpecialType_Function)Actor.P_FunctionPointer)) {
                        return ((SpecialType_Function)Actor.P_FunctionPointer).ToString();
                    } else if (Enum.IsDefined(typeof(SpecialType_GraphicsIndex), (SpecialType_GraphicsIndex)Actor.P_GraphicsIndex)) {
                        return ((SpecialType_GraphicsIndex)Actor.P_GraphicsIndex).ToString();
                    } else {
                        return Actor.ObjectType.ToString();
                    }
                } else {
                    return Actor.ObjectType.ToString();
                }
            }
        }

        public Unity_ObjectManager_GBARRR.GraphicsData GraphicsData => IsTriggerType ? null : ObjManager.GraphicsDatas.ElementAtOrDefault(AnimationGroupIndex)?.ElementAtOrDefault(AnimIndex);

        public bool IsTriggerType => Actor.ObjectType == GBARRR_ActorType.DoorTrigger ||
                                     Actor.ObjectType == GBARRR_ActorType.MurfyTrigger ||
                                     Actor.ObjectType == GBARRR_ActorType.SizeTrigger ||
                                     (Actor.ObjectType == GBARRR_ActorType.Special && Actor.P_GraphicsOffset == 0);
        public Unity_ObjAnimationCollisionPart.CollisionType GetCollisionType
        {
            get
            {
                switch (Actor.ObjectType)
                {
                    case GBARRR_ActorType.MurfyTrigger:
                        return Unity_ObjAnimationCollisionPart.CollisionType.Gendoor;

                    case GBARRR_ActorType.SizeTrigger:
                        return Unity_ObjAnimationCollisionPart.CollisionType.SizeChange;

                    case GBARRR_ActorType.Special:
                        switch ((SpecialType_Function)Actor.P_FunctionPointer)
                        {
                            case SpecialType_Function.LevelEndTrigger:
                            case SpecialType_Function.LevelEntranceTrigger:
                            case SpecialType_Function.MinigameTrigger:
                                return Unity_ObjAnimationCollisionPart.CollisionType.ExitLevel;
                        }
                        break;
                }
                return Unity_ObjAnimationCollisionPart.CollisionType.TriggerBox;
            }
        }
        public enum SpecialType_Function
        {
            LevelEndTrigger = 0x08037B9D,
            LevelEntranceTrigger = 0x08037CA1,
            MinigameTrigger = 0x0804DC65
        }
        public enum SpecialType_GraphicsIndex {
            Rayman = 2,
            LevelExit = 3,
            LevelEntrance = 4
        }

        public override Unity_ObjAnimationCollisionPart[] ObjCollision => IsTriggerType ? new Unity_ObjAnimationCollisionPart[]
        {
            new Unity_ObjAnimationCollisionPart()
            {
                XPosition = 0,
                YPosition = 0,
                Width = (int)Actor.P_SpriteSize,
                Height = (int)Actor.P_SpriteSize,
                Type = GetCollisionType
            }
        } : new Unity_ObjAnimationCollisionPart[0];

        public override Unity_ObjAnimation CurrentAnimation => GraphicsData?.Animation;
        public override int AnimSpeed => GraphicsData?.AnimSpeed ?? 0;
        public override int? GetAnimIndex => AnimIndex;
        protected override int GetSpriteID => AnimationGroupIndex;
        public override IList<Sprite> Sprites => GraphicsData?.AnimFrames;
        public override int? GetLayer(int index) => -index;

        #region UI States
        protected int UIStates_AnimGroupIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => AnimationGroupIndex == UIStates_AnimGroupIndex;

		protected override void RecalculateUIStates()
        {
            UIStates_AnimGroupIndex = AnimationGroupIndex;

            UIStates = ObjManager?.GraphicsDatas.ElementAtOrDefault(AnimationGroupIndex)?.Select((x, i) => (UIState)new RRR_UIState($"Animation {x.BlockIndex}", i)).ToArray() ?? new UIState[0];
        }

        protected class RRR_UIState : UIState
        {
            public RRR_UIState(string displayName, int animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj)
            {
                var rrrObj = (Unity_Object_GBARRR)obj;
                rrrObj.AnimIndex = AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj) => AnimIndex == ((Unity_Object_GBARRR)obj).AnimIndex;
        }
		#endregion

		#region LegacyEditorWrapper
		public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);
        private class LegacyEditorWrapper : ILegacyEditorWrapper {
            public LegacyEditorWrapper(Unity_Object_GBARRR obj) {
                Obj = obj;
            }

            private Unity_Object_GBARRR Obj { get; }

            public ushort Type {
                get => (ushort)Obj.Actor.ObjectType;
                set => Obj.Actor.ObjectType = (GBARRR_ActorType)(byte)value;
            }

            public int DES {
                get => Obj.AnimationGroupIndex;
                set => Obj.AnimationGroupIndex = value;
            }

            public int ETA {
                get => Obj.AnimationGroupIndex;
                set => Obj.AnimationGroupIndex = value;
            }

            public byte Etat { get; set; }

            public byte SubEtat {
                get => (byte)Obj.AnimIndex;
                set => Obj.AnimIndex = value;
            }

            public int EtatLength => 0;
            public int SubEtatLength => Obj.IsTriggerType ? 0 : Obj.ObjManager?.GraphicsDatas?.ElementAtOrDefault(Obj.AnimationGroupIndex)?.Length ?? 0;

            public byte OffsetBX { get; set; }

            public byte OffsetBY { get; set; }

            public byte OffsetHY { get; set; }

            public byte FollowSprite { get; set; }

            public uint HitPoints { get; set; }

            public byte HitSprite { get; set; }

            public bool FollowEnabled { get; set; }
        }
		#endregion
	}
}