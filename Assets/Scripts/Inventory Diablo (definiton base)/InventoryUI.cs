using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static InventoryDiablo.ItemGrid;

namespace InventoryDiablo
{
    //класс является системой управления всех ивентарей, основной функцианал инвентарей находится тут
    public class InventoryUI : MonoBehaviour, IInventoryUI
    {
        [SerializeField] private UIContextMenu contextMenu;

        private ItemGrid buferGrid;
        public List<ItemGrid> grids;
        
        //выбраный инвентарь
        private IInventorySystem owner;
        public bool IsTreid{get; set;} = false;
        private ItemGrid selectedItemGrid;
        public ItemGrid SelectedItemGrid 
        {
            get => selectedItemGrid; 
            
            set 
            {
                selectedItemGrid = value;

                inventoryIHighLight.SetParent(value);
            }
        }

        private UIInventoryItem UIselectedItem;
        private UIInventoryItem overlapItem;
        private RectTransform itemRectTransform;

        [SerializeField] private List<ItemData> items;
        [SerializeField] private UIInventoryItem itemPrefab;

        public InventoryIHighLight inventoryIHighLight;

        [HideInInspector] public SetCharacter Clothes{get; set;}

        private void Update() 
        {
            ItemIconDrah();

            if(Input.GetKeyDown(KeyCode.M))
            {
                if(UIselectedItem == null)
                {
                    CreateRandomItem();
                }
            }

            if(Input.GetKeyDown(KeyCode.N))
            {
                InsertRandomItem();
            }

            if(Input.GetKeyDown(KeyCode.R))
            {
                RotateItem();
            }

            if(SelectedItemGrid == null)
            {
                inventoryIHighLight.Show(false);

                if(Input.GetMouseButtonDown(0))
                {
                    if(UIselectedItem && !IsTreid)DropItem(UIselectedItem.InventoryItem);
                }

                return;
            }
            
            if(UIselectedItem && (SelectedItemGrid.GetGridForItemsType() & UIselectedItem.InventoryItem.ItemData.TypeItem) == 0)
            {
                inventoryIHighLight.Show(false);
                return;
            }

            HandleHighlight();
            
            if(Input.GetMouseButtonDown(0))
            {
                LeftMouseButtonPress();
            }

            if(Input.GetMouseButtonDown(1))
            {
                RightMouseButtonPress();
            }
        }

        public void ShowInventory()
        {
            gameObject.SetActive(!gameObject.activeSelf);

            // RefreshUI();
        }
        
        public void RefreshUI() 
        {
            if(!gameObject.activeSelf) return;

            foreach (ItemGrid grid in grids)
            {
                foreach (UIInventoryItem item in grid.GetItems())
                {
                    grid.CleanGridReference(item);

                    item.DestructSelf();
                }
            }

            foreach (InventoryItem inventoryItem in owner.InventoryController.Inventory.GetInventoryItems())
            {
                
                ItemGrid grid = grids.FirstOrDefault(t => t.gridName == inventoryItem.gridName);

                CreateAndInsertItem(inventoryItem, grid);
            }
            
            // foreach (ItemGrid grid in grids)
            // {
            //     foreach (UIInventoryItem item in grid.GetItems())
            //     {
            //         item.UpdateAmountText();
            //     }
            // }
        }

        private void RotateItem()
        {
            if(UIselectedItem == null) {return;}

            UIselectedItem.Rotated();
        }

        [ContextMenu("InsertRandomItem")]
        public void InsertRandomItem()
        {
            if(selectedItemGrid == null) {return;}

            CreateRandomItem();

            UIInventoryItem itemToInsert = UIselectedItem;

            UIselectedItem = null;

            
            InsertItemOnGrid(itemToInsert, selectedItemGrid);
        }

        private void InsertItemOnGrid(UIInventoryItem itemToInsert, ItemGrid grid)
        {
            Vector2Int? posOnGrid = grid.FindSpaceForObject(itemToInsert);

            if(gameObject.activeSelf) owner.InventoryController.Inventory.AddItem(itemToInsert.InventoryItem);
            
            if(posOnGrid == null) 
            {
                Debug.Log($"На сетке {grid} нет места для {itemToInsert.InventoryItem.ItemData.Title}");
                Debug.Log("ВНИМАНИЕ! Надо переделать так что бы сначало проверялась сетка на наличие места, а потом ставился итем");
                Destroy(itemToInsert.gameObject);

                return;
            }

            grid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
        }

        //проверить есть ли свободное место на одной из сетке
        public ItemGrid CheckFreeSpaceForItem(InventoryItem inventoryItem)
        {
            foreach (ItemGrid grid in grids)
            {
                if(grid.FindSpaceForObject(inventoryItem) != null) return grid;
            }

            return null;
        }
        
        public void CreateAndInsertItem(InventoryItem itemToInsert, ItemGrid grid)
        {
            CreateAndInsertItem(itemToInsert.ItemData, grid, itemToInsert.Amount);
        }

        //!!!Этот метод создает итем и устанавливает его на сетку ОБРАЩАТЬСЯ ЧЕРЕЗ НЕГО
        //создать физически итем и установить его на сетку 
        public void CreateAndInsertItem(ItemData itemData, ItemGrid grid, int amount = 0)
        {
            Debug.Log($"Создан предмет {itemData.Title} {amount} штук");
            
            CreateItem(itemData, grid, amount);
            
            UIInventoryItem itemToInsert = UIselectedItem;
            
            UIselectedItem = null;
            
            InsertItemOnGrid(itemToInsert, grid);
        }

        private Vector2Int _oldPosition;
        private UIInventoryItem _itemToHighLight;

        //метод подсветки предмета
        private void HandleHighlight()
        {
            Vector2Int positionOnGrid = GetTitleGridPosition();
            if(_oldPosition == positionOnGrid){return;}
            
            _oldPosition = positionOnGrid;
            if(UIselectedItem == null)
            {
                _itemToHighLight = SelectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);
                
                if(_itemToHighLight != null)
                {
                    inventoryIHighLight.Show(true);
                    inventoryIHighLight.SetSize(_itemToHighLight);
                    inventoryIHighLight.SetPosition(SelectedItemGrid, _itemToHighLight);
                }
                else
                {
                    inventoryIHighLight.Show(false);
                    
                }

                // GameManager.singleton.SwithInfoItem(false);
            }
            else
            {
                inventoryIHighLight.Show(SelectedItemGrid.BoundryCheck(positionOnGrid.x, positionOnGrid.y,
                                        UIselectedItem.WIDTH, UIselectedItem.HEIGHT));
                inventoryIHighLight.SetSize(UIselectedItem);
                inventoryIHighLight.SetPosition(SelectedItemGrid, UIselectedItem, positionOnGrid.x, positionOnGrid.y);
            }
        }

        //создать случайный итем
        public void CreateRandomItem()
        {
            UIInventoryItem inventoryItem = Instantiate(itemPrefab);
            UIselectedItem = inventoryItem;

            itemRectTransform = inventoryItem.rectTransform;
            itemRectTransform.SetParent(transform);
            itemRectTransform.SetAsLastSibling();

            
            int selectedItemID = UnityEngine.Random.Range(0, items.Count);

            UIselectedItem.Setup(items[selectedItemID], null, items[selectedItemID].MaxAmount);
        }

        private void CreateItem(ItemData itemData, ItemGrid grid, int amount)
        {
            UIInventoryItem inventoryItem = Instantiate(itemPrefab);
            UIselectedItem = inventoryItem;


            itemRectTransform = inventoryItem.rectTransform;
            itemRectTransform.SetParent(transform);
            itemRectTransform.SetAsLastSibling();

            UIselectedItem.Setup(itemData, grid, amount);
        }

        //метод перемещает итем в след за мышкой
        private void ItemIconDrah()
        {
            if(UIselectedItem)
            {
                // itemRectTransform.position = Input.mousePosition
                
                //расчитать позицию итема с учетом изменения размера итема
                itemRectTransform.position = Input.mousePosition - new Vector3((itemRectTransform.sizeDelta.x * itemRectTransform.localScale.x) / (itemRectTransform.lossyScale.x * 10), 0);

            }
        }

        //метод выделяет итем и станавлевает его в ячейку
        private void LeftMouseButtonPress()
        {
            Vector2Int titleGridPosition = GetTitleGridPosition();
            
            if (UIselectedItem == null)
            {
                PickUpItem(titleGridPosition);
            }
            else
            {
                PlaceItem(titleGridPosition);
            }

            contextMenu.Show(false);

            // if(selectedInventory != null) selectedInventory.UpdateChestItems();

            // playerChest.UpdateChestItems();
        }

        //открыть контектсное меню
        private void RightMouseButtonPress()
        {
            if (UIselectedItem != null) return;

            Vector2Int titleGridPosition = GetTitleGridPosition();
            UIInventoryItem uiInventoryItem = SelectedItemGrid.GetItem(titleGridPosition.x, titleGridPosition.y);
            
            if(!uiInventoryItem)return;
            
            contextMenu.Setup(uiInventoryItem);
        }

        //тут мы устанавливаем итем на сетку со смещением. Это для того что бы распологать итем по центру а не с краю мышки
        private Vector2Int GetTitleGridPosition()
        {
            Vector2 position = Input.mousePosition;
            
            if (UIselectedItem != null)
            {
                position.x -= (UIselectedItem.WIDTH - 1) * ItemGrid.titleSizeWidth / 2;
                position.y += (UIselectedItem.HEIGHT - 1) * ItemGrid.titleSizeHeight / 2;
            }

            return SelectedItemGrid.GetTitleGridPosition(position);
        }


        //поднять предмет с сетке
        private void PickUpItem(Vector2Int titleGridPosition)
        {
            UIselectedItem = SelectedItemGrid.SelectIteme(titleGridPosition.x, titleGridPosition.y);

            if(SelectedItemGrid.isSingle)
            {
                // TakeOffClothes(selectedItem.itemData.GetItemTypeIndex());
                owner.TakeOffItem(UIselectedItem);
            }

            if (UIselectedItem)
            {
                UIselectedItem.transform.SetParent(transform);
                itemRectTransform = UIselectedItem.rectTransform;
                itemRectTransform.SetAsLastSibling();

                buferGrid = SelectedItemGrid;

                owner.InventoryController.Inventory.RemoveItem(UIselectedItem.InventoryItem);
            }

        }

        //расположить предмет на сетке 
        private void PlaceItem(Vector2Int titleGridPosition)
        {
            if(IsTreid && buferGrid.chest != SelectedItemGrid.chest)
            {
                PlaceItemInTrade();
            }
            else
            {
                PlaceItemIsNotTraded(titleGridPosition);
            }
        }

        //расположить вне торговли
        private void PlaceItemIsNotTraded(Vector2Int titleGridPosition)
        {
            bool complete = SelectedItemGrid.PlaceItem(UIselectedItem, titleGridPosition.x, titleGridPosition.y, ref overlapItem, owner);        
            
            if(complete)
            {
                owner.InventoryController.Inventory.AddItem(UIselectedItem.InventoryItem);
                if(SelectedItemGrid.isSingle)
                {
                    // PutOnClothesOnBody(selectedItem.itemData.GetItemTypeIndex());
                    owner.EquipItem(UIselectedItem.InventoryItem);
                }
                
                UIselectedItem = null;
                
                if(overlapItem != null)
                {
                    UIselectedItem = overlapItem;
                
                    overlapItem = null;
                
                    itemRectTransform = UIselectedItem.rectTransform;
                
                    itemRectTransform.SetAsLastSibling();
                }
                
            }
        }

        //расположить при торговли
        private void PlaceItemInTrade()
        {
            if(SelectedItemGrid.chest.money < UIselectedItem.InventoryItem.Price)
            {
                // GameManager.Instance.UIManager.GetPlayerInventoryWindowUI().NotEnoughMoneyAnimation();

                return;
            } 

            // SelectedItemGrid.chest.money -= selectedItem.itemData.price;

            buferGrid.chest.money += UIselectedItem.InventoryItem.Price;

            // buferGrid.chest.UpdateMoney();

            // SelectedItemGrid.chest.UpdateMoney();

            UIInventoryItem buferItem = UIselectedItem;

            CreateAndInsertItem(buferItem.InventoryItem.ItemData, selectedItemGrid, UIselectedItem.InventoryItem.Amount);

            buferItem.DestructSelf();

            UIselectedItem = null;
        }

        // //надеть одежду 
        // private void PutOnClothesOnBody(int index)
        // {
        //     playerChest.Clothes.items[index].Prefab = SelectedItemGrid.GetItem(0, 0).itemData.prefab;
            
        //     playerChest.Clothes.AddItem(index);
        // }

        // //снять одежду
        // private void TakeOffClothes(int index)
        // {
        //     playerChest.Clothes.RemoveItem(index);

        //     playerChest.Clothes.items[index].Prefab = null;
        // }

        //метод выкинуть предмет
        public void DropItem(InventoryItem item)
        {
            owner.DropItem(item);
                
            UIselectedItem.DestructSelf();

            UIselectedItem = null;
        }
        
        //задать владельца инвентаря
        public void SetInventoryOwner(IInventorySystem newOwner) => owner = newOwner;


        public void DestroyAllInventoryItem()
        {
            foreach (ItemGrid grid in grids)
            {
                foreach (UIInventoryItem item in grid.GetItems())
                {
                    owner.InventoryController.Inventory.RemoveItem(item.InventoryItem);

                    item.DestructSelf();
                }
            }
        }

        public void DestroyInventoryItem(InventoryItem inventoryItem)
        {
            foreach (ItemGrid grid in grids)
            {
                foreach (UIInventoryItem item in grid.GetItems())
                {
                    if(item.InventoryItem == inventoryItem)
                    {
                        owner.InventoryController.Inventory.RemoveItem(item.InventoryItem);

                        item.DestructSelf();
                        return;
                    }
                }
            }
        }
    }
}