using UnityEngine;

public class BotWaitState : BotCharacterState
{
    float waitToPatrolTime = 2.5f;
    float currentTimeOnPoint;

    public BotWaitState (Transform transform) : base(transform)
    {

    }

    public override void OnEnterState (State previousState = null)
    {
        base.OnEnterState(previousState);

        currentTimeOnPoint = 0f;
    }

    public override void UpdateState ()
    {
        base.UpdateState();

        if (currentTimeOnPoint < waitToPatrolTime)
        {
            currentTimeOnPoint += Time.deltaTime;
        } else
        {
            Context.StateMachine.ChangeState(nameof(BotPatrolMoveState));
        }
    }
}
