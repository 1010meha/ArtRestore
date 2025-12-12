using UnityEngine;

public class ChangeBrushSize : MonoBehaviour
{
    public float radius;
    public PaintColor colorScript;
   
    private const float MIN_BRUSH_SIZE = 5f;
    private const float MAX_BRUSH_SIZE = 50f;
    private const float MIN_SCALE = 0.05f;
    private const float MAX_SCALE = 0.46f;
    private int lastBrushSize = -1;

    void Update()
    {
        if (colorScript == null) return;

        // Only update if brush size changed
        if (colorScript.brushSize != lastBrushSize)
        {
            lastBrushSize = colorScript.brushSize;
            float t = Mathf.InverseLerp(MIN_BRUSH_SIZE, MAX_BRUSH_SIZE, colorScript.brushSize);
            radius = Mathf.Lerp(MIN_SCALE, MAX_SCALE, t);
            transform.localScale = new Vector3(radius, radius, radius);
        }
    }
}
