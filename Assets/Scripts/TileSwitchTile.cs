using UnityEngine;


public class TileSwitchTile : InteractableBase
{
    [Header("Tile Settings")]
    [Tooltip("Index of this tile (0 to totalTiles-1). Must be unique per tile.")]
    public int tileIndex = 0;

    [Tooltip("The puzzle manager that owns this tile.")]
    public TileSwitchPuzzle puzzleManager;

    [Header("Visuals")]
    [Tooltip("Material applied when this tile is correctly pressed.")]
    public Material pressedMaterial;

    [Tooltip("Material applied when this tile is pressed in wrong order.")]
    public Material wrongMaterial;

    private Material originalMaterial;
    private Renderer tileRenderer;
    private bool isPressed = false;

    // ── Unity Messages ───────────────────────────────────────

    private void Awake()
    {
        tileRenderer = GetComponent<Renderer>();

        if (tileRenderer != null)
            originalMaterial = tileRenderer.material;
    }

    private void Reset()
    {
        interactionPrompt = "Press F to Activate";
    }

    // ── InteractableBase Overrides ───────────────────────────

    public override string GetInteractionPrompt()
    {
        if (isPressed)
            return "Already Activated";

        return interactionPrompt;
    }

    public override void Interact(PlayerMovement player)
    {
        if (isPressed || puzzleManager == null)
            return;

        puzzleManager.OnTilePressed(tileIndex);
    }

    // ── Visual Feedback ──────────────────────────────────────

    public void SetPressed()
    {
        isPressed = true;

        if (tileRenderer != null && pressedMaterial != null)
            tileRenderer.material = pressedMaterial;
    }

    public void SetWrong()
    {
        if (tileRenderer != null && wrongMaterial != null)
            tileRenderer.material = wrongMaterial;

        Invoke(nameof(ResetVisual), 0.5f);
    }

    public void ResetVisual()
    {
        isPressed = false;

        if (tileRenderer != null && originalMaterial != null)
            tileRenderer.material = originalMaterial;
    }
}
