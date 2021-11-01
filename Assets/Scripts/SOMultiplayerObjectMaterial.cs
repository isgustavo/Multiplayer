using UnityEngine;

[CreateAssetMenu(fileName = "Materials", menuName = "Multiplayer/MultiplayerMaterials")]
public class SOMultiplayerObjectMaterial : ScriptableObject
{
   public Material LocalPlayer;
   public Material OtherPlayer;
   public Material BotPlayer;
}
