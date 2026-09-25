using UnityEngine;

public class TankPawn : Pawn
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
    public override void moveForward()
    {
        mover.Move(transform.forward, moveSpeed);
    }
    public override void moveBackward()
    {
        mover.Move(transform.forward, -moveSpeed);
   }
    public override void rotateClockwise()
    {
        mover.Rotate(turnSpeed);
    }
    public override void rotateCounterClockwise()
    {
        mover.Rotate(-turnSpeed);
    }
}
