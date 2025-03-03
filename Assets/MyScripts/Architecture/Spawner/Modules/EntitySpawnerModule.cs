using InventoryDiablo;
using ModularEventArchitecture;
using NUnit.Framework.Internal;
using UnityEngine;
using Weapons;
using Zenject;

namespace ModularEventArchitecture
{
    [CompatibleUnit(typeof(SpawnerManager))]
    public class EntitySpawnerModule : ModuleBase
    {        
        //---------------------------------------------------
        [Inject] private DiContainer diContainer;

        //---------------------------------------------------
        public GameObject HitBloodEffect;
        public GameObject HitDirtEffect;
        public GameObject MK3Dead;
        public GameObject MK3;

        public Transform spownPoint;

        //!---------------------------------------------------

        public override void Initialize()
        {
            Entity.Globalevents.Add((EventsSpawner.SpawnUnitOnStreet, (data) => OnSpawnUnitOnStreet((EventDataUnit)data)));
        }

        public override void UpdateMe()
        {
            
        }
        void Test()
        {}

        public WeaponBase SpawnWeaponOnUnit(InventoryItem item)
        {
            item.ItemData.Prefab.GetRigidbody().isKinematic = true;

            item.ItemData.Prefab.GetCollider().enabled = false;
            
            WeaponBase weapon = Instantiate(item.ItemData.Prefab).GetComponent<WeaponBase>();
            
            weapon.InventoryItem = item;
            
            return weapon;
        }

        public void OnSpawnUnitOnStreet(EventDataUnit eventDataUnit)
        {
            // Vector3 vector = eventDataUnit.OwnerTransform.position + eventDataUnit.OwnerTransform.forward * 0.8f;

            //стрельнуть лучем из камеры сквозь курсор мышки
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 

            //получить точку
            Physics.Raycast(ray, out RaycastHit hit);

            // ItemOnstreet itemWorld = Instantiate(eventDataUnit.Item.ItemData.Prefab, new Vector3(vector.x, vector.y + 1, vector.z), Quaternion.identity);
            ItemOnstreet itemWorld = Instantiate(eventDataUnit.Item.ItemData.Prefab, new Vector3(hit.point.x, hit.point.y + 0.3f, hit.point.z), Quaternion.identity);

            itemWorld.GetRigidbody().isKinematic = false;
            
            itemWorld.GetCollider().enabled = true;
            
            // itemWorld.GetRigidbody().AddForce(eventDataUnit.OwnerTransform.forward * 1.5f, ForceMode.Impulse);

            itemWorld.SetupItem(eventDataUnit.Item);
        }

        public void SpawnSpder() => diContainer.InstantiatePrefab(MK3, spownPoint.position, Quaternion.identity, null);
    }
}