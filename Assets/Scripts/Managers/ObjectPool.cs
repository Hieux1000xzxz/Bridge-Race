using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Preallocation
{
    public GameObject gameObject;
    public int count;
    public bool expandable;
}

public class ObjectPool : FastSingleton<ObjectPool>
{
    public List<Preallocation> preAllocations;
    [SerializeField] private List<GameObject> pooledGobjects;

    protected override void Awake()
    {
        base.Awake();
        pooledGobjects = new List<GameObject>();

        foreach (Preallocation item in preAllocations)
        {
            for (int i = 0; i < item.count; ++i)
            {
                GameObject obj = CreateGobject(item.gameObject);
                pooledGobjects.Add(obj);
            }
        }
    }

    public GameObject Spawn(string tag)
    {
        for (int i = 0; i < pooledGobjects.Count; ++i)
        {
            if (!pooledGobjects[i].activeSelf && pooledGobjects[i].CompareTag(tag))
            {
                pooledGobjects[i].SetActive(true);
                return pooledGobjects[i];
            }
        }

        foreach (var item in preAllocations)
        {
            if (item.gameObject.CompareTag(tag) && item.expandable)
            {
                GameObject obj = CreateGobject(item.gameObject);
                pooledGobjects.Add(obj);
                obj.SetActive(true);
                return obj;
            }
        }

        return null;
    }

    public void Despawn(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
    }

    private GameObject CreateGobject(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        return obj;
    }
}
