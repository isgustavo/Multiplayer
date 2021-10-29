using UnityEngine;

[CreateAssetMenu(fileName = "Simulation", menuName = "Multiplayer/MultiplayerSimulation", order = 1)]
public class MultiplayerSimulation : ScriptableObject
{
    public float LatencyInSecond = 0;
    public int PackageLoss = 5;
}
