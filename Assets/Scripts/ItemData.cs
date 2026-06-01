using UnityEngine;

public enum ItemType
{
    Weapon,
    Item
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Item Info")]
    public string itemName;
    public ItemType itemType;

    [Header("UI")]
    public Sprite itemIcon;

    [Header("Equipped Prefab")]
    public GameObject handPrefab;
}