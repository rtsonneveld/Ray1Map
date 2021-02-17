﻿using System;

namespace R1Engine
{
    [Flags]
    public enum R1_EventFlags
    {
        None = 0,

        IsAlways = 1 << 0, // If true the game sets the pos to (-32000, -32000) on init
        IsBalle = 1 << 1, // Indicates if the event is TYPE_BALLE1 or TYPE_BALLE2
        NoCollision = 1 << 2, // Indicates if the event has no collision - does not include follow
        HurtsRayman = 1 << 3, // Indicates if the event damages Rayman
        AlwaysActive = 1 << 4, // Indicates if the event is always set to active
        HasDetectZone = 1 << 5, // Indicates if the detect zone should be set
        Flag_06 = 1 << 6,
        IsBoss = 1 << 7, // Indicates if the boss bar should show

        Flag_08 = 1 << 8,
        IsCollectible = 1 << 9, // Indicates if the event can be collected and thus not respawn again
        Flag_0A = 1 << 10,
        Flag_0B = 1 << 11,
        MoveX = 1 << 12, // Indicates if the object x position should be changed by SpeedX in MOVE_OBJECT
        MoveY = 1 << 13, // Indicates if the object y position should be changed by SpeedY in MOVE_OBJECT
        IsSpecialPlatform = 1 << 14, // Indicates if DO_SPECIAL_PLATFORM should be called
        DoCmds = 1 << 15, // Indicates if commands should be read for the event, otherwise the command is set to 30 (NOP)

        CheckBTyp = 1 << 16, // Indicates if the object reacts to block types (tile collision), thus calling calc_btyp
        Flag_11 = 1 << 17,
        Flag_12 = 1 << 18,
        CanJump = 1 << 19, // Indicates if obj_jump gets called when on a ressort (spring) block
        Flag_14 = 1 << 20,
        Flag_15 = 1 << 21,
        Flag_16 = 1 << 22,
        Flag_17 = 1 << 23,

        Flag_18 = 1 << 24,
        Flag_19 = 1 << 25,
        Flag_1A = 1 << 26,
        Flag_1B = 1 << 27,
        Flag_1C = 1 << 28,
        LinkRequiresGendoor = 1 << 29, // Indicates if the object requires a gendoor in the link group to be valid
        NoLink = 1 << 30, // Indicates that the event can't be linked
        Flag_1F = 1 << 31,
    }
}