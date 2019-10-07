using System.Collections.Generic;
using UnityEngine;

public class TerminatorAIComponent : Component
{
    public TerminatorAIStates CurrentState;
    
    public float VisionRadius;
    
    public float HearingRadius;
    
    public float CooldownDuration;
    
    public float ShootingDuration;
    
    public float CurrentCooldown;
    
    public Vector2 PostCoordinates;
    
    public bool MemorizedPlayer;
    
    public Vector2 LastKnownPlayerPosition;

    public GameObject BulletPrefab;

    public Stack<Vector3> Path = new Stack<Vector3>();
}