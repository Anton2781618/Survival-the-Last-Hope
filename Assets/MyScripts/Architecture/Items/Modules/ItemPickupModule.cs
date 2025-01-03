using System.Collections;
using System.Collections.Generic;
using InventoryDiablo;
using Units;
using UnityEngine;

namespace ModularEventArchitecture
{
    public class ItemPickupModule : ModuleBase
    {
        [SerializeField] private InventoryItem _item;
        [SerializeField] private Collider _collider;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private OutlineSystem.Outline _outline;
        [SerializeField] private GameObject text;
        public Collider GetCollider() => _collider;
        public Rigidbody GetRigidbody() => _rigidbody;
        public OutlineSystem.Outline GetOutline() => _outline;
        public InventoryItem GetItem() => _item;

        public override void Initialize()
        {
            
        }

        public override void UpdateMe()
        {
        }

        public void TakeItem(Inventory inventoryModel)
        {       
            inventoryModel.AddItem(new InventoryItem(_item.ItemData, _item.Amount));

            // GameManager.Instance.RemoveUsableObject(gameObject);

            Destroy(this.gameObject);
        }

        public void Use(Inventory unit)
        {
            IInventorySystem inventorySystem = unit as IInventorySystem;

            ItemGrid grid = inventorySystem.InventoryHandler.InventoryUI.CheckFreeSpaceForItem(_item);

            if(!grid)
            {
                Debug.Log("Нет места в инвентаре");
                
                return;
            }

            inventorySystem.InventoryHandler.InventoryUI.CreateAndInsertItem(_item, grid);

            inventorySystem.InventoryHandler.Inventory.AddItem(_item);

            Destroy(gameObject);
        }

        //переключить текст
        public void ShowText(bool isShow) => text.gameObject.SetActive(isShow);

        private void Dest()
        {
            Destroy(this.gameObject);
        }

        public void SetupItem(InventoryItem item, GameObject textGo)
        {
            _item = item;

            text = textGo;
        }

        void OnMouseEnter()
        {
            _outline.enabled = true;

            ShowText(true);

            text.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
        }

        void OnMouseOver()
        {
            // rend.material.color -= new Color(0.1F, 0, 0) * Time.deltaTime;
        }

        void OnMouseExit()
        {
            ShowText(false);

            _outline.enabled = false;
        }
    }
}