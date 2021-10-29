using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;
using System.Collections;

public class PlayerStateGroupMessage
{
    public float deliveryTime;
    public List<PlayerStateMessage> messages = new List<PlayerStateMessage>();
}

public class PlayerGameManager : MonoBehaviour
{
    public static PlayerGameManager Current;

    bool isServerConnected = false;
    Dictionary<uint, Player> players = new Dictionary<uint, Player>();

    private void Awake ()
    {
        if (Current == null)
            Current = this;
        else
            Destroy(this);

        NetworkServer.ReplaceHandler<PlayerInputMessage>(OnPlayerClientInputReceived);
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
                MultiplayerSimulationGameManager.Current.EnqueuePlayerMessage(players.Values);

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

    public void AddPlayer (uint netId, Player player)
    {
        if (players.ContainsKey(netId) == true)
            return;

        players.Add(netId, player);
    }

    private void OnPlayerClientInputReceived (NetworkConnection conn, PlayerInputMessage message)
    {
        uint netId = conn.identity.netId;
        if (players.ContainsKey(netId) == false)
            return;

        players[netId].PlayerInput.ReceivedInput(message.tick, message.input);
    }

    private void OnPlayerServerStateReceived (PlayerStateMessage message)
    {
        uint netId = message.netId;
        if (players.ContainsKey(netId) == false)
            return;

        players[netId].PlayerInput.LastInputReceived.tick = message.tick;
        players[netId].PlayerInput.LastInputReceived.position = message.position;
        players[netId].PlayerInput.LastInputReceived.rotation = message.rotation;
    }

    private void SendPlayerStateMessage()
    {
        foreach (Player player in players.Values)
        {
            PlayerStateMessage playerStateMessage = new PlayerStateMessage(player.PlayerInput.PlayerInputTick, player.PlayerCharacter.Visual.position, player.PlayerCharacter.Visual.rotation, player.NetworkIdentity.netId);
            NetworkServer.SendToAll<PlayerStateMessage>(playerStateMessage, Channels.Unreliable, true);
        }
    }
}
