using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabColorLegend", menuName = "Database/Prefab color legend", order = 1)]
public class PrefabColorLegend : ScriptableObject
{
    public ColorPrefabPair[] Data;
}