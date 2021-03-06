﻿namespace R1Engine
{
    /// <summary>
    /// Data necessary to spawn an instance of an event for Rayman 1 (Jaguar)
    /// </summary>
    public class R1Jaguar_EventInstance : R1Serializable
    {
        // This value is 0 if the event is not valid
        public ushort Unk_00 { get; set; }

        // Offsets for the position
        public short OffsetX { get; set; }
        public short OffsetY { get; set; }

        public Pointer EventDefinitionPointer { get; set; }

        // Always 0x05?
        public ushort Unk_0A { get; set; }

        public ushort EventIndex { get; set; }

        // Parsed
        public R1Jaguar_EventDefinition EventDefinition { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Unk_00 = s.Serialize<ushort>(Unk_00, name: nameof(Unk_00));

            if (Unk_00 == 0)
                return;

            OffsetX = s.Serialize<short>(OffsetX, name: nameof(OffsetX));
            OffsetY = s.Serialize<short>(OffsetY, name: nameof(OffsetY));
            EventDefinitionPointer = s.SerializePointer(EventDefinitionPointer, name: nameof(EventDefinitionPointer));
            Unk_0A = s.Serialize<ushort>(Unk_0A, name: nameof(Unk_0A));
            EventIndex = s.Serialize<ushort>(EventIndex, name: nameof(EventIndex));

            s.DoAt(EventDefinitionPointer, () => EventDefinition = s.SerializeObject<R1Jaguar_EventDefinition>(EventDefinition, name: nameof(EventDefinition)));
        }
    }
}