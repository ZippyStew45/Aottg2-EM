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

public class AcousticFlare : MonoBehaviourPun, IPunObservable
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

    private float minX;
    private float minY;
    private float timeLeft = 180f;

    public void Setup(Transform _transform, Player _player)
    {
        PhotonView photonView = GetComponent<PhotonView>();
        Color _color = GenerateRandomColor();
        if (photonView.IsMine)
            photonView.RPC("SetupRPC", RpcTarget.AllBuffered, new object[] { _transform.position, _transform.rotation, _transform.localScale, _player, _color });
    }

    [PunRPC]
    private void SetupRPC(Vector3 _position, Quaternion _rotation, Vector3 _scale, Player _player, Color _color, PhotonMessageInfo info)
    {
        uiTransform = gameObject.GetComponent<Transform>();

        uiTransform.transform.position = _position;
        uiTransform.transform.rotation = _rotation;
        uiTransform.transform.localScale = _scale;


        minX = 0;
        minY = 0;
        ownerName.text = _player.GetStringProperty(PlayerProperty.Name);
        ownerName.text = RemoveColorTagsFromName(ownerName.text);
        bannerImage.color = _color;

        flareSound.Play();
        
        if ((int)Vector3.Distance(uiTransform.position, SceneLoader.CurrentCamera.Camera.transform.position) < 250)
        {
            ringingSound.Play();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
{
    if (stream.IsWriting)
    {
        stream.SendNext(this.uiTransform.position);
        stream.SendNext(this.uiTransform.rotation);
        stream.SendNext(this.uiTransform.localScale);
        stream.SendNext(new Vector4(bannerImage.color.r, bannerImage.color.g, bannerImage.color.b, bannerImage.color.a));
        stream.SendNext(this.ownerName.text);
        stream.SendNext(this.distance != null ? this.distance.text : "");
    }
    else
    {
        uiTransform.position = (Vector3)stream.ReceiveNext();
        uiTransform.rotation = (Quaternion)stream.ReceiveNext();
        uiTransform.localScale = (Vector3)stream.ReceiveNext();

        Vector4 colorVector = (Vector4)stream.ReceiveNext();
        bannerImage.color = new Color(colorVector.x, colorVector.y, colorVector.z, colorVector.w);

        ownerName.text = (string)stream.ReceiveNext();
        if (distance != null)
        {
            distance.text = (string)stream.ReceiveNext();
        }
        else
        {
            Debug.LogWarning("Distance text component is null");
        }

        ownerName.text = RemoveColorTagsFromName(ownerName.text);

        minX = 0;
        minY = 0;
    }
}

    private void ChangeCanvasLocation()
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

        if (SceneLoader.CurrentCamera.Camera != null && distance != null)
            distance.text = "-" + ((int)Vector3.Distance(uiTransform.position, SceneLoader.CurrentCamera.Camera.transform.position)).ToString() + "U-";
    }

    private void ScaleUIElements()
    {
        float _distanceValue = Vector3.Distance(uiTransform.position, SceneLoader.CurrentCamera.Camera.transform.position);

        if (_distanceValue > 40f && _distanceValue < 10000f)
        {
            float scale = 50f / _distanceValue;
            marker.transform.localScale = new Vector2( Mathf.Clamp(scale, 0.25f, .75f), Mathf.Clamp(scale, 0.25f, .75f));
        }
        else 
            marker.transform.localScale = new Vector2( .5f, .5f);
    }

    private void ScaleOpacity()
    {
        float _distanceValue = Vector3.Distance(uiTransform.position, SceneLoader.CurrentCamera.Camera.transform.position);
        if (_distanceValue > 40f && _distanceValue <= 10000f)
        {
            float scale = _distanceValue / 600f;
            ApplyOpacity( Mathf.Clamp(scale, .2f, .7f));
        }
        else
        {
            if (_distanceValue > 10000f)
                ApplyOpacity(0);
            else
                ApplyOpacity(.15f);
        }
    }

    private void ApplyOpacity(float opacity)
    {
        foreach (Transform child in marker.transform)
        {
            RawImage _rawImage = child.gameObject.GetComponent<RawImage>();
            Text _text = child.gameObject.GetComponent<Text>();

            if (_text != null)
            {
                _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, opacity);
            }
            if (_rawImage != null)
            {
                _rawImage.color = new Color(_rawImage.color.r, _rawImage.color.g, _rawImage.color.b, opacity);
            }
        }
    }

    private string RemoveColorTagsFromName(string playerName)
    {
        string pattern = @"<color=[^>]+>|</color>";
        string cleanName = System.Text.RegularExpressions.Regex.Replace(playerName, pattern, string.Empty);
        return cleanName;
    }

    private Color GenerateRandomColor()
    {
        float red = UnityEngine.Random.Range(.01f, .99f);  
        float green = UnityEngine.Random.Range(.01f, .99f);
        float blue = UnityEngine.Random.Range(.01f, .99f); 

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

        if ( SceneLoader.CurrentCamera.Camera != null && uiTransform != null )
        {
            ChangeCanvasLocation();
            ScaleUIElements();
            ScaleOpacity();
        }
    }
}