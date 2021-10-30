using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class MultiplayerSimulationGameManager : MonoBehaviour
{
    public static MultiplayerSimulationGameManager Current;

    [SerializeField]
    MultiplayerSimulation multiplayerSimulation;
    public MultiplayerSimulation Simulation => multiplayerSimulation;

    public Queue<GameStateGroupMessage> GameStateGroupMessage { get; private set; } = new Queue<GameStateGroupMessage>();

    public void Awake ()
    {
        if (Current == null)
            Current = this;
        else
            Destroy(this);
    }

    public void EnqueueObjectMessage (Dictionary<uint, Player>.ValueCollection players, Dictionary<uint, MultiplayerPoolID>.ValueCollection objects)
    {
        GameStateGroupMessage groupMessage = new GameStateGroupMessage();
        groupMessage.deliveryTime = Time.time + Simulation.LatencyInSecond;

        foreach (Player player in players)
        {
            PlayerStateMessage playerStateMessage = new PlayerStateMessage(player.PlayerState.ObjectTick, player.Visual.position, player.Visual.rotation, player.NetID);
            groupMessage.playerStateMessages.Add(playerStateMessage);
        }

        foreach (MultiplayerPoolID obj in objects)
        {
            ObjectStateMessage inputMessage = new ObjectStateMessage(obj.ObjectState.ObjectTick, obj.transform.position, obj.transform.rotation, obj.ID);
            groupMessage.objectStateMessage.Add(inputMessage);
        }

        GameStateGroupMessage.Enqueue(groupMessage);
    }

    //public void EnqueuePlayerMessage (Dictionary<uint, SyncObject>.ValueCollection values)
    //{
    //    PlayerStateGroupMessage groupMessage = new PlayerStateGroupMessage();
    //    groupMessage.deliveryTime = Time.time + Simulation.LatencyInSecond;
    //    foreach (SyncObject obj in values)
    //    {
    //        uint tick = 0;
    //        if (MultiplayerObjectGameManager.Current.Players.ContainsKey(obj.NetworkIdentity.netId) == true)
    //            tick = MultiplayerObjectGameManager.Current.Players[obj.NetworkIdentity.netId].PlayerInput.PlayerInputTick;

    //        ObjectStateMessage inputMessage = new ObjectStateMessage(tick, obj.Visual.position, obj.Visual.rotation, obj.NetworkIdentity.netId);
    //        groupMessage.messages.Add(inputMessage);
    //    }

    //    PlayerStateGroupMessage.Enqueue(groupMessage);
    //}

    public bool CanDequeueMessege()
    {
        return GameStateGroupMessage.Count > 0 && Time.time > GameStateGroupMessage.Peek().deliveryTime;
    }

    public void DequeuePlayerMessage ()
    {
        if (Random.Range(0, 100) < Simulation.PackageLoss)
            return;

        GameStateGroupMessage gameStateGroupMessage = GameStateGroupMessage.Dequeue();

        foreach (PlayerStateMessage playerStateMessage in gameStateGroupMessage.playerStateMessages)
        {
            NetworkServer.SendToAll<PlayerStateMessage>(playerStateMessage, Channels.Unreliable, true);
        }

        foreach (ObjectStateMessage objectStateMessage in gameStateGroupMessage.objectStateMessage)
        {
            NetworkServer.SendToAll<ObjectStateMessage>(objectStateMessage, Channels.Unreliable, true);
        }
    }
}
