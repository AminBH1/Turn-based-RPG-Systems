using System;
using UnityEngine;

public class Wallet : MonoBehaviour {

    public event Action OnBalanceUpdated;

    [SerializeField] private float initialBalance;

    private float currentBalance;

    private void Awake() {
        currentBalance = initialBalance;
    }

    public void UpdateBalance(float balance) {
        currentBalance += balance;
        OnBalanceUpdated?.Invoke();
    }

    public float GetCurrentBalance() {
        return currentBalance;
    }

}