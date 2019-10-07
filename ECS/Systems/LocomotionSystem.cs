using System.Collections.Generic;
using UnityEngine;

public class LocomotionSystem : ISystem
{
    private List<Component> locomotionComponents;
    private TimeComponent timeComponent;
    
    public void Cache(WorldContext worldContext)
    {
        locomotionComponents = worldContext.GetComponentsContainer<LocomotionComponent>();
        
        timeComponent = worldContext.Get<TimeComponent>(0);
    }
    
    public void Handle(WorldContext worldContext)
    {
        for (int i = 0; i < locomotionComponents.Count; i++)
        {
            var locomotionComponent = (LocomotionComponent)locomotionComponents[i];
            
            var positionComponent = locomotionComponent.Entity.GetComponent<PositionComponent>();
            
            positionComponent.RectTransform.anchoredPosition += locomotionComponent.Velocity * Time.deltaTime * timeComponent.CustomTimeScale;
        }
    }
}