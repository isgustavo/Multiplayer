using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    public Player Owner { get; private set; }
    public CharacterController CharacterController { get; private set; }
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

        CharacterController = transform.GetComponent<CharacterController>();
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

    public bool IsLocalPlayer ()
    {
        return Player.LocalPlayer != null && Player.LocalPlayer.PlayerCharacter.GetInstanceID() == GetInstanceID();
    }
}
