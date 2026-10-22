
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    protected abstract Powerup CreatePowerup();

    private void OnTriggerEnter(Collider other)
    {
        PowerupManager manager =
            other.GetComponentInParent<PowerupManager>();

        if (manager == null)
            return;

        manager.Add(CreatePowerup());

        Destroy(gameObject);
    }
}
