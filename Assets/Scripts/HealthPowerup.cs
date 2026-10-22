
using UnityEngine;

[System.Serializable]
public class HealthPowerup : Powerup
{
    public float healthToAdd = 25f;

    public override void Apply(PowerupManager target)
    {
        Health health = target.GetComponent<Health>();

        if (health != null)
        {
            health.currentHealth = Mathf.Min(
                health.currentHealth + healthToAdd,
                health.maxHealth
            );
        }
    }

    public override void Remove(PowerupManager target)
    {
    }
}
