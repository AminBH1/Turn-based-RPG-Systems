using UnityEngine;

public struct ItemInBasket {

    private InventoryItemSO itemSO;
    private int amount;

    public ItemInBasket(InventoryItemSO itemSO, int amount) {
        this.itemSO = itemSO;
        this.amount = amount;   
    }

    public InventoryItemSO GetItemSO() {
        return itemSO;
    }

    public int GetAmount() {
        return amount;
    }
}