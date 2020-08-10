﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace R1Engine
{
    /// <summary>
    /// The map names
    /// </summary>
    public static class MapNames
    {
        /// <summary>
        /// Gets the map names for the specified game
        /// </summary>
        /// <param name="game">The game</param>
        /// <returns>The map names</returns>
        public static Dictionary<World, Dictionary<int, string>> GetMapNames(Game game)
        {
            switch (game)
            {
                case Game.Rayman1:
                    return Rayman1MapNames;

                case Game.RaymanDesigner:
                    return RaymanDesignerMapNames;

                case Game.Rayman3GBA:
                    return Rayman3GBAMapNames;

                case Game.RaymanMapper:
                case Game.RaymanByHisFans:
                case Game.Rayman60Levels:
                case Game.RaymanEducational:
                case Game.RaymanQuiz:
                default:
                    return null;
            }
        }

        /// <summary>
        /// The Rayman 1 map names
        /// </summary>
        private static Dictionary<World, Dictionary<int, string>> Rayman1MapNames { get; } = new Dictionary<World, Dictionary<int, string>>()
        {
            [World.Jungle] = new Dictionary<int, string>()
            {
                [1] = "Pink Plant Woods 1",
                [2] = "Pink Plant Woods 2",
                [3] = "Pink Plant Woods 3 - Betilla",
                [4] = "Pink Plant Woods 4",

                [5] = "Anguish Lagoon 1",
                [6] = "Anguish Lagoon 2 - Boss",
                [7] = "Anguish Lagoon 3 - Bzzit Flight",
                [8] = "Anguish Lagoon 4 - Betilla",

                [9] = "The Swamps of Forgetfulness 1",
                [10] = "The Swamps of Forgetfulness 2",
                [11] = "The Swamps of Forgetfulness 3",

                [12] = "Moskito's Nest 1",
                [13] = "Moskito's Nest 2",
                [14] = "Moskito's Nest 3 - Boss Chase",
                [15] = "Moskito's Nest 4",
                [16] = "Moskito's Nest 5 - Boss",
                [17] = "Moskito's Nest 6 - Betilla",

                [18] = "Magician Bonus 1",
                [19] = "Magician Bonus 2",
                [20] = "Magician Bonus 3",
                [21] = "Magician Bonus 4",

                [22] = "Ray Breakout",
                [23] = "Multiplayer",
            },
            [World.Music] = new Dictionary<int, string>()
            {
                [1] = "Bongo Hills 1",
                [2] = "Bongo Hills 2",
                [3] = "Bongo Hills 3",
                [4] = "Bongo Hills 4",
                [5] = "Bongo Hills 5",
                [6] = "Bongo Hills 6",

                [7] = "Allegro Presto 1",
                [8] = "Allegro Presto 2",
                [9] = "Allegro Presto 3",
                [10] = "Allegro Presto 4",
                [11] = "Allegro Presto 5 - Betilla",

                [12] = "Gong Heights 1",
                [13] = "Gong Heights 2",

                [14] = "Mr Sax's Hullaballoo 1",
                [15] = "Mr Sax's Hullaballoo 2 - Boss Chase",
                [16] = "Mr Sax's Hullaballoo 3 - Boss",

                [17] = "Magician Bonus 1",
                [18] = "Magician Bonus 2",

                [19] = "Multiplayer",
            },
            [World.Mountain] = new Dictionary<int, string>()
            {
                [1] = "Twilight Gulch 1",
                [2] = "Twilight Gulch 2",

                [3] = "The Hard Rocks 1",
                [4] = "The Hard Rocks 2",
                [5] = "The Hard Rocks 3",

                [6] = "Mr Stone's Peaks 1",
                [7] = "Mr Stone's Peaks 2",
                [8] = "Mr Stone's Peaks 3",
                [9] = "Mr Stone's Peaks 4",
                [10] = "Mr Stone's Peaks 5 - Boss",
                [11] = "Mr Stone's Peaks 6 - Betilla",

                [12] = "Magician Bonus 1",
                [13] = "Magician Bonus 2",

                [14] = "Multiplayer",
            },
            [World.Image] = new Dictionary<int, string>()
            {
                [1] = "Eraser Plains 1",
                [2] = "Eraser Plains 2",
                [3] = "Eraser Plains 3",
                [4] = "Eraser Plains 4 - Boss",

                [5] = "Pencil Pentathlon 1",
                [6] = "Pencil Pentathlon 2",
                [7] = "Pencil Pentathlon 3",

                [8] = "Space Mama's Crater 1",
                [9] = "Space Mama's Crater 2",
                [10] = "Space Mama's Crater 3",
                [11] = "Space Mama's Crater 4 - Boss",

                [12] = "Magician Bonus 1",
                [13] = "Magician Bonus 2",

                [14] = "Multiplayer",
            },
            [World.Cave] = new Dictionary<int, string>()
            {
                [1] = "Crystal Palace 1",
                [2] = "Crystal Palace 2",

                [3] = "Eat at Joe's 1",
                [4] = "Eat at Joe's 2",
                [5] = "Eat at Joe's 3",
                [6] = "Eat at Joe's 4",
                [7] = "Eat at Joe's 5",
                [8] = "Eat at Joe's 6",

                [9] = "Mr Skops' Stalactites 1",
                [10] = "Mr Skops' Stalactites 2 - Boss",
                [11] = "Mr Skops' Stalactites 3 - Boss",

                [12] = "Magician Bonus 1",

                [13] = "Multiplayer",
            },
            [World.Cake] = new Dictionary<int, string>()
            {
                [1] = "Mr Dark's Dare 1",
                [2] = "Mr Dark's Dare 2",
                [3] = "Mr Dark's Dare 3",
                [4] = "Mr Dark's Dare 4 - Boss",

                [5] = "Multiplayer",
            },
        };

        /// <summary>
        /// The Rayman Designer map names
        /// </summary>
        private static Dictionary<World, Dictionary<int, string>> RaymanDesignerMapNames { get; } = new Dictionary<World, Dictionary<int, string>>()
        {
            [World.Jungle] = new Dictionary<int, string>()
            {
                [1] = "The Sky's the Limit",
                [2] = "Fruity Fun",
                [3] = "Junglemania",
                [4] = "Ring a Ling",
            },
            [World.Music] = new Dictionary<int, string>()
            {
                [1] = "Gone with the Wind",
                [2] = "Scale the Scales",
                [3] = "Music Lessons",
                [4] = "Melodic Maracas",
            },
            [World.Mountain] = new Dictionary<int, string>()
            {
                [1] = "Treetop Adventure",
                [2] = "Tough Climb",
                [3] = "Tip-Top Tempest",
                [4] = "The Diabolical Pursuit",
            },
            [World.Image] = new Dictionary<int, string>()
            {
                [1] = "The Five Doors",
                [2] = "Pencil Pentathalon",
                [3] = "Eraser Mania",
                [4] = "Tic Tack Toe",
            },
            [World.Cave] = new Dictionary<int, string>()
            {
                [1] = "Peaks and Rocks",
                [2] = "Dark Journey",
                [3] = "Dreaded Caves",
                [4] = "Dire Darkness",
            },
            [World.Cake] = new Dictionary<int, string>()
            {
                [1] = "Chocolate Trap",
                [2] = "Crazy Candy",
                [3] = "Bonbon-a-rama",
                [4] = "Whipped Cream Challenge",
            },
        };

        /// <summary>
        /// The Rayman Designer map names
        /// </summary>
        private static Dictionary<World, Dictionary<int, string>> Rayman3GBAMapNames { get; } = new Dictionary<World, Dictionary<int, string>>()
        {
            [World.Jungle] = new Dictionary<int, string>()
            {
                [0] = "Wanderwood Forest 1",
                [1] = "Wanderwood Forest 2",
                [2] = "Shining Glade 1",
                [3] = "Shining Glade 2",
                [4] = "Swamp of Bégoniax",
                [5] = "Garish Gears",
                [6] = "Hoodlum Hideout 1",
                [7] = "Hoodlum Hideout 2",
                [8] = "Magma Mayhem",
                [9] = "Vertigo Wasters 1",
                [10] = "Vertigo Wasters 2",
                [11] = "Void of Bones 1",
                [12] = "Void of Bones 2",
                [13] = "Jano's Nest",
                [14] = "Prickly Passage 1",
                [15] = "Prickly Passage 2",
                [16] = "Swamp of Bégoniax 2",
                [17] = "River of Fire 1",
                [18] = "River of Fire 2",
                [19] = "River of Fire 3",
                [20] = "The Underlands 1",
                [21] = "The Underlands 2",
                [22] = "Boulder Brink 1",
                [23] = "Boulder Brink 2",
                [24] = "Den of Rocky",
                [25] = "Wretched Ruins 1",
                [26] = "Wretched Ruins 2 ",
                [27] = "Wicked Flow 1",
                [28] = "Wicked Flow 2",
                [29] = "Wicked Flow 3",
                [30] = "Creeping Chaos 1",
                [31] = "Creeping Chaos 2",
                [32] = "Scaleman's Keep",
                [33] = "The Mettleworks 1",
                [34] = "The Mettleworks 2",
                [35] = "Magma Mayhem 2",
                [36] = "Razor Slide 1",
                [37] = "Razor Slide 2",
                [38] = "Heart of the Ancients 1",
                [39] = "Heart of the Ancients 2",
                [40] = "Mega Havoc 1",
                [41] = "Mega Havoc 2",
                [42] = "Mega Havoc 3",
                [43] = "Mega Havoc 4",
                [44] = "Lum Challenge",
                [45] = "Ly's Punch Challenge 1",
                [46] = "Ly's Punch Challenge 2",
                [47] = "Ly's Punch Challenge 3",
                [48] = "Ly Power 1 (Wanderwood Forest)",
                [49] = "Ly Power 2 (Garish Gears)",
                [50] = "Ly Power 3 (Vertigo Wasters)",
                [51] = "Ly Power 4 (River of Fire)",
                [52] = "Ly Power 5 (Den of Rocky)",
                [53] = "Ly Power 6 (Scaleman's Keep)",
                [54] = "Forgotten Forests",
                [55] = "Haunted Dreams",
                [56] = "Magmacosm",
                [57] = "Pirate Stronghold",
                [58] = "Worldmap",
                [59] = "Multiplayer (Magma Mayhem)",
                [60] = "Multiplayer (Magma Mayhem)",
                [61] = "Multiplayer (?)",
                [62] = "Multiplayer (?)",
                [63] = "Multiplayer (Ly's Punch Challenge)",
                [64] = "Multiplayer (Magma Mayhem)",

            },
        };
    }
}