using System;

[Flags]
public enum SlotCategories
{
    //NONE = 0,
    //ALL = ~0,
    BEAK = 1 << 0,
    EYES = 1 << 1,
    WINGS = 1 << 2,
    LEGS = 1 << 3,
    DOOMBA_FRONT = 1 << 4,
    DOOMBA_CONTROLLER = 1 << 5
}