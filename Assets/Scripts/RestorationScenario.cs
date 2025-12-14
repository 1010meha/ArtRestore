using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject that defines a restoration scenario for a client.
/// Create these assets to easily set up different restoration jobs.
/// </summary>
[CreateAssetMenu(fileName = "New Restoration Scenario", menuName = "Art Restore/Scenario")]
public class RestorationScenario : ScriptableObject
{
    [Header("Client Information")]
    [Tooltip("Client's name")]
    public string clientName = "Client";
    
    [Header("Dialogue")]
    [Tooltip("YarnSpinner node name to start dialogue")]
    public string dialogueStartNode = "Start";
    
    [Header("Painting Setup")]
    [Tooltip("Main painting sprite to display")]
    public Sprite paintingSprite;

    [Tooltip("Varnish Readable Texture")]
    public Sprite varnishREADSprite;

    [Tooltip("Varnish layer texture")]
    public Sprite varnishLayerSprite;

    [Tooltip("Broken/cracked version of the painting (for jigsaw puzzles)")]
    public Sprite brokenPaintingSprite;
    
    [Tooltip("Underpainting layer (for infrared view)")]
    public Sprite underpaintingSprite;
    
    [Tooltip("UV layer (for UV view)")]
    public Sprite uvLayerSprite;

    [Tooltip("Crack Sprite")]
    public Sprite crackSprite;

    [Header("Restoration Steps Required")]
    [Tooltip("Does this painting need varnish removal?")]
    public bool requiresVarnishRemoval = false;
    
    [Tooltip("Required varnish remover concentration (0-100%)")]
    [Range(0f, 100f)]
    public float requiredVarnishConcentration = 50f;
    
    [Tooltip("Does this painting need to be pieced together (jigsaw)?")]
    public bool requiresJigsawPuzzle = false;
    
    [Tooltip("Jigsaw pieces prefab or setup")]
    public GameObject jigsawPuzzlePrefab;
    
    [Tooltip("Does this painting need gesso application?")]
    public bool requiresGesso = false;
    
    [Tooltip("Does this painting need repainting?")]
    public bool requiresRepainting = false;
    
    [Tooltip("Target colors for repainting (if needed)")]
    public Color[] targetPaintColors;
    
    [Header("Completion")]
    [Tooltip("YarnSpinner node to play when restoration is completed")]
    public string completionDialogueNode = "Completion";
    
    [Tooltip("Next scenario to load after this one (optional)")]
    public RestorationScenario nextScenario;
    
    /// <summary>
    /// Check if all required restoration steps are completed
    /// </summary>
    public bool AreAllStepsCompleted(ScenarioProgressTracker tracker)
    {
        if (requiresVarnishRemoval && !tracker.varnishRemoved)
            return false;
        if (requiresJigsawPuzzle && !tracker.puzzleCompleted)
            return false;
        if (requiresGesso && !tracker.gessoApplied)
            return false;
        if (requiresRepainting && !tracker.repainted)
            return false;
        
        return true;
    }
}

