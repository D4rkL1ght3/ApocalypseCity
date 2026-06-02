using System;
using UnityEngine;
using UnityEngine.Events;

public class Hunger : MonoBehaviour
{
    [Header("Hunger Settings")]
    [SerializeField] private float maxHunger = 100f;
    [SerializeField] private float currentHunger = 100f;
    [SerializeField] private float hungerDrainPerSecond = 0.5f;

    [Header("Starvation")]
    [SerializeField] private bool damageWhenStarving = true;
    [SerializeField] private float starvationDamagePerSecond = 5f;

    public UnityEvent onStarving;

    public event Action<float, float> OnHungerChanged;

    private Health health;

    public float CurrentHunger => currentHunger;
    public float MaxHunger => maxHunger;
    public bool IsStarving => currentHunger <= 0f;

    private void Awake()
    {
        currentHunger = Mathf.Clamp(currentHunger, 0f, maxHunger);
        health = GetComponent<Health>();
    }

    private void Start()
    {
        NotifyHungerChanged();
    }

    private void Update()
    {
        DrainHunger();

        if (IsStarving)
        {
            HandleStarvation();
        }
    }

    private void DrainHunger()
    {
        if (hungerDrainPerSecond <= 0f)
            return;

        ChangeHunger(-hungerDrainPerSecond * Time.deltaTime);
    }

    private void HandleStarvation()
    {
        onStarving?.Invoke();

        if (!damageWhenStarving)
            return;

        if (health != null)
        {
            health.TakeDamage(starvationDamagePerSecond * Time.deltaTime);
        }
    }

    public void RestoreHunger(float amount)
    {
        if (amount <= 0f)
            return;

        ChangeHunger(amount);

        Debug.Log("Restored " + amount + " hunger. Hunger: " + currentHunger + "/" + maxHunger);
    }

    private void ChangeHunger(float amount)
    {
        float oldHunger = currentHunger;

        currentHunger += amount;
        currentHunger = Mathf.Clamp(currentHunger, 0f, maxHunger);

        if (!Mathf.Approximately(oldHunger, currentHunger))
        {
            NotifyHungerChanged();
        }
    }

    public void NotifyHungerChanged()
    {
        OnHungerChanged?.Invoke(currentHunger, maxHunger);
    }
}