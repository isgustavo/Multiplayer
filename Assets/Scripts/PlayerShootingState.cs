using UnityEngine;

public class PlayerShootingState : PlayerServerClientState
{
    float speed = 2.5f;
    float rotationSpeed = 360f;

    public PlayerShootingState (Transform transform) : base(transform)
    {

    }

    public override void UpdateLocalPlayer ()
    {
        base.UpdateLocalPlayer();

        if (Player.LocalPlayer.PlayerInput.GetSpace() == false)
        {
            Context.StateMachine.ChangeState(nameof(PlayerCharacterMoveState));
        }

        TryReconciliation();
        StepBack(Player.LocalPlayer.PlayerInput.GetHorizontalAxis(), Player.LocalPlayer.PlayerInput.GetVerticalAxis());
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

        if (Context.Owner.PlayerInput.GetSpace() == false)
        {
            Context.StateMachine.ChangeState(nameof(PlayerCharacterMoveState));
        }

        StepBack(Context.Owner.PlayerInput.GetHorizontalAxis(), Context.Owner.PlayerInput.GetVerticalAxis());
    }

    void StepBack(float horizontalInput, float verticalInput)
    {
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput);
        moveDirection.Normalize();

        transform.Translate(-Context.Visual.forward * speed * Time.deltaTime, Space.World);

        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotate = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, rotationSpeed * Time.deltaTime);
        }
    }
}
