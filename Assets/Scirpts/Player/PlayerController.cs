using UnityEngine;

[RequireComponent(typeof(PlayerStack))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private FloatingJoystick joystick;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerStack playerStack;

    private bool isRunning;

    void Update()
    {
        MoveHandle();
        UpdateAnimation();
    }

    private void MoveHandle()
    {
        Vector3 direction = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);

        if (direction.magnitude > 0.1f)
        {
            isRunning = true;
            transform.Translate(direction.normalized * moveSpeed * Time.deltaTime, Space.World);
            Quaternion toRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
        }
        else
        {
            isRunning = false;
        }
    }

    private void UpdateAnimation()
    {
        animator.SetBool("IsRunning", isRunning);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Block"))
            return;

        if (!other.TryGetComponent(out PickupBlock pickup))
            return;

        if (pickup.IsCollected)
            return;

        if (pickup.blockColor != playerStack.playerColor)
            return;

        pickup.MarkCollected();
        playerStack.AddBlock(pickup.gameObject);
    }
}
