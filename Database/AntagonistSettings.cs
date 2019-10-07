using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AntagonistSettings", menuName = "Settings/Antagonist settings", order = 1)]
public class AntagonistSettings : ScriptableObject
{
    public AntagonistStageSettings[] StageSettings;
    
    public int ScorePerDirtyTile;
    
    public int ScorePerItemBroken;

    public int ScorePerUnitKilled;
}