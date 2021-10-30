using UnityEngine;
using Mirror;
using System;

public class ObjectTickState
{
    public uint tick;
    public Vector3 position;
    public Quaternion rotation;
}

public class PlayerInput
{ 
    public byte currentInput { get; private set; } = 0;

    public void ReceivedInput (byte input)
    {
        currentInput = input;
    }

    public void ReadInput()
    {
        ReadHorizontalAxis();
        ReadVerticalAxis();
        ReadSpace();
    }

    public void ClearInput()
    {
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
