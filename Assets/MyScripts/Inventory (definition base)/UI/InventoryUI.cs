using System;
using System.Collections.Generic;
using UnityEngine;
using static InventoryDefinition.Item;

namespace InventoryDefinition
{
    public class InventoryUI : MonoBehaviour, IInventoryUI
    {
        [SerializeField] private GameObject ownerGo;
        public IInventorySystem owner {get; internal set;}
        [SerializeField] private ItemUI itemUIPrefab;
        public List<ItemUI> itemUIList = new List<ItemUI>();
        private List<ItemUI> itemUIPool = new List<ItemUI>();
        [SerializeField] private Item selectedItem;
        [SerializeField] private ItemUI currentItemUI;

        private void Start() 
        {
            // owner = ownerGo.GetComponent<IInventorySystem>();

            if(owner != null) Debug.Log("Инициализация владельца инвентаря (успех)");
            else Debug.LogError("Не удалось инициализировать владельца инвентаря");
        }
        

        public static void ResetCards<T>(List<T> cardsList, List<T> cardsPool) where T : MonoBehaviour
        {
            cardsList.ForEach(card => card.gameObject.SetActive(false));

            cardsPool.AddRange(cardsList);

            cardsList.Clear();
        }

        // получить из пула свободную карточку
        public static T GetFreeCard<T>(T cardsPrfab, List<T> CardsPool) where T : MonoBehaviour
        {
            T result;

            if (CardsPool.Count > 0)
            {
                result = CardsPool[0];

                while (CardsPool.Remove(result)) ;
            }
            else
            {
                result = UnityEngine.Object.Instantiate(cardsPrfab, cardsPrfab.transform.parent);
            }

            return result;
        }

        //получить случайное значение перечисления
        private T GetRandomEnumValue<T>() where T : Enum
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(UnityEngine.Random.Range(0, v.Length));
        }

        public Item[] items;

        public void Spawn()
        {
            float a = UnityEngine.Random.Range(-10.0f, 10.0f);
            SpawnItemWorld(ownerGo.transform.position + ownerGo.transform.forward * 1.1f, items[UnityEngine.Random.Range(0, items.Length)]);
        }
        
        //заспавнить итем в мире
        public ItemWorld SpawnItemWorld(Vector3 position, Item item)
        {
            ItemWorld itemWorld = Instantiate(item.Prefab, new Vector3(position.x, position.y + 1, position.z), Quaternion.identity);

            itemWorld.rb.isKinematic = false;
            
            itemWorld.rb.AddForce(Vector3.forward * 1.5f, ForceMode.Impulse);

            itemWorld.SetupItem(item);
            
            return itemWorld;
        }

        public void OpenWindow() => gameObject.SetActive(true);

        public void SetPistolInWeaponHandler() => owner.EquipInventoryItem(selectedItem);
        internal void SelectCurrentItem(Item value)
        {
            Debug.Log("SelectCurrentItem");
            selectedItem = value;
        }

        internal Item GetSelectedItem() => selectedItem;

        //вызывается из кнопки
        public void CreateNewItemUI()
        {
            owner.Inventory.AddItem(selectedItem);

            RefreshInventoryItems();
        }

        //задать владельца инвентаря
        public void SetInventoryOwner(IInventorySystem newOwner) => owner = newOwner;

        public void DropItem(ItemUI itemUI)
        {
            SpawnItemWorld(ownerGo.transform.position + ownerGo.transform.forward * 1.1f, itemUI.item);

            owner.Inventory.RemoveItem(itemUI.item);
            
            RefreshInventoryItems();
        }
        

        //обновить отображение инвентаря
        public void RefreshInventoryItems()
        {
            int x = 0;
            
            int y = 0;
            
            float itemSlotCellSize = 50;

            ResetCards(itemUIList, itemUIPool);

            foreach (Item item in owner.Inventory.GetItemList())
            {
                ItemUI newItemUI = GetFreeCard(itemUIPrefab, itemUIPool);

                itemUIList.Add(newItemUI);

                newItemUI.Setup(this, item);
                
                newItemUI.rectTransform.anchoredPosition = new Vector2(x * itemSlotCellSize, -y * itemSlotCellSize);

                x++;
                if (x > 4)
                {
                    x = 0;
                    y++;
                }

                newItemUI.gameObject.SetActive(true);
            }
        }

        
    }
}
