using System.Collections;
using System.Collections.Generic;
using Characters;
using GameManagers;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class AcousticFlare : MonoBehaviour
{
    [SerializeField]
    private Image markerImage;

    [SerializeField]
    private Image bannerImage;

    [SerializeField]
    Text ownerName;

    [SerializeField]
    Text distance;

    [SerializeField]
    private AudioSource ringingSound;
    [SerializeField]
    private AudioSource flareSound;

    private Transform uiTransform;
    private Human _human; // gonna use this for distance calculation
    Camera _camera;

    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    readonly static private float offset1 = 50f;
    readonly static private float offset2 = 80f;
    private float _offset1 = offset1;
    private float _offset2 = offset2;
    private float timeLeft = 180f;

    public void Setup(Transform _transform, Player _player)
    {
        uiTransform = _transform;
        _human = PhotonExtensions.GetMyHuman().gameObject.GetComponent<Human>();

        PhotonView photonView = GetComponent<PhotonView>();
        photonView.RPC("Initialize", RpcTarget.All, _player);
    }

    [PunRPC]
    private void Initialize(Player PhotonPlayer)
    {
        _camera = FindFirstObjectByType<Camera>();

        minX = markerImage.GetPixelAdjustedRect().width / 2;
        maxX = Screen.width - minX;
        minY = markerImage.GetPixelAdjustedRect().width / 2;
        maxY = Screen.height - minY;
        ownerName.text = PhotonPlayer.GetStringProperty(PlayerProperty.Name);
        bannerImage.color = GenerateRandomColor();

        flareSound.Play();
        if ((int)Vector3.Distance(uiTransform.position, _human.transform.position) < 250)
        {
            ringingSound.Play();
        }
    }

    private void ChangeCanvasLocation()
    {
        Vector2 pos = _camera.WorldToScreenPoint(uiTransform.position);

        if (Vector3.Dot((uiTransform.position - _camera.transform.position), _camera.transform.forward) < 0)
        {
            if (pos.x < Screen.width / 2)
            {
                pos.x = maxX;
            }
            else 
            {
                pos.x = minX;
            }
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        markerImage.transform.position = pos;
        bannerImage.transform.position = new Vector2(pos.x, pos.y - _offset1);
        ownerName.transform.position = new Vector2(pos.x, pos.y - _offset1);
        distance.transform.position = new Vector2(pos.x, pos.y - _offset2);
        distance.text = "-" + ((int)Vector3.Distance(uiTransform.position, _human.transform.position)).ToString() + "U-";
    }

    private void ScaleUIElements() // optionally, add scaling for when player is too close (looks fine to me for now)
    {
        float _distanceValue = Vector3.Distance(uiTransform.position, _human.transform.position);
        if (_distanceValue == 0)
            return;

        float scale = 250f / _distanceValue;

        markerImage.transform.localScale = new Vector2( Mathf.Clamp(scale, 0.25f, 1f), Mathf.Clamp(scale, 0.25f, 1f));
        bannerImage.transform.localScale = new Vector2( Mathf.Clamp(scale, 0.25f, 1f), Mathf.Clamp(scale, 0.25f, 1f));
        Debug.Log("Scale: " + scale);
        ownerName.fontSize = (int)(Mathf.Clamp(scale, .25f, 1f) * 30);
        distance.fontSize = (int)(Mathf.Clamp(scale, .25f, 1f) * 20);

        _offset1 = Mathf.Clamp(scale, .25f, 1f) * offset1;
        _offset2 = Mathf.Clamp(scale, .25f, 1f) * offset2;
    }

    private Color GenerateRandomColor()
    {
        float red = UnityEngine.Random.Range(0f, 1f);  
        float green = UnityEngine.Random.Range(0f, 1f);
        float blue = UnityEngine.Random.Range(0f, 1f); 

        return new Color(red, green, blue);
    }

    private void DestorySelf()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
            DestorySelf();

        if ( _camera != null && uiTransform != null)
        {
            ChangeCanvasLocation();
            ScaleUIElements();
        }
    }
}