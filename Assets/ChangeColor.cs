using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ChangeColor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public SpriteRenderer palleteSR;
    public Color colorSelect;
    private bool isHolding = false;

    private void Start()
    {
        colorSelect = GetComponent<Image>().color;
    }
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        Debug.Log("sugh");
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
           
                AdjustColor(0.999f);
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