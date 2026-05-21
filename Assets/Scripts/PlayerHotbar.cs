using UnityEngine;

public class PlayerHotbar : MonoBehaviour
{
    [Header("Hotbar Settings")]
    [SerializeField] private HotbarSlot[] slots = new HotbarSlot[7];
    [SerializeField] private int selectedSlotIndex = 0;

    [Header("UI")]
    [SerializeField] private AmmoUI ammoUI;

    private GameObject currentSlotObject;
    private Gun currentGun;

    private void Start()
    {
        DisableAllSlotObjects();
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

        if (currentSlotObject != null)
        {
            currentSlotObject.SetActive(false);
        }

        currentSlotObject = null;
        currentGun = null;

        HotbarSlot selectedSlot = slots[selectedSlotIndex];

        if (selectedSlot == null || selectedSlot.itemType == HotbarItemType.Empty || selectedSlot.slotObject == null)
        {
            if (ammoUI != null)
            {
                ammoUI.ClearGun();
            }

            Debug.Log("Selected empty hotbar slot: " + (selectedSlotIndex + 1));
            return;
        }

        currentSlotObject = selectedSlot.slotObject;
        currentSlotObject.SetActive(true);

        if (selectedSlot.itemType == HotbarItemType.Weapon)
        {
            currentGun = currentSlotObject.GetComponentInChildren<Gun>(true);

            if (currentGun != null)
            {
                if (ammoUI != null)
                {
                    ammoUI.SetGun(currentGun);
                }

                currentGun.RefreshAmmoUI();

                Debug.Log("Selected weapon slot " + (selectedSlotIndex + 1) + ": " + selectedSlot.slotName);
            }
            else
            {
                if (ammoUI != null)
                {
                    ammoUI.ClearGun();
                }

                Debug.LogWarning("Slot is marked as Weapon, but no Gun component was found.");
            }
        }
        else if (selectedSlot.itemType == HotbarItemType.Item)
        {
            if (ammoUI != null)
            {
                ammoUI.ClearGun();
            }

            Debug.Log("Selected item slot " + (selectedSlotIndex + 1) + ": " + selectedSlot.slotName);
        }
    }

    private void DisableAllSlotObjects()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].slotObject != null)
            {
                slots[i].slotObject.SetActive(false);
            }
        }
    }
}