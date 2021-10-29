using Mirror;
using UnityEngine;

public class PlayerCharacterMoveState : PlayerServerClientState
{
    float speed = 7f;
    float rotationSpeed = 360f;

    public PlayerCharacterMoveState (Transform transform) : base(transform)
    {

    }

    public override void UpdateLocalPlayer ()
    {
        base.UpdateLocalPlayer();

        TryReconciliation();

        if (Player.LocalPlayer.PlayerInput.GetSpace() == true)
        {
            Context.StateMachine.ChangeState(nameof(PlayerShootingState));
        }
        else
        {
            MoveAndRotate(Player.LocalPlayer.PlayerInput.GetHorizontalAxis(), Player.LocalPlayer.PlayerInput.GetVerticalAxis());
        }
    }

    public override void UpdateOtherPlayer ()
    {
        base.UpdateOtherPlayer();

        transform.position = Context.Owner.PlayerInput.LastInputReceived.position;
        transform.rotation = Context.Owner.PlayerInput.LastInputReceived.rotation;
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
            MoveAndRotate(Context.Owner.PlayerInput.GetHorizontalAxis(), Context.Owner.PlayerInput.GetVerticalAxis());
        }
    }

    //public override void UpdateState ()
    //{
    //    base.UpdateState();

    //    if (Context.IsLocalPlayer() == true)
    //    {
    //        TryReconciliation();

    //        if (Player.LocalPlayer.PlayerInput.GetSpace() == true)
    //        {
    //            Context.StateMachine.ChangeState(nameof(PlayerShootingState));
    //        } else
    //        {
    //            MoveAndRotate(Player.LocalPlayer.PlayerInput.GetHorizontalAxis(), Player.LocalPlayer.PlayerInput.GetVerticalAxis());
    //        }
    //    } else
    //    {
    //        if (NetworkServer.active == true)
    //        {
    //            if (Context.Owner.PlayerInput.GetSpace() == true)
    //            {
    //                Context.StateMachine.ChangeState(nameof(PlayerShootingState));
    //            }
    //            else
    //            {
    //                MoveAndRotate(Context.Owner.PlayerInput.GetHorizontalAxis(), Context.Owner.PlayerInput.GetVerticalAxis());
    //            }
    //        }
    //        else
    //        {
    //            transform.position = Context.Owner.PlayerInput.LastInputReceived.position;
    //            transform.rotation = Context.Owner.PlayerInput.LastInputReceived.rotation;
    //        }
    //    }
    //}

    void MoveAndRotate(float horizontalInput, float verticalInput)
    {
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput);
        moveDirection.Normalize();

        transform.Translate(moveDirection * speed* Time.deltaTime, Space.World);

        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotate = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, rotationSpeed * Time.deltaTime);
        }
    }
}
