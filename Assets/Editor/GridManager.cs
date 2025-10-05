namespace Editor
{
    using UnityEngine;
    using UnityEditor;
    using Movement;

    [CustomEditor(typeof(GridManager))]
    public class GridManagerEditor : Editor {
        private GridManager _gridManager;

        private void OnEnable() {
            _gridManager = (GridManager)target;
        }

        private void OnSceneGUI() {
            var e = Event.current;

            if (e.type == EventType.MouseDown && e.button is 0 or 1) {
                var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                if (Physics.Raycast(ray, out var hit, 200f)) {
                    var cell = _gridManager.GetCell(hit.point);
                    if (cell != null) {
                        Undo.RecordObject(_gridManager, "Edit Grid Cell");

                        if (e.button == 0) {
                            // Left-click → toggle walkable
                            cell.walkable = !cell.walkable;
                        } else if (e.button == 1) {
                            // Right-click → cycle room ID
                            if (cell.roomID < 0) cell.roomID = 0;
                            else cell.roomID = (cell.roomID + 1) % 6; // 6 sample colours
                        }

                        // EditorUtility.SetDirty(_gridManager);
                    }
                }
                e.Use(); // mark event as used so Unity doesn't also select things
            }
        }
    }

}