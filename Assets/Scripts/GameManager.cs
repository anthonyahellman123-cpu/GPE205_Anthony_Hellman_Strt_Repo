
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Active AI Controllers")]
    public List<AIController> activeAI = new List<AIController>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterAI(AIController ai)
    {
        if (ai != null && !activeAI.Contains(ai))
        {
            activeAI.Add(ai);
        }
    }

    public void UnregisterAI(AIController ai)
    {
        activeAI.Remove(ai);
    }

    // Finds the nearest AI, optionally excluding one controller.
    public AIController FindNearestAI(
        Vector3 searchPosition,
        AIController excludedAI = null)
    {
        AIController nearestAI = null;
        float nearestDistanceSquared = Mathf.Infinity;

        foreach (AIController ai in activeAI)
        {
            if (ai == null || ai == excludedAI || ai.pawn == null)
                continue;

            float distanceSquared =
                (ai.pawn.transform.position - searchPosition).sqrMagnitude;

            if (distanceSquared < nearestDistanceSquared)
            {
                nearestDistanceSquared = distanceSquared;
                nearestAI = ai;
            }
        }

        return nearestAI;
    }
}
