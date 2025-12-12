using UnityEngine;
using Yarn.Unity;
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
        
        // Check YarnProject
        if (dialogueRunner.yarnProject == null)
        {
            Debug.LogError("YarnSpinnerSetupChecker: ❌ DialogueRunner has no YarnProject assigned!");
            Debug.LogError("   → Go to DialogueRunner component and assign a YarnProject asset");
            allGood = false;
        }
        else
        {
            Debug.Log($"YarnSpinnerSetupChecker: ✅ YarnProject assigned: {dialogueRunner.yarnProject.name}");
            
            // Check if YarnProject has source files
            #if UNITY_EDITOR
            try
            {
                var project = dialogueRunner.yarnProject;
                var path = AssetDatabase.GetAssetPath(project);
                if (!string.IsNullOrEmpty(path))
                {
                    var importer = AssetImporter.GetAtPath(path);
                    if (importer != null)
                    {
                        Debug.Log($"YarnSpinnerSetupChecker: ✅ YarnProject found at: {path}");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"YarnSpinnerSetupChecker: Could not check YarnProject details: {e.Message}");
            }
            #endif
        }
        
        // Check VariableStorage
        if (dialogueRunner.variableStorage == null)
        {
            Debug.LogWarning("YarnSpinnerSetupChecker: ⚠️ DialogueRunner has no VariableStorage assigned (optional but recommended)");
        }
        else
        {
            Debug.Log($"YarnSpinnerSetupChecker: ✅ VariableStorage assigned: {dialogueRunner.variableStorage.GetType().Name}");
        }
        
        // Check DialogueUI
        if (dialogueRunner.dialogueUI == null)
        {
            Debug.LogWarning("YarnSpinnerSetupChecker: ⚠️ DialogueRunner has no DialogueUI assigned");
        }
        else
        {
            Debug.Log($"YarnSpinnerSetupChecker: ✅ DialogueUI assigned: {dialogueRunner.dialogueUI.GetType().Name}");
        }
        
        if (allGood)
        {
            Debug.Log("YarnSpinnerSetupChecker: ✅ All checks passed! YarnSpinner should be ready to use.");
        }
    }
    
    [ContextMenu("List All Available Nodes")]
    public void ListNodes()
    {
        if (dialogueRunner == null || dialogueRunner.yarnProject == null)
        {
            Debug.LogError("YarnSpinnerSetupChecker: DialogueRunner or YarnProject not assigned!");
            return;
        }
        
        Debug.Log($"YarnSpinnerSetupChecker: YarnProject is '{dialogueRunner.yarnProject.name}'");
        Debug.Log("To see available nodes, check the YarnProject asset in the Inspector or try starting a dialogue node.");
        Debug.Log("If you get 'node not found' errors, check that your .yarn files are included in the YarnProject's source files.");
    }
}

