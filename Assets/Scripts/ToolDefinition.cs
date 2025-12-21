using UnityEngine;

[CreateAssetMenu(fileName = "New Tool", menuName = "Art Restore/Tool")]
public class ToolDefinition : ScriptableObject
{
    [Header("Tool Identity")]
    public SwitchTools.Tools toolType;

    [Header("Cursor Visuals")]
    public Sprite cursorBody;
    public Sprite cursorTip;     // null if none
    [Header("Cursor Scale")]
    public float normalScale = 1f;
    public float pressedScale = 0.6f;

  

}
