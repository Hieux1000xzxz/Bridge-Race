using Lean.Pool;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Character : MonoBehaviour
{
    public Transform StackPoint => stackPoint;

    public Color Color;
    [SerializeField] protected Animator animator;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] private Transform stackPoint;
    [SerializeField] private Transform rayPoint;
    [SerializeField] private LayerMask stepLayer;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private Rigidbody rb;
    public int CurrentFloor { get; private set; } = 1;
    protected bool isFalling = false;
    protected bool isInvincible = false;
    protected bool isAttacking = false;
    protected float attackCooldown = 1f;    
    private float lastAttackTime = -999f; 
    protected float invincibleDuration = 2f;
    protected bool IsInit;

    private readonly List<Brick> brickStack = new();
    public virtual void Init(Color color)
    {
        ClearStack();
        Color = color;
        skinnedMeshRenderer.material.SetColor(GameStatic.BASE_COLOR, Color);
        agent.isStopped = false;
        IsInit = true;
    }

    public virtual void Stop()
    {
        agent.isStopped = true;
        animator.SetBool(GameStatic.RUN, false);
        IsInit = false;
    }
    public void Despawn()
    {
        LeanPool.Despawn(this);
    }

    private void ClearStack()
    {
        int childCount = stackPoint.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            stackPoint.GetChild(i).GetComponent<Brick>().Despawn();
        }
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
        if(other.CompareTag("Finish"))
        {
            animator.Play("Cheer");
        }
    }
    protected virtual void OnCharacterCollide(Character other)
    {
        if (!isFalling && !isInvincible && Time.time - lastAttackTime >= attackCooldown)
        {
            int myCount = GetBrickCount();
            int otherCount = other.GetBrickCount();

            if (myCount > otherCount)
            {
                isAttacking = true;
                lastAttackTime = Time.time;

                animator.SetBool("Attack", true);
                other.FallDown();

                Invoke(nameof(StopAttackAnimation), 0.6f);
            }
            else if (myCount < otherCount)
            {
                FallDown();
            }
        }
    }


    protected virtual void FallDown()
    {
        if (isFalling) return;
        isFalling = true;

        agent.enabled = false;
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        DropAllBricks();
        animator.SetTrigger("Fall");

        Invoke(nameof(RecoverFromFall), 1.7f);
    }
    protected virtual void DropAllBricks()
    {
        int childCount = StackPoint.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Brick brick = StackPoint.GetChild(i).GetComponent<Brick>();
            brick.Drop();
        }
    }
    protected virtual void RecoverFromFall()
    {
        rb.isKinematic = true;
        agent.enabled = true;
        isFalling = false;
        isInvincible = true;
        Invoke(nameof(ActivateInvincibility), invincibleDuration);
    }
    protected virtual void ActivateInvincibility()
    {
        isInvincible = false;
    }

    public bool IsFalling() => isFalling;
    public int GetBrickCount()
    {
        return StackPoint.childCount;
    }
    public void AddBrick(Brick brick)
    {
        brickStack.Add(brick);
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

                int childCount = stackPoint.childCount;
                if (childCount > 0 && stackPoint.GetChild(childCount - 1).TryGetComponent(out Brick brick))
                {
                    brick.Despawn();
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
        animator.SetBool("Attack", false);
        isAttacking = false;
    }
    public void SetFloor(int floor)
    {
        CurrentFloor = floor;
    }
}
