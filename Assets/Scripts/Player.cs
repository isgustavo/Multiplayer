using UnityEngine;
using Mirror;
using System;


//public class SyncObject : MonoBehaviour
//{
//    public SyncObjectState LastSyncObjectReceived = new SyncObjectState();

//    public virtual uint NetID => 0;
//    public virtual Transform Visual => transform;
//}

public class ObjectState
{
    public uint ObjectTick { get; set; }
    public ObjectTickState[] ObjectStateBuffer { get; private set; } = new ObjectTickState[1024];

    public ObjectState ()
    {
        for (int i = 0; i < 1024; i++)
        {
            ObjectStateBuffer[i] = new ObjectTickState();
        }
    }

    public void ReceivedTick(uint tick)
    {
        ObjectTick = tick;
    }

    public void Tick()
    {
        ObjectTick++;
    }
}

public class PlayerState : ObjectState
{
    public byte[] ObjectInputBuffer { get; private set; } = new byte[1024];

    internal void SendInputToServer (byte input)
    {
        PlayerInputMessage inputMessage = new PlayerInputMessage(ObjectTick, input);
        NetworkClient.Send<PlayerInputMessage>(inputMessage, Channels.Unreliable);

        uint bufferIndex = ObjectTick % 1024;
        ObjectInputBuffer[bufferIndex] = input;
        ObjectStateBuffer[bufferIndex].position = Player.LocalPlayer.PlayerCharacter.Visual.position;
        ObjectStateBuffer[bufferIndex].rotation = Player.LocalPlayer.PlayerCharacter.Visual.rotation;
    }
}

public class Player : MonoBehaviour
{
    public static Player LocalPlayer;

    public NetworkIdentity NetworkIdentity { get; private set; }
    public uint NetID => NetworkIdentity.netId;
    public ObjectTickState LastSyncObjectReceived = new ObjectTickState();
    public PlayerInput PlayerInput { get; private set; } = new PlayerInput();
    public PlayerState PlayerState { get; private set; } = new PlayerState();
    public PlayerCharacter PlayerCharacter { get; private set; }
    public  CharacterCamera CharacterCamera { get; private set; }

    public Transform Visual => PlayerCharacter.Visual;

    public virtual void Awake ()
    {
        NetworkIdentity = transform.GetComponent<NetworkIdentity>();
    }

    public virtual void Start ()
    {
        PlayerCharacter = transform.GetComponentInChildren<PlayerCharacter>();
        MultiplayerObjectGameManager.Current.AddPlayer(NetworkIdentity.netId, this);

        if (NetworkIdentity.isLocalPlayer == true)
            PlayerLocalStart();
    }

    void PlayerLocalStart ()
    {
        CharacterCamera = transform.GetComponentInChildren<CharacterCamera>(true);
        if (CharacterCamera == false)
        {
            Debug.Log("Player without characterCamera");
        }

        CharacterCamera.StartCamera();

        LocalPlayer = this;
    }

    private void Update ()
    {
        if (NetworkIdentity.isLocalPlayer == true)
        {
            PlayerInput.ReadInput();
            PlayerState.SendInputToServer(PlayerInput.currentInput);
        }

        PlayerCharacter.UpdateCharacter();
        PlayerState.Tick();
        PlayerInput.ClearInput();
    }

    public bool IsLocalPlayer => NetworkIdentity.isLocalPlayer;
}
