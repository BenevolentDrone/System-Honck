using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AntagonistSystem : ISystem
{
    private List<Component> antagonistComponents;

    private GameStateComponent gameStateComponent;

    private VFXPoolComponent vfxPoolComponent;

    private CameraComponent cameraComponent;

    private CanvasComponent canvasComponent;

    public void Cache(WorldContext worldContext)
    {
        antagonistComponents = worldContext.GetComponentsContainer<AntagonistComponent>();

        gameStateComponent = worldContext.Get<GameStateComponent>(0);

        vfxPoolComponent = worldContext.Get<VFXPoolComponent>(0);

        cameraComponent = worldContext.Get<CameraComponent>(0);

        canvasComponent = worldContext.Get<CanvasComponent>(0);
    }

    public void Handle(WorldContext worldContext)
    {
        for (int i = 0; i < antagonistComponents.Count; i++)
        {
            var antagonistComponent = (AntagonistComponent)antagonistComponents[i];

            if (antagonistComponent.IsMonologuing)
                continue;

            if (antagonistComponent.CurrentStage >= antagonistComponent.Settings.StageSettings.Length)
            {
                worldContext.Get<GameStateComponent>(0).GameState = GameState.VICTORY;

                return;
            }

            var currentStage = antagonistComponent.Settings.StageSettings[antagonistComponent.CurrentStage];

            int scoreCap = currentStage.ScoreCap;

            if (antagonistComponent.Score >= scoreCap)
            {
                gameStateComponent.GameState = GameState.PAUSED;

                var vfxInstance = vfxPoolComponent.VFXPool.Pop("Message (long)", currentStage.TextTypingDuration + 0.41f);

                var typer = vfxInstance.GameObject.GetComponent<Typer>();

                typer.Text = currentStage.TextVariants[UnityEngine.Random.Range(0, currentStage.TextVariants.Length)];

                typer.TextColor = currentStage.TextColor;

                vfxInstance.GameObject.transform.SetParent(canvasComponent.Canvas.transform, true);

                Transform messageTransform = antagonistComponent.MessageTransform;

                vfxInstance.GameObject.transform.position = messageTransform.position;

                vfxInstance.GameObject.GetComponent<AnimatedPopup>().StayDuration = currentStage.TextTypingDuration;

                Sequence sequence = DOTween.Sequence();

                sequence.Append(cameraComponent.Camera.transform.DOShakePosition(
                    currentStage.TextTypingDuration + 0.41f,
                    new Vector3(1f, 1f, 0f).normalized * currentStage.CameraShakeStrength, 15));

                sequence.InsertCallback(
                    currentStage.TextTypingDuration + 0.41f,
                    () =>
                    {
                        gameStateComponent.GameState = GameState.RUNNING;

                        antagonistComponent.IsMonologuing = false;
                    });

                vfxInstance.GameObject.SetActive(true);

                antagonistComponent.CurrentStage++;

                antagonistComponent.IsMonologuing = true;
            }
        }
    }
}