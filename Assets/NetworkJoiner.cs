using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;

public class NetworkJoiner : MonoBehaviour
{
    public bool isHost = true;
    private string ipAddress = "";
    private int port;
    private void Awake()
    {
        if(PlayerPrefs.HasKey("is_host"))
        {
            if(PlayerPrefs.GetInt("is_host") == 0)
            {
                NetworkManager.singleton.StartHost();

                ipAddress = NetworkManager.singleton.networkAddress;
                port = NetworkManager.singleton.networkPort;
            }
            else
            {
                isHost = false;
                ipAddress = PlayerPrefs.GetString("ip_address");

                try
                {
                    port = int.Parse(PlayerPrefs.GetString("port"));
                }
                catch (Exception e)
                {
                    OnError(null);
                    return;
                }

                NetworkManager.singleton.networkAddress = ipAddress;
                NetworkManager.singleton.networkPort = port;

                NetworkClient client = NetworkManager.singleton.StartClient();
                client.RegisterHandler(MsgType.Error, OnError);
                client.RegisterHandler(MsgType.Disconnect, OnError);
            }
        }
    }

    private void OnError(NetworkMessage error)
    {
        SceneManager.LoadScene(0);
    }

    private void OnGUI()
    {
        if(isHost)
        {
            GUI.Box(new Rect(5, 5, 200, 25), "IP Address: " + ipAddress);
            GUI.Box(new Rect(5, 30, 200, 25), "Port: " + port);
        }
    }
}
