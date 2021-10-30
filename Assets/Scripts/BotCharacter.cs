using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BotCharacter : Character
{
    public Bot Owner { get; private set; }
    public StateMachine StateMachine { get; private set; }

    Transform currentTarget;
    public Transform CurrentTarget
    {
        get
        {
            return currentTarget;
        }
        set
        {
            currentTarget = value;
            OnCurrentTargetChanged?.Invoke();
        }
    }

    public CharacterAwareness awareness { get; private set; }
    public event Action OnCurrentTargetChanged;

    public void Awake ()
    {
        if (NetworkServer.active == false)
            return;

        Owner = transform.GetComponentInParent<Bot>();

        Dictionary<string, State> states = new Dictionary<string, State>
        {
             { nameof(BotPatrolMoveState), new BotPatrolMoveState(transform)},
             { nameof(BotWaitState), new BotWaitState(transform)},
             { nameof(BotChaseState), new BotChaseState(transform)},
             { nameof(BotShootingState), new BotShootingState(transform)}
        };

        StateMachine = new StateMachine(states, nameof(BotWaitState));

        awareness = GetComponent<CharacterAwareness>();
    }

    private void OnEnable ()
    {
        awareness.OnAwernessTriggerEnter += OnAwernessTriggerEnter;
        awareness.OnAwernessTriggerExit += OnAwernessTriggerExit;

        awareness.OnClosestTriggerEnter += OnClosestTriggerEnter;
        awareness.OnClosestTriggerExit += OnClosestTriggerExit;
    }

    private void OnDisable ()
    {
        awareness.OnAwernessTriggerEnter -= OnAwernessTriggerEnter;
        awareness.OnAwernessTriggerExit -= OnAwernessTriggerExit;

        awareness.OnClosestTriggerEnter -= OnClosestTriggerEnter;
        awareness.OnClosestTriggerExit -= OnClosestTriggerExit;
    }

    public override void UpdateCharacter ()
    {
        StateMachine.UpdateState();
    }

    public override void FixedUpdateCharacter ()
    {
        StateMachine.FixedUpdateState();
    }

    public override void LateUpdateCharacter ()
    {
        StateMachine.LateUpdateState();
    }

    private void OnClosestTriggerEnter (Collider collider)
    {
        if (1 << collider.gameObject.layer != Player.COLLIDER_LAYER)
            return;

        SetTarget(collider.transform);

        StateMachine.ChangeState(nameof(BotShootingState));
    }

    private void OnClosestTriggerExit (Collider collider)
    {
        if (currentTarget == null)
            return;

        if (1 << collider.gameObject.layer != Player.COLLIDER_LAYER)
            return;

        if (currentTarget.GetInstanceID() != collider.transform.GetInstanceID())
            return;

        StateMachine.ChangeState(nameof(BotChaseState));
    }

    private void OnAwernessTriggerExit (Collider collider)
    {
        if (currentTarget == null)
            return;

        if (1 << collider.gameObject.layer != Player.COLLIDER_LAYER)
            return;

        if (currentTarget.GetInstanceID() != collider.transform.GetInstanceID())
            return;

        CurrentTarget = null;
        StateMachine.ChangeState(nameof(BotPatrolMoveState));
    }

    private void OnAwernessTriggerEnter (Collider collider)
    {
        if (currentTarget != null)
            return;

        if (1 << collider.gameObject.layer != Player.COLLIDER_LAYER)
            return;

        SetTarget(collider.transform);
        StateMachine.ChangeState(nameof(BotChaseState));
    }

    public void SetTarget(Transform target)
    {
        CurrentTarget = target;
    }

    public Transform GetTarget()
    {
        return CurrentTarget;
    }
}
