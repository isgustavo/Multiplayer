using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Multiplayer/MultiplayerCharacter", order = 1)]
public class SOMultiplayerCharacter : ScriptableObject
{
    public float MoveSpeed = 5;
    public float RotateSpeed = 5;
    public int MaxLive = 5;
}
