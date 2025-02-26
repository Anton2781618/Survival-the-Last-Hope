using ModularEventArchitecture;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InventoryDiablo
{
    //класс представляет из себя систему определения ячейки для тыкания в нее

    [RequireComponent(typeof(UIItemGrid))]
    public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private UIItemGrid itemGrid;

        private void Start()
        {
            if(!itemGrid) itemGrid = GetComponent<UIItemGrid>();
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