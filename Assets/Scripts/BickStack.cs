using System.Collections.Generic;
using UnityEngine;

public class BrickStack : MonoBehaviour
{
    [SerializeField] private Transform stackRoot;
    [SerializeField] private float brickHeight = 0.4f;

    private readonly List<Brick> _bricks = new();
    private readonly Stack<Brick> _pooledBricks = new();

    public int Count => _bricks.Count;
    public Transform StackRoot => stackRoot;

    public void AddBrick(Brick brick, Color color)
    {
        brick.transform.SetParent(stackRoot);
        brick.SetColor(color);
        brick.transform.localPosition = new Vector3(0, _bricks.Count * brickHeight, 0);
        _bricks.Add(brick);
    }

    public void RemoveAllBricks()
    {
        foreach (var brick in _bricks)
        {
            brick.Despawn();
            _pooledBricks.Push(brick);
        }
        _bricks.Clear();
    }

    public Brick GetLastBrick()
    {
        if (_bricks.Count == 0) return null;

        var lastIndex = _bricks.Count - 1;
        var brick = _bricks[lastIndex];
        _bricks.RemoveAt(lastIndex);
        return brick;
    }
}