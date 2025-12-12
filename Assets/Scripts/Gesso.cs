using UnityEngine;

public class Gesso : MonoBehaviour
{
    private Texture2D m_Texture;
    private Color[] m_Colors;
    private RaycastHit2D hit;
    
    [Header("Brush Settings")]
    public int brushSize = 5;
    
    [Header("Gesso Settings")]
    private const float ALPHA_INCREMENT = 0.1f;
    private const float INITIAL_ALPHA = 0.1f;
    private const float TRANSPARENT_THRESHOLD = 0.01f;
    private const float WHITE_VALUE = 1f;

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
        var hits = Physics2D.RaycastAll(mouseWorld, Vector2.zero);

        bool mouseDownStart = Input.GetMouseButtonDown(0);

        // Loop through ALL hits under the mouse
        foreach (var h in hits)
        {
            Gesso g = h.collider.GetComponent<Gesso>();
            if (g == null) continue;

            g.hit = h; // pass hit info so it knows where to draw
            g.Drawing = true;

            // Only reset last position when stroke starts
            g.UpdateTexture(resetLastPos: mouseDownStart);
        }
    }



    public void UpdateTexture(bool resetLastPos = false)
    {
        int w = m_Texture.width;
        int h = m_Texture.height;

        var mousePos = hit.point - (Vector2)hit.collider.bounds.min;
        mousePos.x *= w / hit.collider.bounds.size.x;
        mousePos.y *= h / hit.collider.bounds.size.y;
        Vector2Int p = new Vector2Int((int)mousePos.x, (int)mousePos.y);

        // Reset stroke start when requested
        if (resetLastPos)
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
                if (Drawing && dir.sqrMagnitude > 0.0001f)
                {
                    float d = Vector2.Dot(pixel - lastPos, dir) / dir.sqrMagnitude;
                    d = Mathf.Clamp01(d);
                    linePos = Vector2.Lerp(lastPos, p, d);
                }

                if ((pixel - linePos).sqrMagnitude <= brushSize * brushSize)
                {
                    Color current = m_Colors[x + y * w];
                    float newAlpha = Mathf.Clamp01(current.a + ALPHA_INCREMENT);

                    // If pixel was fully transparent, reinitialize its color
                    if (current.a <= TRANSPARENT_THRESHOLD)
                    {
                        m_Colors[x + y * w] = new Color(WHITE_VALUE, WHITE_VALUE, WHITE_VALUE, INITIAL_ALPHA);
                    }
                    else
                    {
                        m_Colors[x + y * w] = new Color(WHITE_VALUE, WHITE_VALUE, WHITE_VALUE, newAlpha);
                    }
                }

            }
        }

        lastPos = p;
        canvas.ApplyChanges();

    }

}
