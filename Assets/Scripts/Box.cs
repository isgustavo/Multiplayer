public class Box : NonPlayer
{
    public override void OnEnable ()
    {
        base.OnEnable();

        MultiplayerBoxManager.Current.Add(ID, this.gameObject);
    }

    public override void OnDisable ()
    {
        base.OnDisable();
        MultiplayerBoxManager.Current.Remove(ID);
    }
}
