using System.Collections.Generic;
using InventoryDiablo;
using UnityEngine;


namespace ModularEventArchitecture
{
    public class UISlotGrid : UIItemGrid
    {
        //-----------------------------------------------------------
        public Inventory Inventory;
        private InventorySlot _slot;
        //-----------------------------------------------------------
        public UnityEngine.UI.Image Icon;

        //-----------------------------------------------------------
        [Header("Блок префабов")]
        //префаб сетки
        [SerializeField] private UIItemGrid _gridPrefab;

        //-----------------------------------------------------------
        //пулл сеток которые находятся внутри итема лежащего в слоте
        private List<UIItemGrid> _UIitemGridsList = new List<UIItemGrid>();
        private List<UIItemGrid> _UIitemGridsPool = new List<UIItemGrid>();
        
        //!-----------------------------------------------------------

        public void Setup(InventorySlot slot)
        {
            _slot = slot;
            base.Setup(slot.SlotGrid);
        }

        public override void PlaceItem(UIInventoryItem UIInventoryItem, int posX, int posY)
        {
            CreateGridsForItems(UIInventoryItem.InventoryItem);

            base.PlaceItem(UIInventoryItem, posX, posY);

            Inventory.Entity.LocalEvents.Publish(EventsInventory.Equip_item_in_slot, new EquipItemEventData
            { 
                InventoryItem = UIInventoryItem.InventoryItem,
                Parent = _slot.SlotObjectToSpawn.gameObject
                
            });
        }

        public override UIInventoryItem SelectIteme(int x, int y)
        {
            Inventory.Entity.LocalEvents.Publish(EventsInventory.Take_off_item, new TakeOffItemEventData
            { 
                Slot = _slot
                
            });

            foreach (var grid in _UIitemGridsList)
            {
                grid.DeactiveGrid();
            }
            
            return base.SelectIteme(x, y);
        }

        //создать сетки которые находятся в итеме который положиле в слот
        public void CreateGridsForItems(InventoryItem item)
        {
            Tool.Helper.ResetCards(_UIitemGridsList, _UIitemGridsPool);

            foreach (var itemGrid in item.Grids)
            {
                UIItemGrid newUIGrid = Tool.Helper.GetFreeCard(_gridPrefab, _UIitemGridsPool);

                _UIitemGridsList.Add(newUIGrid);

                newUIGrid.RectTransform.localPosition = new Vector2(itemGrid.Position.x, -itemGrid.Position.y);

                newUIGrid.Setup(itemGrid);

                newUIGrid.gameObject.SetActive(true);
            }
        }
    }
}