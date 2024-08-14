using InventoryDiablo;
using UnityEngine;
using Weapons;
using Zenject;

namespace MyProject
{
    public class Spawner : MonoBehaviour
    {        
        [Inject] private DiContainer diContainer;
        // public IWeapon SpawnWeapon(GameObject prefab) => Instantiate(prefab).GetComponent<IWeapon>();
        public WeaponBase SpawnWeaponOnUnit(InventoryItem item)
        {
            item.ItemData.Prefab.GetRigidbody().isKinematic = true;

            item.ItemData.Prefab.GetCollider().enabled = false;
            
            WeaponBase weapon = Instantiate(item.ItemData.Prefab).GetComponent<WeaponBase>();
            
            weapon.InventoryItem = item;
            
            return weapon;
        }

        public void SpawnOnStreet(ItemOnstreet prefab, InventoryItem item, Transform ownerTransform)
        {
            Vector3 vector = ownerTransform.position + ownerTransform.forward * 0.8f;

            ItemOnstreet itemWorld = Instantiate(prefab, new Vector3(vector.x, vector.y + 1, vector.z), Quaternion.identity);

            itemWorld.GetRigidbody().isKinematic = false;
            
            itemWorld.GetCollider().enabled = true;
            
            itemWorld.GetRigidbody().AddForce(ownerTransform.forward * 1.5f, ForceMode.Impulse);

            itemWorld.SetupItem(item, TextGo);
        }

        public GameObject TextGo;
        public GameObject HitBloodEffect;
        public GameObject HitDirtEffect;
        public GameObject MK3Dead;
        public GameObject MK3;

        public Transform spownPoint;

        public void SpawnSpder() => diContainer.InstantiatePrefab(MK3, spownPoint.position, Quaternion.identity, null);
    }
}