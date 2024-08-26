using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFootSolver : MonoBehaviour
{
    [SerializeField] private LayerMask terrainLayer = default;
    [SerializeField] private Transform body = default;
    [SerializeField] private IKFootSolver otherFoot = default;
    [SerializeField] private float speed;
    [SerializeField] private float stepDistance;
    [SerializeField] private float stepLength;
    [SerializeField] private float stepHeight;
    [SerializeField] private Vector3 footOffset;
    private float footSpacing;
    Vector3 oldPosition, currentPosition, newPosition;
    Vector3 oldNormal, currentNormal, newNormal;
    private float lerp;

    void Start()
    {
        footSpacing = transform.localPosition.x;
        // currentPosition = newPosition = oldPosition = transform.position;
        currentPosition = body.position + footOffset;
        currentNormal = newNormal = oldNormal = transform.up;
        lerp = 0.9F;
    }

    public bool Mysystem = true;

    public Vector3 a;

    void Update()
    {
        
        if(Mysystem)
        {
            MySystem();
        }
        else
        {
            OldSystem();
        }
    }

    public bool step = false;

    private void MySystem()
    {
        transform.position = currentPosition;

        Ray ray = new Ray(body.position + footOffset, -body.up);


        if(Physics.Raycast(ray, out RaycastHit hit, 5, terrainLayer.value))
        {
            Debug.DrawLine(body.position + footOffset, hit.point, Color.red);

            Debug.Log(Vector3.Distance(currentPosition, hit.point) + " / " + (Vector3.Distance(currentPosition, hit.point) > stepDistance));

            if(Vector3.Distance(currentPosition, body.position) > stepDistance)
            {
                step = true;
                newPosition = body.position + footOffset;
            }

            if(Vector3.Distance(currentPosition, body.position) <= 0.3f)
            {
                step = false;

            }
            
            if(step && !otherFoot.step)
            {

                currentPosition = Vector3.Lerp(currentPosition, newPosition, Time.deltaTime * speed);
            }

        }
    }

    private void OldSystem()
    {
         transform.position = currentPosition;
        transform.up = currentNormal;


        // Ray ray = new Ray(body.position + (body.right + footOffset), Vector3.down);
        Ray ray = new Ray(body.position + footOffset, a);
        Debug.DrawRay(body.position + footOffset, a, Color.red);

        if(Physics.Raycast(ray, out RaycastHit hit, 1, terrainLayer.value))
        {
            if(Vector3.Distance(newPosition, hit.point) > stepDistance && !otherFoot.IsMoving() && lerp > 1)
            {
                lerp = 0;
                int direction = body.InverseTransformPoint(hit.point).z > body.InverseTransformPoint(newPosition).z ? 1 : -1;
                newPosition = hit.point + (body.forward * stepLength * direction) + footOffset;
                newNormal = hit.normal;
            }
        }

        if(lerp < 1)
        {
            Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            currentPosition = tempPosition;
            currentNormal.y = Mathf.Lerp(oldNormal.y, newNormal.y, lerp);
            lerp += Time.deltaTime * speed;
        }
        else
        {
            oldPosition = newPosition;
            oldNormal = newNormal;
        }
    }

    private void OnDrawGizmos() 
    {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(currentPosition, 0.1f);
    }
    
    public bool IsMoving()
    {
        return lerp < 1;
    }
}


