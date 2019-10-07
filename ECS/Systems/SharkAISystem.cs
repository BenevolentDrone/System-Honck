using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class SharkAISystem : ISystem
{
    private List<Component> sharkAIComponents;
    
    private Entity h0ncker;
    
    private PositionComponent playersPositionComponent;
    
    private TimeComponent timeComponent;
    
    private VFXPoolComponent vfxPoolComponent;

    private AntagonistComponent antagonistComponent;

    private int obstaclesLayer;

    private int enemiesLayer;

    private int itemsLayer;

    private int playerLayer;

    public void Cache(WorldContext worldContext)
    {
        sharkAIComponents = worldContext.GetComponentsContainer<SharkAIComponent>();
        
        h0ncker = worldContext.Get<InputComponent>(0).Entity;
        
        playersPositionComponent = h0ncker.GetComponent<PositionComponent>();
        
        timeComponent = worldContext.Get<TimeComponent>(0);
        
        vfxPoolComponent = worldContext.Get<VFXPoolComponent>(0);

        antagonistComponent = worldContext.Get<AntagonistComponent>(0);

        obstaclesLayer = LayerMask.NameToLayer("Obstacles");

        itemsLayer = LayerMask.NameToLayer("Items");

        enemiesLayer = LayerMask.NameToLayer("Enemies");

        playerLayer = LayerMask.NameToLayer("Player");
    }
    
    public void Handle(WorldContext worldContext)
    {
        for (int i = 0; i < sharkAIComponents.Count; i++)
        {
            var sharkAIComponent = (SharkAIComponent)sharkAIComponents[i];
            
            Vector3 scale;
            
            switch (sharkAIComponent.CurrentState)
            {
                case SharkAIStates.IDLE:

                    var positionComponent = sharkAIComponent.Entity.GetComponent<PositionComponent>();
                    
                    var locomotionComponent = sharkAIComponent.Entity.GetComponent<LocomotionComponent>();
                    
                    var distance = (positionComponent.RectTransform.anchoredPosition - playersPositionComponent.RectTransform.anchoredPosition).magnitude;
                    
                    if (distance < sharkAIComponent.VisionRadius)
                    {
                        sharkAIComponent.CurrentState = SharkAIStates.SHOOTING;
                        
                        sharkAIComponent.CurrentCooldown = sharkAIComponent.ShootingDuration;
                        
                        locomotionComponent.Velocity = Vector2.zero;
                        
                        Vector2 direction = playersPositionComponent.RectTransform.anchoredPosition - positionComponent.RectTransform.anchoredPosition;
                        
                        sharkAIComponent.LazerDirection = new Vector3(direction.x, direction.y, 0f);
                        
                        RaycastHit hitInfo;
                        
                        Physics.Raycast(
                            sharkAIComponent.transform.position,
                            sharkAIComponent.LazerDirection,
                            out hitInfo,
                            100f,
                            obstaclesLayer,
                            QueryTriggerInteraction.Ignore);
                        
                        sharkAIComponent.EndPoint = hitInfo.Equals(default(RaycastHit))
                            ? sharkAIComponent.transform.position + sharkAIComponent.LazerDirection.normalized * 100f
                            : hitInfo.point;
                        
                        var vfxInstance = vfxPoolComponent.VFXPool.Pop("Lazer beam", sharkAIComponent.ShootingDuration);
                        
                        vfxInstance.GameObject.transform.position = Vector3.zero;
                        
                        var lineRenderer = vfxInstance.GameObject.GetComponent<LineRenderer>();
                        
                        lineRenderer.SetPosition(0, sharkAIComponent.transform.position);
                        
                        lineRenderer.SetPosition(1, sharkAIComponent.EndPoint);
                        
                        Color redTransparent = new Color(1f, 0f, 0f, 0f);
                        
                        var sequence = DOTween.Sequence();
                        
                        sequence.Append(lineRenderer.DOColor(new Color2(redTransparent, redTransparent), new Color2(Color.red, Color.red), sharkAIComponent.LazerHeatingUpDuration));
                        
                        sequence.InsertCallback(sharkAIComponent.ShootingDuration, () => { vfxInstance.GameObject.SetActive(false); });
                        
                        vfxInstance.GameObject.SetActive(true);
                        
                        sharkAIComponent.LazerBeam = vfxInstance.GameObject;
                        
                        scale = sharkAIComponent.transform.localScale;
                        
                        scale.x = Mathf.Sign(positionComponent.RectTransform.anchoredPosition.x - playersPositionComponent.RectTransform.anchoredPosition.x) * Mathf.Abs(scale.x);
                        
                        sharkAIComponent.transform.localScale = scale;
                        
                        break;
                    }
                    
                    var velocity = locomotionComponent.Speed * sharkAIComponent.CurrentDirection * timeComponent.CustomTimeScale;

                    /*
                    var boxCollider = sharkAIComponent.GetComponent<BoxCollider>();

                    Vector3 center = sharkAIComponent.transform.position + boxCollider.center;

                    Vector3 size = boxCollider.size;

                    if (Physics.BoxCast(
                            center,
                            size / 2f,
                            new Vector3(velocity, 0f, 0f),
                            Quaternion.identity,
                            Mathf.Abs(velocity) / 2f,
                            obstaclesLayer,
                            QueryTriggerInteraction.Ignore))
                        sharkAIComponent.CurrentDirection *= -1f;
                        */

                    sharkAIComponent.CurrentCooldown += Time.deltaTime * timeComponent.CustomTimeScale;

                    if (sharkAIComponent.CurrentCooldown > 5f)
                    {
                        sharkAIComponent.CurrentCooldown = 0f;

                        sharkAIComponent.CurrentDirection *= -1f;
                    }
                    
                    scale = sharkAIComponent.transform.localScale;
                    
                    scale.x = -sharkAIComponent.CurrentDirection * Mathf.Abs(scale.x);
                    
                    sharkAIComponent.transform.localScale = scale;
                    
                    locomotionComponent.Velocity = new Vector2(locomotionComponent.Speed * sharkAIComponent.CurrentDirection, 0f);
                    
                    break;
                
                case SharkAIStates.SHOOTING:
                    positionComponent = sharkAIComponent.Entity.GetComponent<PositionComponent>();
                    
                    bool startsUsingLaser = sharkAIComponent.CurrentCooldown < sharkAIComponent.ShootingDuration - sharkAIComponent.LazerHeatingUpDuration;
                    
                    sharkAIComponent.CurrentCooldown -= Time.deltaTime * timeComponent.CustomTimeScale;
                    
                    if (!startsUsingLaser && sharkAIComponent.CurrentCooldown < sharkAIComponent.ShootingDuration - sharkAIComponent.LazerHeatingUpDuration)
                    {
                        startsUsingLaser = true;
                        
                        sharkAIComponent.LazerBeam.transform.DOShakePosition(
                            sharkAIComponent.CurrentCooldown,
                            new Vector3(0.05f, 0.05f, 0f),
                            20,
                            fadeOut: false);
                    }
                    
                    if (sharkAIComponent.CurrentCooldown < sharkAIComponent.ShootingDuration - sharkAIComponent.LazerHeatingUpDuration)
                    {
                        var lineRenderer = sharkAIComponent.LazerBeam.GetComponent<LineRenderer>();
                        
                        lineRenderer.SetPosition(0, sharkAIComponent.transform.position + sharkAIComponent.LazerBeam.transform.localPosition);
                        
                        lineRenderer.SetPosition(1, sharkAIComponent.EndPoint + sharkAIComponent.LazerBeam.transform.localPosition);
                    }


                    if (sharkAIComponent.CurrentCooldown < sharkAIComponent.ShootingDuration - sharkAIComponent.LazerHeatingUpDuration)
                    {
                        RaycastHit[] hits = Physics.RaycastAll(
                            sharkAIComponent.transform.position,
                            sharkAIComponent.LazerDirection,
                            sharkAIComponent.LazerDirection.magnitude,
                            LayerMask.GetMask("Items", "Enemies", "Player"));

                        for (int j = 0; j < hits.Length; j++)
                        {

                            var collision = hits[j].collider;

                            if (collision == null)
                                continue;

                            if (collision.gameObject == sharkAIComponent.gameObject)
                                continue;

                            if (collision.gameObject.layer == playerLayer)
                            {
                                worldContext.Get<GameStateComponent>(0).GameState = GameState.DEFEAT;

                                return;
                            }

                            if (collision.gameObject.layer == itemsLayer)
                            {
                                var target = collision.gameObject.GetComponent<Entity>();

                                if (target.HasComponent<BreakableComponent>())
                                {
                                    var particlesFX = vfxPoolComponent.VFXPool.Pop("Shattering", 3f);

                                    particlesFX.GameObject.transform.position = target.transform.position;

                                    particlesFX.GameObject.SetActive(true);

                                    antagonistComponent.Score += antagonistComponent.Settings.ScorePerItemBroken;

                                    var vfxInstance = vfxPoolComponent.VFXPool.Pop("Score message", 1.6f);

                                    vfxInstance.GameObject.transform.position = target.transform.position;

                                    vfxInstance.GameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = string.Format("+{0}", antagonistComponent.Settings.ScorePerItemBroken);

                                    vfxInstance.GameObject.SetActive(true);

                                    GameObject.Destroy(target.gameObject);
                                }
                                else
                                    GameObject.Destroy(target.gameObject);
                            }

                            if (collision.gameObject.layer == enemiesLayer)
                            {
                                var particlesFX = vfxPoolComponent.VFXPool.Pop("Shattering", 3f);

                                particlesFX.GameObject.transform.position = collision.gameObject.transform.position;

                                particlesFX.GameObject.SetActive(true);

                                antagonistComponent.Score += antagonistComponent.Settings.ScorePerUnitKilled;

                                var vfxInstance = vfxPoolComponent.VFXPool.Pop("Score message", 1.6f);

                                vfxInstance.GameObject.transform.position = collision.gameObject.transform.position;

                                vfxInstance.GameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = string.Format("+{0}", antagonistComponent.Settings.ScorePerUnitKilled);

                                vfxInstance.GameObject.SetActive(true);

                                GameObject.Destroy(collision.gameObject);
                            }
                        }
                    }
                    
                    if (sharkAIComponent.CurrentCooldown < 0f)
                    {
                        sharkAIComponent.CurrentState = SharkAIStates.COOLDOWN;
                        
                        sharkAIComponent.CurrentCooldown = sharkAIComponent.CooldownDuration;
                        
                        sharkAIComponent.LazerDirection = Vector3.zero;
                        
                        sharkAIComponent.EndPoint = Vector3.zero;
                        
                        sharkAIComponent.LazerBeam = null;
                        
                        break;
                    }
                    
                    break;
                
                case SharkAIStates.COOLDOWN:
                    positionComponent = sharkAIComponent.Entity.GetComponent<PositionComponent>();
                    
                    distance = (positionComponent.RectTransform.anchoredPosition - playersPositionComponent.RectTransform.anchoredPosition).magnitude;
                    
                    if (distance < sharkAIComponent.VisionRadius)
                    {
                        sharkAIComponent.CurrentState = SharkAIStates.SHOOTING;
                        
                        sharkAIComponent.CurrentCooldown = sharkAIComponent.ShootingDuration;
                        
                        Vector2 direction = playersPositionComponent.RectTransform.anchoredPosition - positionComponent.RectTransform.anchoredPosition;
                        
                        sharkAIComponent.LazerDirection = new Vector3(direction.x, direction.y, 0f);
                        
                        RaycastHit hitInfo;
                        
                        Physics.Raycast(
                            sharkAIComponent.transform.position,
                            sharkAIComponent.LazerDirection,
                            out hitInfo,
                            100f,
                            obstaclesLayer,
                            QueryTriggerInteraction.Ignore);

                        sharkAIComponent.EndPoint = hitInfo.Equals(default(RaycastHit))
                            ? sharkAIComponent.transform.position + sharkAIComponent.LazerDirection.normalized * 100f
                            : hitInfo.point;

                        var vfxInstance = vfxPoolComponent.VFXPool.Pop("Lazer beam", sharkAIComponent.ShootingDuration);
                        
                        var lineRenderer = vfxInstance.GameObject.GetComponent<LineRenderer>();
                        
                        lineRenderer.SetPosition(0, sharkAIComponent.transform.position);
                        
                        lineRenderer.SetPosition(1, sharkAIComponent.EndPoint);
                        
                        Color redTransparent = new Color(1f, 0f, 0f, 0f);
                        
                        var sequence = DOTween.Sequence();
                        
                        sequence.Append(lineRenderer.DOColor(new Color2(redTransparent, redTransparent), new Color2(Color.red, Color.red), sharkAIComponent.LazerHeatingUpDuration));
                        
                        sequence.InsertCallback(sharkAIComponent.ShootingDuration, () => { vfxInstance.GameObject.SetActive(false); });
                        
                        vfxInstance.GameObject.SetActive(true);
                        
                        sharkAIComponent.LazerBeam = vfxInstance.GameObject;
                        
                        scale = sharkAIComponent.transform.localScale;
                        
                        scale.x = Mathf.Sign(positionComponent.RectTransform.anchoredPosition.x - playersPositionComponent.RectTransform.anchoredPosition.x) * Mathf.Abs(scale.x);
                        
                        sharkAIComponent.transform.localScale = scale;
                        
                        break;
                    }
                    
                    sharkAIComponent.CurrentCooldown -= Time.deltaTime * timeComponent.CustomTimeScale;
                    
                    if (sharkAIComponent.CurrentCooldown < 0f)
                    {
                        sharkAIComponent.CurrentState = SharkAIStates.IDLE;
                        
                        sharkAIComponent.CurrentCooldown = 0;
                        
                        break;
                    }
                    
                    break;
            }
        }
    }
}