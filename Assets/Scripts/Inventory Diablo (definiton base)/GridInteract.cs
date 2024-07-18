using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InventoryDiablo
{
    //класс представляет из себя систему определения ячейки для тыкания в нее
    
    [RequireComponent(typeof(ItemGrid))]
    public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
    [SerializeField] private InventoryUI inventoryController;
    [SerializeField] private ItemGrid itemGrid;

        private void Start()
        {
            if(!itemGrid) itemGrid = GetComponent<ItemGrid>();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            inventoryController.SelectedItemGrid = itemGrid;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            inventoryController.SelectedItemGrid = null;
        }
    }
}