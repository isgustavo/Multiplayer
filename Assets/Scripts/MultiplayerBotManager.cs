public class MultiplayerBotManager : MultiplayerSpawnerManager
{
    public static MultiplayerBotManager Current;

    public override string tagName => "BotSpawnPoint";
    public override string objectName => "Bot";

    public override void Awake ()
    {
        if (Current == null)
            Current = this;
        else
            Destroy(this);

        base.Awake();
    }
}
