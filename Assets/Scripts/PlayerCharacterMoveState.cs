using Mirror;
using UnityEngine;

public class PlayerCharacterMoveState : PlayerServerClientState
{

    public PlayerCharacterMoveState (Transform transform) : base(transform)
    {

    }

    public override void OnEnterState (State previousState = null)
    {
        base.OnEnterState(previousState);
    }

    public override void UpdateLocalPlayer ()
    {
        base.UpdateLocalPlayer();

        TryLocalPlayerReconciliation();

        if (Player.LocalPlayer.PlayerInput.GetSpace() == true)
        {
            Context.StateMachine.ChangeState(nameof(PlayerShootingState));
        }
        else
        {
            MoveAndRotate(Player.LocalPlayer.PlayerInput.GetHorizontalAxis(), Player.LocalPlayer.PlayerInput.GetVerticalAxis(), Player.LocalPlayer.PlayerInput.currentMouse);
        }
    }

    public override void UpdateOtherPlayer ()
    {
        base.UpdateOtherPlayer();

        transform.position = Context.Owner.LastSyncObjectReceived.position;
        transform.rotation = Context.Owner.LastSyncObjectReceived.rotation;
    }

    public override void UpdateServerPlayer ()
    {
        base.UpdateServerPlayer();

        if (Context.Owner.PlayerInput.GetSpace() == true)
        {
            Context.StateMachine.ChangeState(nameof(PlayerShootingState));
        }
        else
        {
            MoveAndRotate(Context.Owner.PlayerInput.GetHorizontalAxis(), Context.Owner.PlayerInput.GetVerticalAxis(), Context.Owner.PlayerInput.currentMouse);
        }
    }

    void MoveAndRotate(float horizontalInput, float verticalInput, Vector2 rotate)
    {

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput);
        moveDirection.Normalize();

        transform.Translate(moveDirection * Context.Stats.MoveSpeed * Time.deltaTime, Space.World);

        transform.LookAt(new Vector3(rotate.x, transform.position.y, rotate.y));
    }
}
