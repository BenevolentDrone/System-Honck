using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FeetSystem : ISystem
{
    private List<Component> feetComponents;
    
    private TileFieldComponent tileFieldComponent;
    
    private AntagonistComponent antagonistComponent;
    
    private VFXPoolComponent vfxPoolComponent;
    
    public void Cache(WorldContext worldContext)
    {
        feetComponents = worldContext.GetComponentsContainer<FeetComponent>();
        
        tileFieldComponent = worldContext.Get<TileFieldComponent>(0);
        
        antagonistComponent = worldContext.Get<AntagonistComponent>(0);
        
        vfxPoolComponent = worldContext.Get<VFXPoolComponent>(0);
    }
    
    public void Handle(WorldContext worldContext)
    {
        for (int i = 0; i < feetComponents.Count; i++)
        {
            var feetComponent = (FeetComponent)feetComponents[i];
            
            if (feetComponent.Entity.HasComponent<CarriedComponent>()
                || feetComponent.Entity.HasComponent<TossedComponent>())
                continue;
            
            var positionComponent = feetComponent.Entity.GetComponent<PositionComponent>();
            
            var currentTile = tileFieldComponent.TileField.GetTile(positionComponent.RectTransform.anchoredPosition);
            
            if (currentTile == null)
                continue;
            
            if (currentTile.Surface == SurfaceTypes.BLOOD_POOL)
            {
                feetComponent.Surface = SurfaceTypes.BLOOD;
                
                feetComponent.Amount = 5;
                
                continue;
            }
            
            if (currentTile.Surface == SurfaceTypes.OIL_POOL)
            {
                feetComponent.Surface = SurfaceTypes.OIL;
                
                feetComponent.Amount = 5;
                
                continue;
            }
            
            if (currentTile.Surface == SurfaceTypes.SHIT_POOL)
            {
                feetComponent.Surface = SurfaceTypes.SHIT;
                
                feetComponent.Amount = 5;
                
                continue;
            }
            
            if (feetComponent.Surface != SurfaceTypes.EMPTY && feetComponent.Surface != currentTile.Surface)
            {
                currentTile.Surface = feetComponent.Surface;
                
                feetComponent.Amount--;
                
                if (feetComponent.Amount == 0)
                    feetComponent.Surface = SurfaceTypes.EMPTY;
                
                antagonistComponent.Score += antagonistComponent.Settings.ScorePerDirtyTile;
                
                var vfxInstance = vfxPoolComponent.VFXPool.Pop("Score message", 1.6f);
                
                vfxInstance.GameObject.transform.position = currentTile.transform.position;
                
                vfxInstance.GameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = string.Format("+{0}", antagonistComponent.Settings.ScorePerDirtyTile);
                
                vfxInstance.GameObject.SetActive(true);
            }
        }
    }
}