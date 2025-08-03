using UnityEngine;

public class Player : Character
{
    public float rotationSpeed = 10f;
    [SerializeField] private FloatingJoystick joystick;

    private Vector3 movement, forward, right;
    private float movementMagnitude;
    private Quaternion quaternion;
    private Transform camTransform;

    private void Awake()
    {
        camTransform = Camera.main.transform;
        forward = Vector3.forward;
        right = Vector3.right;
        agent.updateRotation = false;
    }

    public override void Init(Color color)
    {
        base.Init(color);
        quaternion = Quaternion.identity;
        transform.forward = Vector3.forward;
        transform.position = Vector3.zero;
    }

    public override void Stop()
    {
        base.Stop();
    }

    private void Update()
    {
        if (!IsInit || isFalling || joystick == null) return;
        float x = joystick.Horizontal;
        float y = joystick.Vertical;
        movementMagnitude = new Vector2(x, y).magnitude;

        if (movementMagnitude > 0.1f)
        {
            animator.SetBool("Run", true);

            UpdateInputDirection(x, y);
            LookDirection(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, rotationSpeed * Time.deltaTime);

            if (CheckSteps())
            {
                Vector3 scaledMovement = agent.speed * Time.deltaTime * movement;
                agent.Move(scaledMovement);
            }
        }
        else
        {
            animator.SetBool("Run", false);
        }
    }

    public void LookDirection(Vector3 dir)
    {
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        quaternion = Quaternion.Euler(new Vector3(0, angle, 0));
    }

    private void UpdateInputDirection(float x, float y)
    {
        forward = camTransform.forward;
        forward.y = 0;
        forward = forward.normalized;
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;

        Vector3 verticalInput = forward * y;
        Vector3 horizontalInput = right * x;
        movement = (verticalInput + horizontalInput).normalized;
    }
}
