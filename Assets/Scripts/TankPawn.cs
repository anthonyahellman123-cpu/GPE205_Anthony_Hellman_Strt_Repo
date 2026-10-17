
using UnityEngine;

public class TankPawn : Pawn
{
    [Header("Shooting")]
    public TankShooter shooter;

    private NoiseMaker noiseMaker;

    public override void Start()
    {
        base.Start();

        if (shooter == null)
        {
            shooter = GetComponent<TankShooter>();
        }

        noiseMaker = GetComponent<NoiseMaker>();
    }

    public override void moveForward()
    {
        if (mover != null)
        {
            mover.Move(transform.forward, moveSpeed);
            MakeMovementNoise();
        }
    }

    public override void moveBackward()
    {
        if (mover != null)
        {
            mover.Move(-transform.forward, moveSpeed);
            MakeMovementNoise();
        }
    }

    public override void rotateClockwise()
    {
        if (mover != null)
        {
            mover.Rotate(turnSpeed);
            MakeMovementNoise();
        }
    }

    public override void rotateCounterClockwise()
    {
        if (mover != null)
        {
            mover.Rotate(-turnSpeed);
            MakeMovementNoise();
        }
    }

    public void Shoot()
    {
        if (shooter != null)
        {
            shooter.Shoot();
        }
    }

    private void MakeMovementNoise()
    {
        if (noiseMaker != null)
        {
            noiseMaker.MakeMovementNoise();
        }
    }
}
