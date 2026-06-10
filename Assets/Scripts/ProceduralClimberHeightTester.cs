using UnityEngine;
using UnityEngine.Tilemaps;

// Generates a chain of worst-case jump platforms to let you manually verify
// that the player can survive every point on the jump-budget curve.
//
// Each step in the chain represents one sample along the budget curve, from
// pure-long (verticalFraction = 0) to pure-tall (verticalFraction = 1).
// Every platform is placed at the MAXIMUM allowed distance for its budget point --
// the hardest version of that jump. If the player can clear it, anything the
// procedural climber generates at that budget point will also be clearable.
//
// Set the parameters here to match your ProceduralClimber values, then attach
// this to a GameObject in a test scene. Hit Play and walk the chain.
//
// Remove this component (or the GameObject) before shipping -- it is a dev tool only.
public class ProceduralClimberHeightTester : MonoBehaviour
{
    // ========================
    // INSPECTOR CONFIG
    // ========================

    [Header("References")]
    // The tile asset used to paint the test platforms. Should match ProceduralClimber.
    [SerializeField] private TileBase tile;

    [Header("Budget Parameters")]
    // Copy these values from your ProceduralClimber to test the same jump envelope.
    [Tooltip("Minimum vertical gap (tiles) -- matches ProceduralClimber.minGapY")]
    [SerializeField] private int minGapY = 3;
    [Tooltip("Maximum vertical gap (tiles) -- matches ProceduralClimber.maxGapY")]
    [SerializeField] private int maxGapY = 7;
    [Tooltip("Max horizontal step at smallest gap -- matches ProceduralClimber.maxHorizontalStep")]
    [SerializeField] private int maxHorizontalStep = 8;
    [Tooltip("Min horizontal step at largest gap -- matches ProceduralClimber.minHorizontalStep")]
    [SerializeField] private int minHorizontalStep = 1;

    [Header("Test Setup")]
    // How many steps to sample along the budget curve.
    // 2 = just the two extremes. 5 gives a good spread. 10 is thorough.
    [SerializeField] private int sampleCount = 5;

    // Width of each test platform in tiles.
    [SerializeField] private int platformWidth = 4;

    // Tile-space position of the very first launch platform.
    [SerializeField] private Vector2Int startTile = new Vector2Int(0, 2);

    // ========================
    // UNITY LIFECYCLE
    // ========================

    // -------------------------------------------------------
    // Start()
    // Generates the full chain of test platforms immediately on play.
    // The chain starts at startTile and walks upward through each
    // budget sample, with each landing platform becoming the next launch.
    // -------------------------------------------------------
    private void Start()
    {
        Tilemap tilemap = FindObjectOfType<Tilemap>();

        int currentX = startTile.x;
        int currentY = startTile.y;

        // Paint the initial launch platform so the player has somewhere to stand.
        PaintPlatform(tilemap, currentX, currentY);

        for (int i = 0; i < sampleCount; i++)
        {
            // verticalFraction: 0 = pure long jump, 1 = pure tall jump.
            // Divide by (sampleCount - 1) so the last sample lands exactly at 1.0.
            float verticalFraction = (sampleCount > 1)
                ? (float)i / (sampleCount - 1)
                : 0f;

            // Derive this sample's gap and step using the same budget math as ProceduralClimber.
            int gapY = Mathf.RoundToInt(Mathf.Lerp(minGapY, maxGapY, verticalFraction));
            int allowedStep = Mathf.RoundToInt(Mathf.Lerp(maxHorizontalStep, minHorizontalStep, verticalFraction));

            // Always step right at maximum distance -- this is the hardest possible version.
            currentX += allowedStep;
            currentY += gapY;

            PaintPlatform(tilemap, currentX, currentY);

            Debug.Log($"[HeightTester] Sample {i + 1}/{sampleCount} | verticalFraction={verticalFraction:F2} | gapY={gapY} | horizontalStep={allowedStep} | landing=({currentX}, {currentY})");
        }
    }

    // ========================
    // HELPERS
    // ========================

    // -------------------------------------------------------
    // PaintPlatform(tilemap, centerX, tileY)
    // Paints a platform of platformWidth tiles centered on centerX at tileY.
    // -------------------------------------------------------
    private void PaintPlatform(Tilemap tilemap, int centerX, int tileY)
    {
        int left = centerX - platformWidth / 2;
        int right = left + platformWidth - 1;

        for (int x = left; x <= right; x++)
            tilemap.SetTile(new Vector3Int(x, tileY, 0), tile);
    }
}
