using UnityEngine;
using UnityEngine.UI;

namespace InventoryDiablo
{
    public class UIInventoryItem : MonoBehaviour
    {
        [SerializeField] private InventoryItem inventoryItem;
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

        public void Setup(InventoryItem item)
        {
            inventoryItem = item;

            icon.sprite = item.ItemData.ItemIcon;

            Vector2 size = new Vector2();

            size.x = item.ItemData.Width * GridData.titleSizeWidth;

            size.y = item.ItemData.Height * GridData.titleSizeHeight;

            rectTransform.sizeDelta = size; 

            rectItemHighLight.sizeDelta = size;

            amauntText.gameObject.SetActive(item.ItemData.MaxStackSize > 1);

            amauntText.rectTransform.sizeDelta = size;

            UpdateAmountText();

            InventoryItem.OnItemsChanged.AddListener(UpdateAmountText);
        }

        public void Rotated()
        {
            inventoryItem.Rotated = !inventoryItem.Rotated;

            rectTransform.rotation = Quaternion.Euler(0, 0, inventoryItem.Rotated ? 90f : 0f);
        }

        public void DestructSelf()
        {
            if(this == null) return;

            InventoryItem.OnItemsChanged.RemoveListener(UpdateAmountText);

            Destroy(gameObject);
        }
    }
}