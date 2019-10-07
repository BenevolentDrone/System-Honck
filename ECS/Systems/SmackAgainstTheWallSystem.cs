using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SmackAgainstTheWallSystem : ISystem
{
    private List<Component> tossedComponents;

    private VFXPoolComponent vfxPoolComponent;

    private AntagonistComponent antagonistComponent;

    private int obstacleLayer;

    private int enemiesLayer;

    public void Cache(WorldContext worldContext)
    {
        tossedComponents = worldContext.GetComponentsContainer<TossedComponent>();

        vfxPoolComponent = worldContext.Get<VFXPoolComponent>(0);

        antagonistComponent = worldContext.Get<AntagonistComponent>(0);

        obstacleLayer = LayerMask.NameToLayer("Obstacles");

        enemiesLayer = LayerMask.NameToLayer("Enemies");
    }

    public void Handle(WorldContext worldContext)
    {
        for (int i = 0; i < tossedComponents.Count; i++)
        {
            var tossedComponent = (TossedComponent)tossedComponents[i];

            var target = tossedComponent.Entity;

            if (!target.HasComponent<TriggerComponent>())
                continue;

            var triggerComponent = target.GetComponent<TriggerComponent>();

            triggerComponent.Collisions.RemoveAll(item => item == null);

            var targetCollider = triggerComponent.Collisions.FirstOrDefault(collider =>
                collider.gameObject.layer == obstacleLayer
                || collider.gameObject.layer == enemiesLayer);

            if (targetCollider != null)
            {
                tossedComponent.Sequence.Kill();

                if (targetCollider.gameObject.layer == enemiesLayer)
                {
                    var enemy = targetCollider.gameObject;

                    var bleedingComponent = enemy.GetComponent<BleedingComponent>();

                    if (bleedingComponent != null)
                    {
                        var positionOnTileComponent = enemy.GetComponent<PositionOnTileComponent>();

                        positionOnTileComponent.CurrentTile.Surface = bleedingComponent.Surface;

                        antagonistComponent.Score += antagonistComponent.Settings.ScorePerDirtyTile;

                        var vfxInstance2 = vfxPoolComponent.VFXPool.Pop("Score message", 1.6f);

                        vfxInstance2.GameObject.transform.position = positionOnTileComponent.CurrentTile.transform.position;

                        vfxInstance2.GameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = string.Format("+{0}", antagonistComponent.Settings.ScorePerDirtyTile);

                        vfxInstance2.GameObject.SetActive(true);
                    }
                }

                GameObject.Destroy(tossedComponent);
            }
        }
    }
}