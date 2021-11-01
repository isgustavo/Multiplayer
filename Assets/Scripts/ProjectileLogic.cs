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
    NonPlayer projectilePool;

    public SOMultiplayerProjectile ProjectileStats => (SOMultiplayerProjectile)Stats;

    float currentLifetime;

    public override void Awake ()
    {
        base.Awake();

        projectilePool = GetComponent<NonPlayer>();
    }

    public override void OnEnable ()
    {
        base.OnEnable();

        currentLifetime = 0f;
    }

    private void Update ()
    {
        if (currentLifetime > ProjectileStats.Lifetime)
            TakeDamage(Stats.MaxHealth);
        else
        {
            currentLifetime += Time.deltaTime;

            transform.Translate(transform.forward * ProjectileStats.MoveSpeed * Time.deltaTime, Space.World);
        }

    }

    public void ForceDespawn ()
    {
        TakeDamage(Stats.MaxHealth);
    }

    public int GetDamage ()
    {
        return ProjectileStats.Damage;
    }

    public uint GetOnwerId ()
    {
        return projectilePool.OwnerID;
    }

    protected override void SetVisual ()
    {
        base.SetVisual();

        if (Player.LocalPlayer.NetworkIdentity.netId == projectilePool.OwnerID)
        {
            characterRenderer.material = MultiplayerObjectGameManager.Current.MultiplayerObjectMaterial.LocalPlayer;
        }
        else if (MultiplayerObjectGameManager.Current.Players.ContainsKey(projectilePool.OwnerID) == true)
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
        MultiplayerGamePoolManager.Current.Despawn(projectilePool);
    }
}
