
using UnityEngine;

public class PlayerController : Controller
{
    public KeyCode moveForwardKey;
    public KeyCode moveBackwardKey;
    public KeyCode rotateClockwiseKey;
    public KeyCode rotateCounterClockwiseKey;
    public KeyCode shootKey = KeyCode.Space;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        ProcessInputs();
        base.Update();
    }

    public override void ProcessInputs()
    {
        if (Input.GetKey(moveForwardKey))
        {
            pawn.moveForward();
        }

        if (Input.GetKey(moveBackwardKey))
        {
            pawn.moveBackward();
        }

        if (Input.GetKey(rotateClockwiseKey))
        {
            pawn.rotateClockwise();
        }

        if (Input.GetKey(rotateCounterClockwiseKey))
        {
            pawn.rotateCounterClockwise();
        }

        if (Input.GetKey(shootKey))
        {
            TankPawn tank = pawn as TankPawn;

            if (tank != null)
            {
                tank.Shoot();
            }
        }
    }
}
