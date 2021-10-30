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

    protected void TryLocalPlayerReconciliation ()
    {
        uint lastTickIndex = Player.LocalPlayer.LastSyncObjectReceived.tick % 1024;
        Vector3 positionDiference = Player.LocalPlayer.LastSyncObjectReceived.position - Player.LocalPlayer.PlayerState.ObjectStateBuffer[lastTickIndex].position;
        if (positionDiference.sqrMagnitude > 0.00001f)
        {
            uint rewindTick = Player.LocalPlayer.LastSyncObjectReceived.tick;
            while (rewindTick < Player.LocalPlayer.PlayerState.ObjectTick)
            {
                lastTickIndex = rewindTick % 1024;
                Player.LocalPlayer.PlayerState.ObjectInputBuffer[lastTickIndex] = Player.LocalPlayer.PlayerInput.currentInput;
                Player.LocalPlayer.PlayerState.ObjectStateBuffer[lastTickIndex].position = transform.position;
                Player.LocalPlayer.PlayerState.ObjectStateBuffer[lastTickIndex].rotation = transform.rotation;

                rewindTick += 1;
            }
        }
    }
}
