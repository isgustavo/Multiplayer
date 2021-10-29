using UnityEngine;
using Mirror;

public struct PlayerStateMessage : NetworkMessage
{
    public uint tick;
    public Vector3 position;
    public Quaternion rotation;
    public uint netId;

    public PlayerStateMessage(uint tick, Vector3 position, Quaternion rotation, uint netId)
    {
        this.tick = tick;
        this.position = position;
        this.rotation = rotation;
        this.netId = netId;
    }
}
