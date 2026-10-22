
using UnityEngine;

public class PatrolAIController : AIController
{
    [Header("Patrol Settings")]
    public Transform[] waypoints;
    public float waypointDistance = 2f;
    public float waitTime = 2f;
    public bool loopPatrol = true;

    [Header("Procedural Patrol")]
    public bool useGeneratedPatrol = false;
    public float patrolWidth = 4f;
    public float patrolHeight = 4f;
    public float randomOffset = 1f;

    private Vector3[] generatedWaypoints;

    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private bool patrolComplete = false;

    public override void Start()
    {
        base.Start();

        if (useGeneratedPatrol)
        {
            GeneratePatrolRoute();
        }
    }

    public override void ProcessInputs()
    {
        if (pawn == null)
            return;

        if (target != null && IsTargetInRange())
        {
            currentState = AIState.Attack;
            AttackTarget();
            return;
        }

        if (patrolComplete)
        {
            currentState = AIState.Idle;
            return;
        }

        int waypointCount = useGeneratedPatrol
            ? (generatedWaypoints != null ? generatedWaypoints.Length : 0)
            : (waypoints != null ? waypoints.Length : 0);

        if (waypointCount == 0)
        {
            currentState = AIState.Idle;
            return;
        }

        currentState = AIState.Patrol;
        Patrol();
    }

    private void GeneratePatrolRoute()
    {
        if (pawn == null)
            return;

        Vector3 center = pawn.transform.position;

        center.x += Random.Range(-randomOffset, randomOffset);
        center.z += Random.Range(-randomOffset, randomOffset);

        float halfWidth = patrolWidth * 0.5f;
        float halfHeight = patrolHeight * 0.5f;

        float angle = Random.Range(0f, 360f);
        Quaternion rotation = Quaternion.Euler(0f, angle, 0f);

        Vector3[] corners =
        {
            new Vector3(-halfWidth, 0f, -halfHeight),
            new Vector3( halfWidth, 0f, -halfHeight),
            new Vector3( halfWidth, 0f,  halfHeight),
            new Vector3(-halfWidth, 0f,  halfHeight)
        };

        generatedWaypoints = new Vector3[4];

        for (int i = 0; i < corners.Length; i++)
        {
            generatedWaypoints[i] = center + rotation * corners[i];
        }

        currentWaypointIndex = 0;
        patrolComplete = false;
        isWaiting = false;
    }

    private void Patrol()
    {
        Vector3 destination;

        if (useGeneratedPatrol)
        {
            destination = generatedWaypoints[currentWaypointIndex];
        }
        else
        {
            Transform currentWaypoint = waypoints[currentWaypointIndex];

            if (currentWaypoint == null)
                return;

            destination = currentWaypoint.position;
        }

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

        Vector3 difference = destination - pawn.transform.position;
        difference.y = 0f;

        if (difference.magnitude <= waypointDistance)
        {
            isWaiting = true;
            waitTimer = waitTime;
            return;
        }

        MoveTowards(destination);
    }

    private void AdvanceWaypoint()
    {
        currentWaypointIndex++;

        int waypointCount = useGeneratedPatrol
            ? generatedWaypoints.Length
            : waypoints.Length;

        if (currentWaypointIndex >= waypointCount)
        {
            if (loopPatrol)
            {
                currentWaypointIndex = 0;
            }
            else
            {
                patrolComplete = true;
                currentWaypointIndex = waypointCount - 1;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (generatedWaypoints == null ||
            generatedWaypoints.Length == 0)
            return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < generatedWaypoints.Length; i++)
        {
            Gizmos.DrawSphere(generatedWaypoints[i], 0.3f);

            int nextIndex = (i + 1) % generatedWaypoints.Length;

            Gizmos.DrawLine(
                generatedWaypoints[i],
                generatedWaypoints[nextIndex]
            );
        }
    }
}
