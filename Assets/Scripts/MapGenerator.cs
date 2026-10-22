
using System;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum SeedMode
    {
        Random,
        Preset,
        Daily
    }

    [Header("Room Prefabs")]
    public GameObject[] gridPrefabs;

    [Header("Map Size")]
    public int rows = 3;
    public int cols = 3;

    [Header("Room Dimensions")]
    public float roomWidth = 10f;
    public float roomHeight = 10f;

    [Header("Random Seed")]
    public SeedMode seedMode = SeedMode.Random;
    public int presetSeed = 12345;

    private RoomTile[,] grid;

    public RoomTile[,] Grid => grid;

    void Start()
    {
        GenerateMap();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateMap();
        }
    }

    public GameObject RandomRoomPrefab()
    {
        return gridPrefabs[UnityEngine.Random.Range(0, gridPrefabs.Length)];
    }

    public void GenerateMap()
    {
        if (gridPrefabs == null || gridPrefabs.Length == 0)
        {
            Debug.LogError("MapGenerator needs room prefabs.");
            return;
        }

        if (rows < 1 || cols < 1)
        {
            Debug.LogError("MapGenerator needs at least one row and column.");
            return;
        }

        if (grid != null)
        {
            foreach (RoomTile oldRoom in grid)
            {
                if (oldRoom != null)
                {
                    Destroy(oldRoom.gameObject);
                }
            }
        }

        int seed;

        switch (seedMode)
        {
            case SeedMode.Preset:
                seed = presetSeed;
                break;

            case SeedMode.Daily:
                seed = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                break;

            default:
                seed = Guid.NewGuid().GetHashCode();
                break;
        }

        UnityEngine.Random.State previousRandomState = UnityEngine.Random.state;
        UnityEngine.Random.InitState(seed);

        Debug.Log("Generating map with seed: " + seed);

        grid = new RoomTile[cols, rows];

        for (int currentRow = 0; currentRow < rows; currentRow++)
        {
            for (int currentCol = 0; currentCol < cols; currentCol++)
            {
                float xPosition = roomWidth * currentCol;
                float zPosition = roomHeight * currentRow;

                Vector3 newPosition = new Vector3(xPosition, 0f, zPosition);

                GameObject roomObject = Instantiate(
                    RandomRoomPrefab(),
                    newPosition,
                    Quaternion.identity,
                    transform
                );

                roomObject.name = "Room_" + currentCol + "," + currentRow;

                RoomTile room = roomObject.GetComponent<RoomTile>();
                grid[currentCol, currentRow] = room;

                bool northOpen = currentRow < rows - 1;
                bool southOpen = currentRow > 0;
                bool eastOpen = currentCol < cols - 1;
                bool westOpen = currentCol > 0;

                room.SetDoors(northOpen, southOpen, eastOpen, westOpen);
            }
        }

        UnityEngine.Random.state = previousRandomState;

        if (GameManager.instance != null)
        {
            GameManager.instance.OnMapGenerated();
        }
    }
}
