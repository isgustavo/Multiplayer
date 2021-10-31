using System;
using UnityEngine;

public class BotShootingState : BotCharacterState
{
    Transform target;

    float cooldown = 1f;
    float currentCooldown = 1f;

    float offsetForward = 2f;
    float offsetUp = 1.6f;

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
            SpawnProjectile();
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
        if (currentCooldown < cooldown)
        {
            currentCooldown += Time.deltaTime;
            return false;
        }

        currentCooldown = 0f;
        return true;
    }


    void SpawnProjectile ()
    {
        //UIConsole.Current.AddConsole($"SpawnProjectile");
        MultiplayerPoolID obj = MultiplayerGamePoolManager.Current.SpawnOnServer("PistolProjectile");
        obj.OwnerID = Context.Owner.ID;
        obj.transform.position = Context.Visual.position + Context.Visual.forward * offsetForward + Context.Visual.up * offsetUp;
        obj.transform.forward = Context.Visual.forward;
        obj.gameObject.SetActive(true);
    }
}
