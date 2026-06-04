using UnityEngine;

public class PlayerItemInteract : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerHotbar playerHotbar;

    [Header("Interaction Settings")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private LayerMask pickupLayerMask = ~0;

    private void Awake()
    {
        if (playerHotbar == null)
        {
            playerHotbar = GetComponent<PlayerHotbar>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            TryPickUpItem();
        }
    }

    private void TryPickUpItem()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("PlayerItemInteractor has no player camera assigned.");
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (!Physics.Raycast(ray, out RaycastHit hit, interactDistance, pickupLayerMask))
            return;

        ItemPickup pickup = hit.collider.GetComponentInParent<ItemPickup>();

        if (pickup == null)
            return;

        if (pickup.ItemData == null)
        {
            Debug.LogWarning("Pickup has no ItemData assigned.");
            return;
        }

        bool pickedUp = playerHotbar.TryAddItem(pickup.ItemData);

        if (pickedUp)
        {
            pickup.PickUp();
        }
    }
}