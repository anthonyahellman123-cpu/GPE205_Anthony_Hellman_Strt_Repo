
using UnityEngine;

public class NoiseMaker : MonoBehaviour
{
    [Header("Noise Settings")]
    public float volumeDistance = 0f;
    public float movementNoise = 5f;
    public float shootingNoise = 15f;
    public float noiseDecayRate = 10f;

    private void Update()
    {
        // Noise fades over time.
        volumeDistance = Mathf.MoveTowards(
            volumeDistance,
            0f,
            noiseDecayRate * Time.deltaTime
        );
    }

    // Called whenever the tank moves.
    public void MakeMovementNoise()
    {
        volumeDistance = Mathf.Max(
            volumeDistance,
            movementNoise
        );
    }

    // Called whenever the tank fires.
    public void MakeShootingNoise()
    {
        volumeDistance = Mathf.Max(
            volumeDistance,
            shootingNoise
        );
    }
}
