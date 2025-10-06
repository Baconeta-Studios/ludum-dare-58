namespace Movement
{
    using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridPathfinder : MonoBehaviour {
    public GridManager gridManager;

    public List<GridCell> FindPath(GridCell start, GridCell goal) {
        if (start == null || goal == null || !goal.walkable) return null;

        List<GridCell> openSet = new List<GridCell> { start };
        HashSet<GridCell> closedSet = new HashSet<GridCell>();
        Dictionary<GridCell, GridCell> cameFrom = new Dictionary<GridCell, GridCell>();

        Dictionary<GridCell, float> gScore = new Dictionary<GridCell, float> { [start] = 0 };
        Dictionary<GridCell, float> fScore = new Dictionary<GridCell, float> { [start] = Heuristic(start, goal) };

        while (openSet.Count > 0) {
            GridCell current = openSet.OrderBy(c => fScore.ContainsKey(c) ? fScore[c] : Mathf.Infinity).First();

            if (current == goal) {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (GridCell neighbor in GetNeighbors(current)) {
                if (!neighbor.walkable || closedSet.Contains(neighbor)) continue;

                float stepCost = (neighbor.coordinates.x == current.coordinates.x ||
                                  neighbor.coordinates.y == current.coordinates.y) ? 1f : 1.414f;

                float tentativeG = gScore[current] + stepCost;


                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor]) {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, goal);

                    if (!openSet.Contains(neighbor)) openSet.Add(neighbor);
                }
            }
        }

        return null; // no path found
    }

    private float Heuristic(GridCell a, GridCell b) {
        // Manhattan distance
        return Mathf.Abs(a.coordinates.x - b.coordinates.x) + Mathf.Abs(a.coordinates.y - b.coordinates.y);
    }

    private List<GridCell> GetNeighbors(GridCell cell) {
        List<GridCell> neighbors = new List<GridCell>();
        Vector2Int[] dirs = {
            new Vector2Int(1,0), new Vector2Int(-1,0),
            new Vector2Int(0,1), new Vector2Int(0,-1),
            new Vector2Int(1,1), new Vector2Int(1,-1),
            new Vector2Int(-1,1), new Vector2Int(-1,-1)
        };


        foreach (var d in dirs) {
            int nx = cell.coordinates.x + d.x;
            int ny = cell.coordinates.y + d.y;

            if (nx >= 0 && nx < gridManager.width && ny >= 0 && ny < gridManager.height) {
                neighbors.Add(gridManager.Grid[nx, ny]);
            }
        }
        return neighbors;
    }

    private List<GridCell> ReconstructPath(Dictionary<GridCell, GridCell> cameFrom, GridCell current) {
        List<GridCell> path = new List<GridCell> { current };
        while (cameFrom.ContainsKey(current)) {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }
}

}