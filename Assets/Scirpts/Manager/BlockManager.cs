using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public static BlockManager Instance { get; private set; }

    private readonly List<PickupBlock> allBlocks = new();

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterBlock(PickupBlock block)
    {
        if (!allBlocks.Contains(block))
            allBlocks.Add(block);
    }

    public void UnregisterBlock(PickupBlock block)
    {
        allBlocks.Remove(block);
    }

    public PickupBlock GetNearestAvailableBlock(Vector3 position, BlockColor color)
    {
        PickupBlock nearest = null;
        float minDist = float.MaxValue;

        foreach (var block in allBlocks)
        {
            if (block.IsCollected || block.blockColor != color)
                continue;

            float dist = Vector3.Distance(position, block.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = block;
            }
        }

        return nearest;
    }

    public List<PickupBlock> GetAllBlocks() => allBlocks;
}
