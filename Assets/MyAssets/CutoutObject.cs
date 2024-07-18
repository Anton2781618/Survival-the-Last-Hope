using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutoutObject : MonoBehaviour
{
    public Transform targetObject;

    public LayerMask wallMask;

    public Camera mainCcamera;

    public float cutout_Size = 0.1f;
    public float fallof_Size = 0.05f;
    public Vector2 offset;


    private void Update() 
    {
        Vector2 cutoutPos = mainCcamera.WorldToViewportPoint(targetObject.position);
        cutoutPos.y /= (Screen.width / Screen.height);

        Vector2 offset = targetObject.position - transform.position;
        RaycastHit[] hitObjects = Physics.RaycastAll(transform.position, offset, offset.magnitude, wallMask);

        for (int i = 0; i < hitObjects.Length; ++i)
        {
            Material[] materials = hitObjects[i].transform.GetComponent<Renderer>().materials;

            for (int m = 0; m < materials.Length; ++m)
            {
                materials[m].SetVector("_Cutout_Position", cutoutPos);
                materials[m].SetFloat("_Cutout_Size", cutout_Size);
                materials[m].SetFloat("_Fallof_Size", fallof_Size);
                // materials[m].SetVector("_Offset", offset);
            }
        }
    }
}
