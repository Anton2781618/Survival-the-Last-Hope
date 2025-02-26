using System.Collections.Generic;
using InventoryDiablo;
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

        private void CreateSlotGrid(InventoryContainer inventoryContainer)
        {
            Tool.Helper.ResetCards(_slotGridsList, _slotGridsPool);

            foreach (var slot in inventoryContainer.Slots)
            {
                UISlotGrid newUISlotGrid = Tool.Helper.GetFreeCard(_slotGridPrefab, _slotGridsPool);

                _slotGridsList.Add(newUISlotGrid);

                newUISlotGrid.rectTransform.localPosition = new Vector2(slot.SlotGrid.Position.x, -slot.SlotGrid.Position.y) ;

                newUISlotGrid.Setup(slot.SlotGrid.Size.x, slot.SlotGrid.Size.y);

                newUISlotGrid.gameObject.SetActive(true);
            }
        }
    }
}