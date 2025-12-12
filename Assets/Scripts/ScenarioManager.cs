using UnityEngine;
using Yarn.Unity;
using System.Collections;
using System.Linq;

/// <summary>
/// Manages the flow of restoration scenarios, loading clients and their paintings
/// </summary>
public class ScenarioManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("YarnSpinner dialogue runner")]
    public DialogueRunner dialogueRunner;
    
    [Tooltip("Progress tracker for current scenario")]
    public ScenarioProgressTracker progressTracker;
    
    [Tooltip("Painting display object (SpriteRenderer)")]
    public SpriteRenderer paintingDisplay;
    
    [Tooltip("Canvas manager for the painting")]
    public CanvasManager paintingCanvas;
    
    [Tooltip("Varnish layer canvas")]
    public CanvasManager varnishCanvas;
    
    [Tooltip("SwitchTools reference")]
    public SwitchTools switchTools;
    
    [Tooltip("Beaker manager for varnish removal")]
    public BeakerManager beakerManager;
    
    [Tooltip("Puzzle manager (if needed)")]
    public PuzzleManager puzzleManager;
    
    [Tooltip("Completion button GameObject")]
    public GameObject completionButton;
    
    [Header("Current Scenario")]
    [SerializeField] private RestorationScenario currentScenario;
    
    public RestorationScenario CurrentScenario => currentScenario;
    
    [Header("Scenario Queue")]
    [Tooltip("Scenarios to play in order (leave empty to manually set)")]
    public RestorationScenario[] scenarioQueue;
    
    private int currentScenarioIndex = 0;
    private bool isScenarioActive = false;
    
    private void Start()
    {
        if (completionButton != null)
            completionButton.SetActive(false);
        
        // Start first scenario if queue is set
        if (scenarioQueue != null && scenarioQueue.Length > 0)
        {
            LoadScenario(scenarioQueue[0]);
        }
    }
    
    /// <summary>
    /// Load a restoration scenario
    /// </summary>
    public void LoadScenario(RestorationScenario scenario)
    {
        if (scenario == null)
        {
            Debug.LogWarning("ScenarioManager: Attempted to load null scenario");
            return;
        }
        
        currentScenario = scenario;
        isScenarioActive = true;
        
        // Initialize progress tracker
        if (progressTracker != null)
        {
            progressTracker.Initialize(scenario);
        }
        
        // Set up painting sprites
        SetupPainting(scenario);
        
        // Set up restoration requirements
        SetupRestorationSteps(scenario);
        
        // Start dialogue
        if (dialogueRunner != null && !string.IsNullOrEmpty(scenario.dialogueStartNode))
        {
            dialogueRunner.StartDialogue(scenario.dialogueStartNode);
        }
        
        // Hide completion button
        if (completionButton != null)
            completionButton.SetActive(false);
        
        Debug.Log($"ScenarioManager: Loaded scenario '{scenario.name}' for client '{scenario.clientName}'");
    }
    
    private void SetupPainting(RestorationScenario scenario)
    {
        // Set main painting sprite
        if (paintingDisplay != null && scenario.paintingSprite != null)
        {
            paintingDisplay.sprite = scenario.paintingSprite;
        }
        
        // Set up canvas if painting canvas exists
        if (paintingCanvas != null && scenario.paintingSprite != null)
        {
            var canvasSR = paintingCanvas.GetComponent<SpriteRenderer>();
            if (canvasSR != null)
            {
                canvasSR.sprite = scenario.paintingSprite;
            }
        }
        
        // Set up varnish layer if provided
        if (varnishCanvas != null && scenario.varnishLayerSprite != null)
        {
            var varnishSR = varnishCanvas.GetComponent<SpriteRenderer>();
            if (varnishSR != null)
            {
                varnishSR.sprite = scenario.varnishLayerSprite;
            }
        }
    }
    
    private void SetupRestorationSteps(RestorationScenario scenario)
    {
        // Enable/disable tools based on requirements
        if (switchTools != null)
        {
            // Reset all tools first
            switchTools.SetAllToFalse();
        }
        
        // Set up varnish removal if needed
        if (scenario.requiresVarnishRemoval)
        {
            // Varnish setup will be handled by the dialogue or UI
            Debug.Log($"Scenario requires varnish removal at {scenario.requiredVarnishConcentration}% concentration");
        }
        
        // Set up jigsaw puzzle if needed
        if (scenario.requiresJigsawPuzzle && puzzleManager != null)
        {
            if (scenario.jigsawPuzzlePrefab != null)
            {
                // Instantiate puzzle prefab or activate it
                // This depends on your puzzle setup
                Debug.Log("Scenario requires jigsaw puzzle assembly");
            }
        }
    }
    
    /// <summary>
    /// Called when player clicks the completion button
    /// </summary>
    public void OnCompletionButtonClicked()
    {
        if (!isScenarioActive || currentScenario == null)
            return;
        
        // Check if all steps are completed
        if (progressTracker != null && progressTracker.IsScenarioComplete())
        {
            CompleteScenario();
        }
        else
        {
            Debug.Log("ScenarioManager: Not all restoration steps are completed yet!");
            // Could show a message to the player here
        }
    }
    
    private void CompleteScenario()
    {
        Debug.Log($"ScenarioManager: Completed scenario '{currentScenario.name}'");
        
        // Play completion dialogue
        if (dialogueRunner != null && !string.IsNullOrEmpty(currentScenario.completionDialogueNode))
        {
            dialogueRunner.StartDialogue(currentScenario.completionDialogueNode);
        }
        
        // Load next scenario or end
        StartCoroutine(LoadNextScenarioAfterDelay(2f));
    }
    
    private IEnumerator LoadNextScenarioAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Try to load next scenario from queue
        if (scenarioQueue != null && currentScenarioIndex < scenarioQueue.Length - 1)
        {
            currentScenarioIndex++;
            LoadScenario(scenarioQueue[currentScenarioIndex]);
        }
        // Or load next scenario from current scenario's nextScenario field
        else if (currentScenario != null && currentScenario.nextScenario != null)
        {
            LoadScenario(currentScenario.nextScenario);
        }
        else
        {
            Debug.Log("ScenarioManager: No more scenarios to load");
            isScenarioActive = false;
        }
    }
    
    /// <summary>
    /// Check completion status and show/hide completion button
    /// </summary>
    public void CheckCompletionStatus()
    {
        if (!isScenarioActive || currentScenario == null || progressTracker == null)
            return;
        
        bool canComplete = progressTracker.IsScenarioComplete();
        
        if (completionButton != null)
        {
            completionButton.SetActive(canComplete);
        }
    }
    
    // YarnSpinner command to load a scenario
    [YarnCommand("loadscenario")]
    public void LoadScenarioFromYarn(string scenarioName)
    {
        RestorationScenario scenario = Resources.Load<RestorationScenario>($"Scenarios/{scenarioName}");
        if (scenario == null)
        {
            // Try loading from all assets
            scenario = Resources.FindObjectsOfTypeAll<RestorationScenario>()
                .FirstOrDefault(s => s.name == scenarioName);
        }
        
        if (scenario != null)
        {
            LoadScenario(scenario);
        }
        else
        {
            Debug.LogError($"ScenarioManager: Could not find scenario '{scenarioName}'");
        }
    }
}

