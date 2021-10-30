using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;
using System.Collections;

public class GameStateGroupMessage
{
    public float deliveryTime;
    public List<PlayerStateMessage> playerStateMessages = new List<PlayerStateMessage>();
    public List<ObjectStateMessage> objectStateMessage = new List<ObjectStateMessage>();
}

public class MultiplayerObjectGameManager : MonoBehaviour
{
    public static MultiplayerObjectGameManager Current;

    bool isServerConnected = false;
    public Dictionary<uint, Player> Players { get; private set; } = new Dictionary<uint, Player>();
    Dictionary<uint, MultiplayerPoolID> objects = new Dictionary<uint, MultiplayerPoolID>();

    private void Awake ()
    {
        if (Current == null)
            Current = this;
        else
            Destroy(this);

        NetworkServer.ReplaceHandler<PlayerInputMessage>(OnPlayerClientInputReceived);
        NetworkClient.ReplaceHandler<ObjectStateMessage>(OnObjectServerStateReceived);
        NetworkClient.ReplaceHandler<PlayerStateMessage>(OnPlayerServerStateReceived);
    }

    private void Start ()
    {
        MultiplayerGameManager.Current.OnServerConnected += ServerConnected;
    }

    private void LateUpdate ()
    {
        if (isServerConnected)
        {
            if(MultiplayerSimulationGameManager.Current.Simulation.LatencyInSecond > 0)
            {
                MultiplayerSimulationGameManager.Current.EnqueueObjectMessage(Players.Values, objects.Values);

                while(MultiplayerSimulationGameManager.Current.CanDequeueMessege())
                {
                    MultiplayerSimulationGameManager.Current.DequeuePlayerMessage();
                }
            } else
            {
                SendPlayerStateMessage();
            }
        }
    }

    public void ServerConnected ()
    {
        MultiplayerGameManager.Current.OnServerConnected -= ServerConnected;
        isServerConnected = true;
    }

    public void AddPlayer(uint netId, Player player)
    {
        if (Players.ContainsKey(netId) == true)
            return;

        Players.Add(netId, player);

        //AddObject(netId, player);
    }

    public void AddObject (uint netId, MultiplayerPoolID poolObj)
    {
        //UIConsole.Current.AddConsole($"Obj Game Manager AddObject {objects.ContainsKey(netId)}");
        if (objects.ContainsKey(netId) == true)
            return;

        //UIConsole.Current.AddConsole($"Obj Game Manager AddObject {netId}");
        objects.Add(netId, poolObj);

        //TODO NEW message TO SPAWN and DESPAWN
        //ObjectStateMessage objectStateMessage = new ObjectStateMessage(MultiplayerGamePoolManager.POOL_POSITION, Quaternion.identity, netId, true);
        //NetworkServer.SendToAll<ObjectStateMessage>(objectStateMessage, Channels.Reliable, true);
    }

    public void RemoveObject (uint netId)
    {
        //UIConsole.Current.AddConsole($"Obj Game Manager RemoveObject");
        if (objects.ContainsKey(netId) == false)
            return;

        objects.Remove(netId);

        //ObjectStateMessage objectStateMessage = new ObjectStateMessage(MultiplayerGamePoolManager.POOL_POSITION, Quaternion.identity, netId, false);
        //NetworkServer.SendToAll<ObjectStateMessage>(objectStateMessage, Channels.Reliable, true);
    }

    private void OnPlayerClientInputReceived (NetworkConnection conn, PlayerInputMessage message)
    {
        uint netId = conn.identity.netId;
        if (Players.ContainsKey(netId) == false)
            return;

        Players[netId].PlayerState.ReceivedTick(message.tick);
        Players[netId].PlayerInput.ReceivedInput(message.input);
    }

    private void OnPlayerServerStateReceived (PlayerStateMessage message)
    {
        uint netId = message.netId;
        if(Players.ContainsKey(netId) == false)
            return;
        
        Players[netId].LastSyncObjectReceived.tick = message.tick;
        Players[netId].LastSyncObjectReceived.position = message.position;
        Players[netId].LastSyncObjectReceived.rotation = message.rotation;
    }

    private void OnObjectServerStateReceived (ObjectStateMessage message)
    {
        uint netId = message.netId;
        if (objects.ContainsKey(netId) == false)
        {
            MultiplayerPoolID multiplayerPoolObject = MultiplayerGamePoolManager.Current.Spawn("PistolProjectile");
            multiplayerPoolObject.ID = netId;
            multiplayerPoolObject.gameObject.SetActive(true);
            objects.Add(netId, multiplayerPoolObject);
        }

        //if (message.isActive == false)
        //{
        //    MultiplayerGamePoolManager.Current.Despawn(objects[netId]);
        //    objects.Remove(netId);
        //    return;
        //}

        objects[netId].LastObjectStateReceived.tick = message.tick;
        objects[netId].LastObjectStateReceived.position = message.position;
        objects[netId].LastObjectStateReceived.rotation = message.rotation;
    }

    private void SendPlayerStateMessage()
    {
        foreach (Player player in Players.Values)
        {
            PlayerStateMessage playerStateMessage = new PlayerStateMessage(player.PlayerState.ObjectTick, player.Visual.position, player.Visual.rotation, player.NetworkIdentity.netId);
            NetworkServer.SendToAll<PlayerStateMessage>(playerStateMessage, Channels.Unreliable, true);
        }

        foreach (MultiplayerPoolID obj in objects.Values)
        {
            ObjectStateMessage objectStateMessage = new ObjectStateMessage(obj.ObjectState.ObjectTick, obj.transform.position, obj.transform.rotation, obj.ID);
            NetworkServer.SendToAll<ObjectStateMessage>(objectStateMessage, Channels.Unreliable, true);
        }
    }
}
