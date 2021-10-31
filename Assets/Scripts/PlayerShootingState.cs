using Mirror;
using UnityEngine;

public class PlayerShootingState : PlayerServerClientState
{

    float cooldown = 1f;
    float currentCooldown = 1f;

    float offsetForward = 1f;
    float offsetUp = 1f;

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
        MoveAndRotate(Context.Owner.PlayerInput.GetHorizontalAxis(), Context.Owner.PlayerInput.GetVerticalAxis());

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

        MoveAndRotate(Context.Owner.PlayerInput.GetHorizontalAxis(), Context.Owner.PlayerInput.GetVerticalAxis());

        if (TryShoot() == false)
        {
            return;
        }

        SpawnProjectile();
    }

    void MoveAndRotate (float horizontalInput, float verticalInput)
    {
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput);
        moveDirection.Normalize();

        transform.Translate(moveDirection * Context.Stats.MoveSpeed * Time.deltaTime, Space.World);

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
        //UIConsole.Current.AddConsole($"SpawnProjectile");
        MultiplayerPoolID obj = MultiplayerGamePoolManager.Current.SpawnOnServer("PistolProjectile");
        obj.OwnerID = Context.Owner.NetworkIdentity.netId;
        UIConsole.Current.AddConsole($"obj.OwnerID {obj.ID}  ");
        obj.transform.position = Context.Visual.position + Context.Visual.forward * offsetForward + Context.Visual.up * offsetUp;
        obj.transform.forward = Context.Visual.forward;

        obj.gameObject.SetActive(true);
    }
}
