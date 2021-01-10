﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBACrash : Unity_Object
    {
        public Unity_Object_GBACrash(Unity_ObjectManager objManager, GBACrash_ObjData.ObjGroup.Object obj)
        {
            ObjManager = objManager;
            Object = obj;
        }

        public Unity_ObjectManager ObjManager { get; }
        public GBACrash_ObjData.ObjGroup.Object Object { get; set; }

        public override short XPosition
        {
            get => Object.XPos;
            set => Object.XPos = value;
        }

        public override short YPosition
        {
            get => Object.YPos;
            set => Object.YPos = value;
        }

        public override string DebugText => String.Empty;

        public override R1Serializable SerializableData => null;
        public override ILegacyEditorWrapper LegacyWrapper => new DummyLegacyEditorWrapper(this);

        public override string PrimaryName => $"Type_{Object.ObjType}";
        public override string SecondaryName => null;

        public override Unity_ObjAnimation CurrentAnimation => null;
        public override int AnimSpeed => 0;
        public override int? GetAnimIndex => null;
        protected override int GetSpriteID => 0;
        public override IList<Sprite> Sprites => null;


        #region UI States
        protected bool UIStates_HasInitialized { get; set; }
        protected override bool IsUIStateArrayUpToDate => UIStates_HasInitialized;

        protected override void RecalculateUIStates() {
            UIStates_HasInitialized = true;
            UIStates = new UIState[0];
        }

        #endregion
    }
}