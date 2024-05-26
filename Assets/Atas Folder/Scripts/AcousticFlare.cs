using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class AcousticFlare : MonoBehaviour
{
    public Image markerImage;
    public Transform target;
    public Text meter;

    private void Update()
    {
        float minX = markerImage.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;
        float minY = markerImage.GetPixelAdjustedRect().height / 2;
        float maxY = Screen.height - minX;

        Vector2 pos = Camera.main.WorldToScreenPoint(target.position);

        if(Vector3.Dot((target.position - transform.position), transform.forward) < 0)
        {
            if (pos.x < Screen.width / 2)
                pos.x = maxX;
            else
                pos.x = minX;
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        
        markerImage.transform.position = pos;

        meter.text = ((int)Vector3.Distance(target.position, transform.position)).ToString();
    }
}