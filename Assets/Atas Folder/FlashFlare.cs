using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashFlare : MonoBehaviour
{
    private float activeTime = 3000f;
    public string innerText = "Marker";
    public RectTransform rectTransform;

    public void Activate()
    {
        if (rectTransform == null)
            return;

        GameObject markerObject = new GameObject("WorldSpaceCanvas");
        Canvas canvas = markerObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        // CanvasScaler canvasScaler = 
    }
}
