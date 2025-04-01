using UnityEngine;
using UnityEngine.AI;

namespace ModularEventArchitecture
{
    public class AIModule : ModuleBase
    {
        //-----------------------------------------------------------
        [SerializeField] private GameObject _target; 
        //-----------------------------------------------------------
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private Animator _animator;
        //!-----------------------------------------------------------


        public override void Initialize()
        {
        }

        public override void UpdateMe()
        {
            if(_target)
            {
                _navMeshAgent.SetDestination(_target.transform.position);

                _animator.SetBool("Walk", _navMeshAgent.stoppingDistance < _navMeshAgent.remainingDistance);
            }            
        }

    }
}