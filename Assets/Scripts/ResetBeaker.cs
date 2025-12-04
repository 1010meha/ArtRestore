using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ResetBeakerButton : MonoBehaviour
{
    public BeakerManager beaker;

    private void OnMouseDown()
    {
        if (beaker != null)
            beaker.ResetBeaker();
    }
}
