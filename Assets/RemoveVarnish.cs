using UnityEngine;

public class RemoveVarnish : MonoBehaviour
{
    private Texture2D sourceTexture;  // Texture of this sprite
    private Color[] sourceColors;      // Colors of this sprite
    public Color redValue;             // Threshold red value
    public int erSize = 5;
    public Vector2Int lastPos;
    public bool Drawing = false;
    public float offset;

    private bool isMouseDown = false;
    private RaycastHit2D hit;

    // The canvas to modify
    public CanvasManager canvas2;

    void Start()
    {
        var canvas = GetComponent<CanvasManager>();
        sourceTexture = canvas.texture;
        sourceColors = canvas.colors;
    }

    void Update()
    {
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
            isMouseDown = true;
        if (Input.GetMouseButtonUp(0))
            isMouseDown = false;

        if (!isMouseDown || canvas2 == null)
            return;

        // Raycast only to detect this sprite
        var hits = Physics2D.RaycastAll(mouseWorld, Vector2.zero);
        foreach (var h in hits)
        {
            if (h.collider != null && h.collider.gameObject == gameObject)
            {
                hit = h;
                Drawing = true;
                UpdateTexture(resetLastPos: Input.GetMouseButtonDown(0));
                break;
            }
        }
    }

    public void UpdateTexture(bool resetLastPos = false)
    {
        if (sourceTexture == null || hit.collider == null || canvas2.texture == null)
            return;

        int w = sourceTexture.width;
        int h = sourceTexture.height;

        Vector2 mousePos = hit.point - (Vector2)hit.collider.bounds.min;
        mousePos.x *= w / hit.collider.bounds.size.x;
        mousePos.y *= h / hit.collider.bounds.size.y;
        Vector2Int p = new Vector2Int((int)mousePos.x, (int)mousePos.y);

        if (resetLastPos || !Drawing)
            lastPos = p;

        Vector2Int start = new Vector2Int(
            Mathf.Clamp(Mathf.Min(p.x, lastPos.x) - erSize, 0, w),
            Mathf.Clamp(Mathf.Min(p.y, lastPos.y) - erSize, 0, h)
        );

        Vector2Int end = new Vector2Int(
            Mathf.Clamp(Mathf.Max(p.x, lastPos.x) + erSize, 0, w),
            Mathf.Clamp(Mathf.Max(p.y, lastPos.y) + erSize, 0, h)
        );

        Vector2 dir = p - lastPos;

        for (int x = start.x; x < end.x; x++)
        {
            for (int y = start.y; y < end.y; y++)
            {
                Vector2 pixel = new Vector2(x, y);
                Vector2 linePos = p;

                if (Drawing && dir.sqrMagnitude > 0.0001f)
                {
                    float d = Vector2.Dot(pixel - lastPos, dir) / dir.sqrMagnitude;
                    d = Mathf.Clamp01(d);
                    linePos = Vector2.Lerp(lastPos, p, d);
                }

                if ((pixel - linePos).sqrMagnitude <= erSize * erSize)
                {
                    Color sourcePixel = sourceColors[x + y * w];
                    float redDiff = Mathf.Abs(redValue.r - sourcePixel.r);

                    /*if (Mathf.Abs(sourcePixel.r-redValue.r) < 0.05f)
                    {
                        if(canvas2.colors[x + y * w].b < 0.1f)
                        {
                            canvas2.colors[x + y * w] = new Color(canvas2.colors[x + y * w].r, canvas2.colors[x + y * w].g, canvas2.colors[x + y * w].b, redDiff); // make transparent                            
                        }
                        
                    }*/

                    if (sourcePixel.r < redValue.r)
                    {
                        if (canvas2.colors[x + y * w].b > 0.95f) 
                        {
                            canvas2.colors[x + y * w] = new Color(0f,1f, 0f, redDiff); // paint blue
                        }
                        else
                        {
                            canvas2.colors[x + y * w] = new Color(0f, 1f, 0f, Mathf.Clamp(redDiff, canvas2.colors[x + y * w].a, 1000f));
                        }
                                                   
                    }
                    else if (sourcePixel.r >= redValue.r)
                    {
                        if (canvas2.colors[x + y * w].b >0.95)
                        {
                            canvas2.colors[x + y * w] = new Color(canvas2.colors[x + y * w].r, canvas2.colors[x + y * w].g, canvas2.colors[x + y * w].b, Mathf.Clamp(canvas2.colors[x + y * w].a-(0.1f*(1-redDiff)), 0f, canvas2.colors[x + y * w].a)); // make transparent                            
                        }
                    }

                }
            }
        }

        lastPos = p;
        canvas2.ApplyChanges();
    }
}
