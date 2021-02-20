﻿using System;

namespace R1Engine
{
    [Flags]
    public enum RandomizerFlags
    {
        None = 0,

        Pos = 1 << 1,
        Des = 1 << 2,
        Eta = 1 << 3,
        CommandOrder = 1 << 4,
        Follow = 1 << 5,
        States = 1 << 6,
        Type = 1 << 7,
        RobinsCageRandomizer = 1 << 8,

        All = ~0
    }
}