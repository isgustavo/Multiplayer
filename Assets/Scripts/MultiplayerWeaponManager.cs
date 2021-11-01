using System.Collections.Generic;
using UnityEngine;

public class MultiplayerWeaponManager : MonoBehaviour
{
    public static MultiplayerWeaponManager Current;

    [SerializeField]
    List<GameObject> WeaponAvailables = new List<GameObject>();

    private void Awake ()
    {
        if (Current == null)
            Current = this;
        else
            Destroy(this);
    }

    public string GetRandonWeapon ()
    {
        return WeaponAvailables[UnityEngine.Random.Range(0, WeaponAvailables.Count - 1)].gameObject.name;
    }
}
