using System;
using UnityEngine;

public class BotChaseState : BotCharacterState
{
    Transform target;

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

        transform.position = Vector3.MoveTowards(transform.position, target.position, Context.Stats.MoveSpeed * Time.deltaTime);
    }
}
