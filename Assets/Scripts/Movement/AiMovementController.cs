using Coherence.Toolkit;
using System.Collections.Generic;
using UnityEngine;

namespace Movement
{
    public class AiMovementController : MonoBehaviour
    {
        private bool canMove = true;

        [Header("Grid Movement")]
        public GridManager gridManager;
        public GridPathfinder pathfinder;
        public float moveSpeed = 2f;
        public bool useJumpAnimation = false;
        public float jumpHeight = 0.5f;
        public float jumpSpeed = 5f;

        [Header("Rotation")]
        public float rotateSpeed = 5f;   // How quickly the pawn turns

        [Header("Spawn Placement")]
        [SerializeField] private bool onlyWalkable = true;
        [SerializeField] private int roomFilter = -1;
        [SerializeField] private bool snapToGround = true;
        [SerializeField] private float rayStartHeight = 3f;
        [SerializeField] private float yOffset = 0f;
        [SerializeField] private LayerMask groundMask = ~0;

        private Queue<GridCell> _path = new Queue<GridCell>();
        private bool _moving = false;

        private bool _isJumping = false;
        private Vector3 _jumpStart, _jumpEnd;
        private float _jumpProgress;
        private CoherenceSync coherenceSync;

        private void Awake()
        {
            gridManager ??= FindFirstObjectByType<GridManager>();
            pathfinder ??= FindFirstObjectByType<GridPathfinder>();
            coherenceSync = GetComponent<CoherenceSync>();
        }

        private void Start()
        {
            if (coherenceSync != null && coherenceSync.HasStateAuthority)
            {
                PlaceAtRandomCell();
                PickNewDestination();
            }
        }

        private void Update()
        {
            if (canMove && coherenceSync && coherenceSync.HasStateAuthority)
            {
                if (_moving && _path.Count > 0)
                {
                    if (useJumpAnimation) HandleJump();
                    else HandleSmooth();
                }
                else if (!_moving)
                {
                    if (Random.value < 0.01f)
                    {
                        PickNewDestination();
                    }
                }
            }
        }

        private void PlaceAtRandomCell()
        {
            if (gridManager == null || gridManager.Grid == null)
            {
                Debug.LogWarning($"{name}: GridManager not ready.");
                return;
            }

            var candidates = new List<GridCell>();
            foreach (var cell in gridManager.Grid)
            {
                if (cell == null) continue;
                if (onlyWalkable && !cell.walkable) continue;
                if (roomFilter >= 0 && cell.roomID != roomFilter) continue;
                candidates.Add(cell);
            }

            if (candidates.Count == 0) return;

            var chosen = candidates[Random.Range(0, candidates.Count)];
            var pos = chosen.worldPosition;

            if (snapToGround)
            {
                var origin = pos + Vector3.up * rayStartHeight;
                if (Physics.Raycast(origin, Vector3.down, out var hit, rayStartHeight * 2f, groundMask, QueryTriggerInteraction.Ignore))
                {
                    pos = hit.point + Vector3.up * yOffset;
                }
                else
                {
                    pos += Vector3.up * yOffset;
                }
            }

            transform.position = pos;
        }

        private void PickNewDestination()
        {
            var current = gridManager.GetCell(transform.position);

            var candidates = new List<GridCell>();
            foreach (var cell in gridManager.Grid)
            {
                if (cell is { walkable: true } && cell.roomID == current.roomID)
                {
                    candidates.Add(cell);
                }
            }

            if (candidates.Count == 0) return;
            var target = candidates[Random.Range(0, candidates.Count)];

            var newPath = pathfinder.FindPath(current, target);
            if (newPath is { Count: > 1 })
            {
                if (newPath[0] == current) newPath.RemoveAt(0);
                _path = new Queue<GridCell>(newPath);
                _moving = true;
            }
        }

        private void HandleSmooth()
        {
            var target = _path.Peek().worldPosition;
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            var direction = (target - transform.position).normalized;
            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotateSpeed);
            }

            if (Vector3.Distance(transform.position, target) < 0.01f)
            {
                _path.Dequeue();
                if (_path.Count == 0) _moving = false;
            }
        }

        private void HandleJump()
        {
            if (!_isJumping && _path.Count > 0)
            {
                _jumpStart = transform.position;
                _jumpEnd = _path.Peek().worldPosition;

                if (Vector3.Distance(_jumpStart, _jumpEnd) < 0.01f)
                {
                    _path.Dequeue();
                    return;
                }

                _jumpProgress = 0f;
                _isJumping = true;
            }

            _jumpProgress += Time.deltaTime * jumpSpeed;
            var t = Mathf.Clamp01(_jumpProgress);

            var horizontal = Vector3.Lerp(_jumpStart, _jumpEnd, t);
            var height = Mathf.Sin(t * Mathf.PI) * jumpHeight;
            transform.position = new Vector3(horizontal.x, horizontal.y + height, horizontal.z);

            var direction = (_jumpEnd - _jumpStart).normalized;
            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotateSpeed);
            }

            if (t >= 1f)
            {
                _path.Dequeue();
                _isJumping = false;
                if (_path.Count == 0) _moving = false;
            }
        }

        public void StopMovement()
        {
            canMove = false;
        }
    }
}
