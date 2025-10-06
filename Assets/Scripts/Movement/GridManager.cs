using System.Collections.Generic;

namespace Movement
{
    using UnityEngine;

    [System.Serializable]
    public class GridCell {
        public Vector2Int coordinates;
        public Vector3 worldPosition;
        public bool walkable = true;
        public int roomID = -1;
    }


    [ExecuteAlways]
    public class GridManager : MonoBehaviour {
        [Range(2, 50)] public int width = 20;
        [Range(2, 50)] public int height = 20;
        public float cellSize = 1f;
        [SerializeField] [Range(0, 1f)] private float cellTransparency = 0.8f;
        [SerializeField] [Range(0, 1f)] private float gridTransparency = 0.8f;

        [SerializeField] private List<GridCell> cells = new List<GridCell>();
        public GridCell[,] Grid { get; private set; }

        private void OnValidate() {
            RestrictWidthHeight();
            BuildGrid();

            Shader.SetGlobalFloat("_HalfCellSize", cellSize / 2);

        }

        private void Awake() {
            BuildGrid();
        }

        // Width or Height should always be an even number for simplicity.
        void RestrictWidthHeight()
        {
            if(width % 2 != 0) {
                width += 1;
            }

            if(height % 2 != 0) { 
                height += 1;
            }
        }

        [ContextMenu("Build Grid")]
        public void BuildGrid() {
            // Ensure we always have the right number of cells
            if (cells.Count != width * height) {
                cells.Clear();
                for (int x = 0; x < width; x++) {
                    for (int y = 0; y < height; y++) {

                        // Offset by half the size so that the game object is centered.
                        Vector2 offset = new Vector2(x - width / 2, y - height / 2) * cellSize;

                        var worldPos = transform.position + new Vector3(
                            offset.x + cellSize / 2f,
                            0,
                            offset.y + cellSize / 2f
                        );

                        cells.Add(new GridCell {
                            coordinates = new Vector2Int(x, y),
                            worldPosition = worldPos,
                            walkable = true,
                            roomID = -1
                        });
                    }
                }
            }

            // Rebuild the 2D array for fast runtime access
            Grid = new GridCell[width, height];
            foreach (var cell in cells) {
                if (cell.coordinates.x < width && cell.coordinates.y < height) {
                    Grid[cell.coordinates.x, cell.coordinates.y] = cell;
                }
            }
        }

        public GridCell GetCell(Vector3 worldPos) {
            if (Grid == null) return null;

            var localPos = transform.InverseTransformPoint(worldPos);
            int x = Mathf.FloorToInt(localPos.x / cellSize);
            int y = Mathf.FloorToInt(localPos.z / cellSize);

            int xIndex = x + width / 2;
            int yIndex = y + height / 2;
            if (xIndex >= 0 && xIndex < width && yIndex >= 0 && yIndex < height) {
                return Grid[xIndex, yIndex];
            }
            return null;
        }

        /// <summary>
        /// Gets the nearest walkable cell to the target within a radius.
        /// </summary>
        /// <param name="targetWorldPos">World position of the target</param>
        /// <param name="originWorldPos">World position of the origin</param>
        /// <param name="maxDistance">Maximum radius (circular)</param>
        /// <param name="canOccupySameCell">If the nearest walkable cell can be the same as the target cell</param>
        /// <param name="nearestWithinRadius">Whether to get the nearest cell to the *origin* within the radius.</param>
        /// <returns></returns>
        public GridCell GetNearestWalkableCell(Vector3 targetWorldPos, Vector3 originWorldPos, int maxDistance, bool canOccupySameCell, bool nearestWithinRadius) {
            if (Grid == null) return null;
            
            GridCell targetCell = GetCell(targetWorldPos);
            
            if (canOccupySameCell && targetCell != null && targetCell.walkable)
            {
                // Return origin cell if its walkable
                return targetCell;
            }
            else
            {
                // Search for nearby walkable cells.
                List<GridCell> nearbyWalkableCells = new List<GridCell>();

                for (int currentSearchRadius = 1; currentSearchRadius <= maxDistance; currentSearchRadius++)
                {
                    bool foundWalkableCellThisRadius = false;
                    
                    int radiusSquared = currentSearchRadius * currentSearchRadius;

                    for (int x = -currentSearchRadius; x <= currentSearchRadius; x++)
                    {
                        // Get the max Y for this X so that xSquared + ySquared ≤ radiusSquared
                        int maxY = (int) Mathf.Floor(Mathf.Sqrt(radiusSquared - (x * x)));

                        for (int y = -maxY; y <= maxY; y++)
                        {
                            GridCell cell = GetCell(targetWorldPos + new Vector3(x, 0, y));
                            if (cell != null && cell.walkable && cell != targetCell)
                            {
                                nearbyWalkableCells.Add(cell);
                                foundWalkableCellThisRadius = true;
                            }
                        }
                    }

                    if (!nearestWithinRadius && foundWalkableCellThisRadius)
                    {
                        break;
                    }
                }

                if (nearbyWalkableCells.Count > 0)
                {
                    // Find the closest nearby cell    
                    GridCell closestCell = nearbyWalkableCells[0];
                    float closestDistance = Vector3.Distance(closestCell.worldPosition, originWorldPos);

                    foreach (var cell in nearbyWalkableCells)
                    {
                        float distance = Vector3.Distance(cell.worldPosition, originWorldPos);
                        if (distance < closestDistance)
                        {
                            closestCell = cell;
                            closestDistance = distance;
                        }
                    }
                    
                    return closestCell;
                }
            }

            // There is no walkable tile within the distance.
            return null;
        }

        private void OnDrawGizmos() {
            if (cells == null || cells.Count == 0) return;

            foreach (var cell in cells) {
                if (cell == null) continue;

                Color cellColour = cell.walkable ? GetRoomColor(cell.roomID) : Color.black;
                cellColour.a = cellTransparency;
                Gizmos.color = cellColour;
                Gizmos.DrawCube(cell.worldPosition, new Vector3(cellSize, 0.05f, cellSize));

                Color gridColour = Color.white;
                gridColour.a = gridTransparency;
                Gizmos.color = gridColour;
                Gizmos.DrawWireCube(cell.worldPosition, new Vector3(cellSize, 0.05f, cellSize));
            }
        }

        private Color GetRoomColor(int roomID) {
            if (roomID < 0) return Color.gray;
            switch (roomID % 6) {
                case 0: return Color.red;
                case 1: return Color.green;
                case 2: return Color.blue;
                case 3: return Color.yellow;
                case 4: return Color.cyan;
                case 5: return Color.magenta;
                case 6: return Color.teal;
                case 7: return Color.rosyBrown;
                case 8: return Color.chocolate;
                case 9: return Color.indigo;
                case 10: return Color.olive;
                case 11: return Color.salmon;
                default: return Color.white;
            }
        }
    }
}