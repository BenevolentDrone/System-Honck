using UnityEngine;

public class VFXInstance
{
    public string Name { get; private set; }

    public GameObject GameObject { get; private set; }

    public VFXInstance(string name, GameObject gameObject)
    {
        Name = name;

        GameObject = gameObject;
    }
}