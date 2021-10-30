using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "Multiplayer/MultiplayerProjectile", order = 1)]
public class SOMultiplayerProjectile : ScriptableObject
{
    public float Speed = 5;
    public int Lifetime = 5;
    public int Damage = 1;
}
