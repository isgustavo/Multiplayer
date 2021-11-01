using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BotCharacter : Character
{
    public Bot Owner { get; private set; }
    public StateMachine StateMachine { get; private set; }

    public CharacterAwareness awareness { get; private set; }

    Player currentTarget;
    public Player CurrentTarget
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

    public event Action OnCurrentTargetChanged;

    public CharacterWeapon CurrentWeapon { get; private set; }

    public override void Awake ()
    {
        base.Awake();

        Owner = transform.GetComponentInParent<Bot>();

        if (NetworkServer.active == false)
            return;

        Dictionary<string, State> states = new Dictionary<string, State>
        {
             { nameof(BotPatrolMoveState), new BotPatrolMoveState(transform)},
             { nameof(BotWaitState), new BotWaitState(transform)},
             { nameof(BotChaseState), new BotChaseState(transform)},
             { nameof(BotShootingState), new BotShootingState(transform)}
        };

        StateMachine = new StateMachine(states, nameof(BotWaitState));

        awareness = GetComponent<CharacterAwareness>();

        CurrentWeapon = GetComponentInChildren<CharacterWeapon>();
    }

    public override void OnEnable ()
    {
        base.OnEnable();

        if (NetworkServer.active == false)
            return;

        awareness.OnAwernessTriggerEnter += OnAwernessTriggerEnter;
        awareness.OnAwernessTriggerExit += OnAwernessTriggerExit;

        awareness.OnClosestTriggerEnter += OnClosestTriggerEnter;
        awareness.OnClosestTriggerExit += OnClosestTriggerExit;
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

    private void OnDisable ()
    {

        if (NetworkServer.active == false)
            return;

        RemoveTarget();

        awareness.OnAwernessTriggerEnter -= OnAwernessTriggerEnter;
        awareness.OnAwernessTriggerExit -= OnAwernessTriggerExit;

        awareness.OnClosestTriggerEnter -= OnClosestTriggerEnter;
        awareness.OnClosestTriggerExit -= OnClosestTriggerExit;
    }

    public override void OnCollision (Collider collider)
    {
        base.OnCollision(collider);

        IProjectile projectile = collider.GetComponent<IProjectile>();
        if (projectile == null)
            return;

        if (NetworkServer.active == true)
        {
            TakeDamage(projectile.GetDamage(), projectile.GetOnwerId());

            projectile.ForceDespawn();
        } else
        {
            GameObject obj = VisualGamePoolManager.Current.Spawn("BotDamage");
            if (obj == null)
                return;

            obj.transform.position = transform.position;
            obj.transform.rotation = Quaternion.FromToRotation(Vector3.forward, collider.transform.forward);
            obj.SetActive(true);
        }
    }

    private void OnClosestTriggerEnter (Collider collider)
    {
        if (1 << collider.gameObject.layer != Player.COLLIDER_LAYER)
            return;

        if (TrySetTarget(collider.transform) == false)
        {
            return;
        }

        StateMachine.ChangeState(nameof(BotShootingState));
    }

    private void OnClosestTriggerExit (Collider collider)
    {
        if (CurrentTarget == null)
            return;

        if (1 << collider.gameObject.layer != Player.COLLIDER_LAYER)
            return;

        Player player = collider.GetComponentInParent<Player>();
        if (player == null)
            return;

        if (CurrentTarget.GetInstanceID() != player.GetInstanceID())
            return;

        StateMachine.ChangeState(nameof(BotChaseState));
    }

    private void OnAwernessTriggerEnter (Collider collider)
    {
        if (CurrentTarget != null)
            return;

        if (1 << collider.gameObject.layer != Player.COLLIDER_LAYER)
            return;

        if(TrySetTarget(collider.transform) == false)
        {
            return;
        }

        StateMachine.ChangeState(nameof(BotChaseState));
    }
    private void OnAwernessTriggerExit (Collider collider)
    {
        if (CurrentTarget == null)
            return;

        if (1 << collider.gameObject.layer != Player.COLLIDER_LAYER)
            return;

        Player player = collider.GetComponentInParent<Player>();
        if (player == null)
            return;

        if (CurrentTarget.GetInstanceID() != player.GetInstanceID())
            return;

        RemoveTarget();

        StateMachine.ChangeState(nameof(BotPatrolMoveState));
    }


    protected override void Dead ()
    {
        base.Dead();

        StartCoroutine(WaitToDespawn());
    }

    IEnumerator WaitToDespawn ()
    {
        yield return new WaitForEndOfFrame();
        MultiplayerGamePoolManager.Current.Despawn(Owner);
    }

    public bool TrySetTarget(Transform collider)
    {
        Player player = collider.GetComponentInParent<Player>();
        if (player == null)
            return false;

        if (player.PlayerCharacter.IsAlive() == false)
            return false;

        if(CurrentTarget != null)
            CurrentTarget.PlayerCharacter.OnDeadEvent -= OnTargetDeadEvent;

        CurrentTarget = player;
        CurrentTarget.PlayerCharacter.OnDeadEvent += OnTargetDeadEvent;

        return true;
    }

    void RemoveTarget()
    {
        if (CurrentTarget != null)
            CurrentTarget.PlayerCharacter.OnDeadEvent -= OnTargetDeadEvent;

        CurrentTarget = null;
    }

    void OnTargetDeadEvent()
    {
        CurrentTarget.PlayerCharacter.OnDeadEvent -= OnTargetDeadEvent;
        CurrentTarget = null;

        StateMachine.ChangeState(nameof(BotPatrolMoveState));
    }

    public Player GetTarget()
    {
        return CurrentTarget;
    }

    protected override void SetVisual ()
    {
        base.SetVisual();

        characterRenderer.material = MultiplayerObjectGameManager.Current.MultiplayerObjectMaterial.BotPlayer;
    }

}
