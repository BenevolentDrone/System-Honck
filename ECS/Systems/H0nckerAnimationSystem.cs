using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class H0nckerAnimationSystem : ISystem
{
    private List<Component> h0nckerAnimationComponents;

    private VFXPoolComponent vfxPoolComponent;

    private CameraComponent cameraComponent;

    private CanvasComponent canvasComponent;
    
    private AntagonistComponent antagonistComponent;

    public void Cache(WorldContext worldContext)
    {
        h0nckerAnimationComponents = worldContext.GetComponentsContainer<H0nckerAnimationComponent>();

        vfxPoolComponent = worldContext.Get<VFXPoolComponent>(0);

        cameraComponent = worldContext.Get<CameraComponent>(0);

        canvasComponent = worldContext.Get<CanvasComponent>(0);
        
        antagonistComponent = worldContext.Get<AntagonistComponent>(0);
    }
    
    public void Handle(WorldContext worldContext)
    {
        for (int i = 0; i < h0nckerAnimationComponents.Count; i++)
        {
            var animationComponent = (H0nckerAnimationComponent)h0nckerAnimationComponents[i];
            
            var inputComponent = animationComponent.Entity.GetComponent<InputComponent>();
            
            animationComponent.animator.SetBool("Walking", inputComponent.IsWalking);
            
            animationComponent.animator.SetBool("Flapping wings", inputComponent.IsFlappingWings);
            
            if (inputComponent.Honk)
            {
                animationComponent.animator.SetTrigger("Honk");

                var vfxInstance = vfxPoolComponent.VFXPool.Pop("Message (small)", 0.71f);

                vfxInstance.GameObject.GetComponent<AnimatedPopup>().LeftToRight = inputComponent.LastInputXSign > 0f;

                vfxInstance.GameObject.transform.GetChild(2).GetComponent<Text>().text = "HONK!";

                vfxInstance.GameObject.transform.SetParent(canvasComponent.Canvas.transform, true);

                Transform messageTransform = animationComponent.Entity.GetComponent<BeakComponent>().MessageTransform;

                vfxInstance.GameObject.transform.position = messageTransform.position;

                vfxInstance.GameObject.SetActive(true);

                inputComponent.Honk = false;
            }
            
            if (inputComponent.PickUp)
            {
                animationComponent.animator.SetTrigger("Pick up");
                
                inputComponent.PickUp = false;
                
                var sequence = DOTween.Sequence();
                
                sequence.InsertCallback(0.5f, () => { animationComponent.Entity.GetComponent<BeakComponent>().MagnetActive = true; });
            }
            
            if (inputComponent.Toss)
            {
                inputComponent.Toss = false;

                Vector3 targetPosition = cameraComponent.Camera.ScreenToWorldPoint(Input.mousePosition);

                targetPosition = new Vector3(targetPosition.x, targetPosition.y, animationComponent.transform.position.z);

                var tile = worldContext.Get<TileFieldComponent>(0).TileField.GetTile(new Vector2(targetPosition.x, targetPosition.y));

                if (tile == null)
                    break;

                animationComponent.animator.SetTrigger("Honk");
                
                var beakComponent = animationComponent.Entity.GetComponent<BeakComponent>();
                
                beakComponent.MagnetActive = true;
                
                var target = beakComponent.Contents;
                
                var tossedComponent = target.gameObject.AddComponent<TossedComponent>();
                
                tossedComponent.StartPoint = animationComponent.transform.position;
                
                tossedComponent.EndPoint = targetPosition;
                
                float distance = (targetPosition - animationComponent.transform.position).magnitude;
                
                float duration = distance / beakComponent.TossSpeed;
                
                var sequence = DOTween.Sequence();

                tossedComponent.Sequence = sequence;

                sequence.Append(target.transform.DOJump(targetPosition, 1f, 1, duration).SetEase(Ease.Linear));
                
                sequence.InsertCallback(
                    duration,
                    () => 
                    {
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
                            GameObject.Destroy(tossedComponent);
                    });
            }
            
            var scale = animationComponent.transform.localScale;
            
            scale.x = inputComponent.LastInputXSign * Mathf.Abs(scale.x);
            
            animationComponent.transform.localScale = scale;
        }
    }
}