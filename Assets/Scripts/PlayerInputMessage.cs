using Mirror;

public struct PlayerInputMessage : NetworkMessage
{
    public uint tick;
    public byte input;

    public PlayerInputMessage (uint tick, byte input)
    {
        this.tick = tick;
        this.input = input;
    }
}
