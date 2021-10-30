using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using System;


public class UIServerController : MonoBehaviour
{
    Transform group;

    TMP_InputField latencyText;
    TMP_InputField packageLossText;

    Button changeValuesButton;
    private void Awake ()
    {
        group = transform.Find("UIServerGroup");

        latencyText = group.Find("LatencyValue").GetComponent<TMP_InputField>();
        packageLossText = group.Find("PackageLossValue").GetComponent<TMP_InputField>();

        changeValuesButton = group.Find("ChangeValuesButton").GetComponent<Button>();
        changeValuesButton.onClick.AddListener(OnClick);

    }

    public void Start ()
    {
        MultiplayerGameManager.Current.OnServerConnected += OnServerConnected;
        group.gameObject.SetActive(false);
    }

    private void OnServerConnected ()
    {
        MultiplayerGameManager.Current.OnServerConnected -= OnServerConnected;
        latencyText.text = MultiplayerSimulationGameManager.Current.Simulation.LatencyInSecond.ToString();
        packageLossText.text = MultiplayerSimulationGameManager.Current.Simulation.PackageLoss.ToString();

        group.gameObject.SetActive(true);

    }

    private void OnDisable ()
    {
        changeValuesButton.onClick.RemoveListener(OnClick);
    }

    private void OnClick ()
    {
        float latencyValue = 0;
        if (float.TryParse(latencyText.text, out latencyValue))
            MultiplayerSimulationGameManager.Current.Simulation.LatencyInSecond = latencyValue;
        else
            latencyText.text = MultiplayerSimulationGameManager.Current.Simulation.LatencyInSecond.ToString();

        int packageLossValue = 0;
        if (int.TryParse(packageLossText.text, out packageLossValue))
            MultiplayerSimulationGameManager.Current.Simulation.PackageLoss = packageLossValue;
        else
            packageLossText.text = MultiplayerSimulationGameManager.Current.Simulation.PackageLoss.ToString();
    }
}
