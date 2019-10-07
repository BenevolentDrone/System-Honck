using System.Collections.Generic;
using UnityEngine;

public class PositionOnTileSystem : ISystem
{
    private List<Component> positionOnTileComponents;
    
    private TileFieldComponent tileFieldComponent;
    
    public void Cache(WorldContext worldContext)
    {
        positionOnTileComponents = worldContext.GetComponentsContainer<PositionOnTileComponent>();
        
        tileFieldComponent = worldContext.Get<TileFieldComponent>(0);
    }
    
    public void Handle(WorldContext worldContext)
    {
        for (int i = 0; i < positionOnTileComponents.Count; i++)
        {
            var positionOnTileComponent = (PositionOnTileComponent)positionOnTileComponents[i];
            
            var positionComponent = positionOnTileComponent.Entity.GetComponent<PositionComponent>();
            
            int intPosition = tileFieldComponent.TileField.ToIntCoordinates(positionComponent.RectTransform.anchoredPosition);
            
            if (positionOnTileComponent.CurrentTile == null
                || positionOnTileComponent.CurrentTile.Coordinates != intPosition)
            {
                if (positionOnTileComponent.CurrentTile != null)
                {
                    positionOnTileComponent.CurrentTile.Entities.Remove(positionOnTileComponent.Entity);
                    
                    positionOnTileComponent.CurrentTile = null;
                }
                
                var newTile = tileFieldComponent.TileField.GetTile(intPosition);
                
                if (newTile != null)
                {
                    positionOnTileComponent.CurrentTile = newTile;
                    
                    positionOnTileComponent.CurrentTile.Entities.Add(positionOnTileComponent.Entity);
                }
            }
        }
    }
}