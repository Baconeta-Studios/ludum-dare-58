using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Movement
{
    public class PlayerClickInteraction : MonoBehaviour {
        [Header("Grid and Masks")]
        public GridManager gridManager;
        public LayerMask groundMask;
        private Interactable targetInteractable;
        
        [Space(20), Header("Movement")]
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

        private Vector3 _invalidWalkingPosition = new Vector3(9999, 9999, 9999);

        [Space(20), Header("Path Finding")]
        public GridPathfinder pathfinder;
        private Queue<GridCell> path = new Queue<GridCell>();

        [Space(20), Header("Interaction")]
        public InteractionType currentInteractionType = InteractionType.Collect;
        [Serializable]public enum InteractionType
        {
            Collect,
            Inspect,
            Murder,
            Scare
        }
        
        public LayerMask interactableMask;

        [Tooltip("Maximum radius to interact with objects")] 
        public int interactionRadius = 1;

        private void Awake() {
            controls = new PlayerControls();

            gridManager ??= FindFirstObjectByType<GridManager>();
            pathfinder ??= FindFirstObjectByType<GridPathfinder>();
            
        }

        // TODO this will only work for prototype single-player
        private void Start() {
            FindFirstObjectByType<CardUIManager>().Init(this);
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
            Vector3 target = path.Peek().worldPosition;
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            
            var direction = (target - transform.position).normalized;
            if (useRotation && direction.sqrMagnitude > 0.001f) {
                var lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotateSpeed);
            }
            if (Vector3.Distance(transform.position, target) < 0.01f) {
                path.Dequeue();
                if (path.Count == 0)
                {
                    ArrivedAtTarget();
                }
            }
        }
        
        private void HandleJumpMovement() {
            if (targetInteractable && targetInteractable.doesMove)
            {
                GridCell nearestCell = GetNearestCell(targetInteractable.transform.position);
                
                if (nearestCell != path.Last())
                {
                    PathToCell(nearestCell);
                }
            }
            
            if (!isJumping) {
                jumpStart = transform.position;
                jumpEnd = path.Peek().worldPosition;
                jumpProgress = 0f;
                isJumping = true;
            }

            jumpProgress += Time.deltaTime * jumpSpeed;
            float t = Mathf.Clamp01(jumpProgress);

            Vector3 horizontal = Vector3.Lerp(jumpStart, jumpEnd, t);
            float height = Mathf.Sin(t * Mathf.PI) * jumpHeight;
            transform.position = new Vector3(horizontal.x, horizontal.y + height, horizontal.z);
            
            var direction = (jumpEnd - jumpStart).normalized;
            if (useRotation && direction.sqrMagnitude > 0.001f) {
                var lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotateSpeed);
            }
            
            if (t >= 1f) {
                path.Dequeue();
                isJumping = false;

                if (path.Count == 0)
                {
                    ArrivedAtTarget();
                }
            }
        }

        private void ArrivedAtTarget(){
            moving = false;
            Shader.SetGlobalInteger("_IsWalking", 0);
            Shader.SetGlobalVector("_WalkingWorldPosition", (Vector4)_invalidWalkingPosition);
                
            if (targetInteractable)
            {
                InteractWithTarget();
            }
        }

        private void OnClick(InputAction.CallbackContext ctx) {
            Vector2 screenPos = controls.Gameplay.InteractPosition.ReadValue<Vector2>();

            Ray ray = Camera.main.ScreenPointToRay(screenPos);

            if (!TryInteract(ray))
            {
                TryMove(ray);
            }
        }

        private bool TryInteract(Ray ray){
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, interactableMask))
            {
                targetInteractable = hit.collider.GetComponent<Interactable>();
                
                if (Vector3.Distance(transform.position, targetInteractable.transform.position) > interactionRadius)
                {
                    PathToCell(GetNearestCell(hit.point));
                }
                else
                {
                    InteractWithTarget();
                }
                return true;
            }

            return false;
        }

        private GridCell GetNearestCell(Vector3 worldPosition){
            GridCell nearestGoal = gridManager.GetNearestWalkableCell(worldPosition,
                transform.position,
                interactionRadius,
                !targetInteractable.occupiesCell,
                targetInteractable.doesMove);
            
            return nearestGoal;
        }

        private bool TryMove(Ray ray)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask)) {
                GridCell goal = gridManager.GetCell(hit.point);
                PathToCell(goal);
                return true;
            }

            return false;
        }

        private void PathToCell(GridCell goalCell)
        {
            isJumping = false;
            jumpProgress = 0f;
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

            GridCell start = gridManager.GetCell(transform.position);

            if (start != goalCell) {

                var newPath = pathfinder.FindPath(start, goalCell);
                if (newPath != null && newPath.Count > 0) {
                    if (newPath[0] == start) newPath.RemoveAt(0);
                    path = new Queue<GridCell>(newPath);
                    moving = path.Count > 0;
                    Shader.SetGlobalInteger("_IsWalking", 1);
                    Shader.SetGlobalVector("_WalkingWorldPosition", (Vector4)goalCell.worldPosition);
                }
            }
        }

        private void InteractWithTarget(){
            targetInteractable.Interact();
            targetInteractable = null;
        }

        private void HoverHighlightCell(GridCell hoveredCell) {
            Shader.SetGlobalInteger("_IsHovering", 1);
            Shader.SetGlobalVector("_HoverWorldPosition", (Vector4)hoveredCell.worldPosition);
        }
        
        public void SetInteraction(InteractionType type) {
            currentInteractionType = type;
        }
    }
}
