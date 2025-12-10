using UnityEngine;

public struct ShopItem {

    private InventoryItemSO itemSO;
    private float price;
    private int amountAvailable;
    private int amountInBasket;

    public ShopItem(InventoryItemSO itemSO, float price, int amountAvailable, int amountInBasket) {
        this.itemSO = itemSO;
        this.price = price;
        this.amountAvailable = amountAvailable;
        this.amountInBasket = amountInBasket;
    }

    public InventoryItemSO GetItemSO() {
        return itemSO;
    }

    public float GetPrice() {
        return price;
    }

    public int GetAmountAvailable() {
        return amountAvailable;
    }

    public int GetAmountInBasket() {
        return amountInBasket;
    }
}
