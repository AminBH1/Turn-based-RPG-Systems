using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Shop : MonoBehaviour {

    public event Action OnShoppingStarted;
    public event Action OnShopUpdated;
    public event Action OnBasketAmountchanged;

    [SerializeField] private string shopName;
    [SerializeField][Range(0, 100)] private float sellPercentage;
    [SerializeField] private int basketAmountMultiplier;
    [SerializeField] private StockConfig[] stockConfigArray; 

    private Dictionary<InventoryItemSO, int> basketAmountLookup = new Dictionary<InventoryItemSO, int>();
    private Dictionary<InventoryItemSO, int> stockAmountLookup = new Dictionary<InventoryItemSO, int>();

    private ItemCategory selectedCategory = ItemCategory.None;
    private bool isBuyingMode = true;
    private Player playerShopper;

    [Serializable]
    private class StockConfig {
        public InventoryItemSO itemSO;
        [Range(0, 100)] public float buyDiscount;
        public int initialStockAmount;
        public int levelToUnlock;
    }

    private void Awake() {
        foreach (StockConfig stockConfig in stockConfigArray) {
            InventoryItemSO itemSO = stockConfig.itemSO;
            basketAmountLookup[itemSO] = 0;
            stockAmountLookup[itemSO] = stockConfig.initialStockAmount;
        }
    }

    public void OpenShop(Player playerShopper) {
        this.playerShopper = playerShopper;
        OnShoppingStarted?.Invoke();
    }


    public void ChangeBasketAmount(InventoryItemSO itemSO, int basketAmount) {
        if (!basketAmountLookup.ContainsKey(itemSO)) {
            basketAmountLookup[itemSO] = 0;
        }

        basketAmountLookup[itemSO] += basketAmountMultiplier * basketAmount;


        if (basketAmountLookup[itemSO] < 0) {
            basketAmountLookup[itemSO] = 0;
        }

        OnBasketAmountchanged?.Invoke();
    }

    public void SelectCatageory(ItemCategory itemCategory) {
        selectedCategory = itemCategory;
        ResetBasketAmount();
        OnShopUpdated?.Invoke();
    }

    public void SwitchMode() {
        isBuyingMode = !isBuyingMode;
        OnShopUpdated?.Invoke();
    }

    public void ResetBasketAmount() {
        foreach (ShopItem shopItem in GetShopItemByCategory()) {
            basketAmountLookup[shopItem.GetItemSO()] = 0;
        }
    }



    public bool CanConfirm() {
        if (!HasAmountInBasket()) {
            return false;
        }

        if (!HasAvailableAmount()) {
            return false;
        }

        if (isBuyingMode) {
            if (!HasSufficiantFunds()) {
                return false;
            }

            if (!ShopperHasSufficiantSpace()) {
                return false;
            }
        }

        return true;
    }

    public void ConfirmTransaction() {
        playerShopper.transform.TryGetComponent<Inventory>(out Inventory inventory);
        playerShopper.transform.TryGetComponent<Wallet>(out Wallet wallet);

        if (isBuyingMode) {

            foreach (ShopItem shopItem in GetShopItemByCategory()) {
                InventoryItemSO itemSO = shopItem.GetItemSO();
                int amount = shopItem.GetAmountInBasket();
                if (amount == 0) {

                    print(itemSO);
                    continue;
                }
                float price = shopItem.GetPrice();



                inventory.AddItemAmount(itemSO, amount);
                ChangeBasketAmount(itemSO, -amount);
                stockAmountLookup[itemSO] -= amount;
                wallet.UpdateBalance(-price);
                OnShopUpdated?.Invoke();
            }
            return;
        }

        if (!isBuyingMode) {

            foreach (ShopItem shopItem in GetShopItemByCategory()) {
                InventoryItemSO itemSO = shopItem.GetItemSO();
                int amount = shopItem.GetAmountInBasket();
                if (amount == 0) {
                    continue;
                }
                float price = shopItem.GetPrice();

                inventory.RemoveItemAmount(itemSO, amount);
                ChangeBasketAmount(itemSO, -amount);
                stockAmountLookup[itemSO] += amount;
                wallet.UpdateBalance(+price);
                OnShopUpdated?.Invoke();
            }
        }
    }
    private bool HasAmountInBasket() {
        foreach (InventoryItemSO basketItem in basketAmountLookup.Keys) {
            int amount = basketAmountLookup[basketItem];
            if (amount > 0) {
                return true;
            }           
        }
        return false;
    }

    private bool HasAvailableAmount() {
        foreach (ShopItem shopItem in GetShopItemByCategory()) {
            if (shopItem.GetAmountInBasket() > shopItem.GetAmountAvailable()) {
                return false;
            }
        }
        return true;
    }



    public bool ShopperHasSufficiantSpace() {
        playerShopper.transform.TryGetComponent<Inventory>(out Inventory inventory);
        List<ItemInBasket> itemInBasketList = new List<ItemInBasket>();

        foreach (InventoryItemSO basketItem in basketAmountLookup.Keys) {
            int amount = basketAmountLookup[basketItem];
            if (amount <= 0) {
                continue;
            }
            ItemInBasket itemInBasket = new ItemInBasket(basketItem, amount);
            itemInBasketList.Add(itemInBasket);
        }

        if (!inventory.HasSufficiantSpaceFor(itemInBasketList)) {
            return false;
        }

        return true;
    }

    public bool HasSufficiantFunds() {
        if (!isBuyingMode) {
            return true;
        }
        playerShopper.transform.TryGetComponent<Wallet>(out Wallet wallet);
        return wallet.GetCurrentBalance() >= GetTotalCostInBasket();
    }


    public float GetTotalCostInBasket() {
        float totalCost = 0;
        foreach (ShopItem shopItem in GetShopItemByCategory()) {
            float shopItemPrice = shopItem.GetPrice() * shopItem.GetAmountInBasket();
            totalCost += shopItemPrice;
        }
        return totalCost;
    }


    public IEnumerable<ShopItem> GetShopItemByCategory() {
        foreach (ShopItem shopItem in GetAllShopItems()) {
            if (selectedCategory == ItemCategory.None || selectedCategory == shopItem.GetItemSO().GetCategory()) {
                yield return shopItem;
            }
        }

    }

    public IEnumerable<ShopItem> GetAllShopItems() {
        foreach (StockConfig stockConfig in stockConfigArray) {
            InventoryItemSO itemSO = stockConfig.itemSO;
            float price = GetPriceByMode(stockConfig);
            int amountAvailable = GetAmountAvailableByMode(stockConfig);
            if (amountAvailable <= 0) {
                continue;
            }
            int basketAmount = basketAmountLookup[itemSO];

            yield return new ShopItem(itemSO, price, amountAvailable, basketAmount);
        }
    }

    private float GetPriceByMode(StockConfig stockConfig) {
        InventoryItemSO itemSO = stockConfig.itemSO;
        if (isBuyingMode) {
            return itemSO.GetPrice() * (1 - stockConfig.buyDiscount / 100);
        }
        // sellMode
        return itemSO.GetPrice() * (sellPercentage / 100);
    }


    private int GetAmountAvailableByMode(StockConfig stockConfig) {
        if (isBuyingMode) {
            return stockAmountLookup[stockConfig.itemSO];
        }
        // sellMode        
        playerShopper.transform.TryGetComponent<Inventory>(out Inventory inventory);      
        return inventory.CountItemAmount(stockConfig.itemSO);
    }

    public int GetBasketAmount(InventoryItemSO itemSO) {
        return basketAmountLookup[itemSO];
    }
}