using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "InventoryData/New Item")]
public class InventoryItemSO : ScriptableObject {

    [SerializeField] private string ID;
    [SerializeField] private string displayName;
    [SerializeField] private Sprite displayIcon;
    [SerializeField] private string displayDescription;
    [SerializeField] private float price;
    [SerializeField] private bool isStackable;
    [SerializeField] private int maxStackAmount;
    [SerializeField] private Transform pickupPrefab;
    [SerializeField] private ItemCategory itemCategory;


    public float GetPrice() {
        return price;
    }

    public string GetDisplayName() {
        return displayName;
    }

    public Sprite GetDisplayIcon() {
        return displayIcon;
    }

    public int GetMaxStackAmount() {
        return maxStackAmount;
    }

    public bool IsStackable() {
        return isStackable;
    }

    public ItemCategory GetCategory() {
        return itemCategory;
    }

    public Pickup SpawnPickup(Transform parentTransform, int amount) {
        Transform pickupTransform = Instantiate(pickupPrefab, parentTransform);
        if (pickupTransform.TryGetComponent<Pickup>(out Pickup pickup)) {
            pickup.Setup(this, amount);            
        }
        return pickup;
    }

}