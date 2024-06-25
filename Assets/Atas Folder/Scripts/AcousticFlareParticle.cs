using System.Collections;
using System.Collections.Generic;
using ApplicationManagers;
using Characters;
using GameManagers;
using Photon.Pun;
using Photon.Realtime;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class AcousticFlareParticle : MonoBehaviourPun
{
    private float cooldown = 180f;

    private void FixedUpdate()
    {
        if (cooldown > 0) 
        {
            cooldown -= Time.fixedDeltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}