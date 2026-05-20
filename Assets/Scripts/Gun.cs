using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Gun References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Projectile projectilePrefab;

    [Header("Gun Stats")]
    [SerializeField] private float damage = 25f;
    [SerializeField] private float projectileSpeed = 80f;
    [SerializeField] private float fireRate = 0.15f;
    [SerializeField] private int magazineSize = 12;
    [SerializeField] private float reloadTime = 1.5f;

    [Header("Input")]
    [SerializeField] private KeyCode shootKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode reloadKey = KeyCode.R;

    private int currentAmmo;
    private float nextFireTime;
    private bool isReloading;

    private void Start()
    {
        currentAmmo = magazineSize;
    }

    private void Update()
    {
        if (isReloading)
            return;

        if (Input.GetKeyDown(reloadKey))
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetKey(shootKey) && Time.time >= nextFireTime)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        nextFireTime = Time.time + fireRate;
        currentAmmo--;

        Vector3 shootDirection = playerCamera.transform.forward;

        Projectile projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(shootDirection)
        );

        projectile.Initialize(damage, projectileSpeed, shootDirection);
    }

    private System.Collections.IEnumerator Reload()
    {
        if (currentAmmo == magazineSize)
            yield break;

        isReloading = true;

        Debug.Log("Reloading...");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;
        isReloading = false;

        Debug.Log("Reloaded!");
    }
}