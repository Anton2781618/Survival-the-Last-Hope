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

            if(inventoryItem.ItemData.TypeItem == ItemData.ItemType.Оружие && inventoryItem.combinedItems.Count > 0)
            {
                if(inventoryItem.combinedItems.ContainsKey(ItemData.ItemType.Обойма_патронов))
                {
                    amauntText.text = inventoryItem.combinedItems[ItemData.ItemType.Обойма_патронов].Amount.ToString();
                }
            }
            else
            {
                amauntText.text = inventoryItem.Amount.ToString();
            }
        }

        public void UpdateAmount(int sum) => amauntText.text = (inventoryItem.Amount += sum).ToString();

        internal void Setup(ItemData itemData, ItemGrid grid, int amount)
        {
            inventoryItem = new InventoryItem(itemData, amount);

            GetComponent<Image>().sprite = itemData.ItemIcon;

            Vector2 size = new Vector2();

            size.x = itemData.Width * ItemGrid.titleSizeWidth;

            size.y = itemData.Height * ItemGrid.titleSizeHeight;

            rectTransform.sizeDelta = size; 

            rectItemHighLight.sizeDelta = size;

            amauntText.gameObject.SetActive(!itemData.IsSingle);

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