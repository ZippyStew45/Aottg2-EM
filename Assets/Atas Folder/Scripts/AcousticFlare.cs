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

    private void Start()
    {
        //Debug.Log(Camera.main.pixelWidth + " " + Camera.main.pixelHeight);
    }

    public void Setup(Transform _transform)
    {
        uiTransform = _transform;
        _human = PhotonExtensions.GetMyHuman().gameObject.GetComponent<Human>();
    }

    private void ChangeCanvasLocation()
    {
        markerImage.transform.position = new Vector3(100f, 100f, 1);
    }

    private void Update()
    {
        ChangeCanvasLocation();
    }
}