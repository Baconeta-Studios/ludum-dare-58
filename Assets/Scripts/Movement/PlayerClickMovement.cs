using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Movement
{
    public class PlayerClickMovement : MonoBehaviour {
        public GridManager gridManager;
        public LayerMask groundMask;
        public float moveSpeed = 3f;
        public bool useJumpAnimation = false;
        public float jumpHeight = 0.5f;
        public float jumpSpeed = 5f;
        public bool useRotation = false;
        public float rotateSpeed = 5f;

        private bool isJumping = false;
        private Vector3 jumpStart;
        private Vector3 jumpEnd;
        private float jumpProgress;

        private PlayerControls controls;
        private Vector3 targetPosition;
        private bool moving = false;

        private Vector3 invalidWalkingPosition = new Vector3(9999, 9999, 9999);

        public GridPathfinder pathfinder;
        private Queue<GridCell> path = new Queue<GridCell>();

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
            Shader.SetGlobalInteger("_IsWalking", 0);
        }

        private void Update() {
            if (moving && path.Count > 0) {
                if (useJumpAnimation) {
                    HandleJumpMovement();
                } else {
                    HandleSmoothMovement();
                }
            }
            HoverHighlightCell(gridManager.GetCell(Vector3.zero));
        }

        private void HandleSmoothMovement() {
            var target = path.Peek().worldPosition;
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            var direction = (target - transform.position).normalized;
            if (useRotation && direction.sqrMagnitude > 0.001f) {
                var lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotateSpeed);
            }
            if (Vector3.Distance(transform.position, target) < 0.01f) {
                path.Dequeue();
                if (path.Count == 0) moving = false;
            }
        }
        
        private void HandleJumpMovement() {
            if (!isJumping) {
                jumpStart = transform.position;
                jumpEnd = path.Peek().worldPosition;
                jumpProgress = 0f;
                isJumping = true;
            }
            jumpProgress += Time.deltaTime * jumpSpeed;
            var t = Mathf.Clamp01(jumpProgress);
            var horizontal = Vector3.Lerp(jumpStart, jumpEnd, t);
            var height = Mathf.Sin(t * Mathf.PI) * jumpHeight;
            transform.position = new Vector3(horizontal.x, horizontal.y + height, horizontal.z);
            var direction = (jumpEnd - jumpStart).normalized;
            if (useRotation && direction.sqrMagnitude > 0.001f) {
                var lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotateSpeed);
            }
            if (t >= 1f) {
                path.Dequeue();
                isJumping = false;
                if (path.Count == 0) {
                    moving = false;
                    Shader.SetGlobalInteger("_IsWalking", 0);
                    Shader.SetGlobalVector("_WalkingWorldPosition", (Vector4)invalidWalkingPosition);
                }
            }
        }

        private void OnClick(InputAction.CallbackContext ctx) {
            var screenPos = controls.Gameplay.InteractPosition.ReadValue<Vector2>();
            var ray = Camera.main.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask)) {
                isJumping = false;
                jumpProgress = 0f;
                transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
                var start = gridManager.GetCell(transform.position);
                var goal = gridManager.GetCell(hit.point);
                if (start != goal) {
                    var newPath = pathfinder.FindPath(start, goal);
                    if (newPath != null && newPath.Count > 0) {
                        if (newPath[0] == start) newPath.RemoveAt(0);
                        path = new Queue<GridCell>(newPath);
                        moving = path.Count > 0;
                        Shader.SetGlobalInteger("_IsWalking", 1);
                        Shader.SetGlobalVector("_WalkingWorldPosition", (Vector4)goal.worldPosition);
                    }
                }
            }
        }

        private void HoverHighlightCell(GridCell hoveredCell) {
            Shader.SetGlobalInteger("_IsHovering", 1);
            Shader.SetGlobalVector("_HoverWorldPosition", (Vector4)hoveredCell.worldPosition);
        }
    }
}
