
using UnityEngine;

public class PlayerController : Controller
{
    [Header("Movement Keys")]
    public KeyCode moveForwardKey;
    public KeyCode moveBackwardKey;
    public KeyCode rotateClockwiseKey;
    public KeyCode rotateCounterClockwiseKey;
    public KeyCode shootKey = KeyCode.Space;

    [Header("Player Score")]
    public int score = 0;
    public int kills = 0;
    public int pointsPerKill = 100;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        if (pawn == null)
            return;

        if (GameManager.instance != null &&
            GameManager.instance.isGameOver)
        {
            return;
        }

        base.Update();
    }

    public override void ProcessInputs()
    {
        if (pawn == null)
            return;

        if (Input.GetKey(moveForwardKey))
            pawn.moveForward();

        if (Input.GetKey(moveBackwardKey))
            pawn.moveBackward();

        if (Input.GetKey(rotateClockwiseKey))
            pawn.rotateClockwise();

        if (Input.GetKey(rotateCounterClockwiseKey))
            pawn.rotateCounterClockwise();

        if (Input.GetKey(shootKey))
        {
            TankPawn tank = pawn as TankPawn;

            if (tank != null)
                tank.Shoot();
        }
    }

    public void AddKill()
    {
        kills++;
        score += pointsPerKill;

        if (GameManager.instance != null)
        {
            GameManager.instance.UpdatePlayerScore(
                score, kills
            );
        }

        Debug.Log(
            "Kills: " + kills +
            " | Score: " + score
        );
    }
}
