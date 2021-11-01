using UnityEngine;
using TMPro;
using System;

public class UIClientController : MonoBehaviour
{
    Transform inGameGroup;
    Transform inDeadGroup;

    TMP_Text playerText;
    TMP_Text scoreText;

    private void Awake ()
    {
        inGameGroup = transform.Find("UIGroup");

        playerText = inGameGroup.Find("PlayerName").GetComponent<TMP_Text>();
        scoreText = inGameGroup.Find("ScoreValue").GetComponent<TMP_Text>();

        inDeadGroup = transform.Find("UIDead");
    }

    public void Start ()
    {
        MultiplayerGameManager.Current.OnClientStarted += OnClientStarted;
    }

    private void OnDisable ()
    {
        if (Player.LocalPlayer == null)
            return;

        Player.LocalPlayer.OnCurrentPointChanged -= OnCurrentPointChanged;
        Player.LocalPlayer.PlayerCharacter.OnDeadChanged -= OnDeadChanged;
    }

    private void OnClientStarted ()
    {
        MultiplayerGameManager.Current.OnClientStarted -= OnClientStarted;

        if (Player.LocalPlayer == null)
            Player.OnLocalPlayerChanged += SetPlayerUI;
        else
            SetPlayerUI();

    }

    public void SetPlayerUI()
    {
        Player.LocalPlayer.OnCurrentPointChanged += OnCurrentPointChanged;
        Player.LocalPlayer.PlayerCharacter.OnDeadChanged += OnDeadChanged;

        playerText.text = $"Player{Player.LocalPlayer.NetworkIdentity.netId.ToString("00")}";
        scoreText.text = "00";

        inGameGroup.gameObject.SetActive(true);
    }

    private void OnCurrentPointChanged ()
    {
        scoreText.text = Player.LocalPlayer.CurrentPoints.ToString("00");
    }

    private void OnDeadChanged()
    {
        inGameGroup.gameObject.SetActive(false);
        inDeadGroup.gameObject.SetActive(true);
    }
}
