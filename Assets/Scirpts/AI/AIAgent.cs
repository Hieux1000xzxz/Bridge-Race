using UnityEngine;

[RequireComponent(typeof(PlayerStack))]
public class AIAgent : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private PlayerStack playerStack;

    private GameObject targetBlock;

    private void Start()
    {
        FindNewTarget();
    }

    private void Update()
    {
        if (targetBlock == null)
        {
            FindNewTarget();
            return;
        }

        MoveToTarget();
        RotateToTarget();
        CheckReachTarget();
    }

    private void FindNewTarget()
    {
        PickupBlock target = BlockManager.Instance.GetNearestAvailableBlock(transform.position, playerStack.playerColor);
        if (target != null)
        {
            targetBlock = target.gameObject;
        }
        else
        {
            targetBlock = null;
        }
    }

    private void MoveToTarget()
    {
        if (targetBlock == null) return;

        Vector3 direction = (targetBlock.transform.position - transform.position);

        if (direction.magnitude < 0.1f || !targetBlock.activeSelf)
        {
            targetBlock = null;
            return;
        }

        direction.Normalize();
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
    }

    private void RotateToTarget()
    {
        if (targetBlock == null) return;

        Vector3 direction = (targetBlock.transform.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
        }
    }

    private void CheckReachTarget()
    {
        if (targetBlock == null) return;

        float distance = Vector3.Distance(transform.position, targetBlock.transform.position);
        if (distance < 0.5f)
        {
            // Có thể nhặt block hoặc thực hiện hành động khác tại đây
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Block")) return;

        PickupBlock pickup = other.GetComponent<PickupBlock>();
        if (pickup == null || pickup.IsCollected) return;
        if (pickup.blockColor != playerStack.playerColor) return;

        pickup.MarkCollected();
        playerStack.AddBlock(pickup.gameObject);

        FindNewTarget(); // Tìm block tiếp theo sau khi nhặt
    }
}
