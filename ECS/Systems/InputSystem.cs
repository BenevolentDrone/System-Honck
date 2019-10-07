using System.Collections.Generic;
using UnityEngine;

public class InputSystem : ISystem
{
    private List<Component> inputComponents;
    
    private InventoryWindowComponent inventoryWindowComponent;
    
    public void Cache(WorldContext worldContext)
    {
        inputComponents = worldContext.GetComponentsContainer<InputComponent>();
        
        inventoryWindowComponent = worldContext.Get<InventoryWindowComponent>(0);
    }
    
    public void Handle(WorldContext worldContext)
    {
        for (int i = 0; i < inputComponents.Count; i++)
        {
            var inputComponent = (InputComponent)inputComponents[i];
            
            var locomotionComponent = inputComponent.Entity.GetComponent<LocomotionComponent>();
            
            Vector2 vector = Vector2.zero;
            
            inputComponent.IsWalking = false;
            
            if (Input.GetKey(KeyCode.W))
            {
                vector += new Vector2(0, 1f);
                
                inputComponent.IsWalking = true;
            }
            
            if (Input.GetKey(KeyCode.S))
            {
                vector += new Vector2(0, -1f);
                
                inputComponent.IsWalking = true;
            }
            
            if (Input.GetKey(KeyCode.A))
            {
                vector += new Vector2(-1f, 0);
                
                inputComponent.IsWalking = true;
            }
            
            if (Input.GetKey(KeyCode.D))
            {
                vector += new Vector2(1f, 0);
                
                inputComponent.IsWalking = true;
            }
            
            locomotionComponent.Velocity = vector.normalized * locomotionComponent.Speed;
            
            if (inputComponent.IsWalking)
                inputComponent.LastInputXSign = Mathf.Sign(locomotionComponent.Velocity.x);
            
            if (Input.GetKeyDown(KeyCode.Space))
                inputComponent.Honk = true;
            
            if (Input.GetKeyDown(KeyCode.E))
                inputComponent.PickUp = true;
            
            inputComponent.IsFlappingWings = Input.GetKey(KeyCode.F);
            
            var inventoryComponent = inputComponent.Entity.GetComponent<InventoryComponent>();
            
            if (!inventoryComponent.Inventory.gameObject.activeInHierarchy
                && Input.GetMouseButtonDown(0) 
                && inputComponent.Entity.GetComponent<BeakComponent>().Contents != null)
            {
                inputComponent.Toss = true;
            }
            
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (inventoryWindowComponent.Window.activeInHierarchy)
                {
                    inventoryWindowComponent.Window.SetActive(false);
                    
                    foreach (Transform child in inventoryWindowComponent.Window.transform)
                        if (child.GetComponent<Inventory>() != null)
                            child.gameObject.SetActive(false);
                }
                else
                {
                    inventoryWindowComponent.Window.SetActive(true);
                    
                    inventoryComponent.Inventory.gameObject.SetActive(true);
                    
                    var beakContents = inputComponent.Entity.GetComponent<BeakComponent>().Contents;
                    
                    if (beakContents != null && beakContents.HasComponent<InventoryComponent>())
                        beakContents.GetComponent<InventoryComponent>().Inventory.gameObject.SetActive(true);
                }
            }
        }
    }
}