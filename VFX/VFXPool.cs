using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXPool : MonoBehaviour
{
    [SerializeField]
    private int poolSize;

    [SerializeField]
    private VFXSettings settings;

    private Dictionary<string, Stack<VFXInstance>> database;

    private void Start()
    {
        database = VFXFactory.BuildVFXPool(settings, this, poolSize);
    }

    public VFXInstance Pop(string name)
    {
        if (!database.ContainsKey(name))
            Debug.LogError(string.Format("[VFXPool] NO EFFECT {0}", name));

        if (database[name].Count == 0)
            Debug.LogError("[VFXPool] OUT OF INSTANCES");

        return database[name].Pop();
    }

    public VFXInstance Pop(string name, float duration)
    {
        VFXInstance result = Pop(name);

        StartCoroutine(WaitThenPushRoutine(result, duration));

        return result;
    }

    private IEnumerator WaitThenPushRoutine(VFXInstance instance, float duration)
    {
        yield return new WaitForSeconds(duration);

        Push(instance);
    }

    public void Push(VFXInstance instance)
    {
        database[instance.Name].Push(instance);
    }
}