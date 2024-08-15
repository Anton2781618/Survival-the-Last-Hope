using Units;
using UnityEngine;
using Weapons;
using Zenject;

namespace MyProject
{
    public class Spider : Unit, IDestroyable, IMoveSystem
    {
        GameObject target;
        public Animator Animator;

        [SerializeField] private bool isWork = true;

        public int hp = 100;

        [Inject] public IMuveHandler MuveHandler { get; }

        public void SetDamage()
        {
            hp -= 100;
            if(hp <= 0)
            {
                Instantiate(Helper.Spawner.MK3Dead, transform.position, transform.rotation);

                Destroy(gameObject);
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
