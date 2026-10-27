
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage, Controller attacker = null)
    {
        if (isDead)
            return;

        currentHealth = Mathf.Clamp(
            currentHealth - damage, 0f, maxHealth
        );

        if (currentHealth <= 0f)
        {
            isDead = true;

            PlayerController player = attacker as PlayerController;

            if (player != null)
            {
                player.AddKill();
            }

            OnDie();
        }
    }

    public void OnDie()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.OnTankDied(this);
        }

        Destroy(gameObject);
    }
}
