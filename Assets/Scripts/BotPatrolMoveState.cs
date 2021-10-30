using UnityEngine;

public class BotPatrolMoveState : BotCharacterState
{
    float movePatrolDuration = 10f;
    float currentMoveDuration;

    Vector3 currentPatrolPoint;
    Vector3 nextPatrolPoint;
    Quaternion toRotate;

    public BotPatrolMoveState (Transform transform) : base(transform)
    {

    }

    public override void OnEnterState (State previousState = null)
    {
        base.OnEnterState(previousState);

        currentMoveDuration = 0f;
        currentPatrolPoint = transform.position;

        nextPatrolPoint = MultiplayerBotManager.Current.GetPatrolPoint(transform.position);
        toRotate = Quaternion.LookRotation((nextPatrolPoint - currentPatrolPoint).normalized, Vector3.up);
    }

    public override void UpdateState ()
    {
        base.UpdateState();

        currentMoveDuration += Time.deltaTime;
        transform.position = Vector3.Lerp(currentPatrolPoint, nextPatrolPoint, currentMoveDuration / movePatrolDuration);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, Context.Stats.RotateSpeed * Time.deltaTime);

        Vector3 positionDiference = transform.position - nextPatrolPoint;
        if (positionDiference.sqrMagnitude < 1)
        {
            Context.StateMachine.ChangeState(nameof(BotWaitState));
        }
    }
}
