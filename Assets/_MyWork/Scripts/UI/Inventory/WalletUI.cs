using TMPro;
using UnityEngine;

public class WalletUI : MonoBehaviour {

    [SerializeField] private Wallet wallet;
    [SerializeField] private TextMeshProUGUI balanceText;

    private void Start() {
        wallet.OnBalanceUpdated += Wallet_OnBalanceUpdated;

        Wallet_OnBalanceUpdated();
    }

    private void Wallet_OnBalanceUpdated() {
        balanceText.text = wallet.GetCurrentBalance().ToString();
    }
}