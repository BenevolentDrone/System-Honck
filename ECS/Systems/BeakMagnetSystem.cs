using System.Collections.Generic;
using UnityEngine;

public class BeakMagnetSystem : ISystem
{
    private List<Component> beakComponents;
    
    private TileFieldComponent tileFieldComponent;

    private List<Tile> tilesInRange = new List<Tile>();
    
    public void Cache(WorldContext worldContext)
    {
        beakComponents = worldContext.GetComponentsContainer<BeakComponent>();
        
        tileFieldComponent = worldContext.Get<TileFieldComponent>(0);
    }
    
    public void Handle(WorldContext worldContext)
    {
        for (int i = 0; i < beakComponents.Count; i++)
        {
            var beakComponent = (BeakComponent)beakComponents[i];
            
            if (!beakComponent.MagnetActive)
                continue;

            if (beakComponent.Contents != null)
            {
                Vector2 targetPosition = tileFieldComponent.TileField.GetCellCoordinates(
                    new Vector2(beakComponent.ParentTransform.position.x, beakComponent.Entity.GetComponent<PositionComponent>().RectTransform.position.y)); //beakComponent.ParentTransform.position.y));
                
                var target = beakComponent.Contents;
                
                var targetsTransform = target.GetComponent<PositionComponent>().RectTransform;
                
                targetsTransform.SetParent(null, true);

                targetsTransform.anchoredPosition = targetPosition;
                
                GameObject.Destroy(target.GetComponent<CarriedComponent>());
                
                beakComponent.Contents = null;

                beakComponent.MagnetActive = false;

                continue;
            }

            tileFieldComponent.TileField.GetTilesInRange(
                new Vector2(beakComponent.ParentTransform.position.x, beakComponent.ParentTransform.position.y), 
                beakComponent.MagnetRadius, 
                tilesInRange);

            foreach (var tile in tilesInRange)
            {
                var entitiesOnTile = tile.Entities;

                foreach (var entity in entitiesOnTile)
                    if (entity.HasComponent<PickUpableComponent>())
                    {
                        beakComponent.Contents = entity;

                        var targetsTransform = entity.GetComponent<PositionComponent>().RectTransform;

                        targetsTransform.SetParent(beakComponent.ParentTransform, true);

                        targetsTransform.localPosition = Vector3.zero;
                        
                        entity.gameObject.AddComponent<CarriedComponent>();
                        
                        break;
                    }

                if (beakComponent.Contents != null)
                    break;
            }
            
            beakComponent.MagnetActive = false;
        }
    }
}