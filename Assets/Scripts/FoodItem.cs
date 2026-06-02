using UnityEngine;

public class FoodItem : MonoBehaviour, IUsable
{
    [Header("Food Settings")]
    [SerializeField] private float hungerRestoreAmount = 25f;
    [SerializeField] private bool consumeOnUse = true;

    public bool Use(GameObject user)
    {
        Hunger hunger = user.GetComponent<Hunger>();

        if (hunger == null)
        {
            Debug.LogWarning(user.name + " tried to eat food, but has no Hunger component.");
            return false;
        }

        hunger.RestoreHunger(hungerRestoreAmount);

        Debug.Log(user.name + " ate " + gameObject.name + ".");

        return consumeOnUse;
    }
}