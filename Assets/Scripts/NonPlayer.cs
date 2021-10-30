using UnityEngine;
using Mirror;

public class NonPlayer : MonoBehaviour
{ 
    public MultiplayerPoolID MultiplayerPoolID { get; protected set; }

    public virtual void Awake ()
    {
        MultiplayerPoolID = GetComponent<MultiplayerPoolID>();
    }

    public virtual void Update ()
    {
        MultiplayerPoolID.ObjectState.Tick();

        if (NetworkServer.active)
            return;

        SyncClientPosition();
        
    }

    public void SyncClientPosition ()
    {
        uint lastTickIndex = MultiplayerPoolID.LastObjectStateReceived.tick % 1024;
        Vector3 positionDiference = MultiplayerPoolID.LastObjectStateReceived.position - MultiplayerPoolID.ObjectState.ObjectStateBuffer[lastTickIndex].position;
        if (positionDiference.sqrMagnitude > 0.00001f)
        {
            uint rewindTick = MultiplayerPoolID.LastObjectStateReceived.tick;
            while (rewindTick < MultiplayerPoolID.ObjectState.ObjectTick)
            {
                lastTickIndex = rewindTick % 1024;
                MultiplayerPoolID.ObjectState.ObjectStateBuffer[lastTickIndex].position = transform.position;
                MultiplayerPoolID.ObjectState.ObjectStateBuffer[lastTickIndex].rotation = transform.rotation;

                rewindTick += 1;
            }
        }

        transform.position = MultiplayerPoolID.LastObjectStateReceived.position;
        transform.rotation = MultiplayerPoolID.LastObjectStateReceived.rotation;
    }

}
