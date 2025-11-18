using Unity.VisualScripting;
using UnityEngine;

public class Gesso : MonoBehaviour
{
    private Texture2D m_Texture;
    private Color[] m_Colors;
    RaycastHit2D hit;
    SpriteRenderer spriteRend;
    public int erSize = 5;
    public Vector2Int lastPos;
    public bool Drawing = false;
    private bool isMouseDown = false;
    public float alphaMin;

    private CanvasManager canvas;

    void Start()
    {
        canvas = GetComponent<CanvasManager>();
        m_Texture = canvas.texture;
        m_Colors = canvas.colors;
        spriteRend = GetComponent<SpriteRenderer>();
    }



    void Update()
    {
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hits = Physics2D.RaycastAll(mouseWorld, Vector2.zero);

        bool mouseDown = Input.GetMouseButton(0);
        bool mouseDownStart = Input.GetMouseButtonDown(0);
        bool mouseDownEnd = Input.GetMouseButtonUp(0);

        if (mouseDownStart)
            isMouseDown = true;
        if (mouseDownEnd)
            isMouseDown = false;

        if (!isMouseDown)
            return;

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
                    Color current = m_Colors[x + y * w];
                    float newAlpha = Mathf.Clamp01(current.a + 0.1f);

                    // If pixel was fully transparent, reinitialize its color
                    if (current.a <= 0.01f)
                    {
                        m_Colors[x + y * w] = new Color(1f, 1f, 1f, 0.1f);
                    }
                    else
                    {
                        m_Colors[x + y * w] = new Color(1f, 1f, 1f, newAlpha);
                    }

                }

            }
        }

        lastPos = p;
        canvas.ApplyChanges();

    }

}
