using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Gun gun;
    [SerializeField] private TMP_Text ammoText;

    private void OnEnable()
    {
        if (gun != null)
        {
            gun.OnAmmoChanged += UpdateAmmoText;
        }
    }

    private void OnDisable()
    {
        if (gun != null)
        {
            gun.OnAmmoChanged -= UpdateAmmoText;
        }
    }

    private void Start()
    {
        if (gun != null && ammoText != null)
        {
            UpdateAmmoText(gun.CurrentAmmo, gun.MagazineSize);
        }
    }

    private void UpdateAmmoText(int currentAmmo, int magazineSize)
    {
        ammoText.text = currentAmmo + " / " + magazineSize;
    }
}