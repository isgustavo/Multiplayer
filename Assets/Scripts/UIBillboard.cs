using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBillboard : MonoBehaviour
{

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Player.LocalPlayer.CharacterCamera.Cam.transform.position);
    }
}
