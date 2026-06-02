using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusBarUI : MonoBehaviour
{
    [Header("Bar References")]
    [SerializeField] private Image fillImage;

    public void UpdateBar(float currentValue, float maxValue)
    {
        if (fillImage != null)
        {
            if (maxValue <= 0f)
            {
                fillImage.fillAmount = 0f;
            }
            else
            {
                fillImage.fillAmount = currentValue / maxValue;
            }
        }
    }
}