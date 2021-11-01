using Mirror;
using UnityEngine;

public class PlayerState : ObjectState
{
    public byte[] ObjectInputBuffer { get; private set; } = new byte[1024];

    internal void SendInputToServer (byte input, Vector2 mouse)
    {
        PlayerInputMessage inputMessage = new PlayerInputMessage(ObjectTick, input, mouse);
        NetworkClient.Send<PlayerInputMessage>(inputMessage, Channels.Unreliable);

        uint bufferIndex = ObjectTick % 1024;
        ObjectInputBuffer[bufferIndex] = input;
        ObjectStateBuffer[bufferIndex].position = Player.LocalPlayer.PlayerCharacter.Visual.position;
        ObjectStateBuffer[bufferIndex].rotation = Player.LocalPlayer.PlayerCharacter.Visual.rotation;
    }
}
