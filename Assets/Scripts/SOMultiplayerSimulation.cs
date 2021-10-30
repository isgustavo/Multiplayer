using UnityEngine;

[CreateAssetMenu(fileName = "Simulation", menuName = "Multiplayer/MultiplayerSimulation")]
public class SOMultiplayerSimulation : ScriptableObject
{
    public float LatencyInSecond = 0;
    public int PackageLoss = 5;
}
