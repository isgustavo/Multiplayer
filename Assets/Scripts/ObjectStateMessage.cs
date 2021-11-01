using UnityEngine;
using Mirror;

public struct ObjectStateMessage : NetworkMessage
{
    public uint tick;
    public uint netId;
    public uint onwerId;
    public float health;
    public Vector3 position;
    public Quaternion rotation;

    public ObjectStateMessage (uint tick, Vector3 position, Quaternion rotation, uint netId, uint onwerId, float health)
    {
        this.tick = tick;
        this.netId = netId;
        this.health = health;
        this.onwerId = onwerId;
        this.position = position;
        this.rotation = rotation;
    }
}

public struct PlayerStateMessage : NetworkMessage
{
    public uint tick;
    public uint netId;
    public int points;
    public float health;
    public Vector3 position;
    public Quaternion rotation;

    public PlayerStateMessage (uint tick, Vector3 position, Quaternion rotation, uint netId, float health, int points)
    {
        this.tick = tick;
        this.netId = netId;
        this.position = position;
        this.rotation = rotation;
        this.health = health;
        this.points = points;
    }
}

