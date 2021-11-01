using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "Multiplayer/MultiplayerProjectile", order = 1)]
public class SOMultiplayerProjectile : SOMultiplayerCharacter
{
    //public float MoveSpeed;
    public int Lifetime = 5;
    public int Damage = 1;
}
