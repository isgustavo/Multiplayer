using UnityEngine;

public class PlayerCharacterState : State
{
    protected PlayerCharacter Context { get; private set; }

    public PlayerCharacterState (Transform transform) : base(transform)
    {
        Context = transform.GetComponent<PlayerCharacter>();
    }
}
