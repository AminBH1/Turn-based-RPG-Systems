using System;
using UnityEngine;

public class SlotUI : MonoBehaviour, IDragContainer<InventoryItemSO> {

    [SerializeField] private ItemUI itemUI;

    private int slotIndex;
    private Inventory Inventory;

    public int GetAmount() {
        return Inventory.GetAmountInSlot(slotIndex);
    }

    public InventoryItemSO GetItem() {
        return Inventory.GetItemInSlot(slotIndex);
    }

    public int GetMaxStackAmount() {
        return GetItem().GetMaxStackAmount();
    }

    public void SetAmount(int amount) {
        Inventory.SetAmountInSlot(amount, slotIndex);
    }

    public void SetItem(InventoryItemSO item) {
        Inventory.SetItemInSlot(item, slotIndex);
    }

    public void Setup(int i, Inventory inventory) {
        this.slotIndex = i;
        this.Inventory = inventory;     
        itemUI.Setup(inventory.GetItemInSlot(slotIndex), inventory, slotIndex);
    }

}