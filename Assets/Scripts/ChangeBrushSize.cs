using System;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class ChangeBrushSize : MonoBehaviour
{
    public float radius;
    public PaintColor colorScript;
   
    // Update is called once per frame
    void Update()
    {

        float t = Mathf.InverseLerp(5, 50, colorScript.erSize);
        radius = Mathf.Lerp(0.05f, 0.46f, t);
        transform.localScale = new Vector3(radius, radius, radius);

    }
}
