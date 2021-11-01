using UnityEngine;

public class CharacterWeapon : MonoBehaviour
{
    [SerializeField]
    SOMultiplayerWeapon stats;
    public SOMultiplayerWeapon Stats => stats;

    public NonPlayer NetworkId { get; private set; }

    [SerializeField]
    public Transform[] spawnPoints;

    private void Awake ()
    {
        NetworkId = GetComponent<NonPlayer>();
    }
}
