using UnityEngine;

public class PlayerHotbar : MonoBehaviour
{
    [Header("Hotbar Settings")]
    [SerializeField] private HotbarSlot[] slots = new HotbarSlot[7];
    [SerializeField] private int selectedSlotIndex = 0;

    [Header("Item Holding")]
    [SerializeField] private Transform itemHolder;

    [Header("Dropping")]
    [SerializeField] private Transform dropPoint;
    [SerializeField] private float dropForwardForce = 2f;
    [SerializeField] private KeyCode dropKey = KeyCode.G;

    [Header("UI")]
    [SerializeField] private AmmoUI ammoUI;
    [SerializeField] private HotbarUI hotbarUI;

    private GameObject currentHandObject;
    private Gun currentGun;

    private void Start()
    {
        if (hotbarUI != null)
        {
            hotbarUI.UpdateHotbar(slots);
        }

        SelectSlot(selectedSlotIndex);
    }

    private void Update()
    {
        HandleNumberKeyInput();
        HandleScrollWheelInput();
        HandleUseInput();
        HandleDropInput();
    }

    private void HandleNumberKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SelectSlot(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SelectSlot(5);
        if (Input.GetKeyDown(KeyCode.Alpha7)) SelectSlot(6);
    }

    private void HandleScrollWheelInput()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (scroll > 0f)
        {
            SelectPreviousSlot();
        }
        else if (scroll < 0f)
        {
            SelectNextSlot();
        }
    }

    private void HandleUseInput()
    {
        if (!Input.GetKeyDown(KeyCode.Mouse0))
            return;

        HotbarSlot selectedSlot = slots[selectedSlotIndex];

        if (selectedSlot == null || selectedSlot.IsEmpty())
            return;

        ItemData itemData = selectedSlot.itemData;

        if (itemData.itemType == ItemType.Weapon)
            return;

        UseSelectedItem();
    }

    private void HandleDropInput()
    {
        if (Input.GetKeyDown(dropKey))
        {
            DropSelectedItem();
        }
    }

    private void SelectPreviousSlot()
    {
        int newIndex = selectedSlotIndex - 1;

        if (newIndex < 0)
        {
            newIndex = slots.Length - 1;
        }

        SelectSlot(newIndex);
    }

    private void SelectNextSlot()
    {
        int newIndex = selectedSlotIndex + 1;

        if (newIndex >= slots.Length)
        {
            newIndex = 0;
        }

        SelectSlot(newIndex);
    }

    private void SelectSlot(int index)
    {
        if (index < 0 || index >= slots.Length)
            return;

        selectedSlotIndex = index;

        if (hotbarUI != null)
        {
            hotbarUI.UpdateSelectedSlot(selectedSlotIndex);
            hotbarUI.UpdateHotbar(slots);
        }

        UnequipCurrentObject();

        HotbarSlot selectedSlot = slots[selectedSlotIndex];

        if (selectedSlot == null || selectedSlot.IsEmpty())
        {
            if (ammoUI != null)
            {
                ammoUI.ClearGun();
            }

            Debug.Log("Selected empty hotbar slot: " + (selectedSlotIndex + 1));
            return;
        }

        EquipSlot(selectedSlot);
    }

    private void EquipSlot(HotbarSlot slot)
    {
        ItemData itemData = slot.itemData;

        if (itemData == null)
            return;

        if (slot.spawnedHandObject == null)
        {
            SpawnSlotItem(slot);
        }

        currentHandObject = slot.spawnedHandObject;

        if (currentHandObject != null)
        {
            currentHandObject.SetActive(true);
        }

        if (itemData.itemType == ItemType.Weapon)
        {
            currentGun = currentHandObject.GetComponentInChildren<Gun>(true);

            if (currentGun != null)
            {
                if (ammoUI != null)
                {
                    ammoUI.SetGun(currentGun);
                }

                currentGun.RefreshAmmoUI();

                Debug.Log("Equipped weapon: " + itemData.itemName);
            }
            else
            {
                if (ammoUI != null)
                {
                    ammoUI.ClearGun();
                }

                Debug.LogWarning(itemData.itemName + " is marked as Weapon, but no Gun component was found.");
            }
        }
        else
        {
            if (ammoUI != null)
            {
                ammoUI.ClearGun();
            }

            Debug.Log("Equipped item: " + itemData.itemName);
        }
    }

    private void SpawnSlotItem(HotbarSlot slot)
    {
        if (slot == null || slot.itemData == null)
            return;

        if (slot.itemData.handPrefab == null)
        {
            Debug.LogWarning(slot.itemData.itemName + " has no hand prefab assigned.");
            return;
        }

        if (itemHolder == null)
        {
            Debug.LogError("PlayerHotbar has no item holder assigned.");
            return;
        }

        GameObject spawnedObject = Instantiate(
            slot.itemData.handPrefab,
            itemHolder.position,
            itemHolder.rotation,
            itemHolder
        );

        spawnedObject.transform.localPosition = Vector3.zero;
        spawnedObject.transform.localRotation = Quaternion.identity;
        spawnedObject.transform.localScale = Vector3.one;

        spawnedObject.SetActive(false);

        slot.spawnedHandObject = spawnedObject;
    }

    private void UseSelectedItem()
    {
        HotbarSlot selectedSlot = slots[selectedSlotIndex];

        if (selectedSlot == null || selectedSlot.IsEmpty())
            return;

        if (selectedSlot.spawnedHandObject == null)
        {
            SpawnSlotItem(selectedSlot);
        }

        if (selectedSlot.spawnedHandObject == null)
            return;

        IUsable usable = selectedSlot.spawnedHandObject.GetComponentInChildren<IUsable>(true);

        if (usable == null)
        {
            Debug.LogWarning(selectedSlot.itemData.itemName + " is selected, but it has no usable script.");
            return;
        }

        bool wasConsumed = usable.Use(gameObject);

        if (wasConsumed)
        {
            RemoveItemFromSlot(selectedSlotIndex, true);
        }
    }

    public bool TryAddItem(ItemData itemData)
    {
        if (itemData == null)
            return false;

        int emptySlotIndex = FindFirstEmptySlot();

        if (emptySlotIndex == -1)
        {
            Debug.Log("Hotbar is full. Could not pick up " + itemData.itemName);
            return false;
        }

        slots[emptySlotIndex].itemData = itemData;
        slots[emptySlotIndex].spawnedHandObject = null;

        if (hotbarUI != null)
        {
            hotbarUI.UpdateHotbar(slots);
        }

        Debug.Log("Picked up " + itemData.itemName + " into slot " + (emptySlotIndex + 1));

        if (emptySlotIndex == selectedSlotIndex)
        {
            SelectSlot(selectedSlotIndex);
        }

        return true;
    }

    private int FindFirstEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                continue;

            if (slots[i].IsEmpty())
            {
                return i;
            }
        }

        return -1;
    }

    public void DropSelectedItem()
    {
        HotbarSlot selectedSlot = slots[selectedSlotIndex];

        if (selectedSlot == null || selectedSlot.IsEmpty())
        {
            Debug.Log("No item selected to drop.");
            return;
        }

        ItemData itemToDrop = selectedSlot.itemData;

        SpawnDroppedPickup(itemToDrop);
        RemoveItemFromSlot(selectedSlotIndex, true);

        Debug.Log("Dropped " + itemToDrop.itemName);
    }

    private void SpawnDroppedPickup(ItemData itemData)
    {
        if (itemData == null)
            return;

        if (itemData.pickupPrefab == null)
        {
            Debug.LogWarning(itemData.itemName + " has no pickup prefab assigned.");
            return;
        }

        Vector3 spawnPosition = transform.position + transform.forward * 1.5f + Vector3.up * 0.5f;
        Quaternion spawnRotation = Quaternion.identity;

        if (dropPoint != null)
        {
            spawnPosition = dropPoint.position;
            spawnRotation = dropPoint.rotation;
        }

        GameObject droppedObject = Instantiate(
            itemData.pickupPrefab,
            spawnPosition,
            spawnRotation
        );

        ItemPickup pickup = droppedObject.GetComponent<ItemPickup>();

        if (pickup != null)
        {
            pickup.SetItemData(itemData);
        }

        Rigidbody rb = droppedObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(transform.forward * dropForwardForce, ForceMode.Impulse);
        }
    }

    private void RemoveItemFromSlot(int slotIndex, bool destroyHandObject)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length)
            return;

        HotbarSlot slot = slots[slotIndex];

        if (slot == null)
            return;

        if (destroyHandObject && slot.spawnedHandObject != null)
        {
            Destroy(slot.spawnedHandObject);
        }

        slot.itemData = null;
        slot.spawnedHandObject = null;

        if (slotIndex == selectedSlotIndex)
        {
            currentHandObject = null;
            currentGun = null;

            if (ammoUI != null)
            {
                ammoUI.ClearGun();
            }
        }

        if (hotbarUI != null)
        {
            hotbarUI.UpdateHotbar(slots);
            hotbarUI.UpdateSelectedSlot(selectedSlotIndex);
        }

        if (slotIndex == selectedSlotIndex)
        {
            SelectSlot(selectedSlotIndex);
        }
    }

    private void UnequipCurrentObject()
    {
        if (currentHandObject != null)
        {
            currentHandObject.SetActive(false);
        }

        currentHandObject = null;
        currentGun = null;
    }
}