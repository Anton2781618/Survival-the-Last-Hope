using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

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

        public WeaponBase Weapon
        {
            get 
            {
                return Instantiate(prefabWeapon).GetComponent<WeaponBase>(); 
            }
        }

        public List<WeaponPositions> WeaponStates; 

        public enum WeaponPositionsStates
        {
            OrReady,
            Aim,
            Reload
        }

        [ContextMenu("Save")]
        public void Save()
        {
            AssetDatabase.SaveAssets();
        }

    }

    [Serializable]
    public class WeaponPositions
    {
        public string Name;
        public Vector3 RightHendPosition;
        public Quaternion RightHendRotation;
        public Vector3 LeftHendPosition;
        public Quaternion LeftHendRotation;

        public void SetPositionsRightHand(Transform rightHend)
        {
            RightHendPosition = rightHend.localPosition;

            RightHendRotation = rightHend.localRotation;

        }
    } 
}