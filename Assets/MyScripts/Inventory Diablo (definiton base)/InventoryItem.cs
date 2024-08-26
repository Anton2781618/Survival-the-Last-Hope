using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static InventoryDiablo.ItemData;
using static InventoryDiablo.ItemGrid;

namespace InventoryDiablo
{

    [Serializable]
    public class InventoryItem
    {    
        [field: SerializeField] public ItemData ItemData { get; private set; }       
        private Dictionary<ItemType,MyDelegate> delegatesDict;
        public Dictionary<ItemType, InventoryItem> CombinedItems;
        [SerializeField] private int amount = 0;
        [SerializeField] private int price = 0; 
        public GridName GridName = GridName.BackpackGrid;
        public UnityEvent OnItemsChanged;

        public delegate void MyDelegate();
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
         
            CombinedItems = new Dictionary<ItemType, InventoryItem>();
            
            Amount = amount;
        }

        private void Start() => InitDict();

        public InventoryItem DetachСombinedItems(ItemType itemType)
        {
            InventoryItem item = null;

            if(CombinedItems.ContainsKey(ItemType.Обойма_патронов))
            {
                item = CombinedItems[ItemType.Обойма_патронов];

                CombinedItems.Remove(ItemType.Обойма_патронов);
            } 
            
            return item;
            
            // StartCoroutine(RemoveClipCoroutine());
        }

        public void InsertСombinedItems(ItemType itemType, InventoryItem inventoryItem)
        {
            if(!CombinedItems.ContainsKey(itemType)) CombinedItems.Add(itemType, inventoryItem);
        }

        public void InitDict() 
        {
            delegatesDict = new Dictionary<ItemType, MyDelegate>();
            
            delegatesDict.Add(ItemType.Шлем, UseHelmet);
            delegatesDict.Add(ItemType.Разгрузка, UseArmor);
            delegatesDict.Add(ItemType.Ремень, UseBelt);
            delegatesDict.Add(ItemType.Рюкзак, UseTrousers);
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