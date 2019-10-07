using UnityEngine;

public class SharkAIComponent : Component
{
    public SharkAIStates CurrentState;
    
    public float MaxDistanceToObstacle;
    
    public float VisionRadius;
    
    public float CooldownDuration;
    
    public float LazerHeatingUpDuration;
    
    public float ShootingDuration;
    
    public float CurrentCooldown;
    
    public float CurrentDirection;
    
    public Vector3 LazerDirection;
    
    public Vector3 EndPoint;
    
    public GameObject LazerBeam;
}