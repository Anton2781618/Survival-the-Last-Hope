using System.Collections.Generic;
using UnityEngine;
using static InventoryDiablo.ItemData;

namespace ModularEventArchitecture
{
    public class UIInventoryContainer : MonoBehaviour
    {
        //-----------------------------------------------------------
        [Header("Блок типа предмета")]

        // тип предмета который можно установить в слот
        public ItemType TypeItem = ItemType.Шлем; 

        //-----------------------------------------------------------
        // [Header("Блок трансфом")]
        [SerializeField] private RectTransform _rectTransform;

        //-----------------------------------------------------------
        [Header("Блок префабов")]
        //префаб Слота
        [SerializeField] private UISlotGrid _slotGridPrefab;

        //-----------------------------------------------------------
        //пулл слотов
        private List<UISlotGrid> _slotGridsList = new List<UISlotGrid>();
        private List<UISlotGrid> _slotGridsPool = new List<UISlotGrid>();
        

        //!-----------------------------------------------------------

        public void Setup(InventoryContainer container)
        {
            _rectTransform.anchoredPosition = new Vector2(container.Position.x, -container.Position.y);

            _rectTransform.sizeDelta = new Vector2(container.Size.x, container.Size.y);
            
            CreateSlotGrid(container);
        }

        public void CreateSlotGrid(InventoryContainer inventoryContainer)
        {
            Tool.Helper.ResetCards(_slotGridsList, _slotGridsPool);

            foreach (InventorySlot slot in inventoryContainer.Slots)
            {
                UISlotGrid newUISlotGrid = Tool.Helper.GetFreeCard(_slotGridPrefab, _slotGridsPool);

                _slotGridsList.Add(newUISlotGrid);

                newUISlotGrid.RectTransform.localPosition = new Vector2(slot.SlotGrid.Position.x, -slot.SlotGrid.Position.y);

                newUISlotGrid.Icon.sprite = slot.Icon;
                
                newUISlotGrid.Setup(slot);

                newUISlotGrid.Inventory = slot.Inventory;

                //показ сетки если в слоте есть предметы
                if(newUISlotGrid.GridDataInfo.ActiveItems.Count > 0) newUISlotGrid.CreateGridsForItems(newUISlotGrid.GridDataInfo.ActiveItems[0]);

                newUISlotGrid.gameObject.SetActive(true);
            }
        }
    }
}