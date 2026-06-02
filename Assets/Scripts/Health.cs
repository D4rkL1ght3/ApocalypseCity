using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private bool destroyOnDeath = true;

    [Header("Events")]
    public UnityEvent onTakeDamage;
    public UnityEvent onDeath;

    public event Action<float, float> OnHealthChanged;

    private float currentHealth;
    private bool isDead;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        NotifyHealthChanged();
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead)
            return;

        if (damageAmount <= 0)
            return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log(gameObject.name + " took " + damageAmount + " damage. HP: " + currentHealth + "/" + maxHealth);

        NotifyHealthChanged();
        onTakeDamage?.Invoke();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        if (isDead)
            return;

        if (healAmount <= 0)
            return;

        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log(gameObject.name + " healed " + healAmount + " HP. HP: " + currentHealth + "/" + maxHealth);

        NotifyHealthChanged();
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log(gameObject.name + " died.");

        onDeath?.Invoke();

        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
    }

    public void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}