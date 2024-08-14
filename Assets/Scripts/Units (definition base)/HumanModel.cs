using UnityEngine;
using UnityEngine.Animations.Rigging;
using System;

namespace Units
{
    [Serializable]
    public class HumanModel
    {
        public enum RigLayers
        {
            NoAim = 0,
            Aim = 1,
        }
        
        public Animator animator;
        public Transform hend;
        public RigBuilder rigBuilder;
        public Transform rigTargetRight;
        public Transform rigTargetLeft;
        public AnimationCurve lerpCurve;

        public Transform PistolHolster;
        public Transform RifleHolster;
        public Transform objectForLocalSpace;
        public Vector3 lerpOffset;
        public AnimationCurve lerpOffsetHend;
        
        //установить вес левой руки плеера
        public void SetWeightLeftHand(float lerpRatio)
        {
            rigBuilder.layers[1].constraints[1].weight = Mathf.InverseLerp(0, 1, lerpOffsetHend.Evaluate(lerpRatio));
        }

        public Transform GetLeftHand()
        {
            return rigBuilder.layers[1].constraints[1].component.transform;
        }

        //установить вес слоя rigBuilder
        public float SetWeightRigBuilderlayer(int layer, float weight, float lerpRatio)
        {
            return rigBuilder.layers[layer].rig.weight = Mathf.Lerp(rigBuilder.layers[layer].rig.weight, weight, lerpRatio);
        }
    }
    
}