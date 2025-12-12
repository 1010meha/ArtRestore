using UnityEngine;

/// <summary>
/// Tracks progress of restoration steps for the current scenario
/// </summary>
public class ScenarioProgressTracker : MonoBehaviour
{
    [Header("Progress Flags")]
    public bool varnishRemoved = false;
    public bool puzzleCompleted = false;
    public bool gessoApplied = false;
    public bool repainted = false;
    
    [Header("Varnish Removal")]
    [Tooltip("Has the correct varnish concentration been used?")]
    public bool correctVarnishConcentrationUsed = false;
    
    private RestorationScenario currentScenario;
    
    public void Initialize(RestorationScenario scenario)
    {
        currentScenario = scenario;
        ResetProgress();
    }
    
    public void ResetProgress()
    {
        varnishRemoved = false;
        puzzleCompleted = false;
        gessoApplied = false;
        repainted = false;
        correctVarnishConcentrationUsed = false;
    }
    
    public void MarkVarnishRemoved(bool correctConcentration)
    {
        varnishRemoved = true;
        correctVarnishConcentrationUsed = correctConcentration;
    }
    
    public void MarkPuzzleCompleted()
    {
        puzzleCompleted = true;
    }
    
    public void MarkGessoApplied()
    {
        gessoApplied = true;
    }
    
    public void MarkRepainted()
    {
        repainted = true;
    }
    
    public bool IsScenarioComplete()
    {
        if (currentScenario == null) return false;
        return currentScenario.AreAllStepsCompleted(this);
    }
    
    /// <summary>
    /// Manually mark all steps as complete (for testing/debugging)
    /// </summary>
    [ContextMenu("Mark All Steps Complete")]
    public void MarkAllStepsComplete()
    {
        if (currentScenario == null) return;
        
        if (currentScenario.requiresVarnishRemoval)
            varnishRemoved = true;
        if (currentScenario.requiresJigsawPuzzle)
            puzzleCompleted = true;
        if (currentScenario.requiresGesso)
            gessoApplied = true;
        if (currentScenario.requiresRepainting)
            repainted = true;
    }
}

