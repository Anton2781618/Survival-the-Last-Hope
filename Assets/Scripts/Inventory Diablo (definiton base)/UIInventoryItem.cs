using UnityEngine;
using UnityEngine.UI;

namespace InventoryDiablo
{
    public class UIInventoryItem : MonoBehaviour
    {
        [SerializeField] private InventoryItem inventoryItem;
        public bool rotated = false;
        public int onGridPositionX;  
        public int onGridPositionY; 
        
        [SerializeField] private Image icon;
        public RectTransform rectItemHighLight;
        public RectTransform rectTransform;
        [SerializeField] public Text amauntText;

        public InventoryItem InventoryItem
        {
            get => inventoryItem;
            set
            {
                inventoryItem = value;
            }
        }

        public int HEIGHT
        {
            get
            {
                if(rotated == false)
                {
                    return InventoryItem.ItemData.Height;
                }
                return InventoryItem.ItemData.Width;
            }
        }

        public int WIDTH
        {
            get
            {
                if(rotated == false)
                {
                    return InventoryItem.ItemData.Width;
                }
                return InventoryItem.ItemData.Height;
            }
        }

        public void UpdateAmountText()
        {
            Debug.Log($"!Обновлен UI предмета {inventoryItem.ItemData.Title} {inventoryItem.Amount}");

            if(inventoryItem.CombinedItems == null) inventoryItem.CombinedItems = new System.Collections.Generic.Dictionary<ItemData.ItemType, InventoryItem>();

            if(inventoryItem.ItemData.TypeItem == ItemData.ItemType.Оружие && inventoryItem.CombinedItems.Count > 0)
            {
                if(inventoryItem.CombinedItems.ContainsKey(ItemData.ItemType.Обойма_патронов))
                {
                    amauntText.text = inventoryItem.CombinedItems[ItemData.ItemType.Обойма_патронов].Amount.ToString();
                }
            }
            else
            {
                amauntText.text = inventoryItem.Amount.ToString();
            }
        }

        public void UpdateAmount(int sum) => amauntText.text = (inventoryItem.Amount += sum).ToString();

        internal void Setup(InventoryItem item, ItemGrid grid, int amount)
        {
            inventoryItem = item;

            icon.sprite = item.ItemData.ItemIcon;

            Vector2 size = new Vector2();

            size.x = item.ItemData.Width * ItemGrid.titleSizeWidth;

            size.y = item.ItemData.Height * ItemGrid.titleSizeHeight;

            rectTransform.sizeDelta = size; 

            rectItemHighLight.sizeDelta = size;

            amauntText.gameObject.SetActive(!item.ItemData.IsSingle);

            amauntText.rectTransform.sizeDelta = size;

            UpdateAmountText();

            InventoryItem.OnItemsChanged.AddListener(UpdateAmountText);
        }

        internal void Rotated()
        {
            rotated = !rotated;

            rectTransform.rotation = Quaternion.Euler(0, 0, rotated ? 90f : 0f);
        }

        public void DestructSelf()
        {
            if(this == null) return;

            InventoryItem.OnItemsChanged.RemoveListener(UpdateAmountText);

            Destroy(gameObject);
        }
    }
}