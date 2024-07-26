using System.Collections;
using InventoryDiablo;
using Unity.VisualScripting.YamlDotNet.Core;
using UnityEngine;

namespace Weapons
{
    public class Weapon : WeaponBase, IWeapon
    {
        private Ray ray;
        private Camera cameraMain;

        private void Start() 
        {
            cameraMain = Camera.main;
        }

        //получить мировые координаты точки мышки
        private Vector3 GetMouseWold()
        {
            Vector3 mouseWorldPosition = Vector3.zero;

            Ray ray = cameraMain.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out RaycastHit hit, 50f))
            {
                mouseWorldPosition = hit.point;
            }
            
            Vector3 aimDirection = (mouseWorldPosition - muzzleFlashPoint.position).normalized;

            return aimDirection;
        }

        public override void Fire()
        {
            if(Time.time > nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / weaponModel.fireRate;

                if(!CheckMagazine())
                {
                    Debug.Log("Нет Обоймы");
                    
                    return;
                }
                else
                if(InventoryItem.combinedItems[InventoryDiablo.ItemData.ItemType.Обойма_патронов].Amount <= 0)
                {
                    Debug.Log("Обойма пуста");
                    
                    return;                
                }          
            
                ray.origin = muzzleFlashPoint.position;

                ray.direction = GetMouseWold();

                if(Physics.Raycast(ray, out RaycastHit hit, 100f, layerMask))
                {
                    if(hit.transform.tag == "Meat")
                    {
                        hitEffect = weaponModel.buletSpawner.HitBloodEffect;

                        hit.transform.GetComponent<Weapons.IDestroyable>().SetDamage(); 
                    } 
                    else
                    if(hit.transform.tag == "Dirt") hitEffect = weaponModel.buletSpawner.HitDirtEffect;
                
                    muzzleFlash.Play();

                    InventoryItem.combinedItems[InventoryDiablo.ItemData.ItemType.Обойма_патронов].Amount--;

                    InventoryItem.OnItemsChanged?.Invoke();

                    CreateShell();

                    CreateTracer(hit);

                    CreateHole(hit);
                }
            }
        }

        //спавн гильз
        private void CreateShell()
        {
            GameObject shell = Instantiate(weaponModel.shellPrefab, shellPoint.position, shellPoint.rotation);

            shell.GetComponent<Rigidbody>().AddForce(shellPoint.forward * Random.Range(1.5f, 5), ForceMode.VelocityChange);
        }

        private void CreateTracer(RaycastHit hit)
        {
            TrailRenderer tracer = Instantiate(tracerEffect, muzzleFlashPoint.transform.position, Quaternion.identity);

            tracer.AddPosition(muzzleFlashPoint.transform.position);
            
            tracer.transform.position = hit.point;
        }
        
        //спавн дырок
        private void CreateHole(RaycastHit hit)
        {
            GameObject hitEffectBufer = Instantiate(hitEffect);
            
            hitEffectBufer.transform.position = hit.point;

            // hitEffectBufer.transform.right = hit.normal;
            hitEffectBufer.transform.rotation = Quaternion.LookRotation(hit.normal);


            Instantiate(holeEffect, hitEffectBufer.transform.position, Quaternion.LookRotation(-hit.normal), hit.transform);
            // Instantiate(holeEffect, hitEffectBufer.transform.position, muzzleFlashPoint.rotation, hit.transform);
        }

        //проверить наличие обоймы в оружие
        public bool CheckMagazine() => InventoryItem.combinedItems.ContainsKey(InventoryDiablo.ItemData.ItemType.Обойма_патронов);



        public override void StopFire()
        {
            Debug.Log("StopFire");
        }

        public override void InsertClip(InventoryItem inventoryItem)
        {
            if(!CheckMagazine()) 
            {
                InventoryItem.InsertСombinedItems(InventoryDiablo.ItemData.ItemType.Обойма_патронов, inventoryItem);
            }
            else
            {
                Debug.Log("Обойма уже есть");
            }
        }

        public override InventoryItem RemoveClip()
        {
            Debug.Log("Извлекаю обойму");
            
            return InventoryItem.DetachСombinedItems(InventoryDiablo.ItemData.ItemType.Обойма_патронов);
            
            // StartCoroutine(RemoveClipCoroutine());
        }

        public override IEnumerator Reload(IInventorySystem IInventorySystem)
        {
            isReloading = true;

            yield return StartCoroutine(Delay(0.5f));


            InventoryItem clip = RemoveClip();

            InventoryItem newClip = IInventorySystem.InventoryController.Inventory.TakeTtem(InventoryItem, ItemData.ItemType.Обойма_патронов);

            if(clip != null) 
            {
                ItemGrid grid = IInventorySystem.InventoryController.InventoryUI.CheckFreeSpaceForItem(clip);

                if(!grid)
                {
                    IInventorySystem.DropItem(clip);
                } 
                else
                {
                    IInventorySystem.InventoryController.InventoryUI.CreateAndInsertItem(clip, grid);
                }
            }
            
            if(newClip == null) 
            {
                isReloading = false;

                yield break;
            }

            yield return StartCoroutine(Delay(0.1f));

            InsertClip(newClip);

            IInventorySystem.InventoryController.InventoryUI.DestroyInventoryItem(newClip);

            isReloading = false;
        }


        private IEnumerator Delay(float time)
        {

            yield return new WaitForSeconds(time);
           
        }

        
    }
}

