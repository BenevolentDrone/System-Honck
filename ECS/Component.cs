using UnityEngine;

public abstract class Component : MonoBehaviour
{
    protected Entity entity;
    public Entity Entity { get { return entity; } }
    
    protected virtual void Start()
    {
        entity = gameObject.GetComponent<Entity>();
        
        var type = GetType();
        
        entity.AddComponent(type, this);
        
        entity.WorldContext.GetComponentsContainer(type).Add(this);
    }
    
    protected virtual void OnDestroy()
    {
        var type = GetType();
        
        entity.RemoveComponent(type);
        
        entity.WorldContext.GetComponentsContainer(type).Remove(this);
    }
}