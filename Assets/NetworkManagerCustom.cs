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

public class RespawnMessage : MessageBase
{
    public const short RespawnType = MsgType.Highest + 2;
    public bool isCatcher = false;
}

public class NetworkManagerCustom : NetworkManager
{
    private NetworkJoiner netJoiner;

    [HideInInspector]
    public GameObject localPlayer;

    [HideInInspector]
    public GameObject foreignPlayer;

    public static NetworkManagerCustom singleton;

    public Transform catcherSpawnPoint;
    public Transform catchedSpawnPoint;

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
        client.RegisterHandler(RespawnMessage.RespawnType, OnRespawnRequest);

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

    private void OnRespawnRequest(NetworkMessage netMsg)
    {
        RespawnMessage msg = netMsg.ReadMessage<RespawnMessage>();
        if(msg.isCatcher)
        {
            localPlayer.transform.position = catcherSpawnPoint.position;
            localPlayer.transform.rotation = catcherSpawnPoint.rotation;
        }
        else
        {
            localPlayer.transform.position = catchedSpawnPoint.position;
            localPlayer.transform.rotation = catchedSpawnPoint.rotation;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F4))
        {
            RespawnPlayers(true);
        }
    }

    public void RespawnPlayers(bool isCatcher)
    {
        if (!netJoiner.isHost)
            return;

        if(isCatcher)
        {
            localPlayer.transform.position = catcherSpawnPoint.position;
            localPlayer.transform.rotation = catcherSpawnPoint.rotation;
        }
        else
        {
            localPlayer.transform.position = catchedSpawnPoint.position;
            localPlayer.transform.rotation = catchedSpawnPoint.rotation;
        }

        RespawnMessage msg = new RespawnMessage();
        msg.isCatcher = !isCatcher;
        NetworkServer.SendToClient(foreignPlayer.GetComponent<PlayerController>().connectionToClient.connectionId, RespawnMessage.RespawnType, msg);
    }
}