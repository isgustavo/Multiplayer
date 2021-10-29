using UnityEngine;
using Mirror;

//[RequireComponent(typeof(NetworkIdentity))]
//public class PlayerOnClient : MonoBehaviour
//{
//    public NetworkIdentity NetworkIdentity { get; private set; }
//    public PlayerInput PlayerInput { get; private set; }
//    public PlayerCharacter PlayerCharacter { get; private set; }

//    CharacterCamera characterCamera;

//    public virtual void Awake ()
//    {
//        NetworkIdentity = transform.GetComponent<NetworkIdentity>();

//        PlayerInput = new PlayerInput();
//        PlayerCharacter = transform.GetComponentInChildren<PlayerCharacter>();

//        characterCamera = transform.GetComponentInChildren<CharacterCamera>(true);
//        if (characterCamera == false)
//        {
//            Debug.Log("Player without characterCamera");
//        }

//        NetworkClient.RegisterHandler<PlayerStateMessage>(OnPlayerStateReceived); 
//    }

//    public virtual void Start ()
//    {
//        if (NetworkIdentity.isLocalPlayer == true)
//            characterCamera.StartCamera();
//    }

//    private void Update ()
//    {
//        if (NetworkIdentity.isLocalPlayer == true)
//            PlayerInput.ReadInput();

//        PlayerCharacter.UpdateCharacter();
//    }

//    private void LateUpdate ()
//    {
//        if (NetworkIdentity.isLocalPlayer == true)
//            PlayerInput.ClearInput();
//    }

//    private void OnPlayerStateReceived (PlayerStateMessage message)
//    {
//        PlayerInput.LastInputReceived.tick = message.tick;
//        PlayerInput.LastInputReceived.position = message.position;
//        PlayerInput.LastInputReceived.rotation = message.rotation;
//    }


//}
