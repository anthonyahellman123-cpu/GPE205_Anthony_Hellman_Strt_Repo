using UnityEngine;

public class PlayerController : Controller
{
    public KeyCode moveForwardKey;

    public KeyCode moveBackwardKey;

    public KeyCode rotateClockwiseKey;

    public KeyCode rotateCounterClockwiseKey;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start(); 
    }

    // Update is called once per frame
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

    }

}
