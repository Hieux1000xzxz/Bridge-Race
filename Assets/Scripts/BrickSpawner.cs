using DG.Tweening;
using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickSpawner : MonoBehaviour
{
    [SerializeField] private int gridX = 10;
    [SerializeField] private int gridZ = 10;
    [SerializeField] private float gridSpacingOffset = 2f;
    [SerializeField] private LayerMask brickLayerMask;
    [SerializeField] private bool spawnAllColors = true;
    [SerializeField] private int floorLevel = 1;

    public List<BrickGridData> brickList = new List<BrickGridData>();
    private WaitForSeconds spawnerDelay = new WaitForSeconds(4f);
    private Coroutine spawnerCoroutine;

    public void Init()
    {
        foreach (BrickGridData data in brickList)
        {
            data.brick?.Despawn();
        }
        brickList.Clear();

        Vector3 startPos = transform.position - new Vector3(gridX * 0.5f * gridSpacingOffset, 0, gridZ * 0.5f * gridSpacingOffset);

        for (int x = 0; x <= gridX; x++)
        {
            for (int z = 0; z <= gridZ; z++)
            {
                Vector3 spawnPos = startPos + new Vector3(x * gridSpacingOffset, 0, z * gridSpacingOffset);

                if (!IsPositionOccupied(spawnPos))
                {
                    Color color = GameManager.Instance.RandomColor;
                    SpawnBrickAtPosition(spawnPos, color, floorLevel);
                }
            }
        }

        if (spawnerCoroutine != null) StopCoroutine(spawnerCoroutine);
        spawnerCoroutine = StartCoroutine(Spawner());
    }

    private bool IsPositionOccupied(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 0.5f, brickLayerMask);
        return colliders.Length > 0;
    }

    private void SpawnBrickAtPosition(Vector3 position, Color color, int floor)
    {
        Brick brick = LeanPool.Spawn(GameAssets.Instance.Brick, position, Quaternion.identity);
        BrickGridData data = new BrickGridData(position, brick, color, floor);
        brick.SetBrickData(data, floor);
        brickList.Add(data);

        bool shouldShow = spawnAllColors;
        brick.SetVisible(shouldShow);

        if (shouldShow)
        {
            brick.transform.localScale = Vector3.zero;
            brick.transform.DOScale(Vector3.one * 1.5f, 0.5f).SetEase(Ease.OutBack);
        }
    }

    private IEnumerator Spawner()
    {
        while (true)
        {
            for (int i = 0; i < brickList.Count; i++)
            {
                var data = brickList[i];
                if (data.IsCollected && Random.value > 0.5f && !IsPositionOccupied(data.position))
                {
                    Brick brick = LeanPool.Spawn(GameAssets.Instance.Brick, data.position, Quaternion.identity);
                    data.brick = brick;
                    data.IsCollected = false;
                    brick.SetBrickData(data, data.FloorLevel);

                    bool shouldShow = spawnAllColors;
                    brick.SetVisible(shouldShow);

                    if (shouldShow)
                    {
                        brick.transform.localScale = Vector3.zero;
                        brick.transform.DOScale(Vector3.one * 1.5f, 0.5f).SetEase(Ease.OutBack);
                    }
                }
            }

            yield return spawnerDelay;
        }
    }
    public void ShowBricksByColor(Color color)
    {
        foreach (var data in brickList)
        {
            if (data.Color == color && data.brick != null && !data.brick.gameObject.activeSelf)
            {
                data.brick.SetVisible(true);
                data.brick.transform.localScale = Vector3.zero;
                data.brick.transform.DOScale(Vector3.one * 1.5f, 0.3f).SetEase(Ease.OutBack);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + new Vector3(0, 0.5f, 0),
            new Vector3(gridX * gridSpacingOffset, 1, gridZ * gridSpacingOffset));
    }
#endif
}


public class BrickGridData
{
    public Vector3 position;
    public Brick brick;
    public bool IsCollected;
    public Color Color;
    public int FloorLevel;

    public BrickGridData(Vector3 position, Brick brick, Color color, int floorLevel)
    {
        this.position = position;
        this.brick = brick;
        this.Color = color;
        this.FloorLevel = floorLevel;
        this.IsCollected = false;
    }
}
