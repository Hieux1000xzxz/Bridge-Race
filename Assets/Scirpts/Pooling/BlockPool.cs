using System.Collections.Generic;
using UnityEngine;

public class BlockPool : MonoBehaviour
{
    [System.Serializable]
    public class BlockPrefabEntry
    {
        public BlockColor color;
        public GameObject prefab;
    }

    public List<BlockPrefabEntry> blockPrefabs;
    public int poolSize = 50;

    private readonly Dictionary<BlockColor, Queue<GameObject>> pool = new();
    private readonly Dictionary<BlockColor, GameObject> prefabMap = new();

    public static BlockPool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        foreach (var entry in blockPrefabs)
        {
            prefabMap[entry.color] = entry.prefab;
            pool[entry.color] = new Queue<GameObject>();

            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(entry.prefab, transform);
                obj.SetActive(false);
                pool[entry.color].Enqueue(obj);
            }
        }
    }

    public GameObject Get(BlockColor color)
    {
        if (pool[color].Count > 0)
        {
            var obj = pool[color].Dequeue();
            obj.SetActive(true);
            return obj;
        }

        // Hết pool, tạo mới nếu cần
        var fallback = Instantiate(prefabMap[color], transform);
        return fallback;
    }

    public void Return(GameObject obj, BlockColor color)
    {
        obj.SetActive(false);
        pool[color].Enqueue(obj);
    }
}
