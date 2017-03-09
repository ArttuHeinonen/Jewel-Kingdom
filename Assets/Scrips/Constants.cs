using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants {

    public static readonly int MaxCols = 6;
    public static readonly int MaxRows = 8;
    public static readonly float HexFallSpeed = 0.1f;
    public static readonly string GRID_SORTING_LAYER = "Grid";
    public static readonly string HIGH_SORTING_LAYER = "Highlighter";
    public static readonly float GridOffset = 0.86f;
    public static readonly int MinMatches = 3;
    public static readonly float AnimationDuration = 0.2f;
    public static readonly float ExplosionDuration = 0.3f;
    public static readonly float MoveAnimationMinDuration = 0.05f;
    public static readonly float MovaAnimationSpeed = 1f;
    public static readonly Vector2 HexSize = new Vector2(1f, 1f);
}
