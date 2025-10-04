using UnityEngine;
using UnityEngine.InputSystem;

namespace Movement
{
    public class PlayerClickMovement : MonoBehaviour {
        public GridManager gridManager;
        public LayerMask groundMask;
        public float moveSpeed = 3f;

        private PlayerControls controls;
        private Vector3 targetPosition;
        private bool moving = false;

        private void Awake() {
            controls = new PlayerControls();
        }

        private void OnEnable() {
            controls.Gameplay.Enable();
            controls.Gameplay.Interact.performed += OnClick;
        }

        private void OnDisable() {
            controls.Gameplay.Interact.performed -= OnClick;
            controls.Gameplay.Disable();
        }

        private void Update() {
            if (moving) {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );
                if (Vector3.Distance(transform.position, targetPosition) < 0.01f) {
                    moving = false;
                }
            }
        }

        private void OnClick(InputAction.CallbackContext ctx) {
            Debug.Log("Click performed!");

            Vector2 screenPos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f);


            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask)) {
                GridCell cell = gridManager.GetCell(hit.point);
                Debug.Log($"Hit {hit.collider.gameObject.name}, cell = {cell?.coordinates}, walkable = {cell?.walkable}");

                if (cell != null && cell.walkable) {
                    targetPosition = cell.worldPosition; // already centre of cell
                    moving = true;
                }
            }
        }
    }

}