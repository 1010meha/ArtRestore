using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ChangeColor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Color Settings")]
    public SpriteRenderer paletteSR;
    public Color colorSelect;
    public float colorAdjustmentAmount;

    private const float MAX_MOUSE_DISTANCE = 1000f;
    private const float MIN_COLOR_MIX = 0.9f;
    private const float MAX_COLOR_MIX = 0.999f;

    private bool isHolding = false;
    private float mouseStartPos;

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

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        isHolding = false;
        StopAllCoroutines();
    }

    private IEnumerator DetectHold()
    {
        while (isHolding)
        {
            // Normalize the input value to a 0-1 range based on mouse movement
            float mouseDistance = Mathf.Abs(mouseStartPos - Input.mousePosition.x);
            float t = Mathf.InverseLerp(0f, MAX_MOUSE_DISTANCE, mouseDistance);

            // Linearly interpolate the color mix amount
            colorAdjustmentAmount = Mathf.Lerp(MAX_COLOR_MIX, MIN_COLOR_MIX, t);
            AdjustColor(colorAdjustmentAmount);
            
            yield return null;
        }
    }

    private void AdjustColor(float percentChange)
    {
        Color paletteColor = paletteSR.color;
        if (paletteColor == Color.white)
        {
            paletteSR.color = colorSelect;
        }
        else
        {
            Color mixedColor = Color.Lerp(colorSelect, paletteColor, percentChange);
            paletteSR.color = mixedColor;
        }
    }
}