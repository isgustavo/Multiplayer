using Mirror;
using UnityEngine;

public class MultiplayerPoolID : MonoBehaviour
{
    public uint ID;
    public uint OwnerID;

    public ObjectTickState LastObjectStateReceived = new ObjectTickState();
    public ObjectState ObjectState { get; protected set; } = new ObjectState();

    public virtual void OnDisable ()
    {
        MultiplayerObjectGameManager.Current.RemoveObject(ID);
    }
}
