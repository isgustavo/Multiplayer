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
    public static int MOUSE_SENSIBILITY = 10;

    public byte currentInput { get; private set; } = 0;
    public float currentMouseAngle { get; private set; } = 0f;

    public void ReceivedInput (byte input, float mouse)
    {
        currentInput = input;
        currentMouseAngle = mouse;
    }

    public void ReadInput()
    {
        ReadHorizontalAxis();
        ReadVerticalAxis();
        ReadMouse();
        ReadSpace();
    }

    public void ClearInput()
    {
        currentInput = 0;
        currentMouseAngle = 0f;
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

    void ReadMouse()
    {
        currentMouseAngle = Input.GetAxis("Mouse X") * MOUSE_SENSIBILITY;
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
