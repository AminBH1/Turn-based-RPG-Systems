using UnityEngine;

public class ShopStarter : MonoBehaviour {

    [SerializeField] private Player playerShopper;
    [SerializeField] private Shop shop;

    public void StartShopping() {
        shop.OpenShop(playerShopper);
    }

}