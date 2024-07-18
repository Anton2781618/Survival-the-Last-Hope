using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mech : MonoBehaviour
{
    [SerializeField] legData[] legs;
    [SerializeField] private float stepLength = 0.75f;
    public bool isAttak = false;



    public bool IsAttacking { get; set; }

    [ContextMenu("Init")]
    private void Init()
    {
        //создать объект LegsReycasts и удочерить его
        var LegsReycasts = new GameObject("LegsReycasts"); 
        LegsReycasts.transform.SetParent(transform);

        //сбросить позицию и поворот
        LegsReycasts.transform.localPosition = Vector3.zero;

        //пройтись по legs и создать для каждого объект LegRaycast
        for (int index = IsAttacking ? 0 : 1; index < legs.Length; index++)
        {
            ref var legData = ref legs[index];

            var legRaycast = new GameObject($"{legData.Leg.transform.name} LegRaycast {index}");
            legRaycast.transform.SetParent(LegsReycasts.transform);
            
            //установить позицию 
            legRaycast.transform.localPosition = legData.Leg.transform.localPosition;
            legRaycast.transform.localPosition = new Vector3(legRaycast.transform.localPosition.x, 1, legRaycast.transform.localPosition.z);
            
            legData.Raycast = legRaycast.AddComponent<LegRaycast>();

        }

    }

    private void Update() 
    {
        if(isAttak)
        {
            legs[0].Leg.MoveTo(legs[0].Raycast.Position);
        }

        for (int index = isAttak == false ? 0 : 1; index < legs.Length; index++)
        {
            ref var legData = ref legs[index];

            //дистанция между ногой и точкой касания
            var distance = Vector3.Distance(legData.Leg.Position, legData.Raycast.Position);
            
            if(!CanMove(index) /* && distance < 1 */) continue;

            if(!legData.Leg.IsMoving && !(distance > stepLength)) continue;
            
            legData.Leg.MoveTo(legData.Raycast.Position);
        }            
    }

    private bool CanMove(int index)
    {
        var legsCount = legs.Length;
        var n1 = legs[(index + legsCount - 1) % legsCount];
        var n2 = legs[(index + 1) % legsCount];

        return !n1.Leg.IsMoving && !n2.Leg.IsMoving;
    }

    [Serializable]
    private struct legData
    {
        public LegTarget Leg;
        public LegRaycast Raycast;
    }
}
