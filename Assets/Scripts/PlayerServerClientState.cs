using Mirror;
using UnityEngine;

public class PlayerServerClientState : PlayerCharacterState
{
    public PlayerServerClientState (Transform transform) : base(transform)
    {

    }

    public override void UpdateState ()
    {
        base.UpdateState();

        if (Context.IsLocalPlayer() == true)
        {
            UpdateLocalPlayer();
        }
        else
        {
            if (NetworkServer.active == true)
            {
                UpdateServerPlayer();
            }
            else
            {
                UpdateOtherPlayer();
            }
        }
    }

    public virtual void UpdateLocalPlayer () { }

    public virtual void UpdateServerPlayer () { }

    public virtual void UpdateOtherPlayer () { }

    protected void TryReconciliation ()
    {
        uint lastTickIndex = Player.LocalPlayer.PlayerInput.LastInputReceived.tick % 1024;
        Vector3 positionDiference = Player.LocalPlayer.PlayerInput.LastInputReceived.position - Player.LocalPlayer.PlayerInput.PlayerStateBuffer[lastTickIndex].position;
        if (positionDiference.sqrMagnitude > 0.00001f)
        {
            uint rewindTick = Player.LocalPlayer.PlayerInput.LastInputReceived.tick;
            while (rewindTick < Player.LocalPlayer.PlayerInput.PlayerInputTick)
            {
                lastTickIndex = rewindTick % 1024;
                Player.LocalPlayer.PlayerInput.PlayerInputBuffer[lastTickIndex] = Player.LocalPlayer.PlayerInput.currentInput;
                Player.LocalPlayer.PlayerInput.PlayerStateBuffer[lastTickIndex].position = transform.position;
                Player.LocalPlayer.PlayerInput.PlayerStateBuffer[lastTickIndex].rotation = transform.rotation;

                rewindTick += 1;
            }
        }
    }
}
