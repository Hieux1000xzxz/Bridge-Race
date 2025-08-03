using UnityEngine;
using UnityEngine.AI;

public class ContestantAI : Character
{
    enum State
    {
        Decision,
        Move,
        Stop,
        Fall
    }
    public LayerMask brickLayer;
    private Vector3 targetPosition;
    private State currentState;
    public int BrickMoveCount = 10;
    private BridgeController currentBridge;
    private float timeStuck;
    private Vector3 lastPosition;
   
    public override void Init(Color color)
    {
        base.Init(color);
        SetState(State.Decision);
        BrickMoveCount = Random.Range(1, BrickMoveCount);
    }

    private void Update()
    {
        if (!IsInit || isFalling) return;

        switch (currentState)
        {
            case State.Decision:
                IdleState();
                break;
            case State.Move:
                HandleMovement();
                break;
        }
    }

    private void HandleMovement()
    {
        if (currentBridge is not null && !CheckSteps())
        {
            currentBridge = null;
            targetPosition = transform.position;
            agent.SetDestination(targetPosition);
            SetState(State.Decision);
        }
        else if (Vector3.Distance(transform.position, targetPosition) < agent.stoppingDistance)
        {
            currentBridge = null;
            SetState(State.Decision);
        }

        if (Vector3.Distance(transform.position, lastPosition) <= 0f)
        {
            timeStuck += Time.deltaTime;
            if (timeStuck > 2f)
            {
                SetState(State.Decision);
            }
        }
        lastPosition = transform.position;
    }

    private void SetState(State state)
    {
        switch (state)
        {
            case State.Decision:
                animator.SetBool(GameStatic.RUN, false);
                break;
            case State.Move:
                timeStuck = 0;
                animator.SetBool(GameStatic.RUN, true);
                agent.SetDestination(targetPosition);
                break;
        }

        currentState = state;
    }

    private void IdleState()
    {
        if (StackPoint.childCount > BrickMoveCount)
        {
            currentBridge = GameManager.Instance.FindBestBridge(Color, CurrentFloor);
            if (currentBridge != null)
            {
                targetPosition = currentBridge.TopPosition;
                SetState(State.Move);
            }
            return;
        }

        if (Radar.FindNearestTarget(transform.position, 40f, brickLayer, Color, out Brick brick, CurrentFloor))
        {
            if (brick.IsHidden) return;

            targetPosition = brick.transform.position;
            SetState(State.Move);
            return;
        }

        for (int i = CurrentFloor - 1; i >= 1; i--)
        {
            if (Radar.FindNearestTarget(transform.position, 40f, brickLayer, Color, out brick, i))
            {
                if (brick.IsHidden) continue;

                targetPosition = brick.transform.position;
                SetState(State.Move);
                return;
            }
        }
    }

    protected override void FallDown()
    {
        base.FallDown();
        SetState(State.Fall);
    }

    protected override void RecoverFromFall()
    {
        base.RecoverFromFall();
        SetState(State.Decision);
    }
}



public static class Radar
{
    private static readonly Collider[] results = new Collider[120];
    public static bool FindNearestTarget(Vector3 origin, float range, LayerMask layer, Color color, out Brick nearest, int floorLevel)
    {
        Collider[] hits = Physics.OverlapSphere(origin, range, layer);
        float minDist = float.MaxValue;
        nearest = null;

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Brick brick))
            {
                if (brick.Color != color) continue;
                if (brick.FloorLevel != floorLevel) continue;
                if (brick.IsTaken) continue;

                float dist = Vector3.Distance(origin, brick.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = brick;
                }
            }
        }

        return nearest != null;
    }

}