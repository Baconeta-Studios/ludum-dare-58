using Coherence.Toolkit;
using UnityEngine;

namespace GameLogic
{
    public class AiSight : MonoBehaviour
    {
        [Header("Sight Settings")]
        public float viewRange = 6f;
        [Range(0, 180)] public float viewAngle = 90f;
        public LayerMask playerMask;
        public LayerMask obstructionMask;
        public float rotateSpeed = 5f;
        
        public bool canSeePlayer;
        public Transform currentTarget;

        private void Update()
        {
            HandleSight();

            if (canSeePlayer && currentTarget is not null)
            {
                RotateTowards(currentTarget.position);
            }
        }

        private void HandleSight()
        {
            canSeePlayer = false;
            currentTarget = null;

            var playersInSight = Physics.OverlapSphere(transform.position, viewRange, playerMask);
            foreach (var player in playersInSight)
            {
                var dirToTarget = (player.transform.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2f)
                {
                    var distToTarget = Vector3.Distance(transform.position, player.transform.position);
                    if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstructionMask))
                    {
                        canSeePlayer = true;
                        currentTarget = player.transform;
                        var sync = GetComponent<CoherenceSync>();
                        return;
                    }
                }
            }
        }

        private void RotateTowards(Vector3 targetPos)
        {
            var dir = (targetPos - transform.position).normalized;
            var lookRot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
            //transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotateSpeed);
        }
        
        public bool WitnessedMurder(Transform murderer)
        {
            return canSeePlayer && currentTarget == murderer;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, viewRange);

            var left = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
            var right = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + left * viewRange);
            Gizmos.DrawLine(transform.position, transform.position + right * viewRange);

            if (canSeePlayer && currentTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, currentTarget.position);
            }
        }
    }
}
