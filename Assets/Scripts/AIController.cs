
using UnityEngine;

public abstract class AIController : Controller
{
    [Header("Target Settings")]
    public TankPawn target;

    [Header("Movement and Combat")]
    public float detectionRange = 15f;
    public float stoppingDistance = 3f;
    public float aimAngle = 10f;

    [Header("Hearing Settings")]
    public float hearingRange = 10f;

    protected enum AIState
    {
        Idle,
        Chase,
        Attack,
        Flee,
        Patrol
    }

    protected AIState currentState = AIState.Idle;

    public override void Start()
    {
        base.Start();

        if (pawn == null)
        {
            pawn = GetComponent<TankPawn>();
        }

        RegisterWithGameManager();
    }

    private void OnEnable()
    {
        RegisterWithGameManager();
    }

    private void OnDisable()
    {
        UnregisterFromGameManager();
    }

    private void OnDestroy()
    {
        UnregisterFromGameManager();
    }

    private void RegisterWithGameManager()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.RegisterAI(this);
        }
    }

    private void UnregisterFromGameManager()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.UnregisterAI(this);
        }
    }

    // Checks whether the target is inside detection range.
    protected bool IsTargetInRange()
    {
        if (pawn == null || target == null)
            return false;

        float distance = Vector3.Distance(
            pawn.transform.position,
            target.transform.position
        );

        return distance <= detectionRange;
    }

    // Returns distance between AI and target.
    protected float DistanceToTarget()
    {
        if (pawn == null || target == null)
            return Mathf.Infinity;

        return Vector3.Distance(
            pawn.transform.position,
            target.transform.position
        );
    }

    // Detects noise produced by the target.
    protected bool CanHearTarget()
    {
        if (pawn == null || target == null)
            return false;

        NoiseMaker noise = target.GetComponent<NoiseMaker>();

        if (noise == null)
            return false;

        float distance = Vector3.Distance(
            pawn.transform.position,
            target.transform.position
        );

        return distance <= hearingRange + noise.volumeDistance;
    }

    // Rotates the tank toward a world position.
    protected void TurnTowards(Vector3 position)
    {
        if (pawn == null)
            return;

        Vector3 direction =
            position - pawn.transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        float angle = Vector3.SignedAngle(
            pawn.transform.forward,
            direction,
            Vector3.up
        );

        if (angle > aimAngle)
        {
            pawn.rotateClockwise();
        }
        else if (angle < -aimAngle)
        {
            pawn.rotateCounterClockwise();
        }
    }

    // Moves toward a world position.
    protected void MoveTowards(Vector3 position)
    {
        if (pawn == null)
            return;

        TurnTowards(position);
        pawn.moveForward();
    }

    // Aims and fires using the shared TankPawn.
    protected void AttackTarget()
    {
        if (pawn == null || target == null)
            return;

        TurnTowards(target.transform.position);

        Vector3 direction =
            target.transform.position -
            pawn.transform.position;

        direction.y = 0f;

        float angle = Vector3.Angle(
            pawn.transform.forward,
            direction
        );

        if (angle <= aimAngle)
        {
            TankPawn tank = pawn as TankPawn;

            if (tank != null)
            {
                tank.Shoot();
            }
        }
    }
}
