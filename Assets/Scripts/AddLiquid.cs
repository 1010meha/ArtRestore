using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AddLiquid : MonoBehaviour
{
    [Header("Beaker Reference")]
    public BeakerManager beaker;

    [Header("Dispense Settings")]
    public float fillRate = 0.5f;

    public bool isConcentrate;
    public ConcentrateTypes concentrateType;
    public BaseTypes baseType;

    private bool isPressed = false;

    public enum ConcentrateTypes { ConcentrateA, ConcentrateB, ConcentrateC }
    public enum BaseTypes { OilBase, WaterBase }

    private void Update()
    {
        if (isPressed && beaker)
        {
            float amount = fillRate * Time.deltaTime;
            beaker.AddLiquidFunction(amount, isConcentrate);
        }
    }

    private void OnMouseDown() => isPressed = true;
    private void OnMouseUp() => isPressed = false;
}
