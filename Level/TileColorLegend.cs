using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileColorLegend", menuName = "Database/Tile color legend", order = 1)]
public class TileColorLegend : ScriptableObject
{
    public ColorSurfacePair[] Data;
}