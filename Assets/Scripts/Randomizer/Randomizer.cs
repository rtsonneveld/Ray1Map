﻿using System;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// The Rayman 1 event randomizer
    /// </summary>
    public static class Randomizer
    {
        /// <summary>
        /// Randomizes the events in a level based on the flags
        /// </summary>
        /// <param name="editorManager">The level editor manager</param>
        /// <param name="flags">The flags</param>
        /// <param name="seed">An optional seed to use</param>
        public static void Randomize(BaseEditorManager editorManager, RandomizerFlags flags, int? seed)
        {
            var random = seed != null ? new Random(seed.Value) : new Random();
            var maxX = editorManager.Level.Width * 16;
            var maxY = editorManager.Level.Height * 16;

            // Enumerate every event
            foreach (Common_EventData eventData in editorManager.Level.EventData
                .Select(eventData => new
                {
                    eventData, 
                    eventFlag = eventData.Type.GetAttribute<EventTypeInfoAttribute>()?.Flag
                })
                .Where(x => x.eventFlag == null || x.eventFlag == EventFlag.Normal)
                .Where(x => x.eventData.Type != EventType.TYPE_RAY_POS && 
                            x.eventData.Type != EventType.TYPE_PANCARTE &&
                            x.eventData.Type != EventType.TYPE_SIGNPOST)
                .Select(x => x.eventData))
            {
                if (flags.HasFlag(RandomizerFlags.Pos))
                {
                    eventData.XPosition = (uint)random.Next(0, maxX);
                    eventData.YPosition = (uint)random.Next(0, maxY);
                }

                if (flags.HasFlag(RandomizerFlags.Des))
                    eventData.DES = random.Next(0, editorManager.GetMaxDES);

                if (flags.HasFlag(RandomizerFlags.Eta))
                    eventData.ETA = random.Next(0, editorManager.GetMaxETA);

                if (flags.HasFlag(RandomizerFlags.CommandOrder))
                {
                    int n = eventData.CommandCollection.Commands.Length - 1;

                    while (n > 1)
                    {
                        n--;
                        int k = random.Next(n + 1);
                        var value = eventData.CommandCollection.Commands[k];
                        eventData.CommandCollection.Commands[k] = eventData.CommandCollection.Commands[n];
                        eventData.CommandCollection.Commands[n] = value;
                    }
                }

                if (flags.HasFlag(RandomizerFlags.Follow))
                {
                    eventData.FollowEnabled = random.Next(0, 1) == 1;
                    eventData.OffsetHY = random.Next(0, 10);
                }

                if (flags.HasFlag(RandomizerFlags.States))
                {
                    eventData.Etat = random.Next(0, editorManager.GetMaxEtat(eventData.ETA));
                    eventData.SubEtat = random.Next(0, editorManager.GetMaxSubEtat(eventData.ETA, eventData.Etat));
                }

                if (flags.HasFlag(RandomizerFlags.Type))
                    eventData.Type = (EventType)random.Next(0, 255);
            }
        }
    }
}