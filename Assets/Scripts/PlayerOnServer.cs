using UnityEngine;
using Mirror;
using System;
//public interface IPlayer
//{
//    PlayerCharacter GetPlayerCharacter ();
//}

//[RequireComponent(typeof(NetworkIdentity))]
//public class PlayerOnServer : MonoBehaviour
//{
//    public NetworkIdentity NetworkIdentity { get; private set; }
//    public PlayerInput PlayerInput { get; private set; }
//    public PlayerCharacter PlayerCharacter { get; private set; }

//    private void Awake ()
//    {
//        NetworkIdentity = transform.GetComponent<NetworkIdentity>();

//        PlayerInput = new PlayerInput();
//        PlayerCharacter = transform.GetComponentInChildren<PlayerCharacter>();

//        NetworkServer.RegisterHandler<PlayerInputMessage>(OnPlayerInputReceived);
//    }


//    public virtual void Update ()
//    {
//        if (NetworkIdentity.isServer == true)
//            return;

//         PlayerCharacter.UpdateCharacter();
//    }

//    public virtual void LateUpdate ()
//    {
//        PlayerCharacter.LateUpdateCharacter();

//        if (NetworkIdentity.isServer == true)
//        {
//            PlayerStateMessage inputMessage = new PlayerStateMessage(PlayerInput.PlayerInputTick, PlayerCharacter.Visual.position, PlayerCharacter.Visual.rotation);
//            //NetworkIdentity.connectionToClient.Send<PlayerStateMessage>(inputMessage, Channels.Unreliable);
//            //NetworkServer.SendToAll<PlayerStateMessage>(inputMessage, Channels.Unreliable);
//        }
//    }

//    private void OnPlayerInputReceived (NetworkConnection conn, PlayerInputMessage message)
//    {
//        PlayerInput.ReceivedInput(message.input);
//    }
//}
