using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Button that appears when restoration is complete
/// </summary>
public class CompletionButton : MonoBehaviour
{
    [Header("References")]
    public ScenarioManager scenarioManager;
    public Button button;
    
    [Header("UI")]
    public Text buttonText;
    
    private void Start()
    {
        if (button == null)
            button = GetComponent<Button>();
        
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
        
        // Hide by default
        gameObject.SetActive(false);
    }
    
    private void OnButtonClicked()
    {
        if (scenarioManager != null)
        {
            scenarioManager.OnCompletionButtonClicked();
        }
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

