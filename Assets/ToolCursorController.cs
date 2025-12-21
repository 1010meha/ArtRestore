using UnityEngine;

public class ToolCursorController : MonoBehaviour
{
    [SerializeField] Color colorPick;

    [Header("Renderers")]
    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private SpriteRenderer tipRenderer;

    [Header("Cursor Settings")]
    [SerializeField] private Vector3 offset;
    [SerializeField] private bool hideSystemCursor = true;
    [SerializeField] private bool toolHasTip;


    [Header("Scale Settings")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float pressedScale = 0.6f;

    [Header("Universal Sprirtes")]
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private ToolDefinition defaultTool;


    private Sprite activeBodySprite;
    private Sprite activeTipSprite;
    private bool activeHasTip;
    private bool isHovering;



    private Camera cam;

    private void Awake()
    {
        ApplyTool(defaultTool);

        cam = Camera.main;

        Cursor.visible = !hideSystemCursor;

        SetTipColor(colorPick);
        Cursor.visible = false;

    }

    private void Update()
    {
        HandleCursorScale();
        FollowMouse();
    }


    public void ApplyTool(ToolDefinition tool)
    {
        if (tool == null) return;

        activeBodySprite = tool.cursorBody;
        activeTipSprite = tool.cursorTip;
        activeHasTip = tool.cursorTip != null;

        SetTool(activeBodySprite, activeTipSprite);

        normalScale = tool.normalScale;
        pressedScale = tool.pressedScale;
    }



    private void HandleCursorScale()
    {
        float targetScale = Input.GetMouseButton(0) ? pressedScale : normalScale;

        visualRoot.localScale = Vector3.Lerp(
            visualRoot.localScale,
            Vector3.one * targetScale,
            Time.deltaTime * 15f
        );
    }

    public void SetToHover()
    {
        if (hoverSprite == null || bodyRenderer == null)
            return;

        isHovering = true;

        bodyRenderer.enabled = true;
        bodyRenderer.sprite = hoverSprite;

        tipRenderer.enabled = false;
    }


    public void ResetHover()
    {
        if (!isHovering || activeBodySprite == null)
            return;

        isHovering = false;

        bodyRenderer.sprite = activeBodySprite;

        if (activeHasTip)
        {
            tipRenderer.sprite = activeTipSprite;
            tipRenderer.enabled = true;
        }
        else
        {
            tipRenderer.enabled = false;
        }
    }




    private void FollowMouse()
    {
        if (cam == null || !gameObject.activeSelf) return;

        Vector3 mouse = Input.mousePosition + offset;
        mouse.z = 100f;

        transform.position = cam.ScreenToWorldPoint(mouse);
    }

    public void SetTool(Sprite body, Sprite tip = null)
    {
        bodyRenderer.sprite = body;

        toolHasTip = tip != null;

        if (toolHasTip)
        {
            tipRenderer.sprite = tip;
            tipRenderer.enabled = true;
        }
        else
        {
            tipRenderer.enabled = false;
        }
    }


    public void SetTipColor(Color color)
    {
        
            tipRenderer.color = color;
    }

    public void ShowCursor(bool show)
    {
        gameObject.SetActive(show);
        Cursor.visible = !show;
    }

    public void SetBodyColor(Color color)
    {
        bodyRenderer.color = color;
    }

}
