using UnityEngine;
using UnityEngine.AI;

public abstract class Character : MonoBehaviour
{
    public Transform StackPoint => brickStack.StackPoint;

    public Color Color;
    [SerializeField] protected Animator animator;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] private BrickStack brickStack;
    [SerializeField] private Transform rayPoint;
    [SerializeField] private LayerMask stepLayer;
    [SerializeField] private LayerMask groundLayer = -1;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float navMeshCheckRadius = 1f;
    [SerializeField] private float groundCheckDistance = 2f; 
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private float maxAllowedHeight = 3f; 

    public int CurrentFloor { get; private set; } = 1;
    protected bool isFalling = false;
    protected bool isInvincible = false;
    protected bool isAttacking = false;
    protected float attackCooldown = 1f;
    private float lastAttackTime = -999f;
    protected float invincibleDuration = 2f;
    protected bool IsInit;
    protected bool isGrounded = true;
    private float groundCheckTimer = 0f;
    private float groundCheckInterval = 0.05f; 
    private Vector3 lastGroundedPosition;
    public virtual void Init(Color color)
    {
        brickStack.ClearStack();
        Color = color;
        skinnedMeshRenderer.material.SetColor(GameStatic.BASE_COLOR, Color);
        agent.isStopped = false;
        IsInit = true;
        isGrounded = true;
        isFalling = false;
        lastGroundedPosition = transform.position;
    }

    public virtual void Stop()
    {
        if (agent == null) return;
        animator.SetBool(GameStatic.RUN, false);
        IsInit = false;
    }

    protected virtual void Update()
    {
        if (!IsInit) return;

        groundCheckTimer += Time.deltaTime;
        if (groundCheckTimer >= groundCheckInterval)
        {
            groundCheckTimer = 0f;
            CheckGroundStatus();
        }

        CheckForCleanup();
    }

    protected virtual void CheckGroundStatus()
    {
        bool wasGrounded = isGrounded;
        bool currentlyGrounded = IsGrounded();
        float distanceToGround = GetDistanceToGround();

        if (currentlyGrounded && !isFalling)
        {
            lastGroundedPosition = transform.position;
        }

        if (!isFalling && (distanceToGround > maxAllowedHeight || !currentlyGrounded))
        {
            isGrounded = false;
            StartFreeFall();
            return;
        }

        isGrounded = currentlyGrounded;

        if (isFalling && currentlyGrounded && HasNavMeshBelow())
        {
            RecoverFromFreeFall();
        }
    }

    protected virtual bool IsGrounded()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;

        if (Physics.Raycast(rayOrigin, Vector3.down, groundCheckDistance + 0.1f, groundLayer))
        {
            return true;
        }

        if (Physics.SphereCast(rayOrigin, groundCheckRadius, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer))
        {
            return true;
        }

        return false;
    }

    private float GetDistanceToGround()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            return hit.distance - 0.1f; 
        }

        return Mathf.Infinity; 
    }

    protected virtual void StartFreeFall()
    {
        if (isFalling) return;

        isFalling = true;
        isGrounded = false;

        if (agent != null && agent.enabled)
        {
            agent.enabled = false;
        }

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        DropAllBricks();

        animator.SetTrigger(GameStatic.FALL);
        animator.SetBool(GameStatic.RUN, false);
    }

    protected virtual void RecoverFromFreeFall()
    {
        if (!isFalling) return;

        if (!IsGrounded() || !HasNavMeshBelow())
        {
            return;
        }

        float distanceToGround = GetDistanceToGround();
        if (distanceToGround > 1f)
        {
            return;
        }


        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y + 0.1f, transform.position.z);
        }

        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
        }

        isFalling = false;
        isGrounded = true;
        isInvincible = true;

        Invoke(nameof(ActivateInvincibility), invincibleDuration);
    }

    public void Despawn()
    {
        ObjectPool.instance.Despawn(this.gameObject);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!isFalling && other.TryGetComponent(out Brick brick) && (brick.Color.Equals(Color) || brick.Color.Equals(GameManager.Instance.NaturalColor)))
        {
            brick.Take(this);
            return;
        }

        if (other.TryGetComponent(out Character otherChar) && otherChar != this)
        {
            OnCharacterCollide(otherChar);
        }

        if (other.CompareTag("Finish"))
        {
            animator.Play(GameStatic.CHEER);
        }
    }

    protected virtual void OnCharacterCollide(Character other)
    {
        if (isFalling || isInvincible || other.isInvincible || Time.time - lastAttackTime < attackCooldown)
            return;

        int myCount = GetBrickCount();
        int otherCount = other.GetBrickCount();

        if (myCount > otherCount)
        {
            isAttacking = true;
            lastAttackTime = Time.time;

            animator.SetBool(GameStatic.ATTACK, true);
            other.FallDown();

            Invoke(nameof(StopAttackAnimation), 0.6f);
        }
        else if (myCount < otherCount)
        {
            FallDown();
        }
    }

    protected virtual void FallDown()
    {
        if (isFalling || isInvincible) return;

        Debug.Log($"{gameObject.name} bị đánh ngã");

        isFalling = true;
        isGrounded = false;

        if (agent != null && agent.enabled)
        {
            agent.enabled = false;
        }

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
        }

        DropAllBricks();
        animator.SetTrigger(GameStatic.FALL);
        animator.SetBool(GameStatic.RUN, false);

        Invoke(nameof(RecoverFromFall), 1.7f);
    }

    protected virtual void DropAllBricks()
    {
        brickStack.DropAllBricks();
    }

    protected virtual void RecoverFromFall()
    {
        float distanceToGround = GetDistanceToGround();
        if (!IsGrounded() || !HasNavMeshBelow() || distanceToGround > 1f)
        {
            Invoke(nameof(RecoverFromFall), 0.5f);
            return;
        }


        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y + 0.1f, transform.position.z);
        }

        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
        }

        isFalling = false;
        isGrounded = true;
        isInvincible = true;

        Invoke(nameof(ActivateInvincibility), invincibleDuration);
    }

    private bool HasNavMeshBelow()
    {
        NavMeshHit hit;
        Vector3 checkPosition = transform.position;

        bool hasNavMesh = NavMesh.SamplePosition(checkPosition, out hit, navMeshCheckRadius, NavMesh.AllAreas);

        if (hasNavMesh)
        {
            float verticalDistance = Mathf.Abs(hit.position.y - checkPosition.y);
            return verticalDistance <= navMeshCheckRadius;
        }

        return false;
    }

    protected virtual void ActivateInvincibility()
    {
        isInvincible = false;
    }

    public bool IsFalling() => isFalling;

    public int GetBrickCount()
    {
        return brickStack.Count;
    }

    public void AddBrick(Brick brick)
    {
        brickStack.AddBrick(brick);
    }

    public Brick GetTopBrick()
    {
        return brickStack.GetTopBrick();
    }

    public bool RemoveTopBrick()
    {
        return brickStack.RemoveTopBrick();
    }

    protected bool CheckSteps()
    {
        RaycastHit hit;
        if (Physics.Raycast(rayPoint.position, rayPoint.TransformDirection(Vector3.down), out hit, 10f, stepLayer, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(rayPoint.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);

            if (hit.transform.TryGetComponent(out Step step))
            {
                if (step.Color == Color)
                {
                    animator.SetBool(GameStatic.RUN, true);
                    return true;
                }
                if (!brickStack.IsEmpty())
                {
                    brickStack.RemoveTopBrick();
                    step.SetColor(Color);
                    animator.SetBool(GameStatic.RUN, true);
                    return true;
                }

                if (Vector3.Dot(transform.forward, step.transform.forward) > 0)
                {
                    animator.SetBool(GameStatic.RUN, false);
                    return false;
                }
            }
            animator.SetBool(GameStatic.RUN, true);
            return true;
        }

        Debug.DrawRay(rayPoint.position, transform.TransformDirection(Vector3.down) * 10f, Color.red);
        animator.SetBool(GameStatic.RUN, false);
        return false;
    }

    protected virtual void StopAttackAnimation()
    {
        animator.SetBool(GameStatic.ATTACK, false);
        isAttacking = false;
    }

    public void SetFloor(int floor)
    {
        CurrentFloor = floor;
    }

    public bool IsPermanentlyFallen()
    {
        return isFalling && (GetDistanceToGround() > maxAllowedHeight || !HasNavMeshBelow());
    }

    public void ForceRecover()
    {
        if (isFalling)
        {
            float distanceToGround = GetDistanceToGround();
            if (IsGrounded() && HasNavMeshBelow() && distanceToGround <= 1f)
            {
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
                {
                    transform.position = new Vector3(transform.position.x, hit.point.y + 0.1f, transform.position.z);
                }

                if (agent != null)
                {
                    agent.enabled = true;
                    agent.isStopped = false;
                }

                isFalling = false;
                isGrounded = true;
                isInvincible = true;
                Invoke(nameof(ActivateInvincibility), invincibleDuration);
            }
          
        }
    }

    public void SetAllBricksColor(Color color)
    {
        brickStack.SetAllBricksColor(color);
    }

    public bool HasBricks()
    {
        return !brickStack.IsEmpty();
    }

    public bool IsGroundedPublic()
    {
        return isGrounded;
    }

    public bool IsInEndlessFall()
    {
        return isFalling && (GetDistanceToGround() > maxAllowedHeight || !HasNavMeshBelow());
    }

    public void CheckForCleanup(float fallThreshold = -5f)
    {
        if (transform.position.y < fallThreshold && isFalling)
        {
            Despawn();
        }
    }
}