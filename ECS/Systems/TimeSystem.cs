using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TimeSystem : ISystem
{
    private TimeComponent timeComponent;
    
    public void Cache(WorldContext worldContext)
    {
        timeComponent = worldContext.Get<TimeComponent>(0);
    }
    
    public void Handle(WorldContext worldContext)
    {
        DOTween.timeScale = timeComponent.CustomTimeScale;
    }
}