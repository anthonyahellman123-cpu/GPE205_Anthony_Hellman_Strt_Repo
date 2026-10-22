
using UnityEngine;

public class HealthPickup : Pickup
{
    public HealthPowerup powerup = new HealthPowerup();

    protected override Powerup CreatePowerup()
    {
        powerup.isPermanent = true;
        return powerup;
    }
}
