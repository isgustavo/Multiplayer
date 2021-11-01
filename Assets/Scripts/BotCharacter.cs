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

        currentTarget = null;

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

    public void SetTarget(Transform target)
    {
        CurrentTarget = target;
    }

    public Transform GetTarget()
    {
        return CurrentTarget;
    }

    protected override void SetVisual ()
    {
        base.SetVisual();

        characterRenderer.material = MultiplayerObjectGameManager.Current.MultiplayerObjectMaterial.BotPlayer;
    }

}
