using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class MultiplayerSimulationGameManager : MonoBehaviour
{
    public static MultiplayerSimulationGameManager Current;

    [SerializeField]
    SOMultiplayerSimulation multiplayerSimulation;
    public SOMultiplayerSimulation Simulation => multiplayerSimulation;

    public Queue<GameStateGroupMessage> GameStateGroupMessage { get; private set; } = new Queue<GameStateGroupMessage>();

    public void Awake ()
    {
        if (Current == null)
            Current = this;
        else
            Destroy(this);
    }

    public void EnqueueObjectMessage (Dictionary<uint, Player>.ValueCollection players, Dictionary<uint, NonPlayer>.ValueCollection objects)
    {
        GameStateGroupMessage groupMessage = new GameStateGroupMessage();
        groupMessage.deliveryTime = Time.time + Simulation.LatencyInSecond;

        foreach (Player player in players)
        {
            PlayerStateMessage playerStateMessage = new PlayerStateMessage(player.PlayerState.ObjectTick, player.Visual.position, player.Visual.rotation, player.NetworkIdentity.netId);
            groupMessage.playerStateMessages.Add(playerStateMessage);
        }

        foreach (NonPlayer obj in objects)
        {
            ObjectStateMessage inputMessage = new ObjectStateMessage(obj.ObjectState.ObjectTick, obj.characterLogic.GetTransform().position, obj.characterLogic.GetTransform().rotation, obj.ID, obj.OwnerID, obj.characterLogic.GetLife());
            groupMessage.objectStateMessage.Add(inputMessage);
        }

        GameStateGroupMessage.Enqueue(groupMessage);
    }

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
