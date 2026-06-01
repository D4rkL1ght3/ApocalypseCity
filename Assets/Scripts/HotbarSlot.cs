using UnityEngine;

[System.Serializable]
public class HotbarSlot
{
    public ItemData itemData;

    [HideInInspector] public GameObject spawnedHandObject;

    public bool IsEmpty()
    {
        return itemData == null;
    }
}