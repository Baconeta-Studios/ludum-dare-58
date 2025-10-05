using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Movement
{
    public class AiMovementController : MonoBehaviour
    {
        [Header("Grid Movement")]
        public GridManager gridManager;
        public GridPathfinder pathfinder;
        public float moveSpeed = 2f;
        public bool useJumpAnimation = false;
        public float jumpHeight = 0.5f;
        public float jumpSpeed = 5f;

        [Header("Rotation")]
        public float rotateSpeed = 5f;   // new: how quickly they turn

        private Queue<GridCell> _path = new Queue<GridCell>();
        private bool _moving = false;

        private bool _isJumping = false;
        private Vector3 _jumpStart, _jumpEnd;
        private float _jumpProgress;

        private void Awake(){
            gridManager ??= FindFirstObjectByType<GridManager>();
            pathfinder ??= FindFirstObjectByType<GridPathfinder>();
        }

        private void Start()
        {
            PickNewDestination();
        }

        private void Update()
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
    }
}
