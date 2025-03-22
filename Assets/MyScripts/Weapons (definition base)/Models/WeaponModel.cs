using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Weapons
{
    [CreateAssetMenu(fileName = "WeaponModel", menuName = "My test/WeaponModel", order = 0)]
    public class WeaponModel : ScriptableObject 
    {
        public ParticleSystem muzzleFlash;
        public GameObject shellPrefab;
        public TrailRenderer tracerEffect;
        public LayerMask layerMask;
        public GameObject prefabWeapon;
        public BuletSpawner buletSpawner;
        public float FireRate = 0.5f;
        public AnimationCurve LerpCurve;
        public Vector3 LerpOffset;
        public GameObject HoleEffect;

        public WeaponBase Weapon
        {
            get 
            {
                return Instantiate(prefabWeapon).GetComponent<WeaponBase>(); 
            }
        }

        public List<WeaponPositions> WeaponStates; 
        public List<WeaponPositions> AnimationsLayers; 

        public enum WeaponPositionsStates
        {
            OrReady,
            Aim,
            OffAim,
            Reload
        }

        [ContextMenu("Save")]
        public void Save()
        {
            #if UNITY_EDITOR
            AssetDatabase.SaveAssets();
            #endif
        }

    }

    [Serializable]
    public class WeaponPositions
    {
        public string Name;
        public List<AnimationPoint> AnimationsPoints; 
    } 

    [Serializable]
    public class AnimationPoint
    {
        [Header("Правая рука")]
        public bool UseRightWeight = true; 
        public Vector3 RightHendPosition;
        public Quaternion RightHendRotation;
        public AnimationCurve LerpCurveRightHend;
        public Vector3 LerpOffsetRightHend;
        

        [Header("Левая рука")]
        public bool UseLeftWeight = true; 
        public Vector3 LeftHendPosition;
        public Quaternion LeftHendRotation;
        public AnimationCurve LerpCurveLeftHend;
        public Vector3 LerpOffsetLeftHend;
        
        public void SetPositionsRightHand(Transform rightHend)
        {
            RightHendPosition = rightHend.localPosition;

            RightHendRotation = rightHend.localRotation;
        }
        public void SetPositionsLeftHand(Transform LeftHend)
        {
            LeftHendPosition = LeftHend.localPosition;

            LeftHendRotation = LeftHend.localRotation;
        }
    }
}