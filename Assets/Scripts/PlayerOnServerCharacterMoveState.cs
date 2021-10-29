using UnityEngine;

public class PlayerOnServerCharacterMoveState : PlayerCharacterState
{
    float speed = 7f;
    float rotationSpeed = 360f;

    public PlayerOnServerCharacterMoveState (Transform transform) : base(transform)
    {

    }

    public override void UpdateState ()
    {
        base.UpdateState();

        float horizontalInput = Context.Owner.PlayerInput.GetHorizontalAxis();
        float verticalInput = Context.Owner.PlayerInput.GetVerticalAxis();

        MoveAndRotate(horizontalInput, verticalInput);
        
    }

    void MoveAndRotate (float horizontalInput, float verticalInput)
    {
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput);
        float magnitude = Mathf.Clamp01(moveDirection.magnitude) * speed;
        moveDirection.Normalize();

        Context.CharacterController.SimpleMove(moveDirection * magnitude);

        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotate = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, rotationSpeed * Time.deltaTime);
        }
    }
}
