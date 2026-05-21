using UnityEngine;

public enum HotbarItemType
{
    Empty,
    Weapon,
    Item
}

[System.Serializable]
public class HotbarSlot
{
    public string slotName;
    public HotbarItemType itemType;
    public GameObject slotObject;
}