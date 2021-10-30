using Mirror;
using UnityEngine;

public class MultiplayerPoolID : MonoBehaviour
{
    public uint ID;

    public ObjectTickState LastObjectStateReceived = new ObjectTickState();
    public ObjectState ObjectState { get; private set; } = new ObjectState();

    private void OnEnable ()
    {
        //UIConsole.Current.AddConsole($"NonPlayer OnEnable NetworkIdentity.sceneId = {multiplayerPoolID.ID} ");

        if (NetworkServer.active)
            MultiplayerObjectGameManager.Current.AddObject(ID, this);
        else
        {
            ObjectState = new ObjectState();

            LastObjectStateReceived.position = transform.position;
            LastObjectStateReceived.rotation = transform.rotation;
        }
    }

    private void OnDisable ()
    {
        MultiplayerObjectGameManager.Current.RemoveObject(ID);
    }
}
