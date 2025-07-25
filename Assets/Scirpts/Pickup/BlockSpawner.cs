using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField] private int totalBlocks = 50;
    [SerializeField] private int minEachColor = 8;
    [SerializeField] private float spawnRange = 10f;

    private void Start()
    {
        SpawnRandomBlocks();
    }

    private void SpawnRandomBlocks()
    {
        int colorCount = System.Enum.GetValues(typeof(BlockColor)).Length;
        int[] colorTotals = new int[colorCount];

        // Đảm bảo mỗi màu có ít nhất minEachColor
        int placed = 0;
        for (int i = 0; i < colorCount; i++)
        {
            for (int j = 0; j < minEachColor; j++)
            {
                SpawnBlock((BlockColor)i);
                colorTotals[i]++;
                placed++;
            }
        }

        // Phần còn lại spawn ngẫu nhiên
        while (placed < totalBlocks)
        {
            int colorIndex = Random.Range(0, colorCount);
            SpawnBlock((BlockColor)colorIndex);
            colorTotals[colorIndex]++;
            placed++;
        }
    }

    private void SpawnBlock(BlockColor color)
    {
        GameObject blockObj = BlockPool.Instance.Get(color);
        blockObj.transform.position = new Vector3(
            Random.Range(-spawnRange, spawnRange),
            0.8f,
            Random.Range(-spawnRange, spawnRange)
        );
        blockObj.transform.rotation = Quaternion.identity;
        blockObj.SetActive(true);

        PickupBlock pickup = blockObj.GetComponent<PickupBlock>();
        pickup.ResetState();

        BlockManager.Instance.RegisterBlock(pickup);
    }
}
