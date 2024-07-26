using System;
using System.Collections;
using InventoryDiablo;
using UnityEditor;
using UnityEngine;

namespace Weapons
{
    public abstract class WeaponBase : MonoBehaviour
    {
        [SerializeField] internal Transform muzzleFlashPoint;
        [SerializeField] internal Transform shellPoint;
        [SerializeField] protected WeaponModel weaponModel;
        internal ParticleSystem muzzleFlash;
        internal GameObject hitEffect;
        internal TrailRenderer tracerEffect;
        public GameObject holeEffect;
        public InventoryItem InventoryItem;
        internal LayerMask layerMask;
        internal float nextTimeToFire = 0f;
        internal bool isReloading = false;

        private void Awake() 
        {
            muzzleFlash = Instantiate(weaponModel.muzzleFlash, muzzleFlashPoint);

            tracerEffect = weaponModel.tracerEffect;

            layerMask = weaponModel.layerMask;
        }

        private void OnEnable() 
        {
            isReloading = false;
        }

        public abstract void Fire();
        public abstract InventoryItem RemoveClip();
        public abstract IEnumerator Reload(IInventorySystem inventorySystem);
        public abstract void InsertClip(InventoryItem inventoryItem);
        public abstract void StopFire();
        public bool IsReloadingNow() => isReloading; 
        public WeaponModel GetModel() => weaponModel;
        public Transform GetWeaponTransform() => transform;

        // убрать оружие в кабуру
        public void HolsterWeapon(Transform holster)
        {
            transform.parent = holster;
            
            transform.localPosition = new Vector3(1.268438f, 0.4823954f, -0.2450715f);

            transform.localRotation = Quaternion.Euler(-165.792f, -112.029f, 97.451f);
        }

        // выхватить оружие
        public void DrawWeapon(Transform heand)
        {
            transform.parent = heand;

            transform.localPosition = new Vector3(0.1287f, 0.0468f, -0.023f);            

            transform.localRotation = Quaternion.Euler(-165.91f, -112.011f, 97.224f);
        }

        public void DestructSelf() => Destroy(gameObject);
    }
}

