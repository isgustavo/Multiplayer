using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBillboard : MonoBehaviour
{
    private void OnEnable ()
    {
        if (Player.LocalPlayer == null)
            gameObject.SetActive(false);
    }

    void Update()
    {
        transform.forward = -Player.LocalPlayer.CharacterCamera.Cam.transform.forward;
    }
}
