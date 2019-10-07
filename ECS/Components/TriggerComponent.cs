using System.Collections.Generic;
using UnityEngine;

public class TriggerComponent : Component
{
    public List<Collider> Collisions = new List<Collider>();

    void OnTriggerEnter(Collider collider)
    {
        Collisions.Add(collider);
    }

    private void OnTriggerExit(Collider collider)
    {
        Collisions.Remove(collider);
    }
}