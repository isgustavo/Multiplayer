using UnityEngine;
using Mirror;
using System;

public struct CreatePlayerMessage : NetworkMessage
{
    public Vector3 position;
}

public class MultiplayerGameManager : NetworkManager
{
    public static MultiplayerGameManager Current;

    GameObject[] spawnPoints;

    public event Action OnServerConnected;

    public override void Awake ()
    {
        base.Awake();

        if (Current == null)
            Current = this;
        else
            Destroy(this);

        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
    }

    public override void OnStartServer ()
    {
        base.OnStartServer();

        OnServerConnected?.Invoke();
        NetworkServer.RegisterHandler<CreatePlayerMessage>(OnCreatePlayer);
    }


    public override void OnClientConnect (NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        CreatePlayerMessage createPlayerMessage = new CreatePlayerMessage
        {
            position = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length - 1)].transform.position
        };

        conn.Send(createPlayerMessage);
    }


    void OnCreatePlayer (NetworkConnection conn, CreatePlayerMessage message)
    {
        GameObject player = Instantiate(playerPrefab, message.position, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player);
    }
}
