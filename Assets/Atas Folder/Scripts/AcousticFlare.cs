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

public class AcousticFlare : MonoBehaviour
{
    [SerializeField]
    private GameObject marker;

    [SerializeField]
    private RawImage bannerImage;
    [SerializeField]
    private Canvas canvas;

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
    private float minY;

    readonly static private float offset1 = 50f;
    readonly static private float offset2 = 80f;
    private float _offset1 = offset1;
    private float _offset2 = offset2;
    private float timeLeft = 180f;
    Player _owner;

    public void Setup(Transform _transform, Player _player)
    {
        PhotonView photonView = GetComponent<PhotonView>();
        photonView.RPC("SetupRPC", RpcTarget.All, new object[] { _transform.position, _transform.rotation, _transform.localScale, _player });
    }

    [PunRPC]
    private void SetupRPC(Vector3 _position, Quaternion _rotation, Vector3 _scale, Player _player, PhotonMessageInfo info)
    {
        if (uiTransform == null)
        {
            gameObject.AddComponent<Transform>();
        }

        uiTransform = gameObject.GetComponent<Transform>();

        uiTransform.transform.position = _position;
        uiTransform.transform.rotation = _rotation;
        uiTransform.transform.localScale = _scale;

        _human = PhotonExtensions.GetMyHuman().gameObject.GetComponent<Human>();
        _camera = FindFirstObjectByType<Camera>();

        minX = 0;
        minY = 0;
        ownerName.text = _player.GetStringProperty(PlayerProperty.Name);
        bannerImage.color = GenerateRandomColor();

        flareSound.Play();
        if ((int)Vector3.Distance(uiTransform.position, _human.transform.position) < 250)
        {
            ringingSound.Play();
        }
    }

    private void ChangeCanvasLocation() // debugging was worthless so pushing this again to see if it fixes anything with a pull //
    {
        Vector3 pos = SceneLoader.CurrentCamera.Camera.WorldToViewportPoint(uiTransform.position);

        pos.x *= canvas.pixelRect.width;
        pos.y *= canvas.pixelRect.height;

        if (Vector3.Dot((uiTransform.position - SceneLoader.CurrentCamera.Camera.transform.position), SceneLoader.CurrentCamera.Camera.transform.forward) < 0)
        {
            if (pos.x < Screen.width / 2)
            {
                pos.x = Screen.width - minX;
            }
            else 
            {
                pos.x = minX;
            }
        }

        pos.x = Mathf.Clamp(pos.x, minX, Screen.width - minX);
        pos.y = Mathf.Clamp(pos.y, minY, Screen.height - minY);

        marker.transform.position = pos;

        
        distance.text = "-" + ((int)Vector3.Distance(uiTransform.position, _human.transform.position)).ToString() + "U-";
    }

    private void ScaleUIElements() // optionally, add scaling for when player is too close (looks fine to me for now)
    {
        float _distanceValue = Vector3.Distance(uiTransform.position, _human.transform.position);
        if (_distanceValue == 0)
            return;

        float scale = 250f / _distanceValue;

        marker.transform.localScale = new Vector2( Mathf.Clamp(scale, 0.25f, 1f), Mathf.Clamp(scale, 0.25f, 1f));
    }

    private Color GenerateRandomColor()
    {
        float red = UnityEngine.Random.Range(0f, 1f);  
        float green = UnityEngine.Random.Range(0f, 1f);
        float blue = UnityEngine.Random.Range(0f, 1f); 

        return new Color(red, green, blue, .5f);
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

        if ( SceneLoader.CurrentCamera.Camera != null && uiTransform != null)
        {
            ChangeCanvasLocation();
            ScaleUIElements();
        }
    }
}