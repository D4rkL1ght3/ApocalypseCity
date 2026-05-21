using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text ammoText;

    private Gun currentGun;

    public void SetGun(Gun newGun)
    {
        if (currentGun != null)
        {
            currentGun.OnAmmoChanged -= UpdateAmmoText;
        }

        currentGun = newGun;

        if (currentGun == null)
        {
            Hide();
            return;
        }

        currentGun.OnAmmoChanged += UpdateAmmoText;

        Show();
        UpdateAmmoText(currentGun.CurrentAmmo, currentGun.MagazineSize);
    }

    public void ClearGun()
    {
        if (currentGun != null)
        {
            currentGun.OnAmmoChanged -= UpdateAmmoText;
            currentGun = null;
        }

        Hide();
    }

    private void UpdateAmmoText(int currentAmmo, int magazineSize)
    {
        if (ammoText == null)
            return;

        ammoText.text = currentAmmo + " / " + magazineSize;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (currentGun != null)
        {
            currentGun.OnAmmoChanged -= UpdateAmmoText;
        }
    }
}