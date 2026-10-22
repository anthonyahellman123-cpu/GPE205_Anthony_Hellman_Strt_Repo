
using UnityEngine;

public class DamagePickup : Pickup
{
    public DamagePowerup powerup = new DamagePowerup();

    protected override Powerup CreatePowerup()
    {
        powerup.isPermanent = true;
        return powerup;
    }
}
