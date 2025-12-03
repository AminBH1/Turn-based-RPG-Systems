using System;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public event Action OnInventoryUpdated;

    [SerializeField] private int inventorySize = 4;

    public class InventorySlot {
        public InventoryItemSO itemSO;
        public int amount;
    }

    private InventorySlot[] SlotArray;

    private void Awake() {
        SlotArray = new InventorySlot[inventorySize];
        for (int i = 0; i < inventorySize; i++) {
            SlotArray[i] = new InventorySlot();
        }
    }

    public int GetAmountInSlot(int slotIndex) {
        return SlotArray[slotIndex].amount;
    }


    public void AddItemAmount(InventoryItemSO itemSO, int amountToAdd) {
        int stackSlot = GetStackSlot(itemSO);       
        if (stackSlot != -1) {
            int existingAmount = SlotArray[stackSlot].amount;
            int maxStackAmount = itemSO.GetMaxStackAmount();
            int toTransferAmount = Mathf.Min(maxStackAmount - existingAmount, amountToAdd);
            int restAmount = amountToAdd - toTransferAmount;

            SlotArray[stackSlot].amount += toTransferAmount;

            OnInventoryUpdated?.Invoke();

            if (restAmount > 0) {
                AddItemAmount(itemSO, restAmount);
            }
            return;
        }
        AddItemAmountToFreeSlot(itemSO, amountToAdd);
    }

    private int GetStackSlot(InventoryItemSO itemToCheck) {
        if (!itemToCheck.IsStackable()) {
            return -1;
        }

        for (int i = 0; i < inventorySize; i++) {
            if (SlotArray[i].itemSO == itemToCheck) {
                if (SlotArray[i].amount < itemToCheck.GetMaxStackAmount()) {
                    return i;
                }
            }
        }
        return -1;
    }

    private void AddItemAmountToFreeSlot(InventoryItemSO itemSO, int amountToAdd) {
        int freeSlot = GetFreeSlot();
        if (freeSlot == -1) {
            Debug.Log("no free slot");
            return;
        } 

        int maxStackAmount = itemSO.GetMaxStackAmount();
        int toTransferAmount = Mathf.Min(maxStackAmount, amountToAdd);
        int restAmount = amountToAdd - toTransferAmount;

        SlotArray[freeSlot].itemSO = itemSO;
        SlotArray[freeSlot].amount = toTransferAmount;

        OnInventoryUpdated?.Invoke();

        if (restAmount > 0) {
            AddItemAmount(itemSO, restAmount);
        }
    }

    private int GetFreeSlot() {
        for (int i = 0; i < inventorySize; i++) {
            if (SlotArray[i].amount == 0) {
                return i;
            }
        }
        return -1;
    }

    public int GetInventorySize() {
        return inventorySize;
    }

    public InventoryItemSO GetItemInSlot(int slotIndex) {
        return SlotArray[slotIndex].itemSO;
    }

    public void SetAmountInSlot(int amount, int slotIndex) {
        SlotArray[slotIndex].amount = amount;
        if (SlotArray[slotIndex].amount == 0) {
            SlotArray[slotIndex].itemSO = null;
        }
        OnInventoryUpdated?.Invoke();
    }

    public void SetItemInSlot(InventoryItemSO item, int slotIndex) {
        SlotArray[slotIndex].itemSO = item;
        OnInventoryUpdated?.Invoke();
    }
}