using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LegTarget : MonoBehaviour
{
    [SerializeField] private float stepSpeed;
    [SerializeField] private AnimationCurve stepCurve;
    [SerializeField] private float HeightOffset = 0.3f;
    private Vector3 position;
    public Vector3 Position => position;

    private Movement? movement;
    public bool IsMoving => movement != null;
    private Transform myTransform;
    
    private void Start() 
    {
        myTransform = transform;

        position = transform.position;
    }

    void Update()
    {
        if(movement != null)
        {
            var m = movement.Value;
            m.Progress = Mathf.Clamp01(m.Progress + Time.deltaTime * stepSpeed);
            position = m.Evaluate(-myTransform.up, stepCurve);
            movement  = m.Progress < 1 ? m : null;
        }
            
        transform.position = position;
        transform.position = new Vector3(transform.position.x, transform.position.y + HeightOffset, transform.position.z);
    }

    public void MoveTo(Vector3 trgetPosition)
    {
        if(movement == null)
        {
            movement = new Movement
            {
                Progress = 0,
                FromPosition = position,
                ToPosition = trgetPosition
            };

        }
        else
        {
            movement = new Movement
            {
                Progress = movement.Value.Progress,
                FromPosition = movement.Value.FromPosition,
                ToPosition = trgetPosition,
            };
        }
    
    }

    private struct Movement
    {
        public float Progress;
        public Vector3 FromPosition;
        public Vector3 ToPosition;         

        //метод получения текущей позиции
        public Vector3 Evaluate(in Vector3 up, AnimationCurve stepCurve)
        {
            return Vector3.Lerp(@FromPosition, ToPosition, Progress) + up * stepCurve.Evaluate(Progress);
        }
    }
}
