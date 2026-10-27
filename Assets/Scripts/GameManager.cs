
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Level")]
    public Level currentLevel;

    [Header("Player Prefab")]
    public GameObject playerTankPrefab;

    [Header("AI Tank Prefabs")]
    public GameObject chasingTankPrefab;
    public GameObject fleeingTankPrefab;
    public GameObject sentryTankPrefab;
    public GameObject patrolTankPrefab;

    [Header("Controller Prefabs")]
    public GameObject playerControllerPrefab;
    public GameObject chaseControllerPrefab;
    public GameObject fleeControllerPrefab;
    public GameObject sentryControllerPrefab;
    public GameObject patrolControllerPrefab;

    [Header("Automatic Spawning")]
    public bool spawnOnMapGenerated = false;

    [Header("Player Respawning")]
    public float respawnDelay = 3f;

    [Header("Player Lives")]
    public int startingLives = 3;

    [Header("AI Respawning")]
    public float aiRespawnDelay = 5f;

    [Header("Active AI Controllers")]
    public List<AIController> activeAI =
        new List<AIController>();

    [Header("Game Status")]
    public int livesRemaining;
    public int playerScore;
    public int playerKills;
    public int highScore;
    public bool isGameOver;

    private readonly List<GameObject> spawnedObjects =
        new List<GameObject>();

    private readonly Dictionary<AIController, GameObject[]>
        aiRespawnPrefabs =
        new Dictionary<AIController, GameObject[]>();

    private TankPawn playerPawn;
    private PlayerController playerController;

    private float respawnTimer;
    private bool waitingToRespawn;
    private bool playerHasSpawned;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    private void Update()
    {
        if (!spawnOnMapGenerated ||
            !playerHasSpawned ||
            isGameOver)
        {
            return;
        }

        if (playerPawn != null)
            return;

        if (!waitingToRespawn)
        {
            waitingToRespawn = true;
            respawnTimer = respawnDelay;

            // Save score before destroying the controller.
            if (playerController != null)
            {
                playerScore = playerController.score;
                playerKills = playerController.kills;

                Destroy(playerController.gameObject);
                playerController = null;
            }

            livesRemaining--;

            Debug.Log("Player died! Lives remaining: " +
                livesRemaining);

            ClearAITargets();

            if (livesRemaining <= 0)
            {
                GameOver();
                return;
            }
        }

        respawnTimer -= Time.deltaTime;

        if (respawnTimer <= 0f)
        {
            waitingToRespawn = false;

            playerPawn = SpawnPlayer();

            if (playerPawn != null)
            {
                UpdateAITargets();
                Debug.Log("Player respawned! Lives: " +
                    livesRemaining);
            }
        }
    }

    public void OnMapGenerated()
    {
        if (!spawnOnMapGenerated)
            return;

        StopAllCoroutines();
        aiRespawnPrefabs.Clear();

        waitingToRespawn = false;
        playerHasSpawned = false;

        ClearSpawnedObjects();

        // Reset match variables.
        livesRemaining = Mathf.Max(1, startingLives);
        playerScore = 0;
        playerKills = 0;
        isGameOver = false;

        if (currentLevel == null ||
            currentLevel.Rooms == null)
        {
            Debug.LogError(
                "GameManager needs a generated Level."
            );
            return;
        }

        playerPawn = SpawnPlayer();

        if (playerPawn == null)
            return;

        playerHasSpawned = true;

        SpawnAIWithController(
            chasingTankPrefab,
            chaseControllerPrefab
        );

        SpawnAIWithController(
            fleeingTankPrefab,
            fleeControllerPrefab
        );

        SpawnAIWithController(
            sentryTankPrefab,
            sentryControllerPrefab
        );

        SpawnAIWithController(
            patrolTankPrefab,
            patrolControllerPrefab
        );

        Debug.Log("Game started! Lives: " +
            livesRemaining);
    }

    private TankPawn SpawnPlayer()
    {
        RoomTile room = GetRandomRoom();

        if (room == null ||
            room.playerSpawn == null ||
            playerTankPrefab == null ||
            playerControllerPrefab == null)
        {
            Debug.LogError(
                "Player spawning references are missing."
            );
            return null;
        }

        GameObject tank = Instantiate(
            playerTankPrefab,
            room.playerSpawn.position,
            room.playerSpawn.rotation
        );

        spawnedObjects.Add(tank);

        foreach (Controller controller
            in tank.GetComponents<Controller>())
        {
            controller.enabled = false;
        }

        TankPawn pawn = tank.GetComponent<TankPawn>();

        GameObject controllerObject =
            Instantiate(playerControllerPrefab);

        spawnedObjects.Add(controllerObject);

        PlayerController controllerScript =
            controllerObject.GetComponent<PlayerController>();

        if (pawn == null || controllerScript == null)
        {
            Debug.LogError(
                "Player tank or controller component is missing."
            );
            return null;
        }

        controllerScript.pawn = pawn;
        playerController = controllerScript;

        // Restore score after respawning.
        controllerScript.score = playerScore;
        controllerScript.kills = playerKills;

        TankShooter shooter =
            tank.GetComponent<TankShooter>();

        if (shooter != null)
        {
            shooter.owner = controllerScript;
        }

        return pawn;
    }

    private void SpawnAIWithController(
        GameObject tankPrefab,
        GameObject controllerPrefab)
    {
        RoomTile room = GetRandomRoom();

        if (room == null ||
            room.aiSpawn == null ||
            tankPrefab == null ||
            controllerPrefab == null)
        {
            Debug.LogWarning(
                "Skipping AI: spawning references are missing."
            );
            return;
        }

        GameObject tank = Instantiate(
            tankPrefab,
            room.aiSpawn.position,
            room.aiSpawn.rotation
        );

        spawnedObjects.Add(tank);

        foreach (AIController oldController
            in tank.GetComponents<AIController>())
        {
            oldController.enabled = false;
            UnregisterAI(oldController);
        }

        TankPawn pawn = tank.GetComponent<TankPawn>();

        GameObject controllerObject =
            Instantiate(controllerPrefab);

        spawnedObjects.Add(controllerObject);

        AIController controllerScript =
            controllerObject.GetComponent<AIController>();

        if (pawn == null || controllerScript == null)
        {
            Debug.LogError(
                "AI tank or controller component is missing."
            );
            return;
        }

        controllerScript.pawn = pawn;
        controllerScript.target = playerPawn;

        TankShooter shooter =
            tank.GetComponent<TankShooter>();

        if (shooter != null)
        {
            shooter.owner = controllerScript;
        }

        RegisterAI(controllerScript);

        aiRespawnPrefabs[controllerScript] =
            new GameObject[]
            {
                tankPrefab,
                controllerPrefab
            };
    }

    public void OnTankDied(Health deadTank)
    {
        if (deadTank == null)
            return;

        AIController deadAI = null;

        foreach (AIController ai in activeAI)
        {
            if (ai == null || ai.pawn == null)
                continue;

            if (ai.pawn.gameObject == deadTank.gameObject)
            {
                deadAI = ai;
                break;
            }
        }

        if (deadAI == null)
            return;

        GameObject[] prefabs;

        if (!aiRespawnPrefabs.TryGetValue(
            deadAI, out prefabs))
        {
            return;
        }

        UnregisterAI(deadAI);
        aiRespawnPrefabs.Remove(deadAI);

        deadAI.enabled = false;
        Destroy(deadAI.gameObject);

        if (!isGameOver)
        {
            StartCoroutine(
                RespawnAI(prefabs[0], prefabs[1])
            );
        }
    }

    private IEnumerator RespawnAI(
        GameObject tankPrefab,
        GameObject controllerPrefab)
    {
        yield return new WaitForSeconds(aiRespawnDelay);

        if (!spawnOnMapGenerated ||
            !playerHasSpawned ||
            isGameOver ||
            currentLevel == null)
        {
            yield break;
        }

        SpawnAIWithController(
            tankPrefab,
            controllerPrefab
        );

        Debug.Log("AI tank respawned!");
    }

    public void UpdatePlayerScore(
        int score,
        int kills)
    {
        playerScore = score;
        playerKills = kills;
    }

    private void GameOver()
    {
        isGameOver = true;
        waitingToRespawn = false;

        StopAllCoroutines();
        ClearAITargets();

        if (playerController != null)
        {
            playerScore = playerController.score;
            playerKills = playerController.kills;
        }

        if (playerScore > highScore)
        {
            highScore = playerScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        Debug.Log("========== GAME OVER ==========");
        Debug.Log("Final Score: " + playerScore);
        Debug.Log("Total Kills: " + playerKills);
        Debug.Log("High Score: " + highScore);
        Debug.Log("===============================");
    }

    private void ClearAITargets()
    {
        foreach (AIController ai in activeAI)
        {
            if (ai != null)
                ai.target = null;
        }
    }

    private void UpdateAITargets()
    {
        foreach (AIController ai in activeAI)
        {
            if (ai != null)
                ai.target = playerPawn;
        }
    }

    private void ClearSpawnedObjects()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                foreach (Controller controller
                    in obj.GetComponents<Controller>())
                {
                    controller.enabled = false;
                }

                Destroy(obj);
            }
        }

        spawnedObjects.Clear();
        activeAI.Clear();
        aiRespawnPrefabs.Clear();

        playerPawn = null;
        playerController = null;
    }

    public void RegisterAI(AIController ai)
    {
        if (ai != null && !activeAI.Contains(ai))
            activeAI.Add(ai);
    }

    public void UnregisterAI(AIController ai)
    {
        activeAI.Remove(ai);
    }

    public void SpawnAI(
        GameObject prefab,
        Transform spawnPoint)
    {
        if (prefab == null || spawnPoint == null)
            return;

        Instantiate(
            prefab,
            spawnPoint.position,
            spawnPoint.rotation
        );
    }

    public RoomTile GetRandomRoom()
    {
        if (currentLevel == null ||
            currentLevel.Rooms == null)
        {
            return null;
        }

        RoomTile[,] rooms = currentLevel.Rooms;

        int randomCol =
            Random.Range(0, rooms.GetLength(0));

        int randomRow =
            Random.Range(0, rooms.GetLength(1));

        return rooms[randomCol, randomRow];
    }

    public AIController FindNearestAI(
        Vector3 searchPosition,
        AIController excludedAI = null)
    {
        AIController nearestAI = null;
        float nearestDistanceSquared = Mathf.Infinity;

        foreach (AIController ai in activeAI)
        {
            if (ai == null ||
                ai == excludedAI ||
                ai.pawn == null)
            {
                continue;
            }

            float distanceSquared =
                (ai.pawn.transform.position -
                 searchPosition).sqrMagnitude;

            if (distanceSquared < nearestDistanceSquared)
            {
                nearestDistanceSquared = distanceSquared;
                nearestAI = ai;
            }
        }

        return nearestAI;
    }
}
