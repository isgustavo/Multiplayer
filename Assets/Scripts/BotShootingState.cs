using System;
using UnityEngine;

public class BotShootingState : BotCharacterState
{
    Transform target;

    float currentCooldown = 1f;

    public BotShootingState (Transform transform) : base(transform)
    {

    }

    public override void OnEnterState (State previousState = null)
    {
        base.OnEnterState(previousState);

        SetCurrentTarget();
        Context.OnCurrentTargetChanged += SetCurrentTarget;
    }

    public override void UpdateState ()
    {
        base.UpdateState();

        Vector3 direction = (target.position - Context.Visual.position).normalized;
        Quaternion toRotate = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, 360f * Time.deltaTime);

        if (TryShoot())
        {
            foreach (Transform spawnPoint in Context.CurrentWeapon.spawnPoints)
            {
                SpawnProjectile(spawnPoint.position, spawnPoint.forward);
            }
        }
    }

    public override void OnLeaveState ()
    {
        base.OnLeaveState();
        Context.OnCurrentTargetChanged -= SetCurrentTarget;
    }

    private void SetCurrentTarget ()
    {
        target = Context.GetTarget();
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


    void SpawnProjectile (Vector3 position, Vector3 forward)
    {
        MultiplayerPoolID obj = MultiplayerGamePoolManager.Current.SpawnOnServer("PistolProjectile");

        if (obj == null)
            return;

        obj.OwnerID = Context.Owner.ID;
        obj.transform.position = position;
        obj.transform.forward = forward;
        obj.gameObject.SetActive(true);
    }
}
