using UnityEngine;
using Mirror;

public interface ICharacterLogic
{
    Transform GetTransform ();
    int GetLife ();
    void SetLife (int life);
}

public class NonPlayer : MultiplayerPoolID
{
    //public MultiplayerPoolID MultiplayerPoolID { get; protected set; }

    //public virtual void Awake ()
    //{
    //    MultiplayerPoolID = GetComponent<MultiplayerPoolID>();
    //}

    public ICharacterLogic characterLogic;

    public virtual void Awake()
    {
        characterLogic = GetComponent<ICharacterLogic>();
    }

    public virtual void OnEnable ()
    {
        if (NetworkServer.active)
            MultiplayerObjectGameManager.Current.AddObject(ID, this);
        else
        {
            ObjectState = new ObjectState();

            LastObjectStateReceived.position = transform.position;
            LastObjectStateReceived.rotation = transform.rotation;
        }
    }

    public virtual void Update ()
    {
        ObjectState.Tick();

        if (NetworkServer.active)
            return;

        SyncClientPosition();
        
    }

    public override void OnDisable ()
    {
        base.OnDisable();
    }

    public void SyncClientPosition ()
    {
        uint lastTickIndex = LastObjectStateReceived.tick % 1024;
        Vector3 positionDiference = LastObjectStateReceived.position - ObjectState.ObjectStateBuffer[lastTickIndex].position;
        if (positionDiference.sqrMagnitude > 0.00001f)
        {
            uint rewindTick = LastObjectStateReceived.tick;
            while (rewindTick < ObjectState.ObjectTick)
            {
                lastTickIndex = rewindTick % 1024;
                ObjectState.ObjectStateBuffer[lastTickIndex].position = transform.position;
                ObjectState.ObjectStateBuffer[lastTickIndex].rotation = transform.rotation;

                rewindTick += 1;
            }
        }

        transform.position = LastObjectStateReceived.position;
        transform.rotation = LastObjectStateReceived.rotation;
    }

}
