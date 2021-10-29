using UnityEngine;
using Mirror;
using System;

[RequireComponent( typeof (NetworkIdentity))]
public class Player : MonoBehaviour
{
    public static Player LocalPlayer;

    public NetworkIdentity NetworkIdentity { get; private set; }

    public PlayerInput PlayerInput { get; private set; } = new PlayerInput();
    public PlayerCharacter PlayerCharacter { get; private set; }
    public  CharacterCamera CharacterCamera { get; private set; }

    private void Awake ()
    {
        NetworkIdentity = transform.GetComponent<NetworkIdentity>();
    }

    public float waitInput = 2f;
    public float current = 0f;

    public virtual void Start ()
    {
        PlayerCharacter = transform.GetComponentInChildren<PlayerCharacter>();
        PlayerGameManager.Current.AddPlayer(NetworkIdentity.netId, this);

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
            PlayerInput.ReadInput();

        PlayerCharacter.UpdateCharacter();
        PlayerInput.ClearInput();
    }

    private void LateUpdate ()
    {
        //if (NetworkIdentity.isLocalPlayer == true)
        //PlayerInput.ClearInput();
    }

    public bool IsLocalPlayer => NetworkIdentity.isLocalPlayer;
}
