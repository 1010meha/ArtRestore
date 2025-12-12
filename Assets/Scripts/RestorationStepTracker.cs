using UnityEngine;

/// <summary>
/// Monitors restoration tools and updates progress tracker
/// </summary>
public class RestorationStepTracker : MonoBehaviour
{
    [Header("References")]
    public ScenarioProgressTracker progressTracker;
    public ScenarioManager scenarioManager;
    public BeakerManager beakerManager;
    public PuzzleManager puzzleManager;
    public SwitchTools switchTools;
    
    [Header("Gesso Tracking")]
    public Gesso[] gessoTools;
    private bool wasGessoActive = false;
    
    [Header("Painting Tracking")]
    public PaintColor paintTool;
    private bool wasPaintActive = false;
    
    private void Update()
    {
        if (progressTracker == null || scenarioManager == null)
            return;
        
        CheckVarnishRemoval();
        CheckPuzzleCompletion();
        CheckGessoApplication();
        CheckRepainting();
        
        // Check completion status periodically
        scenarioManager.CheckCompletionStatus();
    }
    
    private void CheckVarnishRemoval()
    {
        if (scenarioManager == null || scenarioManager.CurrentScenario == null)
            return;
            
        var scenario = scenarioManager.CurrentScenario;
        
        if (scenario.requiresVarnishRemoval && !progressTracker.varnishRemoved)
        {
            // Check if varnish removal tool has been used
            // This is a simplified check - you may want to add more sophisticated detection
            if (switchTools != null && switchTools.removeVarnish != null)
            {
                if (switchTools.removeVarnish.enabled && switchTools.removeVarnish.Drawing)
                {
                    // Check if correct concentration was used
                    if (beakerManager != null)
                    {
                        float requiredConc = scenario.requiredVarnishConcentration;
                        float actualConc = beakerManager.concentrationPercent;
                        float tolerance = 10f; // Allow 10% tolerance
                        
                        bool correctConcentration = Mathf.Abs(actualConc - requiredConc) <= tolerance;
                        progressTracker.MarkVarnishRemoved(correctConcentration);
                    }
                }
            }
        }
    }
    
    private void CheckPuzzleCompletion()
    {
        if (scenarioManager == null || scenarioManager.CurrentScenario == null)
            return;
            
        var scenario = scenarioManager.CurrentScenario;
        
        if (scenario.requiresJigsawPuzzle && !progressTracker.puzzleCompleted)
        {
            if (puzzleManager != null && !puzzleManager.gameObject.activeSelf)
            {
                // Puzzle manager deactivates itself when complete
                progressTracker.MarkPuzzleCompleted();
            }
        }
    }
    
    private void CheckGessoApplication()
    {
        if (scenarioManager == null || scenarioManager.CurrentScenario == null)
            return;
            
        var scenario = scenarioManager.CurrentScenario;
        
        if (scenario.requiresGesso && !progressTracker.gessoApplied)
        {
            // Check if gesso tools are active and being used
            bool gessoActive = false;
            if (gessoTools != null)
            {
                foreach (var gesso in gessoTools)
                {
                    if (gesso != null && gesso.enabled && gesso.Drawing)
                    {
                        gessoActive = true;
                        break;
                    }
                }
            }
            
            // If gesso was active and now isn't, assume it was applied
            if (wasGessoActive && !gessoActive)
            {
                progressTracker.MarkGessoApplied();
            }
            
            wasGessoActive = gessoActive;
        }
    }
    
    private void CheckRepainting()
    {
        if (scenarioManager == null || scenarioManager.CurrentScenario == null)
            return;
            
        var scenario = scenarioManager.CurrentScenario;
        
        if (scenario.requiresRepainting && !progressTracker.repainted)
        {
            // Check if paint tool is active and being used
            bool paintActive = paintTool != null && paintTool.enabled && paintTool.Drawing;
            
            // If paint was active and now isn't, assume repainting is done
            if (wasPaintActive && !paintActive)
            {
                progressTracker.MarkRepainted();
            }
            
            wasPaintActive = paintActive;
        }
    }
}

