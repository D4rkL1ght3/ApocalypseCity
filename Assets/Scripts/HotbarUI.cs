using UnityEngine;
using UnityEngine.UI;

public class HotbarUI : MonoBehaviour
{
    [Header("Slot UI")]
    [SerializeField] private Image[] slotIconImages;
    [SerializeField] private RectTransform selectedSlotHighlight;

    public void UpdateHotbar(HotbarSlot[] slots)
    {
        for (int i = 0; i < slotIconImages.Length; i++)
        {
            if (i >= slots.Length || slots[i] == null || slots[i].IsEmpty())
            {
                slotIconImages[i].sprite = null;
                slotIconImages[i].enabled = false;
                continue;
            }

            slotIconImages[i].sprite = slots[i].itemData.itemIcon;
            slotIconImages[i].enabled = slots[i].itemData.itemIcon != null;
        }
    }

    public void UpdateSelectedSlot(int selectedIndex)
    {
        if (selectedSlotHighlight == null)
            return;

        if (selectedIndex < 0 || selectedIndex >= slotIconImages.Length)
            return;

        selectedSlotHighlight.gameObject.SetActive(true);
        selectedSlotHighlight.position = slotIconImages[selectedIndex].rectTransform.position;
    }
}