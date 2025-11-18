using UnityEngine;
using System;

[RequireComponent(typeof(Collider2D))]
public class JigsawPiece : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("Assign the Transform representing this piece's correct target position.")]
    public Transform targetPosition;

    [Tooltip("How close the piece must be to snap into place.")]
    public float snapDistance = 0.5f;

    public bool IsPlaced { get; private set; } = false;

    public static event Action<JigsawPiece> OnPiecePlaced; // event for PuzzleManager

    private Vector3 offset;
    private bool isDragging = false;
    private SpriteRenderer spriteRenderer;
    private int originalSortingOrder;
    private static int sortingLayerCounter = 10; // brings dragged pieces to front

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalSortingOrder = spriteRenderer.sortingOrder;
    }

    private void OnMouseDown()
    {
        if (IsPlaced) return;

        // Bring piece to front
        if (spriteRenderer != null)
        {
            sortingLayerCounter++;
            spriteRenderer.sortingOrder = sortingLayerCounter;
        }

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mouseWorld.x, mouseWorld.y, transform.position.z);
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (!isDragging || IsPlaced) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mouseWorld.x, mouseWorld.y, transform.position.z) + offset;
    }

    private void OnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;

        // Snap check
        if (Vector2.Distance(transform.position, targetPosition.position) <= snapDistance)
        {
            SnapIntoPlace();
        }
    }

    private void SnapIntoPlace()
    {
        transform.position = targetPosition.position;
        IsPlaced = true;

        // Reset visual order
        if (spriteRenderer != null)
            spriteRenderer.sortingOrder = originalSortingOrder;

        // Disable collider to prevent re-dragging
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        OnPiecePlaced?.Invoke(this);
    }
}
