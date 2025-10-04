namespace Movement
{
    using UnityEngine;

    public class GridCell {
        public Vector2Int coordinates;
        public Vector3 worldPosition;
        public bool walkable = true;
        public int roomID = -1;  // -1 = unassigned
    }

    public class GridManager : MonoBehaviour {
        public int width = 20;
        public int height = 20;
        public float cellSize = 1f;

        private GridCell[,] _grid;

        private void Awake() {
            _grid = new GridCell[width, height];
            for (var x = 0; x < width; x++) {
                for (var y = 0; y < height; y++) {
                    var worldPos = transform.position + new Vector3(
                        x * cellSize + cellSize / 2f,
                        0,
                        y * cellSize + cellSize / 2f
                    );
                    _grid[x, y] = new GridCell {
                        coordinates = new Vector2Int(x, y),
                        worldPosition = worldPos,
                        walkable = true
                    };
                }
            }
        }

        public GridCell GetCell(Vector3 worldPos) {
            var localPos = worldPos - transform.position;

            var x = Mathf.FloorToInt(localPos.x / cellSize);
            var y = Mathf.FloorToInt(localPos.z / cellSize);

            if (x >= 0 && x < width && y >= 0 && y < height) {
                return _grid[x, y];
            }
            return null;
        }


        private void OnDrawGizmos() {
            if (_grid == null) return;
            Gizmos.color = Color.gray;
            for (var x = 0; x < width; x++) {
                for (var y = 0; y < height; y++) {
                    var pos = transform.position + new Vector3(
                        x * cellSize + cellSize / 2f,
                        0,
                        y * cellSize + cellSize / 2f
                    );
                    Gizmos.DrawWireCube(pos, new Vector3(cellSize, 0.05f, cellSize));

                }
            }
        }

    }
}