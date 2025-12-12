using UnityEngine;

public class RemovePigment : MonoBehaviour
{
    private Texture2D m_Texture;
    private Color[] m_Colors;
    
    [Header("Brush Settings")]
    public int brushSize;
    
    [Header("Pigment Removal Settings")]
    public Color colorToIgnore;
    private const float ALPHA_DECREMENT = 0.1f;

    private RaycastHit2D hit;
    private Vector2Int lastPos;
    public bool Drawing { get; private set; } = false;
    private bool isMouseDown = false;
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
        // Early return if mouse is not down
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

        // Only raycast when mouse is down
        if (mainCamera == null) mainCamera = Camera.main;
        Vector2 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        bool mouseDownStart = Input.GetMouseButtonDown(0);

        // Use RaycastAll to detect ALL colliders under the mouse
        var hits = Physics2D.RaycastAll(mouseWorld, Vector2.zero);

        // Loop through all hits and see if this object is one of them
        foreach (var h in hits)
        {
            if (h.collider != null && h.collider.gameObject == gameObject)
            {
                hit = h; // store hit info
                Drawing = true;
                UpdateTexture(resetLastPos: mouseDownStart);
                break; // found ourselves, stop searching
            }
        }
    }

    public void UpdateTexture(bool resetLastPos = false)
    {
        if (m_Texture != null)
        {
            int w = m_Texture.width;
            int h = m_Texture.height;
            var mousePos = hit.point - (Vector2)hit.collider.bounds.min;
            mousePos.x *= w / hit.collider.bounds.size.x;
            mousePos.y *= h / hit.collider.bounds.size.y;
            Vector2Int p = new Vector2Int((int)mousePos.x, (int)mousePos.y);
            if (resetLastPos || !Drawing)
            {
                lastPos = p;
                Drawing = true;
            }
                
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
                        Color currentColor = m_Colors[x + y * w];
                        if (!Mathf.Approximately(currentColor.r, colorToIgnore.r))
                        {
                            float newAlpha = Mathf.Max(0f, currentColor.a - ALPHA_DECREMENT);
                            m_Colors[x + y * w] = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
                        }
                    }
                }
            }
            lastPos = p;

            canvas.ApplyChanges();

        }
    }
}