using UnityEngine;
using UnityEngine.UI;

public class HotbarUI : MonoBehaviour
{
    [Header("Hotbar UI")]
    [SerializeField] private Image[] slotImages;
    [SerializeField] private RectTransform selectedSlotHighlight;

    public void UpdateSelectedSlot(int selectedIndex)
    {
        if (slotImages == null || slotImages.Length == 0)
            return;

        if (selectedSlotHighlight == null)
            return;

        if (selectedIndex < 0 || selectedIndex >= slotImages.Length)
            return;

        selectedSlotHighlight.gameObject.SetActive(true);
        selectedSlotHighlight.position = slotImages[selectedIndex].rectTransform.position;
    }
}