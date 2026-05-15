using UnityEngine;
using UnityEngine.Tilemaps;

/// Paints platforms on a Tilemap as the player climbs.
/// Attach to any GameObject in the scene. Assign the dirt tile asset in the Inspector.
public class ProceduralClimber : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TileBase tile;

    [Header("Spawn Window")]
    [Tooltip("How many units above the player to keep platforms painted")]
    [SerializeField] private float spawnLookahead = 20f;

    [Header("Vertical Spacing")]
    [SerializeField] private int minGapY = 2;
    [SerializeField] private int maxGapY = 4;

    [Header("Horizontal Placement")]
    [SerializeField] private int minX = -8;
    [SerializeField] private int maxX = 8;
    [Tooltip("Max horizontal shift from the previous platform center (tiles)")]
    [SerializeField] private int maxHorizontalStep = 5;

    [Header("Platform Size (tiles)")]
    [SerializeField] private int minWidth = 2;
    [SerializeField] private int maxWidth = 8;

    [Header("Starting Y")]
    [Tooltip("Tile-space Y row where generation begins")]
    [SerializeField] private int startTileY = 2;

    private Tilemap tilemap;
    private Transform player;
    private int nextTileY;
    private int prevCenterX;
    private void Start()
    {
        tilemap = FindObjectOfType<Tilemap>();
        player = FindObjectOfType<PlayerMover>().transform;
        prevCenterX = Random.Range(minX, maxX + 1);
        nextTileY = startTileY;
    }

    private void Update()
    {
        int playerTileY = tilemap.WorldToCell(player.position).y;

        while (nextTileY < playerTileY + WorldToTile(spawnLookahead))
            SpawnNext();
    }

    private void SpawnNext()
    {
        int width = Random.Range(minWidth, maxWidth + 1);

        int cx = Mathf.Clamp(prevCenterX + Random.Range(-maxHorizontalStep, maxHorizontalStep + 1), minX, maxX);

        int left = cx - width / 2;
        int right = left + width - 1;

        for (int x = left; x <= right; x++)
            tilemap.SetTile(new Vector3Int(x, nextTileY, 0), tile);

        prevCenterX = cx;
        nextTileY += (int)Random.Range(minGapY, maxGapY + 1);
    }

    private int WorldToTile(float worldUnits)
    {
        return Mathf.RoundToInt(worldUnits / tilemap.layoutGrid.cellSize.y);
    }
}
