
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 100f;
    public float lifetime = 5f;
    public float impactForce = 15f;

    public Controller owner;
    public GameObject firingTank;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (firingTank != null)
        {
            if (other.transform == firingTank.transform ||
                other.transform.IsChildOf(firingTank.transform))
            {
                return;
            }
        }

        Rigidbody targetRb = other.attachedRigidbody;

        if (targetRb != null)
        {
            Vector3 direction = transform.forward;

            targetRb.AddForce(
                direction * impactForce,
                ForceMode.Impulse
            );
        }

        Health health = other.GetComponentInParent<Health>();

        if (health != null)
        {
            health.TakeDamage(damage, owner);
        }

        Destroy(gameObject);
    }
}
