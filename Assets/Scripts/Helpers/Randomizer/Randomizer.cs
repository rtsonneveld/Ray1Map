using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using UnityEngine;
using Random = System.Random;

namespace R1Engine
{
    /// <summary>
    /// The Rayman 1 event randomizer
    /// </summary>
    public static class Randomizer
    {


        #region Randomizer

        public static async UniTask BatchRandomizeAsync()
        {
            try {
                // Get the settings
                var settings = Settings.GetGameSettings;

                // Init
                await LevelEditorData.InitAsync(settings);

                var manager = Settings.GetGameManager;

                // Get the flags
                var flag = Settings.RandomizerFlags;

                // Enumerate every world
                var worlds = manager.GetLevels(settings).First().Worlds;
                foreach (var world in worlds) {
                    // Set the world
                    settings.World = world.Index;

                    // Enumerate every level
                    foreach (var lvl in world.Maps) {

                        Debug.Log("World: " + world.Index + ", lvl: " + lvl);

                        // Set the level
                        settings.Level = lvl;

                        // Create the context
                        using (var context = new Context(settings)) {

                            // Load the files
                            await manager.LoadFilesAsync(context);

                            // Load the level
                            var level = await manager.LoadAsync(context, true);

                            // Randomize (only first map for now)
                            Randomizer.Randomize(level, flag, $"{world.Index},{lvl},{Settings.RandomizerSeed}".GetHashCode(), 0);

                            context.Close();

                            // Save the level
                            bool saveISO = world == worlds.Last() && lvl == world.Maps.Last();

                            if (manager is R1_PS1_Manager ps1Manager) {
                                await ps1Manager.SaveLevelAsync(context, level, false);

                                if (saveISO) {
                                    Debug.Log("Saving ISO");
                                    ps1Manager.CreateISO(context);
                                }

                            } else {
                                await manager.SaveLevelAsync(context, level);
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                Debug.LogError(ex);
            }
        }

        #endregion

        /// <summary>
        /// Randomizes the events in a level based on the flags
        /// </summary>
        /// <param name="level">The level</param>
        /// <param name="flags">The flags</param>
        /// <param name="seed">An optional seed to use</param>
        /// <param name="map">The map index</param>
        public static void Randomize(Unity_Level level, RandomizerFlags flags, int? seed, int map)
        {
            var random = seed != null ? new Random(seed.Value) : new Random();
            var maxX = level.Maps[map].Width * Settings.CellSize;
            var maxY = level.Maps[map].Height * Settings.CellSize;

            // Enumerate every event
            foreach (var eventData in level.EventData
                .Select(eventData => new
                {
                    obj = (Unity_Object_R1)eventData,
                    isAlways = eventData.IsAlways,
                    isEditor = eventData.IsEditor
                })
                .Where(x => !x.isAlways && !x.isEditor)
                .Where(x => x.obj.EventData.Type != R1_EventType.TYPE_RAY_POS &&
                            x.obj.EventData.Type != R1_EventType.TYPE_PANCARTE &&
                            x.obj.EventData.Type != R1_EventType.TYPE_SIGNPOST)
                .Select(x => x.obj))
            {
                if (flags.HasFlag(RandomizerFlags.Pos))
                {
                    eventData.XPosition = (short)random.Next(0, maxX);
                    eventData.YPosition = (short)random.Next(0, maxY);
                }

                if (flags.HasFlag(RandomizerFlags.Des))
                    eventData.DESIndex = random.Next(1, ((Unity_ObjectManager_R1)level.ObjManager).DES.Length - 2); // One less for BigRay

                if (flags.HasFlag(RandomizerFlags.Eta))
                    eventData.ETAIndex = random.Next(0, ((Unity_ObjectManager_R1)level.ObjManager).ETA.Length - 2); // One less for BigRay

                if (flags.HasFlag(RandomizerFlags.CommandOrder))
                {
                    int n = eventData.EventData.Commands.Commands.Length - 1;

                    while (n > 1)
                    {
                        n--;
                        int k = random.Next(n + 1);
                        var value = eventData.EventData.Commands.Commands[k];
                        eventData.EventData.Commands.Commands[k] = eventData.EventData.Commands.Commands[n];
                        eventData.EventData.Commands.Commands[n] = value;
                    }
                }

                if (flags.HasFlag(RandomizerFlags.Follow))
                {
                    eventData.EventData.SetFollowEnabled(level.ObjManager.Context.Settings, random.Next(0, 1) == 1);
                    eventData.EventData.OffsetHY = (byte)random.Next(0, 10);
                }

                if (flags.HasFlag(RandomizerFlags.States))
                {
                    eventData.EventData.Etat = (byte)random.Next(0, ((Unity_ObjectManager_R1)level.ObjManager).ETA[eventData.ETAIndex].Data.Length - 1);
                    eventData.EventData.SubEtat = (byte)random.Next(0, ((Unity_ObjectManager_R1)level.ObjManager).ETA[eventData.ETAIndex].Data[eventData.EventData.Etat].Length - 1);
                }

                if (flags.HasFlag(RandomizerFlags.Type))
                    eventData.EventData.Type = (R1_EventType)random.Next(0, 255);
            }


            if (flags.HasFlag(RandomizerFlags.RobinsCageRandomizer)) {
                CageLocationRandomizer.Randomize(level, seed);
            }
        }
    }
}