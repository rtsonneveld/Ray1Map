﻿namespace R1Engine
{
    public class GBACrash_ObjData : R1Serializable
    {
        public ushort Ushort_00 { get; set; }
        public ushort ObjGroupsCount { get; set; }
        public Pointer ObjGroupsPointer { get; set; }
        public Pointer Pointer_08 { get; set; }
        public Pointer Pointer_0C { get; set; }
        public Pointer Pointer_10 { get; set; }

        // Serialized from pointers
        
        public ObjGroup[] ObjGroups { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Ushort_00 = s.Serialize<ushort>(Ushort_00, name: nameof(Ushort_00));
            ObjGroupsCount = s.Serialize<ushort>(ObjGroupsCount, name: nameof(ObjGroupsCount));
            ObjGroupsPointer = s.SerializePointer(ObjGroupsPointer, name: nameof(ObjGroupsPointer));
            Pointer_08 = s.SerializePointer(Pointer_08, name: nameof(Pointer_08));
            Pointer_0C = s.SerializePointer(Pointer_0C, name: nameof(Pointer_0C));
            Pointer_10 = s.SerializePointer(Pointer_10, name: nameof(Pointer_10));

            ObjGroups = s.DoAt(ObjGroupsPointer, () => s.SerializeObjectArray<ObjGroup>(ObjGroups, ObjGroupsCount, name: nameof(ObjGroups)));
        }

        public class ObjGroup : R1Serializable
        {
            public ushort Ushort_00 { get; set; }
            public ushort ObjectsCount { get; set; }
            public Pointer ObjectsPointer { get; set; }

            // Serialized from pointers
            public Object[] Objects { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Ushort_00 = s.Serialize<ushort>(Ushort_00, name: nameof(Ushort_00));
                ObjectsCount = s.Serialize<ushort>(ObjectsCount, name: nameof(ObjectsCount));
                ObjectsPointer = s.SerializePointer(ObjectsPointer, name: nameof(ObjectsPointer));

                Objects = s.DoAt(ObjectsPointer, () => s.SerializeObjectArray<Object>(Objects, ObjectsCount, name: nameof(Objects)));
            }

            public class Object : R1Serializable
            {
                public ObjectType ObjType { get; set; }
                public short XPos { get; set; }
                public short YPos { get; set; }
                public ushort Ushort_06 { get; set; }

                public override void SerializeImpl(SerializerObject s)
                {
                    ObjType = s.Serialize<ObjectType>(ObjType, name: nameof(ObjType));
                    XPos = s.Serialize<short>(XPos, name: nameof(XPos));
                    YPos = s.Serialize<short>(YPos, name: nameof(YPos));
                    Ushort_06 = s.Serialize<ushort>(Ushort_06, name: nameof(Ushort_06));
                }

                public enum ObjectType : short
                {
                    Crash = 0,

                    Wumpa = 2,

                    TimeTrialClock = 9,

                    Crate_Normal = 13,
                    Crate_Checkpoint = 14,
                    Crate_AkuAku = 15,

                    Crate_NitroSwitch = 19,
                    Crate_Iron = 20,
                    Crate_IronUp = 20,
                    Crate_Life = 22,
                    Crate_Nitro = 23,
                    Crate_QuestionMark = 24,
                    Crate_Bounce = 25,
                    Crate_Locked = 26,
                    Crate_TNT = 27,
                    Crate_Slot = 28,

                    TutorialMessage = 105,
                }
            }
        }
    }
}