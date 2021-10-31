using UnityEngine;
using Mirror;

public interface IProjectile
{
    int GetDamage ();
    uint GetOnwerId ();
    void ForceDespawn ();
}

public class ProjectileLogic : MonoBehaviour, IProjectile, ICharacterLogic
{
    public static int COLLIDER_LAYER = 1 << 10;
    NonPlayer poolID;

    [SerializeField]
    SOMultiplayerProjectile projectileStats;

    float currentLifetime;

    private void Awake ()
    {
        poolID = GetComponent<NonPlayer>();
    }

    private void OnEnable ()
    {
        currentLifetime = 0f;
    }

    private void Update ()
    {
        if (currentLifetime > projectileStats.Lifetime)
            ForceDespawn();

        currentLifetime += Time.deltaTime;

        transform.Translate(transform.forward * projectileStats.Speed * Time.deltaTime, Space.World);
    }

    public int GetDamage ()
    {
        return projectileStats.Damage;
    }

    public uint GetOnwerId ()
    {
        return poolID.OwnerID;
    }

    public void ForceDespawn ()
    {
        MultiplayerGamePoolManager.Current.Despawn(poolID);
    }

    public Transform GetTransform ()
    {
        return transform;
    }

    public int GetLife ()
    {
        return 1;
    }

    public void SetLife (int life)
    {
    }
}
