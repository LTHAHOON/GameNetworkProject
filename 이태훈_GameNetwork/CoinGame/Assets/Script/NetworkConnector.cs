using Unity.Netcode;
using UnityEngine;

public class NetworkConnector : MonoBehaviour
{
    [SerializeField] 
    private Spawner _defaultPlayerSpawner;

    private void OnConnectedCallBack(ulong clientId)
    {
         NetworkManager networkManager = NetworkManager.Singleton;
        if (networkManager == null || _defaultPlayerSpawner == null)
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
            return;
        }
        networkManager.OnClientConnectedCallback += OnConnectedCallBack;
        networkManager.StartHost();
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