using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class NetworkTestManager : MonoBehaviour
{
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }
}
