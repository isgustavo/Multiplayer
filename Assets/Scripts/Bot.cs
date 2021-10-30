using Mirror;
using UnityEngine;

public class Bot : NonPlayer
{
    public BotCharacter BotCharacter { get; private set; }

    public override void Awake ()
    {
        base.Awake();

        BotCharacter = GetComponent<BotCharacter>();
    }

    public override void Update ()
    {
        base.Update();

        if (NetworkServer.active)
            BotCharacter.UpdateCharacter();
    }
}
