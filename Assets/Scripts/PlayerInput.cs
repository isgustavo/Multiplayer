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
    public Vector2 currentMouse { get; private set; } = Vector2.zero;

    public void ReceivedInput (byte input, Vector2 mouse)
    {
        currentInput = input;
        currentMouse = mouse;
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
        currentMouse = Vector2.zero;
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
        Ray cameraRay = Player.LocalPlayer.CharacterCamera.Cam.ScreenPointToRay(Input.mousePosition);
        Plane virtualPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLenght;

        if (virtualPlane.Raycast(cameraRay, out rayLenght))
        {
            Vector3 pointToLook = cameraRay.GetPoint(rayLenght);
            currentMouse = new Vector2(pointToLook.x, pointToLook.z);
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
