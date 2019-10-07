using System.Collections.Generic;
using UnityEngine;

public static class VFXFactory
{
    public static Stack<VFXInstance> BuildVFXStack(string name, GameObject prefab, int poolSize)
    {
        Stack<VFXInstance> stack = new Stack<VFXInstance>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject instance = GameObject.Instantiate(prefab);

            instance.name = name;

            instance.SetActive(false);

            stack.Push(new VFXInstance(name, instance));
        }

        return stack;
    }

    public static Dictionary<string, Stack<VFXInstance>> BuildVFXPool(VFXSettings settings, MonoBehaviour monoBehaviour, int poolSize)
    {
        Dictionary<string, Stack<VFXInstance>> database = new Dictionary<string, Stack<VFXInstance>>();

        foreach (var entry in settings.VFX)
            database.Add(entry.Name, BuildVFXStack(entry.Name, entry.Prefab, poolSize));

        return database;
    }
}
