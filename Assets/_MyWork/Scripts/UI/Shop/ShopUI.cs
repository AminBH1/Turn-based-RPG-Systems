using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour {

    [SerializeField] private Shop shop;
    [SerializeField] private TextMeshProUGUI shopNameText;
    [SerializeField] private Transform rowRootUI;
    [SerializeField] private Transform rowPrefabUI;
    [SerializeField] private TextMeshProUGUI totalBasketCostText;
    [SerializeField] private Button switchButton;
    [SerializeField] private TextMeshProUGUI switchText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TextMeshProUGUI confirmText;
    [SerializeField] private Button allButton;
    [SerializeField] private Button weaponsButton;
    [SerializeField] private Button armourButton;
    [SerializeField] private Button otherButton;
    [SerializeField] private Button exitButton;

    private void Start() {
        gameObject.SetActive(false);

        SetupButtons();

        shop.OnShoppingStarted += Shop_OnShoppingStarted;
        shop.OnShopUpdated += Shop_OnShopUpdated;
        shop.OnBasketAmountchanged += Shop_OnBasketAmountchanged;
    }

    private void Shop_OnBasketAmountchanged() {
        totalBasketCostText.text = shop.GetTotalCostInBasket().ToString();
        totalBasketCostText.color = Color.white;

        if (!shop.HasSufficiantFunds()) {
            totalBasketCostText.color = Color.red;
        }

        confirmButton.interactable = shop.CanConfirm();
    }

    private void Shop_OnShoppingStarted() {
        gameObject.SetActive(true);
        Shop_OnBasketAmountchanged();
        CreateRowUI();
    }

    private void Shop_OnShopUpdated() {
        CreateRowUI();
    }

    private void CreateRowUI() {
        foreach (Transform child in rowRootUI) {
            Destroy(child.gameObject);
        }

        foreach (ShopItem shopItem in shop.GetShopItemByCategory()) {
            Transform rowTransform = Instantiate(rowPrefabUI, rowRootUI);
            rowTransform.TryGetComponent<RowUI>(out RowUI rowUI);
            rowUI.Setup(shop, shopItem);
        }
    }

    private void SetupButtons() {
        exitButton.onClick.AddListener(() => {
            gameObject.SetActive(false);
        });

        allButton.onClick.AddListener(() => {
            shop.SelectCatageory(ItemCategory.None);
        });

        weaponsButton.onClick.AddListener(() => {
            shop.SelectCatageory(ItemCategory.Weapons);
        });

        armourButton.onClick.AddListener(() => {
            shop.SelectCatageory(ItemCategory.Armour);
        });

        otherButton.onClick.AddListener(() => {
            shop.SelectCatageory(ItemCategory.Other);
        });

        confirmButton.onClick.AddListener(() => {
            shop.ConfirmTransaction();
        });

        switchButton.onClick.AddListener(() => {
            shop.SwitchMode();
        });
    }
}

