using UnityEngine;

public class BeakerManager : MonoBehaviour
{
    [Header("Switch Setup")]
    public RemoveVarnish varnishConc;
    public GameObject varnishSetup;
    public GameObject buttons;
    public GameObject painting;


    [Header("Liquid Visual")]
    public GameObject liquidSprite;
    public float maxHeight = 5f;

    [Header("Stats (Read-Only)")]
    [Range(0, 1)] public float totalFillNormalized;
    [Range(0, 100)] public float concentrationPercent;

    private float totalVolume = 0f;
    private float concentrateVolume = 0f;
    private float baseVolume = 0f;

    private bool isFull = false;

    public void AddLiquidFunction(float amount, bool isConcentrate)
    {
        if (isFull)
            return; // stop filling once full

        float newVolume = totalVolume + amount;

        // If adding this would overfill, only add the remaining capacity
        if (newVolume >= 1f)
        {
            amount = 1f - totalVolume;
            totalVolume = 1f;
            isFull = true;
        }
        else
        {
            totalVolume = newVolume;
        }

        // Update concentrate/base volumes based on final amount added
        if (amount > 0f)
        {
            if (isConcentrate)
                concentrateVolume += amount;
            else
                baseVolume += amount;
        }

        // Update visual fill level
        if (liquidSprite)
        {
            Vector3 scale = liquidSprite.transform.localScale;
            scale.y = Mathf.Lerp(0f, maxHeight, totalVolume);
            liquidSprite.transform.localScale = scale;
        }

        // Compute concentration only when new liquid was added
        float total = concentrateVolume + baseVolume;
        concentrationPercent = total > 0 ? (concentrateVolume / total) * 100f : 0f;
    }

    public void ResetBeaker()
    {
        totalVolume = 0f;
        concentrateVolume = 0f;
        baseVolume = 0f;
        isFull = false;

        if (liquidSprite)
        {
            Vector3 scale = liquidSprite.transform.localScale;
            scale.y = 0f;
            liquidSprite.transform.localScale = scale;
        }

        concentrationPercent = 0f;
    }

    private void OnMouseDown()
    {
        buttons.SetActive(true);
        painting.SetActive(true);
        // Set threshold color based on concentration (red channel represents concentration)
        varnishConc.thresholdColor = new Color((concentrationPercent / 100f), 0f, 0f, 1f);
        varnishSetup.SetActive(false);
        
        // Notify progress tracker that varnish solution was applied
        NotifyVarnishApplied();
    }
    
    private void NotifyVarnishApplied()
    {
        var progressTracker = FindFirstObjectByType<ScenarioProgressTracker>();
        if (progressTracker != null)
        {
            // Check if correct concentration was used
            var scenarioManager = FindFirstObjectByType<ScenarioManager>();
            if (scenarioManager != null && scenarioManager.CurrentScenario != null)
            {
                var scenario = scenarioManager.CurrentScenario;
                if (scenario.requiresVarnishRemoval)
                {
                    float requiredConc = scenario.requiredVarnishConcentration;
                    float actualConc = concentrationPercent;
                    float tolerance = 10f; // Allow 10% tolerance
                    
                    bool correctConcentration = Mathf.Abs(actualConc - requiredConc) <= tolerance;
                    progressTracker.MarkVarnishRemoved(correctConcentration);
                }
            }
        }
    }
    public bool IsFull => isFull;
}
