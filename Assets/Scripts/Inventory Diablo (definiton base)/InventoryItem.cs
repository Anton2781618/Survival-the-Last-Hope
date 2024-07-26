using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static InventoryDiablo.ItemData;

namespace InventoryDiablo
{

    [Serializable]
    public class InventoryItem
    {    
        [field: SerializeField] public ItemData ItemData { get; private set; }       
        public delegate void MyDelegate();
        public UnityEvent OnItemsChanged;
        private Dictionary<ItemType,MyDelegate> delegatesDict;
        public Dictionary<ItemType, InventoryItem> combinedItems;
        [SerializeField] private int amount = 0;
        [SerializeField] private int price = 0; 
        public ItemGrid Grid {get; set;}

            //свойство для доступа к количеству предметов при обращении обновляет текст
        public int Amount 
        {
            get => amount;
            set
            {
                Debug.Log($"Обновлено количество предметов у {ItemData.Title} {value}");
                amount = value;

                OnItemsChanged?.Invoke();
            }
        }

        public int Price
        {
            get => price;

            set => price = value;
        }

        public InventoryItem(ItemData itemData, int amount)
        {
            this.ItemData = itemData;

            OnItemsChanged = new UnityEvent();
         
            combinedItems = new Dictionary<ItemType, InventoryItem>();
            
            Amount = amount;
        }

        private void Start() => InitDict();

        public InventoryItem DetachСombinedItems(ItemType itemType)
        {
            InventoryItem item = null;

            if(combinedItems.ContainsKey(ItemType.Обойма_патронов))
            {
                item = combinedItems[ItemType.Обойма_патронов];

                combinedItems.Remove(ItemType.Обойма_патронов);
            } 
            
            return item;
            
            // StartCoroutine(RemoveClipCoroutine());
        }

        public void InsertСombinedItems(ItemType itemType, InventoryItem inventoryItem)
        {
            if(!combinedItems.ContainsKey(itemType)) combinedItems.Add(itemType, inventoryItem);
        }

        public void InitDict() 
        {
            delegatesDict = new Dictionary<ItemType, MyDelegate>();
            
            delegatesDict.Add(ItemType.Шлем, UseHelmet);
            delegatesDict.Add(ItemType.Броня, UseArmor);
            delegatesDict.Add(ItemType.Ремень, UseBelt);
            delegatesDict.Add(ItemType.Штаны, UseTrousers);
            delegatesDict.Add(ItemType.Сапоги, UseBoots);
            delegatesDict.Add(ItemType.Оружие, UseWeapon);
            delegatesDict.Add(ItemType.Щит, UseShild);
            delegatesDict.Add(ItemType.Кольцо, UseRing);
            delegatesDict.Add(ItemType.Ожерелье, UseNecklace);
            delegatesDict.Add(ItemType.Наплечники, UseShoulder);
            delegatesDict.Add(ItemType.Аптечка, UseHealthPotion);
            delegatesDict.Add(ItemType.Зелье_маны, UseManaPotion);
            delegatesDict.Add(ItemType.Налчные_деньги, UseMoney);
            delegatesDict.Add(ItemType.Еда, UseFood);
        }

        // public void Use(AbstractBehavior applicant)
        // {
        //     this.applicant = applicant;

        //     delegatesDict[itemData.itemType].Invoke();
        // }

        

        public void ShowOutline(bool value)
        {
            throw new NotImplementedException();
        }

        private void UseHelmet()
        {
            Debug.Log("шлем");
        }

        private void UseArmor()
        {
            Debug.Log("броня");
        }

        //ремень
        private void UseBelt()
        {
            Debug.Log("ремень");
        }

        //Штаны
        private void UseTrousers()
        {
            Debug.Log("Штаныень");
        }
        
        //Сапоги
        private void UseBoots()
        {
            Debug.Log("Сапоги");
        }

        //Оружие
        private void UseWeapon()
        {
            Debug.Log("Оружие");
        }

        //Щит
        private void UseShild()
        {
            Debug.Log("Щит");
        }

        //Кольцо
        private void UseRing()
        {
            Debug.Log("Кольцо");
        }

        //Ожерелье
        private void UseNecklace()
        {
            Debug.Log("Ожерелье");
        }

        //еда
        private void UseFood()
        {
            Debug.Log("использована еда");

            // applicant.unitStats.hunger += Amount;

            // DestructSelf();
        }

        //Наплечники
        private void UseShoulder()
        {
            Debug.Log("Наплечники");
        }

        private void UseHealthPotion()
        {
            // applicant.Healing(Amount);

            // DestructSelf();

            Debug.Log("Использовал Зелье здоровья");
        }

        private void UseManaPotion()
        {
            // applicant.RestoreMana(Amount);

            // DestructSelf();

            Debug.Log("Использовал Зелье Маны");
        }

        //деньги
        private void UseMoney()
        {
            // applicant.Chest.money += Amount;

            // applicant.Chest.UpdateMoney();
            
            // DestructSelf();

            Debug.Log("Добавлены деньги");
        }
    }
}