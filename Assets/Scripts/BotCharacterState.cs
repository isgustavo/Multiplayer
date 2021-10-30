using UnityEngine;

public class BotCharacterState : State
{
    protected BotCharacter Context { get; private set; }

    public BotCharacterState (Transform transform) : base(transform)
    {
        Context = transform.GetComponent<BotCharacter>();
    }
}
