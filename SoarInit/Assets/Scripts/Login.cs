using Solana.Unity.SDK;
using Solana.Unity.Wallet;
using TMPro;
using UnityEngine;

public class Login : MonoBehaviour
{
    public TextMeshProUGUI publicKey;
    public TextMeshProUGUI solanaBalance;
    public GameObject login;
    public GameObject app;

    private void OnEnable()
    {
        Web3.OnBalanceChange += OnBalanceChange;
        Web3.OnLogin += OnLogin;
    }


    private void OnDisable()
    {
        Web3.OnBalanceChange -= OnBalanceChange;
        Web3.OnLogin -= OnLogin;
    }

    private void OnLogin(Account account)
    {
        publicKey.text = $"Public Key : {account.PublicKey}";
        login.SetActive(false);
        app.SetActive(true);
    }

    private void OnBalanceChange(double solBalance)
    {
        solanaBalance.text = $"Sol Balance : {solBalance}";
    }

    public void OnClusterChange(int val)
    { 
        Web3.Instance.rpcCluster = val == 0 ? RpcCluster.DevNet : val == 1 ? RpcCluster.MainNet : RpcCluster.TestNet;
    }

    public void WalletConnect()
    {
        Web3.Instance.LoginWithWalletAdapter();
    }
}
