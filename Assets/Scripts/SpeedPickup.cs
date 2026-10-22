
using UnityEngine;

public class SpeedPickup : Pickup
{
    public SpeedPowerup powerup = new SpeedPowerup();

    protected override Powerup CreatePowerup()
    {
        powerup.isPermanent = false;
        powerup.duration = 10f;
        return powerup;
    }
}
