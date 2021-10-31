﻿using UnityEngine;
using Mirror;

public struct ObjectStateMessage : NetworkMessage
{
    public uint tick;
    public uint netId;
    public uint onwerId;
    public int life;
    public Vector3 position;
    public Quaternion rotation;

    public ObjectStateMessage (uint tick, Vector3 position, Quaternion rotation, uint netId, uint onwerId, int life)
    {
        this.tick = tick;
        this.netId = netId;
        this.life = life;
        this.onwerId = onwerId;
        this.position = position;
        this.rotation = rotation;
    }
}

public struct PlayerStateMessage : NetworkMessage
{
    public uint tick;
    public uint netId;
    public Vector3 position;
    public Quaternion rotation;

    public PlayerStateMessage (uint tick, Vector3 position, Quaternion rotation, uint netId)
    {
        this.tick = tick;
        this.netId = netId;
        this.position = position;
        this.rotation = rotation;
    }
}

