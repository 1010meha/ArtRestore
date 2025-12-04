using UnityEngine;

public class PaintColor : MonoBehaviour
{
    private Texture2D m_Texture;
    private Color[] m_Colors;
    public Color colorToIgnore;
    public SpriteRenderer paint;
    public Color paintColor;
    RaycastHit2D hit;
    SpriteRenderer spriteRend;
    Color zeroAlpha = Color.red;
    public int erSize;
    public Vector2Int lastPos;
    public bool Drawing = false;
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

        if (Input.GetMouseButtonDown(0))
        {
            // Get all hits under the mouse
            var hits = Physics2D.RaycastAll(mouseWorld, Vector2.zero);
            foreach (var h in hits)
            {
                if (h.collider != null && h.collider.gameObject == gameObject)
                {
                    hit = h;
                    paintColor = paint.color;
                    UpdateTexture();
                    Drawing = true;
                    break; // found ourselves
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Drawing = false;
        }

        if (Drawing)
        {
            //  Update while dragging
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
        Vector2Int start = new Vector2Int();
        Vector2Int end = new Vector2Int();
        if (!Drawing)
            lastPos = p;
        start.x = Mathf.Clamp(Mathf.Min(p.x, lastPos.x) - erSize, 0, w);
        start.y = Mathf.Clamp(Mathf.Min(p.y, lastPos.y) - erSize, 0, h);
        end.x = Mathf.Clamp(Mathf.Max(p.x, lastPos.x) + erSize, 0, w);
        end.y = Mathf.Clamp(Mathf.Max(p.y, lastPos.y) + erSize, 0, h);
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
                if ((pixel - linePos).sqrMagnitude <= erSize * erSize)
                {
                    if (m_Colors[x + y * w].a > 0.5f) { m_Colors[x + y * w] = paintColor; }
                }
            }
        }
        lastPos = p;

        canvas.ApplyChanges();
    }
}