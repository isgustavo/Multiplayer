using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class MultiplayerSimulationGameManager : MonoBehaviour
{
    public static MultiplayerSimulationGameManager Current;

    [SerializeField]
    MultiplayerSimulation multiplayerSimulation;
    public MultiplayerSimulation Simulation => multiplayerSimulation;

    public Queue<PlayerStateGroupMessage> PlayerStateGroupMessage { get; private set; } = new Queue<PlayerStateGroupMessage>();

    public void Awake ()
    {
        if (Current == null)
            Current = this;
        else
            Destroy(this);
    }

    public void EnqueuePlayerMessage (Dictionary<uint, Player>.ValueCollection values)
    {
        PlayerStateGroupMessage groupMessage = new PlayerStateGroupMessage();
        groupMessage.deliveryTime = Time.time + Simulation.LatencyInSecond;
        foreach (Player player in values)
        {
            PlayerStateMessage inputMessage = new PlayerStateMessage(player.PlayerInput.PlayerInputTick, player.PlayerCharacter.Visual.position, player.PlayerCharacter.Visual.rotation, player.NetworkIdentity.netId);
            groupMessage.messages.Add(inputMessage);
        }

        PlayerStateGroupMessage.Enqueue(groupMessage);
    }

    public bool CanDequeueMessege()
    {
        return PlayerStateGroupMessage.Count > 0 && Time.time > PlayerStateGroupMessage.Peek().deliveryTime;
    }

    public void DequeuePlayerMessage ()
    {
        if (Random.Range(0, 100) < Simulation.PackageLoss)
            return;

        foreach (PlayerStateMessage playerStateMessage in PlayerStateGroupMessage.Dequeue().messages)
        {
            NetworkServer.SendToAll<PlayerStateMessage>(playerStateMessage, Channels.Unreliable, true);
        }
    }
}
