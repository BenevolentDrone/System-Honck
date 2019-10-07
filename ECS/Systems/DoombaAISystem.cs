using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoombaAISystem : ISystem
{
    private List<Component> doombaAIComponents;
    
    private TileFieldComponent tileFieldComponent;
    
    private TimeComponent timeComponent;
    
    private int obstaclesLayer;
    
    public void Cache(WorldContext worldContext)
    {
        doombaAIComponents = worldContext.GetComponentsContainer<DoombaAIComponent>();
        
        tileFieldComponent = worldContext.Get<TileFieldComponent>(0);
        
        timeComponent = worldContext.Get<TimeComponent>(0);
        
        obstaclesLayer = LayerMask.GetMask("Obstacles");
    }
    
    public void Handle(WorldContext worldContext)
    {
        for (int i = 0; i < doombaAIComponents.Count; i++)
        {
            var doombaAIComponent = (DoombaAIComponent)doombaAIComponents[i];
            
            if (doombaAIComponent.Entity.HasComponent<CarriedComponent>()
                || doombaAIComponent.Entity.HasComponent<TossedComponent>())
            {
                var locomotionComponent = doombaAIComponent.Entity.GetComponent<LocomotionComponent>();
                
                locomotionComponent.Velocity = Vector2.zero;
                
                doombaAIComponent.TargetDirection = Vector3.zero;
                
                doombaAIComponent.CurrentState = DoombaAIStates.IDLE;
                
                continue;
            }
            
            switch (doombaAIComponent.CurrentState)
            {
                case DoombaAIStates.IDLE:
                    
                    float rotationChance = doombaAIComponent.DefaultRotationChance;
                    
                    Vector3 direction = doombaAIComponent.transform.right;
                    
                    int x = (int)(direction.x);
                    
                    int y = (int)(direction.y);
                    
                    if (x == 0 && y == 0)
                    {
                        Vector3 newDirection = Vector3.right;
                        
                        doombaAIComponent.CurrentState = DoombaAIStates.ROTATING;
                        
                        doombaAIComponent.TargetDirection = newDirection;
                        
                        break;
                    }
                    
                    var positionOnTileComponent = doombaAIComponent.Entity.GetComponent<PositionOnTileComponent>();
                    
                    var coordinates = positionOnTileComponent.CurrentTile.Coordinates;
                    
                    var tileAhead = tileFieldComponent.TileField.GetTile(coordinates, x, y);
                    
                    if (tileAhead == null)
                    {
                        rotationChance = 1f;
                    }
                    
                    float roll = UnityEngine.Random.Range(0f, 1f);
                    
                    if (roll < rotationChance)
                    {
                        float rotation = 90f * UnityEngine.Random.Range(0, 4);
                        
                        Vector3 newDirection = Quaternion.AngleAxis(rotation, Vector3.forward) * direction;
                        
                        doombaAIComponent.CurrentState = DoombaAIStates.ROTATING;
                        
                        doombaAIComponent.TargetDirection = newDirection;
                    }
                    else
                    {
                        doombaAIComponent.CurrentState = DoombaAIStates.MOVING;
                        
                        doombaAIComponent.TargetPosition = tileAhead.transform.position;
                        
                        break;
                    }
                    
                    break;
                
                case DoombaAIStates.MOVING:
                    
                    var locomotionComponent = doombaAIComponent.Entity.GetComponent<LocomotionComponent>();
                    
                    var distance = (doombaAIComponent.TargetPosition - doombaAIComponent.transform.position).magnitude;
                    
                    if (distance < doombaAIComponent.Epsilon)
                    {
                        doombaAIComponent.transform.position = doombaAIComponent.TargetPosition;
                        
                        doombaAIComponent.TargetPosition = Vector3.zero;
                        
                        locomotionComponent = doombaAIComponent.Entity.GetComponent<LocomotionComponent>();
                        
                        locomotionComponent.Velocity = Vector2.zero;
                        
                        doombaAIComponent.CurrentState = DoombaAIStates.IDLE;
                        
                        break;
                    }
                    
                    locomotionComponent.Velocity = (doombaAIComponent.TargetPosition - doombaAIComponent.transform.position).normalized * locomotionComponent.Speed * timeComponent.CustomTimeScale;
                    
                    break;
                
                case DoombaAIStates.ROTATING:
                    
                    direction = doombaAIComponent.transform.right;
                    
                    float angle = Vector3.SignedAngle(direction, doombaAIComponent.TargetDirection, doombaAIComponent.transform.forward);
                    
                    if (Mathf.Abs(angle) < 5f)
                    {
                        doombaAIComponent.transform.right = doombaAIComponent.TargetDirection;
                        
                        doombaAIComponent.TargetDirection = Vector3.zero;
                        
                        doombaAIComponent.CurrentState = DoombaAIStates.IDLE;
                        
                        break;
                    }
                    
                    doombaAIComponent.transform.Rotate(0f, 0f, 5f * Mathf.Sign(angle), Space.Self);
                    
                    /*
                    doombaAIComponent.transform.right = Vector3.RotateTowards(
                        doombaAIComponent.transform.right,
                        doombaAIComponent.TargetDirection,
                        doombaAIComponent.RotationSpeed,
                        0.0f);
                    */
                    
                    break;
            }
        }
    }
}