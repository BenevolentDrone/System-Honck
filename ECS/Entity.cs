using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    private Dictionary<Type, Component> components = new Dictionary<Type, Component>();
    
    [SerializeField]
    private WorldContext worldContext;
    public WorldContext WorldContext { get { return worldContext; } set { worldContext = value; } }
    
    public Type[] ComponentTypes
    {
        get
        {
            return components.Keys.ToArray();
        }
    }
    
    public bool HasComponent<T>()
    {
        return components.ContainsKey(typeof(T));
    }
    
    public bool HasComponent(Type type)
    {
        return components.ContainsKey(type);
    }
    
    public new T GetComponent<T>() where T : Component
    {
        if (!HasComponent<T>())
            throw new Exception(string.Format("[Entity] № {0}: NO COMPONENT OF TYPE {1}", gameObject.GetInstanceID(), typeof(T).ToString()));
        
        return (T)components[typeof(T)];
    }
    
    public new Component GetComponent(Type type)
    {
        return components[type];
    }
    
    public void AddComponent<T>(Component component)
    {
        components.Add(typeof(T), component);
    }
    
    public void AddComponent(Type type, Component component)
    {
        components.Add(type, component);
    }
    
    public void RemoveComponent<T>()
    {
        components.Remove(typeof(T));
    }
    
    public void RemoveComponent(Type type)
    {
        components.Remove(type);
    }
    
    void Start()
    {
        worldContext.Entities.Add(this);
    }
    
    void OnDestroy()
    {
        foreach (Type componentType in ComponentTypes)
            worldContext.GetComponentsContainer(componentType).Remove(components[componentType]);
    }
}