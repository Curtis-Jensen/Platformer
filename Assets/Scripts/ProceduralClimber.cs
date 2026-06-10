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
    // How far (in tiles) the next platform's center can shift from the previous one.
    // Larger values allow long horizontal leaps; smaller values keep platforms clustered.
    [SerializeField] private int maxHorizontalStep = 8;

    [Header("Platform Size (tiles)")]
    // Short platforms force more precise landings; wide platforms are forgiving.
    [SerializeField] private int minWidth = 2;
    [SerializeField] private int maxWidth = 6;

    [Header("Starting Y")]
    // Tile-space Y row where the first platform is placed.
    // Should be just above the ground so the player has somewhere to jump immediately.
    [SerializeField] private int startTileY = 2;

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

        // Seed the first platform center randomly so each run looks different.
        prevCenterX = Random.Range(minX, maxX + 1);
        nextTileY = startTileY;
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
    // Platform center X is constrained to shift no more than
    // maxHorizontalStep tiles from the previous platform, then
    // clamped to [minX, maxX]. This keeps the path connected
    // while still allowing dramatic diagonal leaps.
    //
    // The platform is painted tile-by-tile from left to right.
    // -------------------------------------------------------
    private void SpawnNext()
    {
        int width = Random.Range(minWidth, maxWidth + 1);

        // cx = center X tile of the new platform.
        // Steps randomly from the last platform's center, then clamps to the valid range.
        int cx = Mathf.Clamp(prevCenterX + Random.Range(-maxHorizontalStep, maxHorizontalStep + 1), minX, maxX);

        // Derive left/right edges from the center.
        // Integer division means odd widths round left (e.g. width=3 → left is 1 tile left of center).
        int left = cx - width / 2;
        int right = left + width - 1;

        for (int x = left; x <= right; x++)
            tilemap.SetTile(new Vector3Int(x, nextTileY, 0), tile);

        // Remember this center so the next platform steps from it.
        prevCenterX = cx;

        // Advance the frontier by a random vertical gap.
        nextTileY += Random.Range(minGapY, maxGapY + 1);
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
