using Mirror;

public struct PlayerInputMessage : NetworkMessage
{
    public uint tick;
    public byte input;
    public float mouse;

    public PlayerInputMessage (uint tick, byte input, float mouse)
    {
        this.tick = tick;
        this.input = input;
        this.mouse = mouse;
    }
}
