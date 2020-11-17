﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBAIsometricSpyro : Unity_Object_3D
    {
        public Unity_Object_GBAIsometricSpyro(GBAIsometric_Object obj, Unity_ObjectManager_GBAIsometricSpyro objManager)
        {
            Object = obj;
            ObjManager = objManager;

            if (IsWaypoint)
                AnimSetIndex = -1;

            // TODO: Init obj
        }

        public GBAIsometric_Object Object { get; }
        public Unity_ObjectManager_GBAIsometricSpyro ObjManager { get; }

        public override short XPosition
        {
            get => Object.XPosition;
            set => Object.XPosition = value;
        }
        public override short YPosition
        {
            get => Object.YPosition;
            set => Object.YPosition = value;
        }
        public override Vector3 Position {
            get => new Vector3(Object.XPosition, Object.YPosition, Object.Height);
            set {
                Object.XPosition = (short)Mathf.RoundToInt(value.x);
                Object.YPosition = (short)Mathf.RoundToInt(value.y);
                Object.Height = (short)Mathf.RoundToInt(value.z);
            }
        }

        public override string DebugText => String.Empty;

        public GBAIsometric_ObjectType ObjType => ObjManager.Types?.ElementAtOrDefault(Object.ObjectType);

        private int _animSetIndex;
        private byte _animationGroupIndex;

        public int AnimSetIndex
        {
            get => _animSetIndex;
            set
            {
                _animSetIndex = value;
                AnimationGroupIndex = 0;
            }
        }

        public byte AnimationGroupIndex
        {
            get => _animationGroupIndex;
            set
            {
                _animationGroupIndex = value;
                AnimIndex = 0;
            }
        }

        public byte AnimIndex { get; set; } // Relative to the group

        public Unity_ObjectManager_GBAIsometricSpyro.AnimSet AnimSet => ObjManager.AnimSets?.ElementAtOrDefault(AnimSetIndex);
        public GBAIsometric_Spyro_AnimGroup AnimGroup => AnimSet?.AnimSetObj?.AnimGroups?.ElementAtOrDefault(AnimationGroupIndex);
        public Unity_ObjectManager_GBAIsometricSpyro.AnimSet.Animation Anim => AnimSet?.Animations?.ElementAtOrDefault(AnimGroup?.AnimIndex + AnimIndex ?? -1);

        public bool IsWaypoint => Object.ObjectType == 0;

        public override R1Serializable SerializableData => Object;
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => IsWaypoint ? "Waypoint" : $"Type_{Object.ObjectType}";
        public override string SecondaryName => null;

        public override bool IsEditor => IsWaypoint;
        public override ObjectType Type => IsWaypoint ? ObjectType.Waypoint : ObjectType.Object;

        public override bool CanBeLinked => true;
        public override IEnumerable<int> Links
        {
            get
            {
                // Waypoint links
                for (int i = 0; i < Object.WaypointCount; i++)
                    yield return Object.WaypointIndex + i;
            }
        }

        public override Unity_ObjAnimation CurrentAnimation => Anim?.ObjAnimation;
        public override int AnimSpeed => Anim?.AnimSpeed ?? 0;
        public override int? GetAnimIndex => AnimGroup?.AnimIndex + AnimIndex;
        protected override int GetSpriteID => AnimSetIndex;
        public override IList<Sprite> Sprites => Anim?.AnimFrames;
        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_GBAIsometricSpyro obj)
            {
                Obj = obj;
            }

            private Unity_Object_GBAIsometricSpyro Obj { get; }

            public ushort Type
            {
                get => Obj.Object.ObjectType;
                set => Obj.Object.ObjectType = value;
            }

            public int DES
            {
                get => Obj.AnimSetIndex;
                set => Obj.AnimSetIndex = value;
            }

            public int ETA
            {
                get => Obj.AnimSetIndex;
                set => Obj.AnimSetIndex = value;
            }

            public byte Etat
            {
                get => Obj.AnimationGroupIndex;
                set => Obj.AnimationGroupIndex = value;
            }

            public byte SubEtat
            {
                get => Obj.AnimIndex;
                set => Obj.AnimIndex = value;
            }

            public int EtatLength => Obj.AnimSet?.AnimSetObj?.AnimGroups?.Length ?? 0;
            public int SubEtatLength => Obj.AnimGroup?.AnimCount ?? 0;

            public byte OffsetBX { get; set; }

            public byte OffsetBY { get; set; }

            public byte OffsetHY { get; set; }

            public byte FollowSprite { get; set; }

            public uint HitPoints { get; set; }

            public byte HitSprite { get; set; }

            public bool FollowEnabled { get; set; }
        }

        #region UI States
        protected int UIStates_AnimSetIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => AnimSetIndex == UIStates_AnimSetIndex;

        protected class GBAIsometricSpyro_UIState : UIState
        {
            public GBAIsometricSpyro_UIState(string displayName, byte animGroupIndex, byte animIndex) : base(displayName, animIndex) => AnimGroupIndex = animGroupIndex;

            public byte AnimGroupIndex { get; }

            public override void Apply(Unity_Object obj)
            {
                ((Unity_Object_GBAIsometricSpyro)obj).AnimationGroupIndex = AnimGroupIndex;
                ((Unity_Object_GBAIsometricSpyro)obj).AnimIndex = (byte)AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj)
            {
                return AnimIndex == ((Unity_Object_GBAIsometricSpyro)obj).AnimIndex && AnimGroupIndex == ((Unity_Object_GBAIsometricSpyro)obj).AnimationGroupIndex;
            }
        }

        protected override void RecalculateUIStates()
        {
            UIStates_AnimSetIndex = AnimSetIndex;

            List<UIState> uiStates = new List<UIState>();

            byte groupIndex = 0;

            foreach (var group in AnimSet?.AnimSetObj?.AnimGroups ?? new GBAIsometric_Spyro_AnimGroup[0])
            {
                for (byte i = 0; i < group.AnimCount; i++)
                    uiStates.Add(new GBAIsometricSpyro_UIState($"Animation {groupIndex}-{i}", animIndex: i, animGroupIndex: groupIndex));

                groupIndex++;
            }

            UIStates = uiStates.ToArray();
        }
        #endregion
    }
}