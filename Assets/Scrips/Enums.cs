using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum BonusType
{
    None,
    DestroyWholeColumn
}

public enum Type : int
{
    Food = 0,
    Stone = 1,
    Wood = 2,
    Science = 3,
    Empty = 999
}

public enum GameState
{
    None,
    SelectionStarted,
    Animating
}