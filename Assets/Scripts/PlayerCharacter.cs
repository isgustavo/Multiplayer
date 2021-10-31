using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    public Player Owner { get; private set; }
    public StateMachine StateMachine { get; private set; }

    public void Awake ()
    {
        Owner = transform.GetComponentInParent<Player>();

        Dictionary<string, State> states = new Dictionary<string, State>
        {
             { nameof(PlayerCharacterMoveState), new PlayerCharacterMoveState(transform)},
             { nameof(PlayerShootingState), new PlayerShootingState(transform)},
        };

        StateMachine = new StateMachine(states, nameof(PlayerCharacterMoveState));
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

    public override void OnCollision (Collider collider)
    {
        base.OnCollision(collider);

        IProjectile projectile = collider.GetComponent<IProjectile>();
        if (projectile == null)
            return;

        TakeDamage(projectile.GetDamage());
    }

    public bool IsLocalPlayer ()
    {
        return Player.LocalPlayer != null && Player.LocalPlayer.PlayerCharacter.GetInstanceID() == GetInstanceID();
    }
}
