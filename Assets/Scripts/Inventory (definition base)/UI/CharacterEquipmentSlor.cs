using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InventoryDefinition
{
    public class CharacterEquipmentSlor : MonoBehaviour, IDropHandler, IBeginDragHandler
    {
        [SerializeField] private InventoryUI root;

        private Item item;

        [SerializeField] private Item.ItemType slotType;

        public void OnBeginDrag(PointerEventData eventData)
        {
            root.owner.ToDressUp(item);
        }

        public void OnDrop(PointerEventData eventData)
        {
            
            if (eventData.pointerDrag != null)
            {
                item = root.GetSelectedItem();

                if(item.itemType == slotType)
                {
                    eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;

                    root.owner.EquipInventoryItem(item);
                }
            }
        }
    }
}
