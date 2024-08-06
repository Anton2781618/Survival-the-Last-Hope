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
        
        [Inject] public IWeaponStates WeaponStates { get; set;}

        [Inject] public IRaycastHandler raycastHandler { get; set;}

        [Inject] public InventoryHandler InventoryHandler { get; set;}

        [Inject] public CharacterController CharacterControllerPlayer { get; }


        private void Awake()
        {
            InventoryHandler.InventoryUI.SetInventoryOwner(this);
            
            WeaponStates.SetInventoryOwner(this);

            WeaponStates.StateCompleted += UpdateInventory;

            Helper.GameDataBase.AddUnit(CharacterControllerPlayer, this);
        }

        [ContextMenu("проверить")]
        public void dasd()
        {
            Debug.Log(Helper.GameDataBase);
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

            WeaponStates.UpdateMe();

            SelectedObjectOnstreet();
            
            //!временно
            if(Input.GetKeyUp(KeyCode.I))
            {
                ShowInventory();
            }
        }

        public void ShowInventory()
        {
            InventoryHandler.InventoryUI.ShowInventory();
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
            //!! временно
            if(item.ItemData.TypeItem != ItemData.ItemType.Оружие) return;

            if(!WeaponStates.WeaponIsNull())
            {
                WeaponStates.TakeAwayWeapon();
            }
            
            WeaponStates.SetWeapon(Helper.Spawner.SpawnWeaponOnUnit(item));
        }

        [ContextMenu("Показать инвентарь")]
        public void ChecksInventory()
        {
            InventoryHandler.Inventory.ShowInventory();
        }

        public void TakeOffItem(UIInventoryItem item)
        {
            if(!WeaponStates.WeaponIsNull())
            {
                WeaponStates.TakeAwayWeapon();
            }
        }
        
        //!временно
        public void UpdateInventory()
        {
            // Debug.Log("Обновить инвентарь");
            // InventoryController.InventoryUI.RefreshUI();
        }

        public void DropItem(InventoryItem item)
        {
            Helper.Spawner.SpawnWeaponOnStreet(item.ItemData.Prefab, item, transform);

            InventoryHandler.Inventory.RemoveItem(item);
        }
    }
}
