﻿using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Event block data for Rayman 1 (PS1)
    /// </summary>
    public class R1_PS1_EventBlock : R1Serializable
    {
        /// <summary>
        /// Pointer to the events
        /// </summary>
        public Pointer EventsPointer { get; set; }

        /// <summary>
        /// The amount of events in the file
        /// </summary>
        public byte EventCount { get; set; }

        /// <summary>
        /// Pointer to the event links
        /// </summary>
        public Pointer EventLinksPointer { get; set; }

        /// <summary>
        /// The amount of event links in the file
        /// </summary>
        public byte EventLinkCount { get; set; }

        /// <summary>
        /// The events
        /// </summary>
        public R1_EventData[] Events { get; set; }

        /// <summary>
        /// Data table for event linking
        /// </summary>
        public byte[] EventLinkingTable { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header
            EventsPointer = s.SerializePointer(EventsPointer, name: nameof(EventsPointer));
            EventCount = s.Serialize<byte>(EventCount, name: nameof(EventCount));
            s.SerializeArray<byte>(new byte[3], 3, name: "Padding");
            EventLinksPointer = s.SerializePointer(EventLinksPointer, name: nameof(EventLinksPointer));
            EventLinkCount = s.Serialize<byte>(EventLinkCount, name: nameof(EventLinkCount));
            s.SerializeArray<byte>(new byte[3], 3, name: "Padding");

            if (EventCount != EventLinkCount)
                Debug.LogError("Event counts don't match");

            s.DoAt(EventsPointer, (() =>
            {
                // Serialize every event
                Events = s.SerializeObjectArray<R1_EventData>(Events, EventCount, name: nameof(Events));
            }));

            s.DoAt(EventLinksPointer, (() =>
            {
                // Serialize the event linking table
                EventLinkingTable = s.SerializeArray<byte>(EventLinkingTable, EventLinkCount, name: nameof(EventLinkingTable));
            }));
        }
    }
}