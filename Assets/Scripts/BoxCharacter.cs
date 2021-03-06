using System.Collections;
using Mirror;
using UnityEngine;

public class BoxCharacter : Character
{
    public NonPlayer Owner { get; private set; }

    public override void Awake ()
    {
        base.Awake();

        Owner = GetComponent<NonPlayer>();
    }

    public override void OnCollision (Collider collider)
    {
        base.OnCollision(collider);

        IProjectile projectile = collider.GetComponent<IProjectile>();
        if (projectile == null)
            return;

        if (NetworkServer.active == true)
        {
            TakeDamage(projectile.GetDamage(), projectile.GetOnwerId());

            projectile.ForceDespawn();
        }
    }

    protected override void Dead ()
    {
        base.Dead();

        StartCoroutine(WaitToDespawn());

        //if (NetworkServer.active == true)
        //{
        //    //SpawnNewWeapon();
        //}
    }

    IEnumerator WaitToDespawn ()
    {
        yield return new WaitForEndOfFrame();
        MultiplayerGamePoolManager.Current.Despawn(Owner);
    }

    //void SpawnNewWeapon()
    //{
    //    //UIConsole.Current.AddConsole($"SpawnProjectile");
    //    string weapon = MultiplayerWeaponManager.Current.GetRandonWeapon();
    //    MultiplayerPoolID obj = MultiplayerGamePoolManager.Current.SpawnOnServer(weapon);
    //    //obj.OwnerID = Context.Owner.NetworkIdentity.netId;
    //    obj.transform.position = transform.position;

    //    obj.gameObject.SetActive(true);
    //}

}
