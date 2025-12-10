using System;
using UnityEngine;
using System.Collections.Generic;
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

    public void RemoveItemAmount(InventoryItemSO itemSO, int amountToRemove) {
        foreach (InventorySlot slot in SlotArray) {
            if (slot.itemSO == itemSO) {
                if (slot.amount >= amountToRemove) {
                    slot.amount -= amountToRemove;
                    if (slot.amount <= 0) {
                        slot.amount = 0;
                        slot.itemSO = null; 
                    }
                    OnInventoryUpdated?.Invoke();
                    return;
                }
                amountToRemove -= slot.amount;
                slot.amount = 0;
                slot.itemSO = null;
            }
        }
        OnInventoryUpdated?.Invoke();
    }


    public void AddItemAmount(InventoryItemSO itemSO, int amountToAdd) {
        print("called");
        int stackSlot = GetStackSlot(itemSO);
        if (stackSlot != -1) {
            print(itemSO);
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

    private void AddItemAmountToFreeSlot(InventoryItemSO itemSO, int amountToAdd) {
        int freeSlot = GetFreeSlot();
        if (freeSlot == -1) {
            return;
        }

        int maxAmountInSlot = 1;
        if (itemSO.IsStackable()) {
            maxAmountInSlot = itemSO.GetMaxStackAmount();
        }

        int toTransferAmount = Mathf.Min(maxAmountInSlot, amountToAdd);
        int restAmount = amountToAdd - toTransferAmount;

        SlotArray[freeSlot].itemSO = itemSO;
        SlotArray[freeSlot].amount = toTransferAmount;

        OnInventoryUpdated?.Invoke();

        if (restAmount > 0) {
            AddItemAmount(itemSO, restAmount);
        }
    }

    public bool HasSufficiantSpaceFor(List<ItemInBasket> itemInBasketList ) {
        int emptySlotsAmount = GetEmptySlotsAmount();
        foreach (ItemInBasket itemInBasket in itemInBasketList) {
            InventoryItemSO itemSO = itemInBasket.GetItemSO();
            int amount = itemInBasket.GetAmount();

            if (itemSO.IsStackable()) {
                int emptyStackSpaceAmount = GetEmptyStackSpaceAmount(itemSO);
                int slotsNeeded = 0;

                if (emptyStackSpaceAmount >= amount) {
                    emptyStackSpaceAmount -= amount;
                    continue;
                }

                if (amount > emptyStackSpaceAmount) {
                    amount -= emptyStackSpaceAmount;

                    slotsNeeded = (amount + itemSO.GetMaxStackAmount() - 1) / itemSO.GetMaxStackAmount();  // roundup the rest as an additional slot needed
                    emptySlotsAmount -= slotsNeeded;

                    if (emptySlotsAmount < 0) {
                        return false;
                    }
                    continue;
                }
            }

            emptySlotsAmount -= amount;
            if (emptySlotsAmount < 0) {
                return false;
            }
        }
        return true;
    }

    public int GetEmptySlotsAmount() {
        int emptySlotAmount = 0;
        foreach (InventorySlot slot in SlotArray) {
            if (slot.amount == 0) {
                emptySlotAmount++;
            }
        }
        return emptySlotAmount;
    }

    public int GetEmptyStackSpaceAmount(InventoryItemSO itemSO) {
        int emptyAmount = 0;
        foreach (InventorySlot slot in SlotArray) {
            if (slot.itemSO == itemSO) {
                int existingAmount = slot.amount;
                int maxStackAmount = itemSO.GetMaxStackAmount();
                emptyAmount += (maxStackAmount - existingAmount);
            }
        }      
        return emptyAmount;
    }

    public int CountItemAmount(InventoryItemSO itemSO) {
        int count = 0;
        foreach (InventorySlot slot in SlotArray) {
            if (slot.itemSO == itemSO) {
                count += slot.amount;
            }
        }
        return count;
    }

    public int GetAmountInSlot(int slotIndex) {
        return SlotArray[slotIndex].amount;
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

}