using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProjectileSystem : ISystem
{
    private List<Component> projectileComponents;

    private TimeComponent timeComponent;

    private int obstacleLayer;

    private int enemiesLayer;

    private int itemsLayer;

    private int playerLayer;

    private VFXPoolComponent vfxPoolComponent;

    private AntagonistComponent antagonistComponent;

    public void Cache(WorldContext worldContext)
    {
        projectileComponents = worldContext.GetComponentsContainer<ProjectileComponent>();

        timeComponent = worldContext.Get<TimeComponent>(0);

        vfxPoolComponent = worldContext.Get<VFXPoolComponent>(0);

        antagonistComponent = worldContext.Get<AntagonistComponent>(0);

        obstacleLayer = LayerMask.NameToLayer("Obstacles");

        itemsLayer = LayerMask.NameToLayer("Items");

        enemiesLayer = LayerMask.NameToLayer("Enemies");

        playerLayer = LayerMask.NameToLayer("Player");
    }

    public void Handle(WorldContext worldContext)
    {
        for (int i = projectileComponents.Count - 1; i >= 0; i--)
        {
            var projectileComponent = (ProjectileComponent)projectileComponents[i];

            var triggerComponent = projectileComponent.Entity.GetComponent<TriggerComponent>();

            triggerComponent.Collisions.RemoveAll(item => item == null);

            bool alive = true;

            //foreach (var collision in triggerComponent.Collisions)
            for (int j = 0; j < triggerComponent.Collisions.Count; j++)
            {
                var collision = triggerComponent.Collisions[j];

                if (collision == null)
                    continue;

                if (collision.gameObject.layer == obstacleLayer)
                    alive = false;

                if (collision.gameObject == projectileComponent.Source.gameObject)
                    continue;

                if (collision.gameObject.layer == playerLayer)
                {
                    worldContext.Get<GameStateComponent>(0).GameState = GameState.DEFEAT;

                    return;
                }

                if (collision.gameObject.layer == itemsLayer)
                {
                    alive = false;

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

            if (!alive)
                GameObject.Destroy(projectileComponent.gameObject);
        }
    }
}