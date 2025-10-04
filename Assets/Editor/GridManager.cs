namespace Editor
{
    using UnityEngine;
    using UnityEditor;
    using Movement; // matches your namespace

    [CustomEditor(typeof(GridManager))]
    public class GridManagerEditor : Editor {
        private GridManager gridManager;

        private void OnEnable() {
            gridManager = (GridManager)target;
        }

        private void OnSceneGUI() {
            Event e = Event.current;

            if (e.type == EventType.MouseDown && (e.button == 0 || e.button == 1)) {
                // Raycast from mouse into the scene
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 200f)) {
                    GridCell cell = gridManager.GetCell(hit.point);
                    if (cell != null) {
                        Undo.RecordObject(gridManager, "Edit Grid Cell");

                        if (e.button == 0) {
                            // Left-click → toggle walkable
                            cell.walkable = !cell.walkable;
                        } else if (e.button == 1) {
                            // Right-click → cycle room ID
                            if (cell.roomID < 0) cell.roomID = 0;
                            else cell.roomID = (cell.roomID + 1) % 6; // 6 sample colours
                        }

                        EditorUtility.SetDirty(gridManager);
                    }
                }
                e.Use(); // mark event as used so Unity doesn't also select things
            }
        }
    }

}