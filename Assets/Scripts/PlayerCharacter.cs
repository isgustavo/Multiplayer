using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerCharacter : Character
{
    public Player Owner { get; private set; }
    public StateMachine StateMachine { get; private set; }

    public CharacterWeapon CurrentWeapon { get; private set; }

    public override void Awake ()
    {
        base.Awake();

        Owner = transform.GetComponentInParent<Player>();
        Player.OnLocalPlayerChanged += OnLocalPlayerChanged;

        Dictionary<string, State> states = new Dictionary<string, State>
        {
             { nameof(PlayerCharacterMoveState), new PlayerCharacterMoveState(transform)},
             { nameof(PlayerShootingState), new PlayerShootingState(transform)},
        };

        StateMachine = new StateMachine(states, nameof(PlayerCharacterMoveState));
        CurrentWeapon = GetComponentInChildren<CharacterWeapon>();
    }

    public override void UpdateCharacter ()
    {
        if(IsAlive())
            StateMachine.UpdateState();
    }

    public override void FixedUpdateCharacter ()
    {
        if (IsAlive())
            StateMachine.FixedUpdateState();
    }

    public override void LateUpdateCharacter ()
    {
        if (IsAlive())
            StateMachine.LateUpdateState();
    }

    private void OnDisable ()
    {
        Player.OnLocalPlayerChanged -= OnLocalPlayerChanged;
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
        }
    }

    private void OnLocalPlayerChanged ()
    {
        SetVisual();
    }

    public bool IsLocalPlayer ()
    {
        return Player.LocalPlayer != null && Player.LocalPlayer.PlayerCharacter.GetInstanceID() == GetInstanceID();
    }

    protected override void SetVisual ()
    {
        base.SetVisual();

        if(IsLocalPlayer())
        {
            characterRenderer.material = MultiplayerObjectGameManager.Current.MultiplayerObjectMaterial.LocalPlayer;
        }
        else
        {
            characterRenderer.material = MultiplayerObjectGameManager.Current.MultiplayerObjectMaterial.OtherPlayer;
        }
    }

    protected override void Dead ()
    {
        base.Dead();

        if (NetworkServer.active == false)
        {
            characterRenderer.enabled = false;
        }
    }

    void TryScore (uint ownerId)
    {
        if (MultiplayerObjectGameManager.Current.Players.ContainsKey(ownerId))
            MultiplayerObjectGameManager.Current.Players[ownerId].AddStore(Stats.HitPoint);
    }
}
