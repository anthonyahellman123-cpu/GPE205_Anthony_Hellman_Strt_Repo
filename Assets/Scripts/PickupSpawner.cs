
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    public GameObject[] pickupPrefabs;
    public float respawnDelay = 15f;

    private GameObject activePickup;
    private float respawnTimer;
    private bool waitingToRespawn;

    private void Start()
    {
        SpawnPickup();
    }

    private void Update()
    {
        if (activePickup != null)
            return;

        if (!waitingToRespawn)
        {
            respawnTimer = respawnDelay;
            waitingToRespawn = true;
        }

        respawnTimer -= Time.deltaTime;

        if (respawnTimer <= 0f)
        {
            SpawnPickup();
        }
    }

    private void SpawnPickup()
    {
        if (pickupPrefabs == null || pickupPrefabs.Length == 0)
            return;

        int randomIndex = Random.Range(0, pickupPrefabs.Length);

        if (pickupPrefabs[randomIndex] == null)
            return;

        activePickup = Instantiate(
            pickupPrefabs[randomIndex],
            transform.position,
            Quaternion.identity,
            transform
        );

        waitingToRespawn = false;
    }
}
