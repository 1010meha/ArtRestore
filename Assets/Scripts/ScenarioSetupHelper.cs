using UnityEngine;

/// <summary>
/// Helper script to easily set up scenarios in the Unity Inspector
/// Attach this to a GameObject to quickly configure scenario settings
/// </summary>
public class ScenarioSetupHelper : MonoBehaviour
{
    [Header("Quick Setup")]
    [Tooltip("Drag your scenario ScriptableObject here")]
    public RestorationScenario scenarioToLoad;
    
    [ContextMenu("Load Scenario")]
    public void LoadScenario()
    {
        if (scenarioToLoad == null)
        {
            Debug.LogError("ScenarioSetupHelper: No scenario assigned!");
            return;
        }
        
        var scenarioManager = FindObjectOfType<ScenarioManager>();
        if (scenarioManager != null)
        {
            scenarioManager.LoadScenario(scenarioToLoad);
        }
        else
        {
            Debug.LogError("ScenarioSetupHelper: No ScenarioManager found in scene!");
        }
    }
    
    [ContextMenu("Create New Scenario Asset")]
    public void CreateNewScenario()
    {
        #if UNITY_EDITOR
        var scenario = ScriptableObject.CreateInstance<RestorationScenario>();
        scenario.name = "New Restoration Scenario";
        
        string path = $"Assets/Scenarios/{scenario.name}.asset";
        string directory = "Assets/Scenarios";
        if (!System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
        }
        
        UnityEditor.AssetDatabase.CreateAsset(scenario, path);
        UnityEditor.AssetDatabase.SaveAssets();
        
        Debug.Log($"Created new scenario at {path}");
        #endif
    }
}

