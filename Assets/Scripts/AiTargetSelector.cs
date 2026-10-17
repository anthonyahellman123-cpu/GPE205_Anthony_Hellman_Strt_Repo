
using UnityEngine;

public class AITargetSelector : MonoBehaviour
{
    [Header("Target Search")]
    public float searchInterval = 1f;

    [Header("Current Result")]
    public AIController nearestAI;

    private float nextSearchTime;

    private void Update()
    {
        if (Time.time < nextSearchTime)
            return;

        nextSearchTime = Time.time + Mathf.Max(0.1f, searchInterval);

        if (GameManager.instance == null)
            return;

        nearestAI = GameManager.instance.FindNearestAI(
            transform.position
        );
    }
}
