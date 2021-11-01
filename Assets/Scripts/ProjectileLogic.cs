using UnityEngine;
using Mirror;
using System.Collections;

public interface IProjectile
{
    int GetDamage ();
    uint GetOnwerId ();
    void ForceDespawn ();
}

public class ProjectileLogic : Character, IProjectile
{
    public static int COLLIDER_LAYER = 1 << 10;
    NonPlayer nonPlayer;

    public SOMultiplayerProjectile ProjectileStats => (SOMultiplayerProjectile)Stats;

    float currentLifetime;

    public override void Awake ()
    {
        base.Awake();

        nonPlayer = GetComponent<NonPlayer>();
    }

    public override void OnEnable ()
    {
        base.OnEnable();

        currentLifetime = 0f;
    }

    private void Update ()
    {
        if (currentLifetime > ProjectileStats.Lifetime)
            TakeDamage(Stats.MaxHealth, nonPlayer.ID);
        else
        {
            currentLifetime += Time.deltaTime;

            transform.Translate(transform.forward * ProjectileStats.MoveSpeed * Time.deltaTime, Space.World);
        }

    }

    public void ForceDespawn ()
    {
        TakeDamage(Stats.MaxHealth, nonPlayer.ID);
    }

    public int GetDamage ()
    {
        return ProjectileStats.Damage;
    }

    public uint GetOnwerId ()
    {
        return nonPlayer.OwnerID;
    }

    protected override void SetVisual ()
    {
        base.SetVisual();

        if (Player.LocalPlayer.NetworkIdentity.netId == nonPlayer.OwnerID)
        {
            characterRenderer.material = MultiplayerObjectGameManager.Current.MultiplayerObjectMaterial.LocalPlayer;
        }
        else if (MultiplayerObjectGameManager.Current.Players.ContainsKey(nonPlayer.OwnerID) == true)
        {
            characterRenderer.material = MultiplayerObjectGameManager.Current.MultiplayerObjectMaterial.OtherPlayer;
        }
        else
        {
            characterRenderer.material = MultiplayerObjectGameManager.Current.MultiplayerObjectMaterial.BotPlayer;
        }
    }

    protected override void Dead ()
    {
        base.Dead();

        StartCoroutine(WaitToDespawn());
    }

    IEnumerator WaitToDespawn ()
    {
        yield return new WaitForEndOfFrame();
        MultiplayerGamePoolManager.Current.Despawn(nonPlayer);
    }
}
