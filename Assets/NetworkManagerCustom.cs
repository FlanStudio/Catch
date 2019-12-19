using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class CustomMessage : MessageBase
{
    public short controllerID = 0;
    public uint prefabIndex = 0u;
}

public class NetworkManagerCustom : NetworkManager
{
    private NetworkJoiner netJoiner;

    public static NetworkManagerCustom singleton;

    private void Awake()
    {
        singleton = this;
        netJoiner = GetComponent<NetworkJoiner>();
    }

    public override void OnStartServer()
    {
        NetworkServer.RegisterHandler(MsgType.Highest + 1, OnPrefabResponse);
        base.OnStartServer();
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        client.RegisterHandler(MsgType.Highest + 1, OnPrefabRequest);
        base.OnClientConnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        CustomMessage msg = new CustomMessage();
        msg.controllerID = playerControllerId;

        NetworkServer.SendToClient(conn.connectionId, MsgType.Highest + 1, msg);
    }

    private void OnPrefabRequest(NetworkMessage netMsg)
    {
        CustomMessage msg = netMsg.ReadMessage<CustomMessage>();
        msg.prefabIndex = (uint) (netJoiner.isHost ? 0 : 1);
        client.Send(MsgType.Highest + 1, msg);
    }

    private void OnPrefabResponse(NetworkMessage netMsg)
    {
        CustomMessage msg = netMsg.ReadMessage<CustomMessage>();
        playerPrefab = spawnPrefabs[(int)msg.prefabIndex];
        base.OnServerAddPlayer(netMsg.conn, msg.controllerID);
    }
}