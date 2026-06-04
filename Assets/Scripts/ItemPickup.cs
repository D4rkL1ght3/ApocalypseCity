using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Pickup Data")]
    [SerializeField] private ItemData itemData;

    public ItemData ItemData => itemData;

    public void SetItemData(ItemData newItemData)
    {
        itemData = newItemData;
    }

    public void PickUp()
    {
        Destroy(gameObject);
    }
}