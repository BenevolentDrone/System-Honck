using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class SUPERHONKSystem : ISystem
{
    private InputComponent inputComponent;
    
    private SUPERHONKComponent superhonkComponent;
    
    private TimeComponent timeComponent;
    
    public void Cache(WorldContext worldContext)
    {
        inputComponent = worldContext.Get<InputComponent>(0);
        
        superhonkComponent = worldContext.Get<SUPERHONKComponent>(0);
        
        timeComponent = worldContext.Get<TimeComponent>(0);
    }
    
    public void Handle(WorldContext worldContext)
    {
        var inventoryComponent = inputComponent.Entity.GetComponent<InventoryComponent>();
        
        if (inventoryComponent.Inventory.Slots.Any(slot => slot.Item != null && slot.Item.ItemName == "SUPERHONK"))
        {
            superhonkComponent.SUPERHONKUI.SetActive(true);
            
            timeComponent.CustomTimeScale = 
                (inputComponent.IsWalking || inputComponent.IsFlappingWings)
                ? 1f
                : 0f;
        }
    }
}