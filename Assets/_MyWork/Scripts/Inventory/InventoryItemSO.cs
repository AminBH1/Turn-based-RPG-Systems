using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "InventoryData/New Item")]
public class InventoryItemSO : ScriptableObject {

    [SerializeField] private string ID;
    [SerializeField] private string displayName;
    [SerializeField] private Sprite displayIcon;
    [SerializeField] private string displayDescription;
    [SerializeField] private bool isStackable;
    [SerializeField] private int maxStackAmount;
    [SerializeField] private Transform pickupPrefab;


    public Sprite GetDisplayIcon() {
        return displayIcon;
    }

    public bool IsStackable() {
        return isStackable;
    }

    public int GetMaxStackAmount() {
        return maxStackAmount;
    }

    public Pickup SpawnPickup(Transform parentTransform, int amount) {
        Transform pickupTransform = Instantiate(pickupPrefab, parentTransform);
        if (pickupTransform.TryGetComponent<Pickup>(out Pickup pickup)) {
            pickup.Setup(this, amount);            
        }
        return pickup;
    }

}