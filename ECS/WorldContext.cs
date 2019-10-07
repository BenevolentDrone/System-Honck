using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldContext : MonoBehaviour
{
    private List<Entity> entities = new List<Entity>();
    public List<Entity> Entities { get { return entities; } }

    private Dictionary<Type, List<Component>> components = new Dictionary<Type, List<Component>>();

    public List<Component> GetComponentsContainer<T>()
    {
        if (!components.ContainsKey(typeof(T)))
            components.Add(typeof(T), new List<Component>());
        
        return components[typeof(T)];
    }

    public List<Component> GetComponentsContainer(Type type)
    {
        if (!components.ContainsKey(type))
            components.Add(type, new List<Component>());
        
        return components[type];
    }

    public T Get<T>(int index) where T : Component
    {
        return (T)components[typeof(T)][index];
    }
}