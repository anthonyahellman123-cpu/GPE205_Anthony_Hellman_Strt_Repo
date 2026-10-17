
using UnityEngine;

public class SentryAIController : AIController
{
    private enum SentryState
    {
        Scanning,
        Investigating,
        Attacking
    }

    [Header("Sentry Settings")]
    public float viewDistance = 20f;
    public float fieldOfView = 120f;
    public float scanTurnSpeed = 30f;

    [Header("Nearest Ally Search")]
    public float allySearchInterval = 1f;

    [Header("Runtime Information")]
    [SerializeField] private AIController nearestAlly;

    private SentryState sentryState = SentryState.Scanning;
    private float nextAllySearchTime;

    public override void ProcessInputs()
    {
        if (pawn == null)
            return;

        UpdateNearestAlly();

        if (target == null)
        {
            sentryState = SentryState.Scanning;
        }
        else if (CanSeeTarget())
        {
            sentryState = SentryState.Attacking;
        }
        else if (CanHearTarget())
        {
            sentryState = SentryState.Investigating;
        }
        else
        {
            sentryState = SentryState.Scanning;
        }

        switch (sentryState)
        {
            case SentryState.Scanning:
                Scan();
                break;

            case SentryState.Investigating:
                TurnTowards(target.transform.position);
                break;

            case SentryState.Attacking:
                AttackTarget();
                break;
        }
    }

    // Uses the GameManager list and a nearest-distance algorithm.
    private void UpdateNearestAlly()
    {
        if (Time.time < nextAllySearchTime)
            return;

        nextAllySearchTime =
            Time.time + Mathf.Max(0.1f, allySearchInterval);

        if (GameManager.instance == null)
        {
            nearestAlly = null;
            return;
        }

        nearestAlly = GameManager.instance.FindNearestAI(
            pawn.transform.position,
            this
        );
    }

    private void Scan()
    {
        pawn.transform.Rotate(
            0f,
            scanTurnSpeed * Time.deltaTime,
            0f
        );
    }

    private bool CanSeeTarget()
    {
        if (target == null)
            return false;

        Vector3 eyePosition =
            pawn.transform.position + Vector3.up * 0.5f;

        Vector3 targetPosition =
            target.transform.position + Vector3.up * 0.5f;

        Vector3 direction = targetPosition - eyePosition;
        float distance = direction.magnitude;

        if (distance > viewDistance || distance <= 0f)
            return false;

        float angle = Vector3.Angle(
            pawn.transform.forward,
            direction
        );

        if (angle > fieldOfView * 0.5f)
            return false;

        if (Physics.Raycast(
            eyePosition,
            direction.normalized,
            out RaycastHit hit,
            distance))
        {
            return hit.transform == target.transform
                || hit.transform.IsChildOf(target.transform);
        }

        return false;
    }
}
