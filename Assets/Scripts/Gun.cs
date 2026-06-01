using System;
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

    public event Action<int, int> OnAmmoChanged;

    private int currentAmmo;
    private float nextFireTime;
    private bool isReloading;
    private bool hasInitialized;

    public int CurrentAmmo => currentAmmo;
    public int MagazineSize => magazineSize;

    private void Awake()
    {
        InitializeAmmo();

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    private void OnEnable()
    {
        InitializeAmmo();
        RefreshAmmoUI();
    }

    private void Update()
    {
        if (isReloading)
            return;

        if (Input.GetKeyDown(reloadKey) || currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetKey(shootKey) && Time.time >= nextFireTime)
        {
            Shoot();
        }
    }

    private void InitializeAmmo()
    {
        if (hasInitialized)
            return;

        currentAmmo = magazineSize;
        hasInitialized = true;
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

        RefreshAmmoUI();

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

        Debug.Log(gameObject.name + " reloading...");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;
        isReloading = false;

        RefreshAmmoUI();

        Debug.Log(gameObject.name + " reloaded!");
    }

    public void RefreshAmmoUI()
    {
        OnAmmoChanged?.Invoke(currentAmmo, magazineSize);
    }
}