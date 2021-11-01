using Mirror;
using UnityEngine;

public struct PlayerInputMessage : NetworkMessage
{
    public uint tick;
    public byte input;
    public Vector2 mouse;

    public PlayerInputMessage (uint tick, byte input, Vector2 mouse)
    {
        this.tick = tick;
        this.input = input;
        this.mouse = mouse;
    }
}
