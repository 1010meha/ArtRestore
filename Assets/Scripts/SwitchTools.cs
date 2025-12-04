using System;
using UnityEngine;

public class SwitchTools : MonoBehaviour
{
    public Gesso crackGesso;
    public Gesso overGesso;
    public RemoveVarnish removeVarnish;
    public RemovePigment removeGesso;
    public PaintColor paintCrack;
    public GameObject FluorecentCrack;
    public GameObject UVPainting;
    public GameObject UnderPainting;
    public GameObject varnishSetup;
    public GameObject buttons;
    public GameObject paintingParent;
    public GameObject brushSize;
    private float ogMousePosition;

    public static SwitchTools Instance;

    public enum Tools { UseGesso, RemoveGesso, UsePaint, UseVarnishRemover, VisibleLight, UVLight, InfraredLight, VarnishSetup }
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }       
        SetAllToFalse();

    }

    public void SetAllToFalse()
    {
        crackGesso.enabled = false;
        overGesso.enabled = false;
        removeVarnish.enabled = false;
        removeGesso.enabled = false;
        paintCrack.enabled = false;
        FluorecentCrack.SetActive(false);
        UVPainting.SetActive(false);
        UnderPainting.SetActive(false);
        varnishSetup.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (paintCrack.enabled)
        {
            
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                ogMousePosition = Input.mousePosition.x;
                brushSize.SetActive(true);
                brushSize.GetComponent<ChangeBrushSize>().colorScript = paintCrack;
            }
            if (Input.GetKey(KeyCode.Mouse1))
            {
                float t = Mathf.InverseLerp(0, 500f, Mathf.Abs(ogMousePosition - Input.mousePosition.x));
                paintCrack.erSize = Convert.ToInt32(Mathf.Lerp(5f, 50f, t));
            }
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                brushSize.SetActive(false);
            }
        }
    }
    public void UseGesso()
    {
        SetAllToFalse();
        crackGesso.enabled = true;
        overGesso.enabled = true;
      
    }

    public void RemoveGesso()
    {
        SetAllToFalse();
        removeGesso.enabled = true;
    }

    public void UsePaint()
    {
        SetAllToFalse();
        paintCrack.enabled = true;
    }

    public void UseVarnishRemover()
    {
        SetAllToFalse();
        removeVarnish.enabled = true;

    }

    public void VisibleLight()
    {
        SetAllToFalse();
        FluorecentCrack.SetActive(true);
    }
    public void UVLight()
    {
        SetAllToFalse();
        UVPainting.SetActive(true);
    }
    public void InfraredLight()
    {
        SetAllToFalse();
        UnderPainting.SetActive(true);
    }

    public void GoToVarnishSetup()
    {
        SetAllToFalse();
        buttons.SetActive(false);
        paintingParent.SetActive(false);
        varnishSetup.SetActive(true);

    }
}
