using UnityEngine;
using UnityEngine.UI;

public class ConnectionUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private int playerId;
    [SerializeField] private Button connectButton;
    [SerializeField] private Button disconnectButton;

    void Start()
    {
        if (connectButton != null)
            connectButton.onClick.AddListener(Connect);

        if (disconnectButton != null)
            disconnectButton.onClick.AddListener(Disconnect);

        UpdateButtonStates();
    }

    public void Connect()
    {
        gameManager.ConnectPlayer(playerId);
        UpdateButtonStates();
    }

    public void Disconnect()
    {
        gameManager.DisconnectPlayer(playerId);
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        bool isConnected = gameManager.IsPlayerConnected(playerId);
        if (connectButton != null)
            connectButton.interactable = !isConnected;
        if (disconnectButton != null)
            disconnectButton.interactable = isConnected;
    }
}