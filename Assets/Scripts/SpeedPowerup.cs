
using UnityEngine;

[System.Serializable]
public class SpeedPowerup : Powerup
{
    public float speedToAdd = 5f;

    public override void Apply(PowerupManager target)
    {
        TankPawn tank = target.GetComponent<TankPawn>();

        if (tank != null)
        {
            tank.moveSpeed += speedToAdd;
        }
    }

    public override void Remove(PowerupManager target)
    {
        TankPawn tank = target.GetComponent<TankPawn>();

        if (tank != null)
        {
            tank.moveSpeed -= speedToAdd;
        }
    }
}
