using UnityEngine;
using TMPro;

public class UIConsole : MonoBehaviour
{
    public static UIConsole Current;

    Transform panel;
    TMP_Text consoleText;

    private void Awake ()
    {
        if (Current == null)
            Current = this;

        panel = transform.Find("Panel");

        consoleText = panel.Find("Console").GetComponent<TMP_Text>();
    }

    public void AddConsole(string text)
    {
        consoleText.text = consoleText.text +"\n"+ text;
    }
}
