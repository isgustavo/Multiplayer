using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;

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
    Dictionary<uint, NonPlayer> objects = new Dictionary<uint, NonPlayer>();

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
    }

    public void AddObject (uint netId, NonPlayer poolObj)
    {
        if (objects.ContainsKey(netId) == true)
            return;

        objects.Add(netId, poolObj);
    }

    public void RemoveObject (uint netId)
    {
        if (objects.ContainsKey(netId) == false)
            return;

        objects.Remove(netId);
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
            NonPlayer multiplayerPoolObject = MultiplayerGamePoolManager.Current.SpawnOnClient(netId);
            if (multiplayerPoolObject == null)
            {
                return;
            }

            multiplayerPoolObject.OwnerID = message.onwerId;
            multiplayerPoolObject.gameObject.SetActive(true);
            objects.Add(netId, multiplayerPoolObject);
        }

        objects[netId].characterLogic.SetLife(message.life);
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

        foreach (NonPlayer obj in objects.Values)
        {
            ObjectStateMessage objectStateMessage = new ObjectStateMessage(obj.ObjectState.ObjectTick, obj.characterLogic.GetTransform().position, obj.characterLogic.GetTransform().rotation, obj.ID, obj.OwnerID, obj.characterLogic.GetLife());
            NetworkServer.SendToAll<ObjectStateMessage>(objectStateMessage, Channels.Unreliable, true);
        }
    }
}
