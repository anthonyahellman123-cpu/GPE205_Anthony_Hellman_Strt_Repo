
using UnityEngine;

public class FleeAIController : AIController
{
    [Header("Flee Detection")]
    public float fleeDistance = 15f;
    public float safeDistance = 25f;

    [Header("Dynamic Safe Point")]
    public float minimumRetreatDistance = 8f;
    public float maximumRetreatDistance = 25f;
    public float safePointStoppingDistance = 2f;
    public float stuckTime = 2f;
    public float stuckMovementThreshold = 0.5f;

    [Header("Combat")]
    public float attackRange = 18f;
    public float retreatTime = 3f;
    public float attackTime = 1.5f;

    private Vector3 safePosition;
    private Vector3 lastPosition;

    private float stuckTimer;
    private float stateTimer;

    private bool isFleeing;
    private bool isAttacking;

    public override void Start()
    {
        base.Start();

        if (pawn != null)
            lastPosition = pawn.transform.position;
    }

    public override void ProcessInputs()
    {
        if (pawn == null || target == null)
            return;

        float distanceToPlayer = DistanceToTarget();

        if (!isFleeing && distanceToPlayer <= fleeDistance)
        {
            StartFleeing();
        }

        if (!isFleeing)
        {
            currentState = AIState.Idle;
            return;
        }

        if (distanceToPlayer >= safeDistance)
        {
            StopFleeing();
            return;
        }

        stateTimer -= Time.deltaTime;

        if (isAttacking)
        {
            currentState = AIState.Attack;

            if (distanceToPlayer <= attackRange)
                AttackTarget();

            if (stateTimer <= 0f)
            {
                isAttacking = false;
                stateTimer = retreatTime;
                CalculateSafePoint();
            }

            return;
        }

        currentState = AIState.Flee;

        CheckIfStuck();

        if (stateTimer <= 0f)
        {
            isAttacking = true;
            stateTimer = attackTime;
            return;
        }

        FleeToSafety();
    }

    private void StartFleeing()
    {
        isFleeing = true;
        isAttacking = false;

        stateTimer = retreatTime;
        stuckTimer = 0f;

        lastPosition = pawn.transform.position;

        CalculateSafePoint();
    }

    private void CalculateSafePoint()
    {
        Level level = GameManager.instance != null
            ? GameManager.instance.currentLevel
            : null;

        RoomTile[,] rooms = level != null ? level.Rooms : null;

        if (rooms == null)
        {
            Vector3 away = pawn.transform.position
                - target.transform.position;

            away.y = 0f;

            if (away.sqrMagnitude < 0.01f)
                away = -pawn.transform.forward;

            safePosition = pawn.transform.position
                + away.normalized * minimumRetreatDistance;

            return;
        }

        float bestScore = float.NegativeInfinity;
        Vector3 bestPosition = pawn.transform.position;

        for (int col = 0; col < rooms.GetLength(0); col++)
        {
            for (int row = 0; row < rooms.GetLength(1); row++)
            {
                RoomTile room = rooms[col, row];

                if (room == null || room.aiSpawn == null)
                    continue;

                Vector3 candidate = room.aiSpawn.position;

                float distanceFromPlayer = Vector3.Distance(
                    candidate,
                    target.transform.position
                );

                float travelDistance = Vector3.Distance(
                    candidate,
                    pawn.transform.position
                );

                if (travelDistance < minimumRetreatDistance)
                    continue;

                float score = distanceFromPlayer
                    - travelDistance * 0.25f
                    + Random.Range(0f, 5f);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestPosition = candidate;
                }
            }
        }

        safePosition = bestScore > float.NegativeInfinity
            ? bestPosition
            : pawn.transform.position;
    }

    private void FleeToSafety()
    {
        Vector3 difference = safePosition - pawn.transform.position;
        difference.y = 0f;

        if (difference.magnitude <= safePointStoppingDistance)
        {
            CalculateSafePoint();
            return;
        }

        MoveTowards(safePosition);
    }

    private void CheckIfStuck()
    {
        float movement = Vector3.Distance(
            pawn.transform.position,
            lastPosition
        );

        if (movement < stuckMovementThreshold)
        {
            stuckTimer += Time.deltaTime;
        }
        else
        {
            stuckTimer = 0f;
        }

        if (stuckTimer >= stuckTime)
        {
            CalculateSafePoint();
            stuckTimer = 0f;
        }

        lastPosition = pawn.transform.position;
    }

    private void StopFleeing()
    {
        isFleeing = false;
        isAttacking = false;
        stuckTimer = 0f;
        currentState = AIState.Idle;
    }
}
