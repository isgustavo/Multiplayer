using Mirror;
using UnityEngine;

public class PlayerShootingState : PlayerServerClientState
{
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

        if (TryShoot() == false)
        {
            return;
        }

        foreach(Transform spawnPoint in Context.CurrentWeapon.spawnPoints)
        {
            SpawnProjectile(spawnPoint.position, spawnPoint.forward);
        }
    }

    bool TryShoot ()
    {
        
        if (currentCooldown < Context.CurrentWeapon.Stats.cooldown)
        {
            currentCooldown += Time.deltaTime;
            return false;
        }

        currentCooldown = 0f;
        return true;
    }

    void SpawnProjectile(Vector3 position, Vector3 forward)
    {
        MultiplayerPoolID obj = MultiplayerGamePoolManager.Current.SpawnOnServer("PistolProjectile");
        if (obj == null)
            return;

        obj.OwnerID = Context.Owner.NetworkIdentity.netId;

        obj.transform.position = position;
        obj.transform.forward = forward;

        obj.gameObject.SetActive(true);
    }
}
