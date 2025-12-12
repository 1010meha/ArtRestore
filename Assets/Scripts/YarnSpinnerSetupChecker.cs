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
            var project = dialogueRunner.yarnProject;
            var importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(project)) as Yarn.Unity.YarnProjectImporter;
            if (importer != null)
            {
                Debug.Log($"YarnSpinnerSetupChecker: ✅ YarnProject has {importer.sourceFiles.Count} source file(s)");
                
                // List available nodes
                var compilationResult = project.GetCompilationResult();
                if (compilationResult != null && compilationResult.Program != null)
                {
                    var nodes = compilationResult.Program.Nodes.Keys;
                    Debug.Log($"YarnSpinnerSetupChecker: ✅ Available dialogue nodes: {string.Join(", ", nodes)}");
                }
                else
                {
                    Debug.LogWarning("YarnSpinnerSetupChecker: ⚠️ YarnProject has not been compiled. Check Console for compilation errors.");
                }
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
        
        #if UNITY_EDITOR
        var compilationResult = dialogueRunner.yarnProject.GetCompilationResult();
        if (compilationResult != null && compilationResult.Program != null)
        {
            var nodes = compilationResult.Program.Nodes.Keys;
            Debug.Log($"Available dialogue nodes:\n{string.Join("\n", nodes)}");
        }
        else
        {
            Debug.LogWarning("YarnSpinnerSetupChecker: YarnProject has not been compiled. Check Console for errors.");
        }
        #endif
    }
}

