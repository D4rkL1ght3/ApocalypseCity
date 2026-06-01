using UnityEngine;

public class PlayerHotbar : MonoBehaviour
{
    [Header("Hotbar Settings")]
    [SerializeField] private HotbarSlot[] slots = new HotbarSlot[7];
    [SerializeField] private int selectedSlotIndex = 0;

    [Header("Item Holding")]
    [SerializeField] private Transform itemHolder;

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