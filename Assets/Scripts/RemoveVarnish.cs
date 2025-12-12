using UnityEngine;

public class RemoveVarnish : MonoBehaviour
{
    private Texture2D sourceTexture;
    private Color[] sourceColors;
    
    [Header("Varnish Removal Settings")]
    public Color thresholdColor;  // Threshold color value for varnish detection
    public int brushSize = 5;
    
    private const float VARNISH_DETECTION_THRESHOLD = 0.95f;
    private const float ALPHA_DECREMENT_FACTOR = 0.1f;
    private const float MIN_DIRECTION_MAGNITUDE = 0.0001f;

    private Vector2Int lastPos;
    public bool Drawing { get; private set; } = false;
    private bool isMouseDown = false;
    private RaycastHit2D hit;
    private Camera mainCamera;

    // The canvas to modify
    public CanvasManager canvas2;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    void Start()
    {
        var canvas = GetComponent<CanvasManager>();
        sourceTexture = canvas.texture;
        sourceColors = canvas.colors;
    }

    void Update()
    {
        // Early return if mouse is not down or canvas is null
        if (!isMouseDown)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isMouseDown = true;
            }
            else
            {
                return;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMouseDown = false;
            Drawing = false;
            return;
        }

        if (canvas2 == null)
            return;

        // Only raycast when mouse is down
        if (mainCamera == null) mainCamera = Camera.main;
        Vector2 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);

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
            Mathf.Clamp(Mathf.Min(p.x, lastPos.x) - brushSize, 0, w),
            Mathf.Clamp(Mathf.Min(p.y, lastPos.y) - brushSize, 0, h)
        );

        Vector2Int end = new Vector2Int(
            Mathf.Clamp(Mathf.Max(p.x, lastPos.x) + brushSize, 0, w),
            Mathf.Clamp(Mathf.Max(p.y, lastPos.y) + brushSize, 0, h)
        );

        Vector2 dir = p - lastPos;

        for (int x = start.x; x < end.x; x++)
        {
            for (int y = start.y; y < end.y; y++)
            {
                Vector2 pixel = new Vector2(x, y);
                Vector2 linePos = p;

                if (Drawing && dir.sqrMagnitude > MIN_DIRECTION_MAGNITUDE)
                {
                    float d = Vector2.Dot(pixel - lastPos, dir) / dir.sqrMagnitude;
                    d = Mathf.Clamp01(d);
                    linePos = Vector2.Lerp(lastPos, p, d);
                }

                if ((pixel - linePos).sqrMagnitude <= brushSize * brushSize)
                {
                    Color sourcePixel = sourceColors[x + y * w];
                    Color canvasColor = canvas2.colors[x + y * w];
                    float colorDifference = Mathf.Abs(thresholdColor.r - sourcePixel.r);

                    // If source pixel is below threshold (likely varnish)
                    if (sourcePixel.r < thresholdColor.r)
                    {
                        // If canvas pixel is mostly blue (varnish layer), mark for removal
                        if (canvasColor.b > VARNISH_DETECTION_THRESHOLD)
                        {
                            canvas2.colors[x + y * w] = new Color(0f, 1f, 0f, colorDifference);
                        }
                        else
                        {
                            // Gradually increase removal marker
                            float newAlpha = Mathf.Clamp(colorDifference, canvasColor.a, 1000f);
                            canvas2.colors[x + y * w] = new Color(0f, 1f, 0f, newAlpha);
                        }
                    }
                    // If source pixel is at or above threshold (likely paint)
                    else if (sourcePixel.r >= thresholdColor.r)
                    {
                        // If canvas pixel is mostly blue (varnish layer), make it transparent
                        if (canvasColor.b > VARNISH_DETECTION_THRESHOLD)
                        {
                            float alphaReduction = ALPHA_DECREMENT_FACTOR * (1f - colorDifference);
                            float newAlpha = Mathf.Clamp(canvasColor.a - alphaReduction, 0f, canvasColor.a);
                            canvas2.colors[x + y * w] = new Color(canvasColor.r, canvasColor.g, canvasColor.b, newAlpha);
                        }
                    }
                }
            }
        }

        lastPos = p;
        canvas2.ApplyChanges();
    }
}
