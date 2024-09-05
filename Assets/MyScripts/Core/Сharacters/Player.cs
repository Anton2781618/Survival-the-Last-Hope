using InventoryDiablo;
using ModestTree;
using States;
using Units;
using UnityEngine;
using UnityEngine.InputSystem;
using Weapons;
using Zenject;
using TMPro;
using Unity.VisualScripting;
using System;

/// <summary>
/// Класс представляет из себя обертку над юнитом, позволяет управлять юнитом через один класс
/// </summary>
/// 
namespace MyProject
{
    public class Player : UnitHuman, IDestroyable, IInventorySystem, IMoveSystem, IWeaponStateSystem, IRaycastSysytem
    {
        [Inject] public IMuveHandler MuveHandler { get; set; }
        
        [Inject] public IWeaponStates WeaponHandler { get; set;}

        [Inject] public IRaycastHandler raycastHandler { get; set;}

        [Inject] public InventoryHandler InventoryHandler { get; set;}

        [Inject] public CharacterController CharacterControllerPlayer { get; }


        private void Awake()
        {
            InventoryHandler.InventoryUI.SetInventoryOwner(this);
            
            WeaponHandler.SetInventoryOwner(this);

            Helper.GameDataBase.AddUnit(CharacterControllerPlayer, this);
        }

        public void SetDamage()
        {
            Debug.Log(transform.name + " получил урон");
        }

        public void ToDressUp(UIInventoryItem item)
        {
            Debug.Log("Снять оружие!!! прикрутить это к инвентарю или к чему нить еще");
        }

        private void Update()
        {
            raycastHandler.UpdateMe();

            MuveHandler.UpdateMe();

            WeaponHandler.UpdateMe();

            SelectedObjectOnstreet();
            
            //!временно
            if(Input.GetKeyUp(KeyCode.I))
            {
                ShowInventory();
            }
        }

        public void ShowInventory()
        {
            InventoryHandler.InventoryUI.TurnInventory();
        }
        
        //!временно
        private void SelectedObjectOnstreet()
        {
            if(Input.GetKeyUp(KeyCode.E))
            {
                if(raycastHandler.GetHitCollider() == null)
                {
                    Debug.Log("луч не попал в объект");
                    return;
                } 
                
                Units.Unit itemWorld = raycastHandler.GetHitCollider().GetComponent<Units.Unit>();

                
                if(itemWorld == null)
                {
                    Debug.Log("Объект не найден");

                    return;
                } 

                itemWorld.Use(this);

                // InventoryController.Inventory.AddItem(itemWorld.GetItem());
                // InventoryController.InventoryUI.RefreshUI();
            } 
        }

        public void EquipItem(InventoryItem item)
        {
            Debug.Log("Одеть предмет");
            
            if(item.ItemData.TypeItem == ItemData.ItemType.Оружие)
            {
                if(!WeaponHandler.WeaponIsNull()) WeaponHandler.TakeAwayWeapon(item);
                
                WeaponHandler.SetWeapon(Helper.Spawner.SpawnWeaponOnUnit(item));
            }
            else
            {
                addClothes(item.ItemData.Prefab.gameObject);
            }
        }

        [ContextMenu("Показать инвентарь")]
        public void ChecksInventory()
        {
            InventoryHandler.Inventory.ShowInventory();
        }

        public void TakeOffItem(InventoryItem item)
        {
            if(!WeaponHandler.WeaponIsNull())
            {
                WeaponHandler.TakeAwayWeapon(item);
            }
        }
        
        public void DropItem(InventoryItem item)
        {
            Helper.Spawner.SpawnOnStreet(item.ItemData.Prefab, item, transform);

            InventoryHandler.Inventory.RemoveItem(item);
        }

        
        [SerializeField] private SkinnedMeshRenderer playerSkin;
        void addClothes(GameObject prefab)
        {
            GameObject clothObj = Instantiate(prefab, playerSkin.transform.parent.parent);
            SkinnedMeshRenderer[] renderers = clothObj.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                renderer.bones = playerSkin.bones;
                renderer.rootBone = playerSkin.rootBone;
            }
        }
        
        public GameObject clo;
        [ContextMenu("Одеть")]
        public void ADDCLOTHES()
        {
            addClothes(clo);
        }
    }
}
