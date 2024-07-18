using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegRaycast : MonoBehaviour
{
    private RaycastHit hit;
    private RaycastHit hit2;
    private Transform myTransform;
    public Vector3 Position => hit.point;
    public Vector3 Normal => hit.normal;

    private void Awake() 
    {
        myTransform = transform;        
    }

    void Update()
    {
        var ray = new Ray(myTransform.position, -myTransform.up);
        if(Physics.Raycast(ray, out hit2, 5))
        {
            //проверка на нормальное положение пола
            if(hit2.normal.y < 1f) 
            {
                //продвигать луч назад пока не найдем нормальное положение
                while(hit2.normal.y < 1f)
                {
                    if(!Physics.Raycast(ray, out hit2, 5)) break;
                    ray.origin += myTransform.forward * 0.1f;
                }
            
            }
            else
            {
                hit = hit2;
            }
            
            Debug.DrawLine(myTransform.position, hit.point, Color.red);
        }        
    }
}
