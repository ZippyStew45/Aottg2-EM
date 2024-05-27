using System.Collections;
using System.Collections.Generic;
using Characters;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class AcousticFlare : MonoBehaviour
{
    [SerializeField]
    private Image markerImage;
    private Transform uiTransform;
    private Human _human; // gonna use this for distance calculation
    Camera _camera;

    private float minX;
    private float maxX;
    private float minY;
    private float maxY;
    private float timeLeft = 10f;

    private void Start()
    {
        _camera = FindFirstObjectByType<Camera>();

        minX = markerImage.GetPixelAdjustedRect().width / 2;
        maxX = Screen.width - minX;
        minY = markerImage.GetPixelAdjustedRect().width / 2;
        maxY = Screen.height -minY;
    }

    public void Setup(Transform _transform)
    {
        uiTransform = _transform;
        _human = PhotonExtensions.GetMyHuman().gameObject.GetComponent<Human>();
    }

    private void ChangeCanvasLocation()
    {
        Vector2 pos = _camera.WorldToScreenPoint(uiTransform.position);

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        if (Vector3.Dot((uiTransform.position - _human.transform.position), _human.transform.forward) < 0)
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

        markerImage.transform.position = pos;
    }

    private void DestorySelf()
    {
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        timeLeft -= Time.fixedDeltaTime;
        if (timeLeft <= 0)
            DestorySelf();

        if ( _camera != null && uiTransform != null)
            ChangeCanvasLocation();
    }
}