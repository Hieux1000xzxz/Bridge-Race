using UnityEngine;

public class PickupBlock : MonoBehaviour
{
    [field: SerializeField] public BlockColor blockColor { get; private set; }
    public bool IsCollected { get; private set; }

    private void OnEnable()
    {
        IsCollected = false;
    }

    public void MarkCollected()
    {
        IsCollected = true;
    }

    public void ResetState()
    {
        IsCollected = false;
        gameObject.SetActive(true);
    }

    public void ReturnToPool()
    {
        IsCollected = false;
        BlockManager.Instance.UnregisterBlock(this);
        BlockPool.Instance.Return(gameObject, blockColor);
    }
}
