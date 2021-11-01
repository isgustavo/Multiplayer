using UnityEngine;
using Mirror;
using System;

public class Player : MonoBehaviour
{
    public static int COLLIDER_LAYER = 1 << 9;

    public static Player LocalPlayer;
    public static event Action OnLocalPlayerChanged;

    public NetworkIdentity NetworkIdentity { get; private set; }

    public ObjectTickState LastSyncObjectReceived = new ObjectTickState();

    public PlayerInput PlayerInput { get; private set; } = new PlayerInput();
    public PlayerState PlayerState { get; private set; } = new PlayerState();

    public PlayerCharacter PlayerCharacter { get; private set; }
    public  CharacterCamera CharacterCamera { get; private set; }

    public Transform Visual => PlayerCharacter.Visual;

    public int currentPoints;
    public int CurrentPoints
    {
        get
        {
            return currentPoints;
        }
        set
        {
            currentPoints = value;
            OnCurrentPointChanged?.Invoke();
        }
    }

    public event Action OnCurrentPointChanged;

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
        OnLocalPlayerChanged?.Invoke();
    }

    private void Update ()
    {
        if (NetworkIdentity.isLocalPlayer == true)
        {
            PlayerInput.ReadInput();
            PlayerState.SendInputToServer(PlayerInput.currentInput, PlayerInput.currentMouse);
        }

        PlayerCharacter.UpdateCharacter();
        PlayerState.Tick();
        PlayerInput.ClearInput();
    }

    public bool IsLocalPlayer => NetworkIdentity.isLocalPlayer;

    public void AddStore(int point)
    {
        currentPoints += point;
    }
}
