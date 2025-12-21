using UnityEngine;
using System;
using System.Collections.Generic;

public class SelectThis : MonoBehaviour
{
    [SerializeField] private SwitchTools.Tools toolType;
    [SerializeField] private Animator animator;
    private ToolCursorController cursor;
    private static readonly int HoverHash = Animator.StringToHash("Hovered");

    private static Dictionary<SwitchTools.Tools, Action> toolActions;
    
    private void Awake()
    {
        if (toolActions == null)
        {
            InitializeToolActions();
        }
        animator = GetComponent<Animator>();
        cursor = FindAnyObjectByType<ToolCursorController>();

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

    private void OnMouseEnter()
    {
        animator.SetBool("Hover",true);
        cursor.SetToHover();
    }

    private void OnMouseExit()
    {
        animator.SetBool("Hover", false);
        cursor.ResetHover();
    }

}
