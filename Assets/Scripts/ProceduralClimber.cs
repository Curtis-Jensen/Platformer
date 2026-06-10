using UnityEngine;
using UnityEngine.Tilemaps;

// Paints platforms on a Tilemap as the player climbs upward.
// Attach to any GameObject in the scene. Assign the dirt tile asset in the Inspector.
// Generation is driven by the player's Y position -- platforms spawn ahead of the player
// and are never removed (this is an infinite climber, not a runner).
public class ProceduralClimber : MonoBehaviour
{
    // ========================
    // INSPECTOR CONFIG
    // ========================

    [Header("References")]
    // The tile asset painted onto the Tilemap to form each platform.
    [SerializeField] private TileBase tile;

    [Header("Spawn Window")]
    // How far above the player (in world units) to keep platforms generated.
    // If the player is at Y=10 and lookahead is 20, platforms exist up to Y≈30.
    // Too low and the player outruns generation; too high wastes tile memory.
    [SerializeField] private float spawnLookahead = 20f;

    [Header("Vertical Spacing")]
    // Minimum tile rows between the top of one platform and the next.
    // Raising minGapY makes every jump require effort; raising maxGapY adds tall leaps.
    [SerializeField] private int minGapY = 3;
    // Large maxGapY values create dramatic high jumps -- make sure the player's jump
    // height can actually reach this or it becomes an unwinnable gap.
    [SerializeField] private int maxGapY = 7;

    [Header("Horizontal Placement")]
    // Left and right tile-space boundaries for platform centers.
    // Platforms will never center outside this range.
    [SerializeField] private int minX = -8;
    [SerializeField] private int maxX = 8;
    // Maximum horizontal shift (tiles) when the vertical gap is at its smallest.
    // This is the "long jump" ceiling -- a nearly-flat hop can cover this full distance.
    [SerializeField] private int maxHorizontalStep = 8;
    // Minimum horizontal shift (tiles) allowed even when the vertical gap is at its largest.
    // Keeps tall jumps from being perfectly straight up -- there's still a little sideways movement.
    // Set to 0 if you want pure vertical jumps to be possible.
    [SerializeField] private int minHorizontalStep = 1;

    [Header("Platform Size (tiles)")]
    // Short platforms force more precise landings; wide platforms are forgiving.
    [SerializeField] private int minWidth = 2;
    [SerializeField] private int maxWidth = 6;

    [Header("Starting Platform")]
    // Tile-space position where the first platform is placed.
    // X sets the center of the first platform; Y sets its row.
    // All subsequent platforms step outward from this anchor.
    [SerializeField] private Vector2Int startTile = new Vector2Int(0, 2);

    // ========================
    // PRIVATE STATE
    // ========================

    // The Tilemap this script paints platforms onto.
    private Tilemap tilemap;

    // The player's transform, used each frame to decide how far ahead to generate.
    private Transform player;

    // The Y tile row where the NEXT platform will be placed.
    // Incremented by a random gap after each platform is spawned.
    private int nextTileY;

    // The center X tile coordinate of the most recently spawned platform.
    // Each new platform steps from here, so platforms form a connected (if challenging) path.
    private int prevCenterX;

    // ========================
    // UNITY LIFECYCLE
    // ========================

    // -------------------------------------------------------
    // Start()
    // Caches scene references and seeds the generation state.
    // Starts nextTileY at startTileY so the first platform appears
    // just above the ground where the player spawns.
    // -------------------------------------------------------
    private void Start()
    {
        tilemap = FindObjectOfType<Tilemap>();
        player = FindObjectOfType<PlayerMover>().transform;

        // Anchor the first platform at the fixed start position.
        // Subsequent platforms step randomly from here.
        prevCenterX = startTile.x;
        nextTileY = startTile.y;
    }

    // -------------------------------------------------------
    // Update()
    // Checks each frame whether the generation frontier is far
    // enough ahead of the player. Spawns platforms in a loop
    // until the lookahead window is filled. Multiple platforms
    // can spawn in a single frame if the player moves quickly
    // (e.g. on first load when the window is empty).
    // -------------------------------------------------------
    private void Update()
    {
        int playerTileY = tilemap.WorldToCell(player.position).y;

        // Keep generating until the next platform is beyond the lookahead window.
        while (nextTileY < playerTileY + WorldToTile(spawnLookahead))
            SpawnNext();
    }

    // ========================
    // GENERATION
    // ========================

    // -------------------------------------------------------
    // SpawnNext()
    // Places a single platform at nextTileY, then advances
    // nextTileY by a random vertical gap so the following call
    // lands higher up.
    //
    // Vertical gap and horizontal step share a "jump budget":
    // the taller the gap, the smaller the allowed horizontal shift,
    // and vice versa. This prevents any single jump from being
    // both high AND long, which could make it physically impossible.
    //
    // The platform is painted tile-by-tile from left to right.
    // -------------------------------------------------------
    private void SpawnNext()
    {
        int width = Random.Range(minWidth, maxWidth + 1);

        // Pick the vertical gap first -- this drives the horizontal budget.
        int gapY = Random.Range(minGapY, maxGapY + 1);

        // Normalize gapY to 0-1 within its possible range.
        // 0 = smallest gap (cheapest vertical spend), 1 = tallest gap (full vertical budget used).
        float verticalFraction = (float)(gapY - minGapY) / (maxGapY - minGapY);

        // Lerp the allowed horizontal step inversely: tall gap → small step, short gap → large step.
        // This is the "jump budget" tradeoff -- high OR long, not both.
        int allowedHorizontalStep = Mathf.RoundToInt(Mathf.Lerp(maxHorizontalStep, minHorizontalStep, verticalFraction));

        // cx = center X tile of the new platform.
        // Steps randomly within the budget from the last platform's center, then clamps to the valid range.
        int cx = Mathf.Clamp(prevCenterX + Random.Range(-allowedHorizontalStep, allowedHorizontalStep + 1), minX, maxX);

        // Derive left/right edges from the center.
        // Integer division means odd widths round left (e.g. width=3 → left is 1 tile left of center).
        int left = cx - width / 2;
        int right = left + width - 1;

        for (int x = left; x <= right; x++)
            tilemap.SetTile(new Vector3Int(x, nextTileY, 0), tile);

        // Remember this center so the next platform steps from it.
        prevCenterX = cx;

        // Advance the frontier by the gap we already committed to above.
        nextTileY += gapY;
    }

    // ========================
    // HELPERS
    // ========================

    // -------------------------------------------------------
    // WorldToTile(worldUnits)
    // Converts a world-space distance (e.g. spawnLookahead) into
    // tile rows using the Tilemap's cell height. Needed because
    // player.position is in world space but nextTileY is in tile space.
    // -------------------------------------------------------
    private int WorldToTile(float worldUnits)
    {
        return Mathf.RoundToInt(worldUnits / tilemap.layoutGrid.cellSize.y);
    }
}
