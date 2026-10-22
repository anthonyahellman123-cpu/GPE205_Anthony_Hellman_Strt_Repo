
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    public List<Powerup> powerups = new List<Powerup>();

    private List<Powerup> removedPowerupQueue = new List<Powerup>();

    public void Add(Powerup powerupToAdd)
    {
        if (powerupToAdd == null)
            return;

        Powerup activePowerup = (Powerup)System.Activator.CreateInstance(
            powerupToAdd.GetType()
        );

        JsonUtility.FromJsonOverwrite(
            JsonUtility.ToJson(powerupToAdd),
            activePowerup
        );

        activePowerup.Apply(this);

        if (!activePowerup.isPermanent && activePowerup.duration > 0f)
        {
            powerups.Add(activePowerup);
        }
    }

    public void Remove(Powerup powerupToRemove)
    {
        if (powerupToRemove == null ||
            removedPowerupQueue.Contains(powerupToRemove))
            return;

        powerupToRemove.Remove(this);
        removedPowerupQueue.Add(powerupToRemove);
    }

    private void Update()
    {
        foreach (Powerup powerup in powerups)
        {
            powerup.duration -= Time.deltaTime;

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
