using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BrickStack : MonoBehaviour
{
    [SerializeField] private Transform stackPoint;
    [SerializeField] private float stackHeight = 0.4f;

    private readonly List<Brick> brickStack = new List<Brick>();

    public Transform StackPoint => stackPoint;
    public int Count => brickStack.Count;

    public void AddBrick(Brick brick)
    {
        if (brick == null) return;

        brickStack.Add(brick);
        brick.transform.SetParent(stackPoint);

        int index = brickStack.Count - 1;
        Vector3 localPos = new Vector3(0f, index * stackHeight, 0f);

        brick.transform.DOLocalJump(localPos, 0.001f, 1, 0.5f);
        brick.transform.DOLocalRotate(Vector3.zero, 0.5f);
    }

    public void RemoveBrick(Brick brick)
    {
        if (brickStack.Remove(brick))
        {
            brick.transform.SetParent(null);
            ReorganizeStack();
        }
    }

    public Brick GetTopBrick()
    {
        if (brickStack.Count > 0)
        {
            return brickStack[brickStack.Count - 1];
        }
        return null;
    }

    public bool RemoveTopBrick()
    {
        Brick topBrick = GetTopBrick();
        if (topBrick != null)
        {
            brickStack.RemoveAt(brickStack.Count - 1);
            topBrick.Despawn();
            ReorganizeStack();
            return true;
        }
        return false;
    }

    public void DropAllBricks()
    {
        for (int i = brickStack.Count - 1; i >= 0; i--)
        {
            Brick brick = brickStack[i];
            if (brick != null)
            {
                brick.Drop();
            }
        }
        brickStack.Clear();
    }

    public void ClearStack()
    {
        for (int i = brickStack.Count - 1; i >= 0; i--)
        {
            Brick brick = brickStack[i];
            if (brick != null)
            {
                brick.Despawn();
            }
        }
        brickStack.Clear();
    }

    private void ReorganizeStack()
    {
        for (int i = 0; i < brickStack.Count; i++)
        {
            if (brickStack[i] != null)
            {
                Vector3 targetPos = new Vector3(0f, i * stackHeight, 0f);
                brickStack[i].transform.DOLocalMove(targetPos, 0.3f);
            }
        }
    }

    public Brick GetBrickAtIndex(int index)
    {
        if (index >= 0 && index < brickStack.Count)
        {
            return brickStack[index];
        }
        return null;
    }

    public bool IsEmpty()
    {
        return brickStack.Count == 0;
    }

    public List<Brick> GetAllBricks()
    {
        return new List<Brick>(brickStack);
    }

    public void SetAllBricksColor(Color color)
    {
        foreach (Brick brick in brickStack)
        {
            if (brick != null)
            {
                brick.SetColor(color);
            }
        }
    }
}