using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class Controll : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private NavMeshAgent agent;

    [Space]
    [Header("Объект для смещения центра тяжести при повороте")]
    public Transform root;

    [Space]
    [Header("Левая нога")]
    [SerializeField] private Transform leftFoot;
    private Vector3 targetLeftFootPos;
    [SerializeField] private Transform LeftFootRig;
    [SerializeField] private TwoBoneIKConstraint leftRig;

    [Space]
    [Header("Правая нога")]
    [SerializeField] private Transform rightFoot;
    private Vector3 targetRightFootPos;
    [SerializeField] private Transform rightFootRig;
    [SerializeField] private TwoBoneIKConstraint RightRig;

    [Space]
    [Header("Точка назначения")]
    public Transform target;

    public float speed = 2;

    private void Start() 
    {
        // animator = GetComponent<Animator>();
        root = animator.transform;
        agent = GetComponent<NavMeshAgent>();

        leftFoot = leftRig.data.tip;
        rightFoot = RightRig.data.tip;

        LeftFootRig = leftRig.data.target;
        rightFootRig = RightRig.data.target;

        SetPosLeftTarget();
        SetPosRightTarget();
        
    }

    private void Update() 
    {
        if(agent.enabled)
        {
            agent.SetDestination(target.position);
        } 

        
        leftRig.data.targetPositionWeight = animator.GetFloat("LeftCurv");
        RightRig.data.targetPositionWeight = animator.GetFloat("RightCurv");
        
        // if(leftRig.data.targetPositionWeight < 0.5f) FaceToPoint(targetLeftFootPos, target.position);
        LeftFootRig.position = targetLeftFootPos;
        // LeftFootRig.transform.rotation = targetLeftFootPos;

        rightFootRig.position = targetRightFootPos;
        // rightFootRig.transform.rotation = targetRightFootPos.rotation;
        // rightFootRig.transform.rotation = Quaternion.Lerp(rightFootRig.transform.rotation, targetRightFootPos.rotation, Time.deltaTime * 5);

        if(animator.GetFloat("LeftCurv") < 0)SetPosLeftTarget();
        if(animator.GetFloat("RightCurv") < 0)SetPosRightTarget();


        if(AngletToTarget(transform, target.position) > 90)
        {
            Vector3 direction = (target.position - targetLeftFootPos).normalized;
            if(direction.x > 0)
            {
                root.localPosition = math.lerp(root.localPosition, new Vector3(-1, 0, 0), Time.deltaTime * 5);
                RightRig.data.targetPositionWeight = math.lerp(RightRig.data.targetPositionWeight, 1, Time.deltaTime * 5);
            }
            else
            {
                root.localPosition = math.lerp(root.localPosition, new Vector3(-1, 0, 0), Time.deltaTime * 5);
                leftRig.data.targetPositionWeight = math.lerp(leftRig.data.targetPositionWeight, 1, Time.deltaTime * 5);
            }
        }
        else
        {
            root.localPosition = math.lerp(root.localPosition, new Vector3(0, 0, 0), Time.deltaTime * 5);
        }

        if(AngletToTarget(transform, target.position) > 90)
        {           
            animator.SetFloat("Blend", math.lerp(animator.GetFloat("Blend"), 0.7f, Time.deltaTime * 5));
            agent.speed = math.lerp(agent.speed, 0.05f, Time.deltaTime * 5);
        }
        else
        {
            animator.SetFloat("Blend", math.lerp(animator.GetFloat("Blend"), 1, Time.deltaTime * 5));
            agent.speed = math.lerp(agent.speed, speed, Time.deltaTime * 5);
        }
    }

    private void SetPosLeftTarget()
    {
        targetLeftFootPos = leftFoot.position;
    }

    private void SetPosRightTarget()
    {
        targetRightFootPos = rightFoot.position;
    }

    private void FaceToPoint(Transform obj,Vector3 target)
    {
        Vector3 direction = (target - obj.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        obj.rotation = Quaternion.Slerp(obj.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private float AngletToTarget(Transform obj, Vector3 target)
    {
        Vector3 direction = (target - obj.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        return Quaternion.Angle(obj.rotation, lookRotation);
    }

}
