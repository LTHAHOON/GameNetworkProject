using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkConnector : MonoBehaviour
{
    [SerializeField] 
    private Spawner _defaultPlayerSpawner;

    private void OnConnectedCallBack(ulong clientId)
    {
         NetworkManager networkManager = NetworkManager.Singleton;
        if (networkManager == null || !networkManager.IsServer || _defaultPlayerSpawner == null)
        {
            return;
        }

        if (!networkManager.ConnectedClients.ContainsKey(clientId))
        {
            return;
        }

        _defaultPlayerSpawner.SpawnPlayer(clientId);
    }

    public void ConnectHost()
    {
        NetworkManager networkManager = NetworkManager.Singleton;
        if (networkManager == null || networkManager.IsListening || networkManager.IsClient)
        {
            Debug.Log("이미 서버가 실행중이거나 NetworkManager가 NULL일 수있습니다.");
            return;
        }
        
        networkManager.OnClientConnectedCallback += OnConnectedCallBack;
        bool started = networkManager.StartHost();
        if (!started)
        {
            networkManager.OnClientConnectedCallback -= OnConnectedCallBack;
            Debug.Log("이미 서버가 실행중입니다.");
        }
    }

    public void ConnectClient()
    {
        NetworkManager networkManager = NetworkManager.Singleton;
        if (networkManager == null || networkManager.IsListening)
        {
            return;
        }
        networkManager.OnClientConnectedCallback += OnConnectedCallBack;
        networkManager.StartClient();
    }

    public void DisConnectClient()
    {
        NetworkManager networkManager = NetworkManager.Singleton;
        if (networkManager == null || !networkManager.IsListening || networkManager.IsServer)
        {
            return;
        }
        networkManager.OnClientConnectedCallback -= OnConnectedCallBack;
        networkManager.Shutdown();
    }
}
