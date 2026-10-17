
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

    private Vector3 safePosition;
    private bool isFleeing = false;

    public override void ProcessInputs()
    {
        if (pawn == null || target == null)
            return;

        float distanceToPlayer = DistanceToTarget();

        // Start fleeing when the player gets too close.
        if (!isFleeing && distanceToPlayer <= fleeDistance)
        {
            isFleeing = true;
            CalculateSafePoint();
        }

        // Stop fleeing once the player is far enough away.
        if (isFleeing && distanceToPlayer >= safeDistance)
        {
            StopFleeing();
        }

        currentState = isFleeing ? AIState.Flee : AIState.Idle;

        switch (currentState)
        {
            case AIState.Idle:
                break;

            case AIState.Flee:
                FleeToSafety();
                break;
        }
    }

    private void CalculateSafePoint()
    {
        Vector3 awayFromPlayer =
            pawn.transform.position - target.transform.position;

        awayFromPlayer.y = 0f;

        if (awayFromPlayer.sqrMagnitude < 0.01f)
        {
            awayFromPlayer = -pawn.transform.forward;
        }

        awayFromPlayer.Normalize();

        // Closer player = larger retreat distance.
        float closeness = 1f - Mathf.Clamp01(
            DistanceToTarget() / Mathf.Max(fleeDistance, 0.01f)
        );

        float retreatDistance = Mathf.Lerp(
            minimumRetreatDistance,
            maximumRetreatDistance,
            closeness
        );

        safePosition = pawn.transform.position
                     + awayFromPlayer * retreatDistance;

        Debug.Log(
            "Flee Tank: Player distance = "
            + DistanceToTarget().ToString("F1")
            + ", retreat distance = "
            + retreatDistance.ToString("F1")
        );
    }

    private void FleeToSafety()
    {
        Vector3 difference = safePosition - pawn.transform.position;
        difference.y = 0f;

        if (difference.magnitude <= safePointStoppingDistance)
        {
            StopFleeing();
            return;
        }

        MoveTowards(safePosition);
    }

    private void StopFleeing()
    {
        isFleeing = false;
        currentState = AIState.Idle;
    }
}
