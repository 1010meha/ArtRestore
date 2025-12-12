using UnityEngine;
using Yarn.Unity;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Helper script to check if YarnSpinner is properly set up
/// Attach to a GameObject and use the context menu to check setup
/// </summary>
public class YarnSpinnerSetupChecker : MonoBehaviour
{
    [Header("References")]
    public DialogueRunner dialogueRunner;
    
    [ContextMenu("Check YarnSpinner Setup")]
    public void CheckSetup()
    {
        if (dialogueRunner == null)
        {
            Debug.LogError("YarnSpinnerSetupChecker: DialogueRunner is not assigned!");
            return;
        }
        
        bool allGood = true;
        
        // Check YarnProject using reflection to avoid API version issues
        #if UNITY_EDITOR
        try
        {
            var yarnProjectProperty = typeof(DialogueRunner).GetProperty("yarnProject");
            if (yarnProjectProperty != null)
            {
                var yarnProject = yarnProjectProperty.GetValue(dialogueRunner);
                if (yarnProject == null)
                {
                    Debug.LogError("YarnSpinnerSetupChecker: ❌ DialogueRunner has no YarnProject assigned!");
                    Debug.LogError("   → Go to DialogueRunner component and assign a YarnProject asset");
                    allGood = false;
                }
                else
                {
                    var nameProperty = yarnProject.GetType().GetProperty("name");
                    if (nameProperty != null)
                    {
                        Debug.Log($"YarnSpinnerSetupChecker: ✅ YarnProject assigned: {nameProperty.GetValue(yarnProject)}");
                    }
                    else
                    {
                        Debug.Log("YarnSpinnerSetupChecker: ✅ YarnProject is assigned");
                    }
                }
            }
            else
            {
                Debug.LogWarning("YarnSpinnerSetupChecker: Could not find 'yarnProject' property. YarnSpinner API may have changed.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"YarnSpinnerSetupChecker: Could not check YarnProject: {e.Message}");
        }
        #else
        Debug.Log("YarnSpinnerSetupChecker: DialogueRunner assigned. Use Editor mode for detailed checks.");
        #endif
        
        // Check VariableStorage
        try
        {
            var variableStorageProperty = typeof(DialogueRunner).GetProperty("variableStorage");
            if (variableStorageProperty != null)
            {
                var variableStorage = variableStorageProperty.GetValue(dialogueRunner);
                if (variableStorage == null)
                {
                    Debug.LogWarning("YarnSpinnerSetupChecker: ⚠️ DialogueRunner has no VariableStorage assigned (optional but recommended)");
                }
                else
                {
                    Debug.Log($"YarnSpinnerSetupChecker: ✅ VariableStorage assigned: {variableStorage.GetType().Name}");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"YarnSpinnerSetupChecker: Could not check VariableStorage: {e.Message}");
        }
        
        // Check DialogueUI
        try
        {
            var dialogueUIProperty = typeof(DialogueRunner).GetProperty("dialogueUI");
            if (dialogueUIProperty != null)
            {
                var dialogueUI = dialogueUIProperty.GetValue(dialogueRunner);
                if (dialogueUI == null)
                {
                    Debug.LogWarning("YarnSpinnerSetupChecker: ⚠️ DialogueRunner has no DialogueUI assigned");
                }
                else
                {
                    Debug.Log($"YarnSpinnerSetupChecker: ✅ DialogueUI assigned: {dialogueUI.GetType().Name}");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"YarnSpinnerSetupChecker: Could not check DialogueUI: {e.Message}");
        }
        
        if (allGood)
        {
            Debug.Log("YarnSpinnerSetupChecker: ✅ All checks passed! YarnSpinner should be ready to use.");
        }
    }
    
    [ContextMenu("List All Available Nodes")]
    public void ListNodes()
    {
        if (dialogueRunner == null)
        {
            Debug.LogError("YarnSpinnerSetupChecker: DialogueRunner not assigned!");
            return;
        }
        
        #if UNITY_EDITOR
        try
        {
            var yarnProjectProperty = typeof(DialogueRunner).GetProperty("yarnProject");
            if (yarnProjectProperty != null)
            {
                var yarnProject = yarnProjectProperty.GetValue(dialogueRunner);
                if (yarnProject == null)
                {
                    Debug.LogError("YarnSpinnerSetupChecker: YarnProject not assigned!");
                    return;
                }
                
                var nameProperty = yarnProject.GetType().GetProperty("name");
                if (nameProperty != null)
                {
                    Debug.Log($"YarnSpinnerSetupChecker: YarnProject is '{nameProperty.GetValue(yarnProject)}'");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"YarnSpinnerSetupChecker: Could not get YarnProject: {e.Message}");
        }
        #endif
        
        Debug.Log("To see available nodes, check the YarnProject asset in the Inspector or try starting a dialogue node.");
        Debug.Log("If you get 'node not found' errors, check that your .yarn files are included in the YarnProject's source files.");
    }
}

