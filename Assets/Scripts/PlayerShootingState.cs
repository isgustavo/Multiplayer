using Mirror;
using UnityEngine;

public class PlayerShootingState : PlayerServerClientState
{

    float cooldown = 1f;
    float currentCooldown = 1f;

    float offset = 1f;

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
    }

    void Rotate(float horizontalInput, float verticalInput)
    {
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput);
        moveDirection.Normalize();

        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotate = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, Context.Stats.RotateSpeed * Time.deltaTime);
        }
    }


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
        MultiplayerPoolID obj = MultiplayerGamePoolManager.Current.SpawnOnServer("PistolProjectile");
        obj.OwnerID = Context.Owner.NetworkIdentity.netId;
        obj.transform.position = Context.Visual.position + Context.Visual.forward * offset;
        obj.transform.forward = Context.Visual.forward;

        obj.gameObject.SetActive(true);
    }
}
