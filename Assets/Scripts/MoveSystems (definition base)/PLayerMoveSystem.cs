using StarterAssets;
using UnityEngine;
using Zenject;

namespace MyProject
{
    public class PLayerMoveSystem : IMuveHandler
    {
        private StarterAssetsInputs inputs;
        private Transform bodyTransform;
        private Transform cameraTransform;
        private Animator animator;

        private Vector3 camForwarod;
        private Vector3 move;
        private Vector3 moveInput;
        private float forwardAmount;
        private float turnAmount; 
        
        [Inject]
        public PLayerMoveSystem(StarterAssetsInputs starterAssetsInputs, Animator anim)
        {
            inputs = starterAssetsInputs;

            bodyTransform = anim.transform;

            cameraTransform = Camera.main.transform;

            animator = anim;
        }

        public void UpdateMe()
        {
            HandleRotation();

            RotateToTarget();
        }

        
        private void RotateToTarget()
        {
            Vector3 mouseWorldPosition = Vector3.zero;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out RaycastHit hit, 50f))
            {
                mouseWorldPosition = hit.point;
            }
            
            Vector3 worldAimTarget = mouseWorldPosition;

            worldAimTarget.y = bodyTransform.position.y;
            
            Vector3 aimDirection = (worldAimTarget - bodyTransform.position).normalized;

            bodyTransform.forward = Vector3.Lerp(bodyTransform.forward, aimDirection, Time.deltaTime * 10f);
        }
        


        //определить поворот персонажа в зависимости от ввода
        private void HandleRotation()
        {
            float horizontal = Input.GetAxis("Horizontal");

            float vertical = Input.GetAxis("Vertical");
            
            camForwarod = Vector3.Scale(cameraTransform.up, new Vector3(1, 0, 1)).normalized;

            move = vertical * camForwarod + horizontal * cameraTransform.right;

            if(move.magnitude > 1f) move.Normalize();

            Move(move);
        }

        //перемещение персонажа
        private void Move(Vector3 move)
        {
            if(move.magnitude > 1f) move.Normalize();

            moveInput = move;

            ConvertMoveInput();

            UpdateAnimator();
        }

        //преобразование ввода перемещения в зависимости от камеры
        private void ConvertMoveInput()
        {
            Vector3 localMove = bodyTransform.InverseTransformDirection(moveInput);

            turnAmount = localMove.x;

            forwardAmount = localMove.z;
        }

        //обновление аниматора
        private void UpdateAnimator()
        {
            animator.SetFloat("Forward", inputs.sprint ? forwardAmount * 2 : forwardAmount , 0.1f, Time.deltaTime);

            animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
        }

        //!!явно не должно тут быть но пока не пойму как это убрать отсюда
        public void SetTarget(Transform target)
        {
            throw new System.NotImplementedException();
        }
        
        //!!явно не должно тут быть но пока не пойму как это убрать отсюда
        public bool TargetIsNull()
        {
            throw new System.NotImplementedException();
        }

        //!!явно не должно тут быть но пока не пойму как это убрать отсюда
        public bool IsMoveNow()
        {
            throw new System.NotImplementedException();
        }
    }
}