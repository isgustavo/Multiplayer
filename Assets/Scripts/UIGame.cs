using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class UIGame : MonoBehaviour
{
    Button createLocalServerButton;
    Button joinClientButton;

    private void Awake ()
    {
        createLocalServerButton = transform.Find("CreateLocalServerButton").GetComponent<Button>();
        createLocalServerButton.onClick.AddListener(OnServerClick);

        joinClientButton = transform.Find("JoinClientButton").GetComponent<Button>();
        joinClientButton.onClick.AddListener(OnJoinClick);
    }

    public void Start ()
    {
        MultiplayerGameManager.Current.OnClientConnected += OnClientConnected;
    }

    private void OnClientConnected ()
    {
        MultiplayerGameManager.Current.OnClientConnected -= OnClientConnected;
        
    }

    private void OnJoinClick ()
    {
        MultiplayerGameManager.Current.StartClient();
        gameObject.SetActive(false);
    }

    private void OnServerClick ()
    {
        MultiplayerGameManager.Current.StartServer();
        gameObject.SetActive(false);
    }
}
