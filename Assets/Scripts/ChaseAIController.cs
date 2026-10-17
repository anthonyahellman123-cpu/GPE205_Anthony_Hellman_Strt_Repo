
using UnityEngine;

public class ChaseAIController : AIController
{
    public override void ProcessInputs()
    {
        if (pawn == null || target == null) return;

        if (!IsTargetInRange())
        {
            currentState = AIState.Idle;
        }
        else if (DistanceToTarget() > stoppingDistance)
        {
            currentState = AIState.Chase;
        }
        else
        {
            currentState = AIState.Attack;
        }

        switch (currentState)
        {
            case AIState.Idle:
                break;

            case AIState.Chase:
                MoveTowards(target.transform.position);
                break;

            case AIState.Attack:
                AttackTarget();
                break;
        }
    }
}
