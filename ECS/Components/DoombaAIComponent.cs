using UnityEngine;

public class DoombaAIComponent : Component
{
    public DoombaAIStates CurrentState;
    
    public float RotationSpeed;
    
    public Vector3 TargetDirection;
    
    public Vector3 TargetPosition;
    
    public float DefaultRotationChance;
    
    public float Epsilon;
}