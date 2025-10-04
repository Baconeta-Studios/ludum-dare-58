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
        public int width = 20;
        public int height = 20;
        public float cellSize = 1f;

        [SerializeField] private List<GridCell> cells = new List<GridCell>();
        public GridCell[,] Grid { get; private set; }

        private void OnValidate() {
            BuildGrid();
        }

        private void Awake() {
            BuildGrid();
        }

        public void BuildGrid() {
            // Ensure we always have the right number of cells
            if (cells.Count != width * height) {
                cells.Clear();
                for (int x = 0; x < width; x++) {
                    for (int y = 0; y < height; y++) {
                        var worldPos = transform.position + new Vector3(
                            x * cellSize + cellSize / 2f,
                            0,
                            y * cellSize + cellSize / 2f
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

            var localPos = worldPos - transform.position;
            int x = Mathf.FloorToInt(localPos.x / cellSize);
            int y = Mathf.FloorToInt(localPos.z / cellSize);

            if (x >= 0 && x < width && y >= 0 && y < height) {
                return Grid[x, y];
            }
            return null;
        }

        private void OnDrawGizmos() {
            if (cells == null || cells.Count == 0) return;

            foreach (var cell in cells) {
                if (cell == null) continue;

                Gizmos.color = cell.walkable ? GetRoomColor(cell.roomID) : Color.black;
                Gizmos.DrawCube(cell.worldPosition, new Vector3(cellSize, 0.05f, cellSize));
                Gizmos.color = Color.white;
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
                default: return Color.white;
            }
        }
    }
}