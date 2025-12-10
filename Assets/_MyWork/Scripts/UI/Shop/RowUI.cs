using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RowUI : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private Image displayImage;
    [SerializeField] private TextMeshProUGUI amountAvailable;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI amountInBasketText;
    [SerializeField] private Button addButton;
    [SerializeField] private Button removeButton;
    [SerializeField] private TextMeshProUGUI basketAmountText;


    private Shop shop;
    private ShopItem shopItem;
    private int basketAmount = 1;

    public void Setup(Shop shop, ShopItem shopItem) {
        this.shop = shop;
        this.shopItem = shopItem;
        SetupButtons();
       
        shop.OnBasketAmountchanged += Shop_OnBasketAmountchanged;

        SetVisual();
    }

    private void Shop_OnBasketAmountchanged() {
        basketAmountText.text = shop.GetBasketAmount(shopItem.GetItemSO()).ToString();
    }

    public void SetVisual() {
        itemNameText.text = shopItem.GetItemSO().GetDisplayName();
        displayImage.sprite = shopItem.GetItemSO().GetDisplayIcon();
        priceText.text = shopItem.GetPrice().ToString();
        amountAvailable.text = shopItem.GetAmountAvailable().ToString();
        amountInBasketText.text = shopItem.GetAmountInBasket().ToString();
        basketAmountText.text = shop.GetBasketAmount(shopItem.GetItemSO()).ToString();

    }

    private void SetupButtons() {
        addButton.onClick.AddListener(() => {     
            shop.ChangeBasketAmount(shopItem.GetItemSO(), basketAmount);
        });

        removeButton.onClick.AddListener(() => {
            shop.ChangeBasketAmount(shopItem.GetItemSO(), -basketAmount);
        });
    }

}