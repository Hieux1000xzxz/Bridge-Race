using System.Collections.Generic;
using UnityEngine;

public class PlayerStack : MonoBehaviour
{
    [SerializeField] private Transform stackParent;
    [SerializeField] private float blockHeight = 0.4f;
    public BlockColor playerColor;

    private readonly List<GameObject> stackList = new();

    public void AddBlock(GameObject block)
    {
        stackList.Add(block);

        // Gắn block vào stackParent
        block.transform.SetParent(stackParent);

        // Reset vị trí tương đối
        block.transform.localRotation = Quaternion.identity;
        block.transform.localPosition = new Vector3(0f, blockHeight * (stackList.Count - 1), 0f);
    }

    public bool RemoveBlock(out GameObject block)
    {
        if (stackList.Count == 0)
        {
            block = null;
            return false;
        }

        block = stackList[^1];
        stackList.RemoveAt(stackList.Count - 1);
        return true;
    }

    public int StackCount => stackList.Count;
    public bool HasBlocks => stackList.Count > 0;
}

public enum BlockColor
{
    Red,
    Green,
    Purple,
    Yellow
}
