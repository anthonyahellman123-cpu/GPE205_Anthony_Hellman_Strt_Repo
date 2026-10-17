
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(
            currentHealth - damage, 0f, maxHealth
        );

        if (currentHealth <= 0f)
        {
            OnDie();
        }
    }

    public void OnDie()
    {
        Destroy(gameObject);
    }
}
