using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace InventoryDefinition
{
    public class ItemUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
    {
        public Item item;
        public Image Icon;
        public TextMeshProUGUI textAmount;
        public RectTransform rectTransform;
        [SerializeField] private Canvas canvas;
        [SerializeField] private CanvasGroup canvasGroup;
        private InventoryUI root;
        
        private void Awake() 
        {
            rectTransform = GetComponent<RectTransform>();

            canvasGroup = GetComponent<CanvasGroup>();
        }

        internal void Setup(InventoryUI inventoryUI, Item item)
        {
            root = inventoryUI;
            
            this.item = item;
            
            Icon.sprite = item.Icon;

            textAmount.text = item.amount > 1 ? item.amount.ToString() : "";
        }

        public void Select()
        {
            root.SelectCurrentItem(item);
            root.SetPistolInWeaponHandler();
        }

        //нажади на объект
        public void OnBeginDrag(PointerEventData eventData)
        {
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;

        }

        //тащим объект
        public void OnDrag(PointerEventData eventData)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
        
        //отпутили объект
        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {

            root.SelectCurrentItem(item);
            if(Input.GetKeyDown(KeyCode.Mouse1))
            {
                root.DropItem(this);
            }
            else
            {
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            
        }
    }
}
