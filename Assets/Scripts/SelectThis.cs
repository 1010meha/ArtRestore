using UnityEngine;
using System;
using System.Collections.Generic;

public class SelectThis : MonoBehaviour
{
    [SerializeField] private SwitchTools.Tools toolType;
    
    private static Dictionary<SwitchTools.Tools, Action> toolActions;
    
    private void Awake()
    {
        if (toolActions == null)
        {
            InitializeToolActions();
        }
    }
    
    private static void InitializeToolActions()
    {
        toolActions = new Dictionary<SwitchTools.Tools, Action>
        {
            { SwitchTools.Tools.UseGesso, () => SwitchTools.Instance?.UseGesso() },
            { SwitchTools.Tools.RemoveGesso, () => SwitchTools.Instance?.RemoveGesso() },
            { SwitchTools.Tools.UsePaint, () => SwitchTools.Instance?.UsePaint() },
            { SwitchTools.Tools.UseVarnishRemover, () => SwitchTools.Instance?.UseVarnishRemover() },
            { SwitchTools.Tools.VisibleLight, () => SwitchTools.Instance?.VisibleLight() },
            { SwitchTools.Tools.UVLight, () => SwitchTools.Instance?.UVLight() },
            { SwitchTools.Tools.InfraredLight, () => SwitchTools.Instance?.InfraredLight() },
            { SwitchTools.Tools.VarnishSetup, () => SwitchTools.Instance?.GoToVarnishSetup() }
        };
    }
    
    private void OnMouseDown()
    {
        if (toolActions != null && toolActions.TryGetValue(toolType, out Action action))
        {
            action?.Invoke();
        }
    }
}
