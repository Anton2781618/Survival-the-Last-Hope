using ModularEventArchitecture;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InventoryDiablo
{
    //класс представляет из себя систему определения ячейки для тыкания в нее

    [RequireComponent(typeof(ItemGrid))]
    public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private ItemGrid itemGrid;

        private void Start()
        {
            if(!itemGrid) itemGrid = GetComponent<ItemGrid>();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            GlobalEventBus.Instance.Publish(EventsInventory.SeletGrid, new SelectGridEventData{ItemGrid = itemGrid});
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            GlobalEventBus.Instance.Publish(EventsInventory.SeletGrid, new SelectGridEventData{ItemGrid = null});
        }
    }
}