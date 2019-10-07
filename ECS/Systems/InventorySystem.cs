using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : ISystem
{
    private List<Component> inventoryComponents;
    
    private InventoryWindowComponent inventoryWindowComponent;
    
    public void Cache(WorldContext worldContext)
    {
        inventoryComponents = worldContext.GetComponentsContainer<InventoryComponent>();
        
        inventoryWindowComponent = worldContext.Get<InventoryWindowComponent>(0);
    }
    
    public void Handle(WorldContext worldContext)
    {
        for (int i = 0; i < inventoryComponents.Count; i++)
        {
            var inventoryComponent = (InventoryComponent)inventoryComponents[i];
            
            if (inventoryComponent.Inventory == null && inventoryComponent.Prefab != null)
            {
                GameObject instance = GameObject.Instantiate(inventoryComponent.Prefab, inventoryWindowComponent.Window.transform);
                
                inventoryComponent.Inventory = instance.GetComponent<Inventory>();
            }
        }
    }
}