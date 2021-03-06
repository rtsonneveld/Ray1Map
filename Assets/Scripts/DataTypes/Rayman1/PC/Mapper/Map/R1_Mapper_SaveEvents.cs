﻿using System.Collections.Generic;

namespace R1Engine
{
    // Same struct as R1Jaguar_EventBlock, but in text
    public class R1_Mapper_SaveEvents : R1TextSerializable
    {
        public R1_Mapper_SaveEventInstance[][] SaveEventInstances { get; set; }

        public override void Read(R1TextParser parser)
        {
            var tempList = new List<R1_Mapper_SaveEventInstance[]>();
            var tempGroupList = new List<R1_Mapper_SaveEventInstance>();

            parser.SupportsComments = false;
            parser.SeparateAtPadding = true;

            string value;

            while ((value = parser.ReadValue()) != null)
            {
                // NOTE: There is data before the instances, but the game doesn't read it. It's auto-generated by the editor when saving based on memory offsets, probably for debugging and/or compiling on Jaguar.

                // Check for event instances
                if (value == "ev_ty1")
                {
                    R1_Mapper_SaveEventInstance item = new R1_Mapper_SaveEventInstance();
                    item.IsValid = parser.ReadShortValue();
                    item.OffsetX = parser.ReadShortValue();
                    item.OffsetY = parser.ReadShortValue();
                    item.EventDefinitionKey = parser.ReadValue();
                    item.HitPoints = parser.ReadShortValue();
                    item.DisplayPrio = parser.ReadByteValue();
                    item.LinkID = parser.ReadShortValue();
                    tempGroupList.Add(item);
                }
                // Check for group end
                else if (value == "ev_end")
                {
                    tempList.Add(tempGroupList.ToArray());
                    tempGroupList.Clear();
                }
            }

            SaveEventInstances = tempList.ToArray();
        }
    }
}