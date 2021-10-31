using Mirror;
using UnityEngine;

public class PlayerCharacterMoveState : PlayerServerClientState
{
    float turnValue;

    public PlayerCharacterMoveState (Transform transform) : base(transform)
    {

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
            MoveAndRotate(Player.LocalPlayer.PlayerInput.GetHorizontalAxis(), Player.LocalPlayer.PlayerInput.GetVerticalAxis(), Player.LocalPlayer.PlayerInput.currentMouseAngle);
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
            MoveAndRotate(Context.Owner.PlayerInput.GetHorizontalAxis(), Context.Owner.PlayerInput.GetVerticalAxis(), Context.Owner.PlayerInput.currentMouseAngle);
        }
    }


    void MoveAndRotate(float horizontalInput, float verticalInput, float mouseAngle)
    {

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput);
        moveDirection.Normalize();

        transform.Translate(moveDirection * Context.Stats.MoveSpeed * Time.deltaTime, Space.World);

        turnValue += mouseAngle;

        transform.localRotation = Quaternion.Euler(0f, turnValue, 0f);

        //if (moveDirection != Vector3.zero)
        //{
        //    Quaternion toRotate = Quaternion.LookRotation(moveDirection, Vector3.up);
        //    transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, Context.Stats.RotateSpeed * Time.deltaTime);
        //}
    }
}
