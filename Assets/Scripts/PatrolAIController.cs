
using UnityEngine;

public class PatrolAIController : AIController
{
    [Header("Patrol Settings")]
    public Transform[] waypoints;
    public float waypointDistance = 2f;
    public float waitTime = 2f;
    public bool loopPatrol = true;

    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private bool patrolComplete = false;

    public override void ProcessInputs()
    {
        if (pawn == null)
            return;

        // Attack the player if they enter detection range.
        if (target != null && IsTargetInRange())
        {
            currentState = AIState.Attack;
            AttackTarget();
            return;
        }

        // Stop patrolling if the route is complete.
        if (patrolComplete)
        {
            currentState = AIState.Idle;
            return;
        }

        if (waypoints == null || waypoints.Length == 0)
        {
            currentState = AIState.Idle;
            return;
        }

        currentState = AIState.Patrol;
        Patrol();
    }

    private void Patrol()
    {
        Transform currentWaypoint = waypoints[currentWaypointIndex];

        if (currentWaypoint == null)
            return;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                isWaiting = false;
                AdvanceWaypoint();
            }

            return;
        }

        Vector3 difference =
            currentWaypoint.position - pawn.transform.position;

        difference.y = 0f;

        if (difference.magnitude <= waypointDistance)
        {
            isWaiting = true;
            waitTimer = waitTime;
            return;
        }

        MoveTowards(currentWaypoint.position);
    }

    private void AdvanceWaypoint()
    {
        currentWaypointIndex++;

        if (currentWaypointIndex >= waypoints.Length)
        {
            if (loopPatrol)
            {
                currentWaypointIndex = 0;
            }
            else
            {
                patrolComplete = true;
                currentWaypointIndex = waypoints.Length - 1;
            }
        }
    }
}
