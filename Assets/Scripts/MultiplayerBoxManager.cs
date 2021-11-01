public class MultiplayerBoxManager : MultiplayerSpawnerManager
{
    public static MultiplayerBoxManager Current;
    public override string tagName => "BoxSpawnPoint";
    public override string objectName => "Box";

    public override void Awake ()
    {
        if (Current == null)
            Current = this;
        else
            Destroy(this);

        base.Awake();
    }
}
