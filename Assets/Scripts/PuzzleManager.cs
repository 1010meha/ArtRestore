using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Header("Puzzle Configuration")]
    [Tooltip("List of all jigsaw pieces in this puzzle.")]
    public JigsawPiece[] pieces;

    private int totalPieces;
    private int placedCount = 0;
    public GameObject finishedPainting;
    public GameObject buttons;

    private void Awake()
    {
        totalPieces = pieces.Length;
    }

    private void OnEnable()
    {
        JigsawPiece.OnPiecePlaced += HandlePiecePlaced;
    }

    private void OnDisable()
    {
        JigsawPiece.OnPiecePlaced -= HandlePiecePlaced;
    }

    private void HandlePiecePlaced(JigsawPiece piece)
    {
        placedCount++;

        if (placedCount >= totalPieces)
            OnPuzzleCompleted();
    }

    private void OnPuzzleCompleted()
    {
        finishedPainting.SetActive(true);
        buttons.SetActive(true);
        gameObject.SetActive(false);

    }
}
