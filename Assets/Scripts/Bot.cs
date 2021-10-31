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

    public override void OnEnable()
    {
        base.OnEnable();
        MultiplayerBotManager.Current.AddBot(this);
    }

    public override void Update ()
    {
        base.Update();

        if (NetworkServer.active)
            BotCharacter.UpdateCharacter();
    }

    public override void OnDisable ()
    {
        base.OnDisable();
        MultiplayerBotManager.Current.RemoveBot(ID);
    }
}
