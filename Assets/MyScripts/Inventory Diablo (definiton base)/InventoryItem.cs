using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static InventoryDiablo.ItemData;

namespace InventoryDiablo
{
    [Serializable]
    public class InventoryItem
    {    
        [field: SerializeField] public ItemData ItemData { get; set; }       
        
        //-------------------------------------------------------------------------------------
        [Header("Блок Настройки сетки" )]
        public GridData2[] Grids = new GridData2[0];

        //-------------------------------------------------------------------------------------
        [Header("Блок Настройки количества в стеке" )]
        [SerializeField] private int amount = 0;

        //-------------------------------------------------------------------------------------
        public Dictionary<ItemType, InventoryItem> CombinedItems;

        //-------------------------------------------------------------------------------------
        public UnityEvent OnItemsChanged; // {get; set;}
        public Vector2Int OnGridPosition;
        private Dictionary<ItemType,MyDelegate> delegatesDict; 

        //-------------------------------------------------------------------------------------
        [Header("Блок настроек повороте итема")]
        public bool Rotated = false;

        //-------------------------------------------------------------------------------------
        //настройки сериалиации
        // Указываем максимальную глубину сериализации
        private const int MaxDepth = 3;
        //поле для хранения текущей глубины
        [NonSerialized] private int currentDepth;
        
        //-------------------------------------------------------------------------------------


        public int HEIGHT
        {
            get
            {
                if(Rotated == false)
                {
                    return ItemData.Height;
                }
                return ItemData.Width;
            }
        }

        public int WIDTH
        {
            get
            {
                if(Rotated == false)
                {
                    return ItemData.Width;
                }
                return ItemData.Height;
            }
        }

        //-------------------------------------------------------------------------------------
        public delegate void MyDelegate();
            //свойство для доступа к количеству предметов при обращении обновляет текст
        public int Amount 
        {
            get => amount;
            set
            {
                // Debug.Log($"Обновлено количество предметов у {ItemData.Title} {value}");
                amount = value;

                OnItemsChanged?.Invoke();
            }
        }

        public int Price {get => ItemData.Price;}

        public InventoryItem(){}
        public InventoryItem(ItemData itemData, int amount)
        {
            this.ItemData = itemData;

            OnItemsChanged = new UnityEvent();
         
            CombinedItems = new Dictionary<ItemType, InventoryItem>();
            
            Amount = amount;

            InitGrid();
        }

        //делаем копию итема
        public InventoryItem(InventoryItem inventoryItem)
        {
            this.ItemData = inventoryItem.ItemData;

            OnItemsChanged = new UnityEvent();
         
            CombinedItems = new Dictionary<ItemType, InventoryItem>();
            
            Amount = inventoryItem.Amount;
            
            if(inventoryItem.Grids != null)
            {
                Grids = new GridData2[inventoryItem.Grids.Length];

                for (int i = 0; i < inventoryItem.Grids.Length; i++)
                {
                    Grids[i] = new GridData2
                    {
                        Size = inventoryItem.Grids[i].Size,
                    };
                }

                Array.Copy(Grids, inventoryItem.Grids, inventoryItem.Grids.Length); 
            }
            
        }

        public InventoryItem Clone()
        {
            var clone = new InventoryItem();
            
            clone.ItemData = this.ItemData;
            clone.OnItemsChanged = new UnityEvent();
            if (this.CombinedItems != null) clone.CombinedItems = new Dictionary<ItemType, InventoryItem>(this.CombinedItems);
            clone.Amount = this.Amount;
            clone.Rotated = this.Rotated;
            clone.OnGridPosition = this.OnGridPosition;
            
            if (this.Grids != null)
            {
                clone.Grids = new GridData2[this.Grids.Length];
                for (int i = 0; i < this.Grids.Length; i++)
                {
                    clone.Grids[i] = new GridData2
                    {
                        Size = this.Grids[i].Size,
                        activeItems = new List<InventoryItem>(this.Grids[i].activeItems.Select(item => item.Clone())),
                        Position = this.Grids[i].Position,
                        MaxStackSize = this.Grids[i].MaxStackSize,
                        CompatibilityGridMod = this.Grids[i].CompatibilityGridMod,
                        SpecificItemCombined = this.Grids[i].SpecificItemCombined,
                        CompatibleGroup = this.Grids[i].CompatibleGroup,

                    };
                }
            }
            
            return clone;
        }

        public void InitGrid()
        {
            // Grids = new GridData[ItemData.Grids.Length];

            // Array.Copy(ItemData.Grids, Grids, ItemData.Grids.Length);
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