
using UnityEngine;

public class RoomTile : MonoBehaviour
{
    [Header("Room Doors")]
    [SerializeField] private GameObject northDoor;
    [SerializeField] private GameObject southDoor;
    [SerializeField] private GameObject eastDoor;
    [SerializeField] private GameObject westDoor;

    [Header("Spawn Points")]
    public Transform playerSpawn;
    public Transform aiSpawn;
    public Transform pickupSpawn;

    [Header("Pickup Spawning")]
    public GameObject pickupSpawnerPrefab;

    private GameObject spawnedPickupSpawner;

    private void Start()
    {
        if (pickupSpawnerPrefab != null && pickupSpawn != null)
        {
            spawnedPickupSpawner = Instantiate(
                pickupSpawnerPrefab,
                pickupSpawn.position,
                pickupSpawn.rotation,
                transform
            );
        }
    }

    public void SetDoors(bool northOpen, bool southOpen, bool eastOpen, bool westOpen)
    {
        northDoor.SetActive(!northOpen);
        southDoor.SetActive(!southOpen);
        eastDoor.SetActive(!eastOpen);
        westDoor.SetActive(!westOpen);
    }
}
