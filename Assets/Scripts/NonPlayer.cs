using UnityEngine;
using Mirror;

public class NonPlayer : MonoBehaviour
{ 
    MultiplayerPoolID multiplayerPoolID;

    public virtual void Awake ()
    {
        multiplayerPoolID = GetComponent<MultiplayerPoolID>();
    }

    private void Update ()
    {
        if (NetworkServer.active)
            return;

        SyncClientPosition();
        multiplayerPoolID.ObjectState.Tick();
    }

    public void SyncClientPosition ()
    {
        uint lastTickIndex = multiplayerPoolID.LastObjectStateReceived.tick % 1024;
        Vector3 positionDiference = multiplayerPoolID.LastObjectStateReceived.position - multiplayerPoolID.ObjectState.ObjectStateBuffer[lastTickIndex].position;
        if (positionDiference.sqrMagnitude > 0.00001f)
        {
            uint rewindTick = multiplayerPoolID.LastObjectStateReceived.tick;
            while (rewindTick < multiplayerPoolID.ObjectState.ObjectTick)
            {
                lastTickIndex = rewindTick % 1024;
                multiplayerPoolID.ObjectState.ObjectStateBuffer[lastTickIndex].position = transform.position;
                multiplayerPoolID.ObjectState.ObjectStateBuffer[lastTickIndex].rotation = transform.rotation;

                rewindTick += 1;
            }

            transform.position = multiplayerPoolID.LastObjectStateReceived.position;
            transform.rotation = multiplayerPoolID.LastObjectStateReceived.rotation;
        }
    }

}
