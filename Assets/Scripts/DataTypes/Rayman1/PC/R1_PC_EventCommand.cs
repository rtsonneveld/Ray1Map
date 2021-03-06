﻿namespace R1Engine
{
    /// <summary>
    /// Event command for PC
    /// </summary>
    public class R1_PC_EventCommand : R1Serializable
    {
        /// <summary>
        /// The amount of bytes for the commands
        /// </summary>
        public ushort CommandLength { get; set; }

        /// <summary>
        /// The amount of label offsets
        /// </summary>
        public ushort LabelOffsetCount { get; set; }

        /// <summary>
        /// The commands
        /// </summary>
        public R1_EventCommandCollection Commands { get; set; }

        /// <summary>
        /// The label offsets
        /// </summary>
        public ushort[] LabelOffsetTable { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer objects</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize the lengths
            CommandLength = s.Serialize<ushort>(CommandLength, name: nameof(CommandLength));
            LabelOffsetCount = s.Serialize<ushort>(LabelOffsetCount, name: nameof(LabelOffsetCount));

            if (CommandLength > 0)
                // Serialize the commands
                Commands = s.SerializeObject<R1_EventCommandCollection>(Commands, name: nameof(Commands));
            else
                Commands = new R1_EventCommandCollection()
                {
                    Commands = new R1_EventCommand[0]
                };

            // Serialize the label offsets
            LabelOffsetTable = s.SerializeArray<ushort>(LabelOffsetTable, LabelOffsetCount, name: nameof(LabelOffsetTable));
        }
    }
}