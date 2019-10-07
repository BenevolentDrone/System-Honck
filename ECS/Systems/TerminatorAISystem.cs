using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

public class TerminatorAISystem : ISystem
{
    private List<Component> terminatorAIComponents;
    
    private Entity h0ncker;
    
    private PositionComponent playersPositionComponent;
    
    private TimeComponent timeComponent;
    
    private VFXPoolComponent vfxPoolComponent;
    
    private int obstaclesLayer;
    
    private int playerLayer;
    
    private NavMeshPath navMeshPath;
    
    public void Cache(WorldContext worldContext)
    {
        terminatorAIComponents = worldContext.GetComponentsContainer<TerminatorAIComponent>();
        
        h0ncker = worldContext.Get<InputComponent>(0).Entity;
        
        playersPositionComponent = h0ncker.GetComponent<PositionComponent>();
        
        timeComponent = worldContext.Get<TimeComponent>(0);
        
        vfxPoolComponent = worldContext.Get<VFXPoolComponent>(0);
        
        obstaclesLayer = LayerMask.GetMask("Obstacles");
        
        playerLayer = LayerMask.GetMask("Player");
        
        navMeshPath = new NavMeshPath();
    }
    
    public void Handle(WorldContext worldContext)
    {
        for (int i = 0; i < terminatorAIComponents.Count; i++)
        {
            var terminatorAIComponent = (TerminatorAIComponent)terminatorAIComponents[i];
            
            if (terminatorAIComponent.PostCoordinates.magnitude < 0.1f)
                terminatorAIComponent.PostCoordinates = terminatorAIComponent.Entity.GetComponent<PositionComponent>().RectTransform.anchoredPosition;
            
            Vector3 scale;
            
            switch (terminatorAIComponent.CurrentState)
            {
                case TerminatorAIStates.IDLE:
                    
                    var positionComponent = terminatorAIComponent.Entity.GetComponent<PositionComponent>();
                    
                    var locomotionComponent = terminatorAIComponent.Entity.GetComponent<LocomotionComponent>();
                    
                    var distance = (positionComponent.RectTransform.anchoredPosition - playersPositionComponent.RectTransform.anchoredPosition).magnitude;
                    
                    if (distance < terminatorAIComponent.VisionRadius)
                    {
                        terminatorAIComponent.CurrentState = TerminatorAIStates.SHOOTING;
                        
                        terminatorAIComponent.CurrentCooldown = terminatorAIComponent.ShootingDuration;
                        
                        locomotionComponent.Velocity = Vector2.zero;
                        
                        Vector2 direction = playersPositionComponent.RectTransform.anchoredPosition - positionComponent.RectTransform.anchoredPosition;

                        
                        Vector2 spawnPosition = positionComponent.RectTransform.anchoredPosition; // + direction.normalized * 0.6f;
                        var bullet = GameObject.Instantiate(
                            terminatorAIComponent.BulletPrefab,
                            new Vector3(spawnPosition.x, spawnPosition.y, 0f),
                            Quaternion.identity) as GameObject;
                        var bulletEntity = bullet.GetComponent<Entity>();
                        bulletEntity.WorldContext = worldContext;
                        var bulletsLocomotion = bullet.GetComponent<LocomotionComponent>();
                        bulletsLocomotion.Velocity = direction.normalized * bulletsLocomotion.Speed * timeComponent.CustomTimeScale;
                        bullet.GetComponent<ProjectileComponent>().Source = terminatorAIComponent.Entity;


                        terminatorAIComponent.MemorizedPlayer = true;
                        
                        terminatorAIComponent.LastKnownPlayerPosition = playersPositionComponent.RectTransform.anchoredPosition;
                        
                        scale = terminatorAIComponent.transform.localScale;
                        
                        scale.x = -Mathf.Sign(positionComponent.RectTransform.anchoredPosition.x - playersPositionComponent.RectTransform.anchoredPosition.x) * Mathf.Abs(scale.x);
                        
                        terminatorAIComponent.transform.localScale = scale;
                        
                        break;
                    }
                    
                    break;
                
                case TerminatorAIStates.SHOOTING:
                    
                    positionComponent = terminatorAIComponent.Entity.GetComponent<PositionComponent>();
                    
                    scale = terminatorAIComponent.transform.localScale;
                    
                    scale.x = -Mathf.Sign(positionComponent.RectTransform.anchoredPosition.x - playersPositionComponent.RectTransform.anchoredPosition.x) * Mathf.Abs(scale.x);
                    
                    terminatorAIComponent.transform.localScale = scale;
                    
                    
                    terminatorAIComponent.CurrentCooldown -= Time.deltaTime * timeComponent.CustomTimeScale;
                    
                    if (terminatorAIComponent.CurrentCooldown < 0f)
                    {
                        terminatorAIComponent.CurrentState = TerminatorAIStates.COOLDOWN;
                        
                        terminatorAIComponent.CurrentCooldown = terminatorAIComponent.CooldownDuration;
                        
                        break;
                    }
                    
                    break;
                
                case TerminatorAIStates.COOLDOWN:
                    
                    positionComponent = terminatorAIComponent.Entity.GetComponent<PositionComponent>();
                    
                    distance = (positionComponent.RectTransform.anchoredPosition - playersPositionComponent.RectTransform.anchoredPosition).magnitude;
                    
                    if (distance < terminatorAIComponent.VisionRadius)
                    {
                        terminatorAIComponent.CurrentState = TerminatorAIStates.SHOOTING;
                        
                        terminatorAIComponent.CurrentCooldown = terminatorAIComponent.ShootingDuration;
                        
                        Vector2 direction = playersPositionComponent.RectTransform.anchoredPosition - positionComponent.RectTransform.anchoredPosition;
                        

                        Vector2 spawnPosition = positionComponent.RectTransform.anchoredPosition; // + direction.normalized * 0.6f;
                        var bullet = GameObject.Instantiate(
                            terminatorAIComponent.BulletPrefab,
                            new Vector3(spawnPosition.x, spawnPosition.y, 0f),
                            Quaternion.identity) as GameObject;
                        var bulletEntity = bullet.GetComponent<Entity>();
                        bulletEntity.WorldContext = worldContext;
                        var bulletsLocomotion = bullet.GetComponent<LocomotionComponent>();
                        bulletsLocomotion.Velocity = direction.normalized * bulletsLocomotion.Speed * timeComponent.CustomTimeScale;
                        bullet.GetComponent<ProjectileComponent>().Source = terminatorAIComponent.Entity;


                        terminatorAIComponent.MemorizedPlayer = true;
                        
                        terminatorAIComponent.LastKnownPlayerPosition = playersPositionComponent.RectTransform.anchoredPosition;
                        
                        scale = terminatorAIComponent.transform.localScale;
                        
                        scale.x = -Mathf.Sign(positionComponent.RectTransform.anchoredPosition.x - playersPositionComponent.RectTransform.anchoredPosition.x) * Mathf.Abs(scale.x);
                        
                        terminatorAIComponent.transform.localScale = scale;
                        
                        break;
                    }
                    
                    if (terminatorAIComponent.MemorizedPlayer)
                    {
                        terminatorAIComponent.MemorizedPlayer = false;
                        
                        var path = NavMesh.CalculatePath(
                            terminatorAIComponent.transform.position,
                            terminatorAIComponent.LastKnownPlayerPosition,
                            NavMesh.AllAreas,
                            navMeshPath);
                        
                        for (int j = navMeshPath.corners.Length - 1; j >= 0; j--)
                            terminatorAIComponent.Path.Push(navMeshPath.corners[j]);
                        
                        terminatorAIComponent.LastKnownPlayerPosition = Vector3.zero;
                        
                        terminatorAIComponent.CurrentState = TerminatorAIStates.WALKING;
                    }
                    
                    terminatorAIComponent.CurrentCooldown -= Time.deltaTime * timeComponent.CustomTimeScale;
                    
                    if (terminatorAIComponent.CurrentCooldown < 0f)
                    {
                        terminatorAIComponent.CurrentCooldown = 0;
                        
                        distance = (positionComponent.RectTransform.anchoredPosition - terminatorAIComponent.PostCoordinates).magnitude;
                        
                        if (distance > 0.05f)
                        {
                            var path = NavMesh.CalculatePath(
                            terminatorAIComponent.transform.position,
                            terminatorAIComponent.PostCoordinates,
                            NavMesh.AllAreas,
                            navMeshPath);
                        
                        for (int j = navMeshPath.corners.Length - 1; j >= 0; j--)
                            terminatorAIComponent.Path.Push(navMeshPath.corners[j]);
                        
                        terminatorAIComponent.CurrentState = TerminatorAIStates.WALKING;
                        }
                        else
                            terminatorAIComponent.CurrentState = TerminatorAIStates.IDLE;
                        
                        break;
                    }
                    
                    break;
                
                case TerminatorAIStates.WALKING:
                    
                    positionComponent = terminatorAIComponent.Entity.GetComponent<PositionComponent>();
                    
                    locomotionComponent = terminatorAIComponent.Entity.GetComponent<LocomotionComponent>();
                    
                    distance = (positionComponent.RectTransform.anchoredPosition - playersPositionComponent.RectTransform.anchoredPosition).magnitude;
                    
                    if (distance < terminatorAIComponent.VisionRadius)
                    {
                        terminatorAIComponent.Path.Clear();
                        
                        locomotionComponent.Velocity = Vector2.zero;
                        
                        terminatorAIComponent.CurrentState = TerminatorAIStates.SHOOTING;
                        
                        terminatorAIComponent.CurrentCooldown = terminatorAIComponent.ShootingDuration;
                        
                        Vector2 direction = playersPositionComponent.RectTransform.anchoredPosition - positionComponent.RectTransform.anchoredPosition;

                        
                        Vector2 spawnPosition = positionComponent.RectTransform.anchoredPosition; // + direction.normalized * 0.6f;
                        var bullet = GameObject.Instantiate(
                            terminatorAIComponent.BulletPrefab,
                            new Vector3(spawnPosition.x, spawnPosition.y, 0f),
                            Quaternion.identity) as GameObject;
                        var bulletEntity = bullet.GetComponent<Entity>();
                        bulletEntity.WorldContext = worldContext;
                        var bulletsLocomotion = bullet.GetComponent<LocomotionComponent>();
                        bulletsLocomotion.Velocity = direction.normalized * bulletsLocomotion.Speed * timeComponent.CustomTimeScale;
                        bullet.GetComponent<ProjectileComponent>().Source = terminatorAIComponent.Entity;


                        terminatorAIComponent.MemorizedPlayer = true;
                        
                        terminatorAIComponent.LastKnownPlayerPosition = playersPositionComponent.RectTransform.anchoredPosition;
                        
                        scale = terminatorAIComponent.transform.localScale;
                        
                        scale.x = -Mathf.Sign(positionComponent.RectTransform.anchoredPosition.x - playersPositionComponent.RectTransform.anchoredPosition.x) * Mathf.Abs(scale.x);
                        
                        terminatorAIComponent.transform.localScale = scale;
                        
                        break;
                    }
                    
                    if (terminatorAIComponent.Path.Count == 0)
                    {
                        terminatorAIComponent.CurrentState = TerminatorAIStates.COOLDOWN;
                        
                        terminatorAIComponent.CurrentCooldown = terminatorAIComponent.CooldownDuration;
                        
                        locomotionComponent.Velocity = Vector2.zero;
                        
                        break;
                    }
                    
                    Vector2 currentPosition = positionComponent.RectTransform.anchoredPosition;
                    
                    Vector3 nextPoint = terminatorAIComponent.Path.Peek();
                    
                    Vector2 nextPoint2D = new Vector2(nextPoint.x, nextPoint.y);
                    
                    while ((nextPoint2D - currentPosition).magnitude < 0.05f)
                    {
                        terminatorAIComponent.Path.Pop();
                        
                        if (terminatorAIComponent.Path.Count == 0)
                            break;
                        
                        nextPoint = terminatorAIComponent.Path.Peek();
                        
                        nextPoint2D = new Vector2(nextPoint.x, nextPoint.y);
                    }
                    
                    if (terminatorAIComponent.Path.Count == 0)
                    {
                        terminatorAIComponent.CurrentState = TerminatorAIStates.COOLDOWN;
                        
                        terminatorAIComponent.CurrentCooldown = terminatorAIComponent.CooldownDuration;
                        
                        locomotionComponent.Velocity = Vector2.zero;
                        
                        break;
                    }
                    
                    locomotionComponent.Velocity = (nextPoint2D - currentPosition).normalized * locomotionComponent.Speed * timeComponent.CustomTimeScale;
                    
                    break;
            }
        }
    }
}