
using UnityEngine;

public class TankShooter : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public Controller owner;

    [Header("Shooting Settings")]
    public float fireRate = 1f;
    public float fireForce = 20f;
    public float damage = 10f;

    private float nextFireTime;
    private NoiseMaker noiseMaker;

    private void Start()
    {
        noiseMaker = GetComponent<NoiseMaker>();
    }

    public void Shoot()
    {
        if (Time.time < nextFireTime)
            return;

        if (projectilePrefab == null || firePoint == null)
            return;

        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        Projectile projectileScript =
            projectile.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            projectileScript.damage = damage;
            projectileScript.owner = owner;
            projectileScript.firingTank = gameObject;
        }

        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(
                firePoint.forward * fireForce,
                ForceMode.Impulse
            );
        }

        // Generate sound when a shot is fired.
        if (noiseMaker != null)
        {
            noiseMaker.MakeShootingNoise();
        }

        nextFireTime = Time.time + fireRate;
    }
}
