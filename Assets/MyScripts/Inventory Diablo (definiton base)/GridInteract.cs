using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace InventoryDiablo
{
    //класс представляет из себя систему определения ячейки для тыкания в нее
    
    [RequireComponent(typeof(ItemGrid))]
    public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
    [Inject] private InventoryManager InventoryManager;
    [SerializeField] private ItemGrid itemGrid;

        private void Start()
        {
            if(!itemGrid) itemGrid = GetComponent<ItemGrid>();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            InventoryManager.SelectedItemGrid = itemGrid;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            InventoryManager.SelectedItemGrid = null;
        }
    }
}