using UnityEngine;

public class PlayerStatusUI : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private Hunger playerHunger;

    [Header("UI Bars")]
    [SerializeField] private StatusBarUI healthBar;
    [SerializeField] private StatusBarUI hungerBar;

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealthBar;
        }

        if (playerHunger != null)
        {
            playerHunger.OnHungerChanged += UpdateHungerBar;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthBar;
        }

        if (playerHunger != null)
        {
            playerHunger.OnHungerChanged -= UpdateHungerBar;
        }
    }

    private void Start()
    {
        if (playerHealth != null)
        {
            UpdateHealthBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        }

        if (playerHunger != null)
        {
            UpdateHungerBar(playerHunger.CurrentHunger, playerHunger.MaxHunger);
        }
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.UpdateBar(currentHealth, maxHealth);
        }
    }

    private void UpdateHungerBar(float currentHunger, float maxHunger)
    {
        if (hungerBar != null)
        {
            hungerBar.UpdateBar(currentHunger, maxHunger);
        }
    }
}