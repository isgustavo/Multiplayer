using UnityEngine;
using Mirror;
using System;

public class PlayerInputState
{
    public uint tick;
    public Vector3 position;
    public Quaternion rotation;
}

public class PlayerInput
{
    public uint PlayerInputTick { get; set; }

    public PlayerInputState LastInputReceived = new PlayerInputState();
    public PlayerInputState[] PlayerStateBuffer { get; private set; } = new PlayerInputState[1024];
    public byte[] PlayerInputBuffer { get; private set; } = new byte[1024];

    public byte currentInput { get; private set; } = 0;

    public PlayerInput()
    {
        for (int i = 0; i < 1024; i++)
        {
            PlayerStateBuffer[i] = new PlayerInputState();
        }
    }

    public void ReceivedInput(uint tick , byte input)
    {
        PlayerInputTick = tick;
        currentInput = input;
    }

    public void ReadInput()
    {
        ReadHorizontalAxis();
        ReadVerticalAxis();
        ReadSpace();

        PlayerInputMessage inputMessage = new PlayerInputMessage(PlayerInputTick, currentInput);
        NetworkClient.Send<PlayerInputMessage>(inputMessage, Channels.Unreliable);
        //Player.LocalPlayer.NetworkIdentity.connectionToServer.Send<PlayerInputMessage>(inputMessage, Channels.Unreliable);

        uint bufferIndex = PlayerInputTick % 1024;
        PlayerInputBuffer[bufferIndex] = currentInput;
        PlayerStateBuffer[bufferIndex].position = Player.LocalPlayer.PlayerCharacter.Visual.position;
        PlayerStateBuffer[bufferIndex].rotation = Player.LocalPlayer.PlayerCharacter.Visual.rotation;

        
        //return currentInput;
    }

    public void ClearInput()
    {
        PlayerInputTick += 1;
        currentInput = 0;
    }

    void ReadHorizontalAxis ()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput > 0f)
        {
            currentInput |= 1 << 1;
        }
        else if (horizontalInput < 0f)
        {
            currentInput |= 1 << 2;
        }
    }

    void ReadVerticalAxis()
    {
        float veritcalInput = Input.GetAxisRaw("Vertical");
        if (veritcalInput > 0f)
        {
            currentInput |= 1 << 3;
        }
        else if (veritcalInput < 0f)
        {
            currentInput |= 1 << 4;
        }
    }

    void ReadSpace ()
    {
        bool spaceInput = Input.GetButton("Fire1");
        if (spaceInput)
        {
            currentInput |= 1 << 5;
        } else
        {
            currentInput |= 0 << 5;
        }
    }

    public float GetHorizontalAxis()
    {
        if ((currentInput | 1 << 1) == currentInput)
        {
            return 1f;
        }
        else if ((currentInput | 1 << 2) == currentInput)
        {
            return -1f;
        }
        else
        {
            return 0;
        }
    }

    public float GetVerticalAxis ()
    {
        if ((currentInput | 1 << 3) == currentInput)
        {
            return 1f;
        }
        else if ((currentInput | 1 << 4) == currentInput)
        {
            return -1f;
        }
        else
        {
            return 0f;
        }
    }

    public bool GetSpace()
    {
        return (currentInput | 1 << 5) == currentInput;
    }
}
