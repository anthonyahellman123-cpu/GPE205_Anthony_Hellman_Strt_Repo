
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    [Header("HUD Text")]
    public Text scoreText;
    public Text killsText;
    public Text livesText;

    [Header("Player Health")]
    public Slider healthSlider;
    public Text healthText;

    [Header("Buff Display")]
    public Text damageBuffText;
    public Text speedBuffText;

    [Header("Game Over Screen")]
    public GameObject gameOverPanel;
    public Text finalScoreText;
    public Text highScoreText;

    private GameManager gameManager;

    private Health playerHealth;
    private PowerupManager playerPowerups;

    private GameObject currentPlayerTank;

    private void Start()
    {
        gameManager = GameManager.instance;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.instance;
        }

        if (gameManager == null)
            return;

        UpdateScore();
        FindPlayerTank();
        UpdateHealth();
        UpdateBuffs();

        if (gameManager.isGameOver)
        {
            ShowGameOver();
        }
    }

    private void UpdateScore()
    {
        if (scoreText != null)
        {
            scoreText.text =
                "Score: " + gameManager.playerScore;
        }

        if (killsText != null)
        {
            killsText.text =
                "Kills: " + gameManager.playerKills;
        }

        if (livesText != null)
        {
            livesText.text =
                "Lives: " + gameManager.livesRemaining;
        }
    }

    private void FindPlayerTank()
    {
        // Search for the currently controlled player tank.
        PlayerController[] controllers =
            FindObjectsByType<PlayerController>(
                FindObjectsSortMode.None
            );

        GameObject foundTank = null;

        foreach (PlayerController controller in controllers)
        {
            if (controller == null ||
                !controller.enabled ||
                controller.pawn == null)
            {
                continue;
            }

            foundTank = controller.pawn.gameObject;
            break;
        }

        if (foundTank == currentPlayerTank)
            return;

        currentPlayerTank = foundTank;

        playerHealth = null;
        playerPowerups = null;

        if (currentPlayerTank != null)
        {
            playerHealth =
                currentPlayerTank.GetComponent<Health>();

            playerPowerups =
                currentPlayerTank.GetComponent<PowerupManager>();
        }
    }

    private void UpdateHealth()
    {
        if (playerHealth == null)
        {
            if (healthSlider != null)
                healthSlider.value = 0f;

            if (healthText != null)
                healthText.text = "HP: 0 / 0";

            return;
        }

        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = playerHealth.maxHealth;
            healthSlider.value = playerHealth.currentHealth;
        }

        if (healthText != null)
        {
            healthText.text =
                "HP: " +
                Mathf.CeilToInt(playerHealth.currentHealth) +
                " / " +
                Mathf.CeilToInt(playerHealth.maxHealth);
        }
    }

    private void UpdateBuffs()
    {
        if (playerPowerups == null)
        {
            if (damageBuffText != null)
                damageBuffText.text = "Damage Buffs: 0";

            if (speedBuffText != null)
                speedBuffText.text = "Speed Buffs: 0";

            return;
        }

        if (damageBuffText != null)
        {
            damageBuffText.text =
                "Damage Buffs: " +
                playerPowerups.damageStacks;
        }

        if (speedBuffText != null)
        {
            if (playerPowerups.speedStacks > 0)
            {
                speedBuffText.text =
                    "Speed Buffs: " +
                    playerPowerups.speedStacks +
                    " (" +
                    playerPowerups.speedTimeRemaining.ToString("F1") +
                    "s)";
            }
            else
            {
                speedBuffText.text = "Speed Buffs: 0";
            }
        }
    }

    private void ShowGameOver()
    {
        if (gameOverPanel == null ||
            gameOverPanel.activeSelf)
        {
            return;
        }

        gameOverPanel.SetActive(true);

        if (finalScoreText != null)
        {
            finalScoreText.text =
                "Final Score: " + gameManager.playerScore;
        }

        if (highScoreText != null)
        {
            highScoreText.text =
                "High Score: " + gameManager.highScore;
        }
    }

    public void RestartGame()
    {
        if (GameManager.instance != null)
        {
            Destroy(GameManager.instance.gameObject);
        }

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
