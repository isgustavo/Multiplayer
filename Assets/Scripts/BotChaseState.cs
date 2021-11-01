using System;
using UnityEngine;

public class BotChaseState : BotCharacterState
{
    Player target;

    public BotChaseState (Transform transform) : base(transform)
    {

    }

    public override void OnEnterState (State previousState = null)
    {
        base.OnEnterState(previousState);

        target = Context.GetTarget();
    }

    public override void UpdateState ()
    {
        base.UpdateState();

        transform.position = Vector3.MoveTowards(transform.position, target.Visual.transform.position, Context.Stats.MoveSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

        Vector3 direction = (target.Visual.transform.position - Context.Visual.position).normalized;
        Quaternion toRotate = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, 360f * Time.deltaTime);
    }
}
