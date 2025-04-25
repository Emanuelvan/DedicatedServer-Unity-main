using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    [SerializeField] private ApiClient api;
    [SerializeField] private List<PlayerController> players;
    [SerializeField] private float pollInterval = 0.1f; // Polling interval in seconds
    private Dictionary<int, Coroutine> pollingCoroutines = new Dictionary<int, Coroutine>();
    private Dictionary<int, bool> connectedPlayers = new Dictionary<int, bool>();
    public string gameId;

    public void Start()
    {
        api.OnDataReceived += OnDataReceived;
        // Start polling for each player's position
        foreach (PlayerController player in players)
        {
            StartPollingForPlayer(player.playerId);
        }
    }

    // Connects a player to the game and starts position synchronization
    public void ConnectPlayer(int playerId)
    {
        if (!connectedPlayers.ContainsKey(playerId) || !connectedPlayers[playerId])
        {
            connectedPlayers[playerId] = true;
            StartPollingForPlayer(playerId);
            players[playerId].UpdateVisualState(true);
            Debug.Log($"Player {playerId} connected");
        }
    }

    // Disconnects a player and stops position synchronization
    public void DisconnectPlayer(int playerId)
    {
        if (connectedPlayers.ContainsKey(playerId) && connectedPlayers[playerId])
        {
            connectedPlayers[playerId] = false;
            StopPollingForPlayer(playerId);
            players[playerId].UpdateVisualState(false);
            Debug.Log($"Player {playerId} disconnected");
        }
    }

    // Checks if a player is currently connected
    public bool IsPlayerConnected(int playerId)
    {
        return connectedPlayers.ContainsKey(playerId) && connectedPlayers[playerId];
    }

    // Requests player position data from the server
    public void GetPlayerData(int playerId)
    {
        StartCoroutine(api.GetPlayerData(gameId, playerId.ToString()));
    }

    // Callback executed when receiving player position data from server
    public void OnDataReceived(int playerId, ServerData data)
    {
        Vector3 position = new Vector3(data.posX, data.posY, data.posZ);
        players[playerId].MovePlayer(position);
    }

    // Sends the current player position to the server
    public void SendPlayerPosition(int playerId)
    {
        Vector3 position = players[playerId].GetPosition();
        ServerData data = new ServerData
        {
            posX = position.x,
            posY = position.y,
            posZ = position.z
        };
        StartCoroutine(api.PostPlayerData(gameId, playerId.ToString(), data));
    }

    // Starts continuous polling for a specific player's position
    public void StartPollingForPlayer(int playerId)
    {
        if (!pollingCoroutines.ContainsKey(playerId))
        {
            pollingCoroutines[playerId] = StartCoroutine(PollPlayerData(playerId));
        }
    }

    // Stops polling for a specific player's position
    public void StopPollingForPlayer(int playerId)
    {
        if (pollingCoroutines.ContainsKey(playerId))
        {
            StopCoroutine(pollingCoroutines[playerId]);
            pollingCoroutines.Remove(playerId);
        }
    }

    // Coroutine that continuously polls for player position updates
    private IEnumerator PollPlayerData(int playerId)
    {
        while (true)
        {
            GetPlayerData(playerId);
            yield return new WaitForSeconds(pollInterval);
        }
    }

    private void OnDestroy()
    {
        // Stop all polling coroutines when the object is destroyed
        foreach (var coroutine in pollingCoroutines.Values)
        {
            StopCoroutine(coroutine);
        }
        pollingCoroutines.Clear();
    }
}
