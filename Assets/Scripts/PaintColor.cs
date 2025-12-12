using UnityEngine;

public class PaintColor : MonoBehaviour
{
    private Texture2D m_Texture;
    private Color[] m_Colors;
    
    [Header("Paint Settings")]
    public SpriteRenderer paint;
    public Color paintColor;
    public int brushSize;
    
    private const float MIN_ALPHA_TO_PAINT = 0.5f;

    private RaycastHit2D hit;
    private Vector2Int lastPos;
    public bool Drawing { get; private set; } = false;
    private CanvasManager canvas;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    void Start()
    {
        canvas = GetComponent<CanvasManager>();
        m_Texture = canvas.texture;
        m_Colors = canvas.colors;
    }

    void Update()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        Vector2 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            // Get all hits under the mouse
            var hits = Physics2D.RaycastAll(mouseWorld, Vector2.zero);
            foreach (var h in hits)
            {
                if (h.collider != null && h.collider.gameObject == gameObject)
                {
                    hit = h;
                    if (paint != null) paintColor = paint.color;
                    UpdateTexture();
                    Drawing = true;
                    break; // found ourselves
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Drawing = false;
            return;
        }

        if (Drawing)
        {
            // Update while dragging - only raycast when actually drawing
            var hits = Physics2D.RaycastAll(mouseWorld, Vector2.zero);
            bool overThis = false;
            foreach (var h in hits)
            {
                if (h.collider != null && h.collider.gameObject == gameObject)
                {
                    hit = h;
                    UpdateTexture();
                    overThis = true;
                    break;
                }
            }
            if (!overThis) Drawing = false; // stop if the cursor left this sprite
        }
    }


    public void UpdateTexture()
    {
        int w = m_Texture.width;
        int h = m_Texture.height;
        var mousePos = hit.point - (Vector2)hit.collider.bounds.min;
        mousePos.x *= w / hit.collider.bounds.size.x;
        mousePos.y *= h / hit.collider.bounds.size.y;
        Vector2Int p = new Vector2Int((int)mousePos.x, (int)mousePos.y);
        if (!Drawing)
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
                if (Drawing)
                {
                    float d = Vector2.Dot(pixel - lastPos, dir) / dir.sqrMagnitude;
                    d = Mathf.Clamp01(d);
                    linePos = Vector2.Lerp(lastPos, p, d);
                }
                if ((pixel - linePos).sqrMagnitude <= brushSize * brushSize)
                {
                    if (m_Colors[x + y * w].a > MIN_ALPHA_TO_PAINT)
                    {
                        m_Colors[x + y * w] = paintColor;
                    }
                }
            }
        }
        lastPos = p;

        canvas.ApplyChanges();
    }
}