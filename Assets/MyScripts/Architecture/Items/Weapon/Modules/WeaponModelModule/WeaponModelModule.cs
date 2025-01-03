using System.Collections;
using System.Collections.Generic;
using InventoryDiablo;
using ModularEventArchitecture;
using UnityEngine;
using Weapons;

namespace Entitys.Weapon.Modules.WeaponModelModule
{
    public class WeaponModelModule : WeaponBaseModule
    {
        private Ray ray;
        private Camera cameraMain;
        
        private bool Shoot = false;

        public override void Initialize()
        {
            cameraMain = Camera.main;

            Entity.LocalEvents.Subscribe<EventBase>(Entitys.Player.Events.EventsWeapon.StartFire, OnStartFire);

            Entity.LocalEvents.Subscribe<EventBase>(Entitys.Player.Events.EventsWeapon.StopFire, OnStopFire);
        }

        public override void UpdateMe()
        {
            if(Shoot)
            {
                Fire();
            }
        }

        private void OnStartFire(EventBase eventBase)
        {
            Shoot = true;
            Debug.Log($"<color=green>Выстрел</color> на объекте {Entity.name}");
        }

        private void OnStopFire(EventBase @base)
        {
            Shoot = false;
            Debug.Log($"<color=green>Выстрел</color> на объекте {Entity.name}");
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
                nextTimeToFire = Time.time + 1f / WeaponModel.FireRate;

                // if(!CheckMagazine())
                // {
                //     Debug.Log("Нет Обоймы");
                    
                //     return;
                // }
                // else
                // if(InventoryItem.CombinedItems[InventoryDiablo.ItemData.ItemType.Обойма_патронов].Amount <= 0)
                // {
                //     Debug.Log("Обойма пуста");
                    
                //     return;                
                // }          
            
                ray.origin = muzzleFlashPoint.position;

                ray.direction = GetMouseWold();

                if(Physics.Raycast(ray, out RaycastHit hit, 100f, layerMask))
                {
                    if(hit.transform.tag == "Meat")
                    {
                        hitEffect = WeaponModel.buletSpawner.HitBloodEffect;

                        hit.transform.root.GetComponent<Weapons.IDestroyable>().SetDamage(); 
                    } 
                    else
                    if(hit.transform.tag == "Dirt") hitEffect = WeaponModel.buletSpawner.HitDirtEffect;
                
                    muzzleFlash.Play();

                    // InventoryItem.CombinedItems[InventoryDiablo.ItemData.ItemType.Обойма_патронов].Amount--;

                    // InventoryItem.OnItemsChanged?.Invoke();

                    CreateShell();

                    CreateTracer(hit);

                    CreateHole(hit);
                }
            }
        }

        //спавн гильз
        private void CreateShell()
        {
            GameObject shell = Instantiate(WeaponModel.shellPrefab, shellPoint.position, shellPoint.rotation);

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


            // Instantiate(holeEffect, hitEffectBufer.transform.position, Quaternion.LookRotation(-hit.normal), hit.transform);
            Instantiate(WeaponModel.HoleEffect, hitEffectBufer.transform.position, Quaternion.LookRotation(hit.normal), hit.transform);
            // Instantiate(holeEffect, hitEffectBufer.transform.position, muzzleFlashPoint.rotation, hit.transform);
        }

        //проверить наличие обоймы в оружие
        public bool CheckMagazine() => InventoryItem.CombinedItems.ContainsKey(InventoryDiablo.ItemData.ItemType.Обойма_патронов);



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

            InventoryItem newClip = IInventorySystem.InventoryHandler.Inventory.TakeTtem(InventoryItem, ItemData.ItemType.Обойма_патронов);

            if(clip != null) 
            {
                ItemGrid grid = IInventorySystem.InventoryHandler.InventoryUI.CheckFreeSpaceForItem(clip);

                if(!grid)
                {
                    IInventorySystem.DropItem(clip);
                } 
                else
                {
                    IInventorySystem.InventoryHandler.InventoryUI.CreateAndInsertItem(clip, grid);

                    IInventorySystem.InventoryHandler.Inventory.AddItem(clip);
                }
            }
            
            if(newClip == null) 
            {
                isReloading = false;

                yield break;
            }

            yield return StartCoroutine(Delay(0.1f));

            InsertClip(newClip);

            IInventorySystem.InventoryHandler.InventoryUI.DestroyInventoryItem(newClip);

            isReloading = false;
        }


        private IEnumerator Delay(float time)
        {

            yield return new WaitForSeconds(time);
           
        }

        
    }
}
