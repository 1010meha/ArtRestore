using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI;
using System;

public class ChangeColor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public SpriteRenderer palleteSR;
    public Color colorSelect;
    private bool isHolding = false;
    private float mouseStartPos;
    public float amtToAdjustColor;

    private void Start()
    {
        colorSelect = GetComponent<Image>().color;
    }
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        mouseStartPos = Input.mousePosition.x;
        isHolding = true;
        StartCoroutine(DetectHold());

    }

    //Detect if clicks are no longer registering
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        isHolding = false;
        StopAllCoroutines();
    }

    private IEnumerator DetectHold()
    {
        while (isHolding)
        {
           

            // Normalize the input value to a 0-1 range
            float t = Mathf.InverseLerp(0 , 1000f, Mathf.Abs(mouseStartPos - Input.mousePosition.x));

            // Linearly interpolate the 0-1 value to the output range
             amtToAdjustColor = Mathf.Lerp(.999f, .9f, t);

                AdjustColor(amtToAdjustColor);
            yield return null; // Wait for the next frame
        }
    }

    private void Update()
    {
        
    }

    void AdjustColor(float percentChange)
    {
        Color palleteColor = palleteSR.color;
        if (palleteColor == Color.white)
        {
            palleteSR.color = colorSelect;
        }
        else
        {
            Color averageColor = Color.Lerp(colorSelect, palleteColor, percentChange);
            Color daColor = averageColor;
            palleteSR.color = daColor;
        }
    }
}