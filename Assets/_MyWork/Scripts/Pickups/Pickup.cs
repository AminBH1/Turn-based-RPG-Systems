using System;
using UnityEngine;

public class Pickup : MonoBehaviour, IInteractable {   

    private InventoryItemSO inventoryItemSO;
    private int amount;

    private Inventory inventory;

    public void Interact(Player player) {
        inventory = player.transform.GetComponent<Inventory>();

       // inventory.AddItemAmount(inventoryItemSO, amount);
    }

    public void Setup(InventoryItemSO inventoryItemSO, int amount) {
        this.amount = amount;   
        this.inventoryItemSO = inventoryItemSO;
    }

}