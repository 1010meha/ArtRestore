using System.Collections.Generic;
using UnityEngine;

public class SwitchTools : MonoBehaviour
{
    [Header("Painting Tools")]
    public Gesso crackGesso;
    public Gesso overGesso;
    public RemoveVarnish removeVarnish;
    public RemovePigment removeGesso;
    public PaintColor paintCrack;
    
    [Header("Lighting Views")]
    public GameObject FluorecentCrack; // Note: Keeping original name for Unity serialization
    public GameObject UVPainting;
    public GameObject UnderPainting;
    
    [Header("UI Elements")]
    public GameObject varnishSetup;
    public GameObject buttons;
    public GameObject paintingParent;
    public GameObject brushSize;

    [Header("Tool Definitions")]
    [SerializeField] private ToolDefinition[] tools;
    [SerializeField] private ToolCursorController cursor;


    public static SwitchTools Instance { get; private set; }

    public enum Tools 
    { 
        
        UseGesso, 
        RemoveGesso, 
        UsePaint, 
        UseVarnishRemover, 
        VisibleLight, 
        UVLight, 
        InfraredLight, 
        VarnishSetup,
            Select,
    }
    
    private const float MAX_MOUSE_DISTANCE = 500f;
    private const float MIN_BRUSH_SIZE = 5f;
    private const float MAX_BRUSH_SIZE = 50f;
    
    private float originalMousePosition;
    private ChangeBrushSize brushSizeComponent;
    private Dictionary<Tools, ToolDefinition> toolLookup;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            toolLookup = new Dictionary<Tools, ToolDefinition>();
            foreach (var tool in tools)
            {
                toolLookup[tool.toolType] = tool;
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        SetAllToFalse();
        if (brushSize != null)
        {
            brushSizeComponent = brushSize.GetComponent<ChangeBrushSize>();
        }
    }

    public void SetAllToFalse()
    {
        // Disable all painting tools
        if (crackGesso != null) crackGesso.enabled = false;
        if (overGesso != null) overGesso.enabled = false;
        if (removeVarnish != null) removeVarnish.enabled = false;
        if (removeGesso != null) removeGesso.enabled = false;
        if (paintCrack != null) paintCrack.enabled = false;
        
        // Hide all lighting views
        if (FluorecentCrack != null) FluorecentCrack.SetActive(false);
        if (UVPainting != null) UVPainting.SetActive(false);
        if (UnderPainting != null) UnderPainting.SetActive(false);
        if (varnishSetup != null) varnishSetup.SetActive(false);
    }

    private void Update()
    {
        HandleBrushSizeAdjustment();
    }
    
    private void HandleBrushSizeAdjustment()
    {
        if (paintCrack == null || !paintCrack.enabled || brushSize == null)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            originalMousePosition = Input.mousePosition.x;
            brushSize.SetActive(true);
            Camera cam = Camera.main;

            Vector3 mouse = Input.mousePosition;
            mouse.z = Mathf.Abs(cam.transform.position.z); // distance from camera to world plane

            brushSize.transform.position = cam.ScreenToWorldPoint(mouse);

            if (brushSizeComponent != null)
            {
                brushSizeComponent.colorScript = paintCrack;
            }
        }
        
        if (Input.GetKey(KeyCode.Mouse1))
        {
            float mouseDistance = Mathf.Abs(originalMousePosition - Input.mousePosition.x);
            float t = Mathf.InverseLerp(0f, MAX_MOUSE_DISTANCE, mouseDistance);
            paintCrack.brushSize = Mathf.RoundToInt(Mathf.Lerp(MIN_BRUSH_SIZE, MAX_BRUSH_SIZE, t));
        }
        
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            brushSize.SetActive(false);
        }
    }

    private void ApplyTool(Tools toolType)
    {
        if (toolLookup.TryGetValue(toolType, out var tool))
        {
            cursor.ApplyTool(tool);
        }
    }
    public void UseGesso()
    {
        SetAllToFalse();
        if (crackGesso != null) crackGesso.enabled = true;
        if (overGesso != null) overGesso.enabled = true;
        ApplyTool(Tools.UseGesso);
    }

    public void RemoveGesso()
    {
        SetAllToFalse();
        if (removeGesso != null) removeGesso.enabled = true;
        ApplyTool(Tools.RemoveGesso);
    }

    public void UsePaint()
    {
        SetAllToFalse();
        if (paintCrack != null) paintCrack.enabled = true;
        ApplyTool(Tools.UsePaint);
    }

    public void UseVarnishRemover()
    {
        SetAllToFalse();
        if (removeVarnish != null) removeVarnish.enabled = true;
        ApplyTool(Tools.UseVarnishRemover);
    }

    public void VisibleLight()
    {
        SetAllToFalse();
        if (FluorecentCrack != null) FluorecentCrack.SetActive(true);
        ApplyTool(Tools.VisibleLight);
    }
    
    public void UVLight()
    {
        SetAllToFalse();
        if (UVPainting != null) UVPainting.SetActive(true);
        ApplyTool(Tools.UVLight);
    }
    
    public void InfraredLight()
    {
        SetAllToFalse();
        if (UnderPainting != null) UnderPainting.SetActive(true);
        ApplyTool(Tools.InfraredLight);
    }

    public void GoToVarnishSetup()
    {
        SetAllToFalse();
        if (buttons != null) buttons.SetActive(false);
        if (paintingParent != null) paintingParent.SetActive(false);
        if (varnishSetup != null) varnishSetup.SetActive(true);
        ApplyTool(Tools.VarnishSetup);
    }
}
