using UnityEngine;
using UnityEngine.AI;
using Zenject;


namespace MyProject
{
    public class MoveSpiderHanler : IMuveHandler
    {
        private Transform target;
        [Inject] private NavMeshAgent agent;

        public void UpdateMe()
        {
            if (target) MoveToTarget();
        }

        public void SetTarget(Transform target) => this.target = target;

        public bool TargetIsNull() => target == null;

        public void ResetTarget() => target = null;

        public void MoveToTarget() => agent.SetDestination(target.position);

        public bool IsMoveNow()
        {
            if(!agent.pathPending) 
            {
                if(agent.remainingDistance <= agent.stoppingDistance) 
                {
                    if(!agent.hasPath || agent.velocity.sqrMagnitude == 0f) 
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}