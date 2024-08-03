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
        [SerializeField] private TextMeshProUGUI _text;
        [Inject] public IMuveHandler MuveHandler { get; set; }
        
        [Inject] public IWeaponStates WeaponStates { get; set;}

        [Inject] public IRaycastHandler raycastHandler { get; set;}

        [Inject] public InventoryHandler InventoryHandler { get; set;}


        private void Awake()
        {
            InventoryHandler.InventoryUI.SetInventoryOwner(this);
            
            WeaponStates.SetInventoryOwner(this);

            WeaponStates.StateCompleted += UpdateInventory;
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
                InventoryHandler.InventoryUI.ShowInventory();
            }
        }
        
        //!временно
        private void SelectedObjectOnstreet()
        {
            if(Input.GetKeyUp(KeyCode.E))
            {
                if(raycastHandler.GetHitGameObject() == null) return;
                
                // ItemOnstreet itemWorld = raycastHandler.GetHitGameObject().GetComponent<ItemOnstreet>();
                Units.Unit itemWorld = raycastHandler.GetHitGameObject().GetComponent<Units.Unit>();

                itemWorld.ShowText(false);
                
                if(itemWorld == null) return;

                itemWorld.Use();

                ItemGrid grid = InventoryHandler.InventoryUI.CheckFreeSpaceForItem(itemWorld.GetItem());
                

                if(!grid)
                {
                    Debug.Log("Нет места в инвентаре");
                    
                    return;
                }
                
                Destroy(itemWorld.gameObject);

                // InventoryController.Inventory.AddItem(itemWorld.GetItem());
                InventoryHandler.InventoryUI.CreateAndInsertItem(itemWorld.GetItem(), grid);
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
            
            WeaponStates.SetWeapon(Helper.spawner.SpawnWeaponOnUnit(item));
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
            Helper.spawner.SpawnWeaponOnStreet(item.ItemData.Prefab, item, transform);

            InventoryHandler.Inventory.RemoveItem(item);
        }

        [Serializable]
        public class SaveData
        {
            // public Inventory Inventory;

            public float x;
            public float y;
            public float z;

        }
            

        public void Save()
        {
            Debug.Log("Сохранение");
            SaveData saveData = new SaveData
            {

                // Inventory = InventoryController.Inventory as Inventory,
                
                x = transform.position.x,
                y = transform.position.y,
                z = transform.position.z,
                
            };
            

            
            BlazeSave.SaveData("Player", saveData);
        }
        
        [ContextMenu("Загрузить")]
        public void Load()
        {
            Debug.Log("Загрузка");
            SaveData saveData = BlazeSave.LoadData<SaveData>("Player");
            

            Vector3 position = new Vector3(saveData.x, saveData.y, saveData.z);
            this.transform.position = position;
            Debug.Log(saveData.x + " " + saveData.y + " " + saveData.z);
            
            // InventoryController.Inventory = saveData.Inventory;

        }
    }
}
