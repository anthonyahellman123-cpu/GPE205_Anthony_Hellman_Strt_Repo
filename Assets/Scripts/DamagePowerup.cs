
using UnityEngine;

[System.Serializable]
public class DamagePowerup : Powerup
{
    public float damageToAdd = 5f;

    public override void Apply(PowerupManager target)
    {
        TankShooter shooter = target.GetComponent<TankShooter>();

        if (shooter != null)
        {
            shooter.damage += damageToAdd;
        }
    }

    public override void Remove(PowerupManager target)
    {
        TankShooter shooter = target.GetComponent<TankShooter>();

        if (shooter != null)
        {
            shooter.damage -= damageToAdd;
        }
    }
}
