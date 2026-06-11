using UnityEngine;
using UnityEngine.Tilemaps;

// Generates a chain of worst-case jump platforms covering every possible gap value.
// One platform per gapY step from minGapY to maxGapY -- each placed at the maximum
// allowed horizontal distance for that gap via the budget curve.
//
// This lets you walk the full jump envelope and verify the player can survive every
// combination. If any jump is impossible, reduce maxGapY or maxHorizontalStep accordingly.
//
// Copy the parameter values and budgetCurve from your ProceduralClimber to match exactly.
// Remove this component (or its GameObject) before shipping -- dev tool only.
public class ProceduralClimberHeightTester : MonoBehaviour
{
    // ========================
    // INSPECTOR CONFIG
    // ========================

    [Header("References")]
    // The tile asset used to paint the test platforms. Should match ProceduralClimber.
    [SerializeField] private TileBase tile;

    [Header("Budget Parameters")]
    // Copy these values exactly from your ProceduralClimber.
    [Tooltip("Matches ProceduralClimber.minGapY")]
    [SerializeField] private int minGapY = 3;
    [Tooltip("Matches ProceduralClimber.maxGapY")]
    [SerializeField] private int maxGapY = 7;
    [Tooltip("Matches ProceduralClimber.maxHorizontalStep")]
    [SerializeField] private int maxHorizontalStep = 8;
    [Tooltip("Matches ProceduralClimber.minHorizontalStep")]
    [SerializeField] private int minHorizontalStep = 1;

    // Copy the exact same curve from ProceduralClimber so the tester reflects the real budget shape.
    // X axis: vertical fraction (0 = smallest gap, 1 = tallest gap).
    // Y axis: horizontal budget multiplier (0 = min step, 1 = max step).
    [SerializeField] private AnimationCurve budgetCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

    [Header("Starting Platform")]
    // Tile-space position of the first launch platform.
    [SerializeField] private Vector2Int startTile = new Vector2Int(0, 2);

    // ========================
    // UNITY LIFECYCLE
    // ========================

    // -------------------------------------------------------
    // Start()
    // Iterates over every integer gapY value from minGapY to maxGapY,
    // placing one platform per step at the maximum horizontal distance
    // allowed by the budget curve for that gap. Each landing becomes
    // the next launch, forming a single continuous chain.
    // Platform width is 1 tile -- the hardest possible landing target.
    // -------------------------------------------------------
    private void Start()
    {
        Tilemap tilemap = FindObjectOfType<Tilemap>();

        int currentX = startTile.x;
        int currentY = startTile.y;

        // Paint the initial launch platform.
        tilemap.SetTile(new Vector3Int(currentX, currentY, 0), tile);

        int gapRange = maxGapY - minGapY;

        for (int gapY = minGapY; gapY <= maxGapY; gapY++)
        {
            // Normalize this gap to 0-1 within the full gap range.
            // 0 = flattest/longest jump, 1 = tallest/shortest jump.
            float verticalFraction = (gapRange > 0)
                ? (float)(gapY - minGapY) / gapRange
                : 0f;

            // Sample the budget curve to get the horizontal multiplier for this gap.
            float horizontalMultiplier = Mathf.Clamp01(budgetCurve.Evaluate(verticalFraction));

            // Max allowed horizontal step at this gap -- worst-case distance for this budget point.
            int allowedStep = Mathf.RoundToInt(Mathf.Lerp(minHorizontalStep, maxHorizontalStep, horizontalMultiplier));

            currentX += allowedStep;
            currentY += gapY;

            // Paint a single tile -- smallest possible landing, maximum difficulty.
            tilemap.SetTile(new Vector3Int(currentX, currentY, 0), tile);

            Debug.Log($"[HeightTester] gapY={gapY} | verticalFraction={verticalFraction:F2} | horizontalStep={allowedStep} | landing=({currentX}, {currentY})");
        }
    }
}
