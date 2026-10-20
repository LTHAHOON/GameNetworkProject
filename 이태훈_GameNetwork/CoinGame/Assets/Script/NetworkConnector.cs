using Unity.Netcode;
using UnityEngine;

public class NetworkConnector : MonoBehaviour
{
    public void ConnectHost()
    {
        if(NetworkManager.Singleton.IsListening)
        {
            return;
        }
        NetworkManager.Singleton.StartHost();
    }

    public void ConnectClient()
    {
        if(NetworkManager.Singleton.IsListening)
        {
            return;
        }
        NetworkManager.Singleton.StartClient();
    }

    public void DisConnectClient()
    {
        if(!NetworkManager.Singleton.IsListening)
        {
            return;
        }
        NetworkManager.Singleton.Shutdown();
    }
}
