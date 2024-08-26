using Units;
using UnityEngine;
using UnityEngine.AI;
using Weapons;
using Zenject;

namespace MyProject
{
    public class Spider : Unit, IDestroyable, IMoveSystem
    {
        public Animator Animator;
        public NTC.Global.System.RagdollOperations RagdollOperations;
        public Collider MyCollider;
        public NavMeshAgent agent;

        [SerializeField] private bool isWork = true;

        public int hp = 100;

        [Inject] public IMuveHandler MuveHandler { get; }

        public void SetDamage()
        {
            hp -= 100;
            if(hp <= 0)
            {
                // Destroy(gameObject);
                // Instantiate(Helper.Spawner.MK3Dead, transform.position, transform.rotation);
                // Animator.enabled = false;

                // RagdollOperations.EnableRagdoll();
                
                MyCollider.enabled = false;
                
                agent.enabled = false;

                isWork = false;
                Animator.SetBool("IsDead", true);
                Animator.SetBool("walk", false);
                Animator.SetBool("Attack_1_bool", false);
            }
            
        }

        private void Update() 
        {
            if (isWork)
            {
                if(MuveHandler.TargetIsNull()) MuveHandler.SetTarget(GameObject.Find("Player").transform);
    
                MuveHandler.UpdateMe();

                if(!MuveHandler.IsMoveNow() && !MuveHandler.TargetIsNull())
                {
                    Debug.Log("Атакую");
                    
                    Animator.SetBool("walk", false);
                    // Animator.SetTrigger("Attack_1");
                    Animator.SetBool("Attack_1_bool", true);
                }
                else
                {
                    Animator.SetBool("Attack_1_bool", false);
                    Animator.SetBool("walk", true);
                }

            }    
        }
    }
}
