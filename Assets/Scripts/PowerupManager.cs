
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    [Header("Active Temporary Powerups")]
    public List<Powerup> powerups = new List<Powerup>();

    [Header("Buff Tracking")]
    public int damageStacks = 0;

    public int speedStacks = 0;

    public float speedTimeRemaining = 0f;

    private List<Powerup> removedPowerupQueue =
        new List<Powerup>();

    public void Add(Powerup powerupToAdd)
    {
        if (powerupToAdd == null)
            return;

        Powerup activePowerup =
            (Powerup)System.Activator.CreateInstance(
                powerupToAdd.GetType()
            );

        JsonUtility.FromJsonOverwrite(
            JsonUtility.ToJson(powerupToAdd),
            activePowerup
        );

        activePowerup.Apply(this);

        // Count permanent damage buffs.
        if (activePowerup is DamagePowerup)
        {
            damageStacks++;
        }

        // Count temporary speed buffs.
        if (activePowerup is SpeedPowerup)
        {
            speedStacks++;
        }

        // Store temporary powerups for expiration.
        if (!activePowerup.isPermanent &&
            activePowerup.duration > 0f)
        {
            powerups.Add(activePowerup);
        }
    }

    public void Remove(Powerup powerupToRemove)
    {
        if (powerupToRemove == null ||
            removedPowerupQueue.Contains(powerupToRemove))
        {
            return;
        }

        powerupToRemove.Remove(this);

        if (powerupToRemove is SpeedPowerup)
        {
            speedStacks = Mathf.Max(0, speedStacks - 1);
        }

        removedPowerupQueue.Add(powerupToRemove);
    }

    private void Update()
    {
        speedTimeRemaining = 0f;

        foreach (Powerup powerup in powerups)
        {
            if (powerup == null)
                continue;

            powerup.duration -= Time.deltaTime;

            if (powerup is SpeedPowerup)
            {
                speedTimeRemaining = Mathf.Max(
                    speedTimeRemaining,
                    powerup.duration
                );
            }

            if (powerup.duration <= 0f)
            {
                Remove(powerup);
            }
        }
    }

    private void LateUpdate()
    {
        foreach (Powerup powerup in removedPowerupQueue)
        {
            powerups.Remove(powerup);
        }

        removedPowerupQueue.Clear();
    }
}
