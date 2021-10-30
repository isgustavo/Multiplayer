using Mirror;
using UnityEngine;

public class PlayerShootingState : PlayerServerClientState
{
    float forceBack = 1f;
    float rotationSpeed = 360f;

    float cooldown = 1f;
    float currentCooldown = 1f;

    public PlayerShootingState (Transform transform) : base(transform)
    {

    }

    public override void OnEnterState (State previousState = null)
    {
        base.OnEnterState(previousState);

        currentCooldown = 1f;
    }

    public override void UpdateLocalPlayer ()
    {
        base.UpdateLocalPlayer();

        if (Player.LocalPlayer.PlayerInput.GetSpace() == false)
        {
            Context.StateMachine.ChangeState(nameof(PlayerCharacterMoveState));
            return;
        }

        TryLocalPlayerReconciliation();
        Rotate(Player.LocalPlayer.PlayerInput.GetHorizontalAxis(), Player.LocalPlayer.PlayerInput.GetVerticalAxis());
        //if (TryShoot() == false)
        //{
        //    return;
        //}

        //SpawnProjectile();
        //StepBack();
        
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

        if (Context.Owner.PlayerInput.GetSpace() == false)
        {
            Context.StateMachine.ChangeState(nameof(PlayerCharacterMoveState));
            return;
        }

        Rotate(Context.Owner.PlayerInput.GetHorizontalAxis(), Context.Owner.PlayerInput.GetVerticalAxis());
        if (TryShoot() == false)
        {
            return;
        }

        SpawnProjectile();
        //StepBack();
    }

    void Rotate(float horizontalInput, float verticalInput)
    {
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput);
        moveDirection.Normalize();

        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotate = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, rotationSpeed * Time.deltaTime);
        }
    }
    void StepBack()
    {
        //UIConsole.Current.AddConsole($"Context.Owner{Context.Owner.name} StepBack");
        transform.Translate(-Context.Visual.forward * forceBack, Space.World);
    }

    float offset = 1f;

    bool TryShoot ()
    {
        if (currentCooldown < cooldown)
        {
            currentCooldown += Time.deltaTime;
            return false;
        }

        currentCooldown = 0f;
        return true;
    }

    void SpawnProjectile()
    {
        UIConsole.Current.AddConsole($"SpawnProjectile");
        MultiplayerPoolID obj = MultiplayerGamePoolManager.Current.Spawn("PistolProjectile");
        obj.transform.position = Context.Visual.position + Context.Visual.forward * offset;
        obj.transform.forward = Context.Visual.forward;

        obj.gameObject.SetActive(true);
    }
}
