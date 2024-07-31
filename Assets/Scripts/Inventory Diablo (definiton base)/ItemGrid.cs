
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static InventoryDiablo.ItemData;


namespace InventoryDiablo
{
    //класс представляет из себя сетку с ячейками и данные о размерах
    //устанавливается на UI сетки
    public class ItemGrid : MonoBehaviour
    {
        public const float titleSizeWidth = 32;
        public const float titleSizeHeight = 32;

        //это ссылка на того чей инвентарь
        public Inventory chest;// {get; set;}
        private UIInventoryItem[,] inventoryItemSlot;
        private RectTransform rectTransform;
        private Vector2 positionOnTheGrid = new Vector2();
        private Vector2Int titeGridPosition = new Vector2Int();

        [SerializeField] private int GridSizeWidth = 20; 
        [SerializeField] private int GridSizeHeight = 10; 
        [SerializeField] private ItemType gridForItemsType;

        //поле определяет сетка для одного предмета или нет
        public bool isSingle = false;
        internal GridName gridName = GridName.backpack;

        public enum GridName
        {
            backpack,
        }

        private void Awake() 
        {
            rectTransform = GetComponent<RectTransform>();    
            
            Init(GridSizeWidth,GridSizeHeight);
        }

        public ItemType GetGridForItemsType()
        {
            return gridForItemsType;
        }

        [InventoryDiablo.Button] 
        public void Обновить_размер_сетки()
        {
            if(!rectTransform) rectTransform = GetComponent<RectTransform>();    

            Init(GridSizeWidth,GridSizeHeight);
        }

        //устанавлмвает начальный размер сетки
        private void Init(int width, int height)
        {
            inventoryItemSlot = new UIInventoryItem[width, height];
            Vector2 size = new Vector2(width * titleSizeWidth, height * titleSizeHeight);
            rectTransform.sizeDelta = size;
        }

        internal UIInventoryItem GetItem(int x, int y)
        {
            return inventoryItemSlot[x, y];
        }

        //метод возвращает позицию на ячейки по сетке
        public Vector2Int GetTitleGridPosition(Vector2 mousePosition)
        {
            positionOnTheGrid.x = mousePosition.x - rectTransform.position.x;
            positionOnTheGrid.y = rectTransform.position.y - mousePosition.y;
        
            titeGridPosition.x = (int)((positionOnTheGrid.x / titleSizeWidth) / transform.localScale.x); 
            titeGridPosition.y = (int)((positionOnTheGrid.y / titleSizeHeight) / transform.localScale.y );

            return titeGridPosition;
        }

        public Vector2Int? FindSpaceForObject(UIInventoryItem itemToInsert)
        {
            int heght = GridSizeHeight - itemToInsert.HEIGHT + 1;
            int wight = GridSizeWidth - itemToInsert.WIDTH + 1; 
            
            for (int y = 0; y < heght; y++)
            {
                for (int x = 0; x < wight ; x++)
                {
                    if(CheckAvailabeSpace(x, y, itemToInsert.WIDTH, itemToInsert.HEIGHT) == true)
                    {
                        return new Vector2Int(x, y); 
                    }
                }
            }

            return null;
        }
        public Vector2Int? FindSpaceForObject(InventoryItem itemToInsert)
        {
            int heght = GridSizeHeight - itemToInsert.ItemData.Height + 1;
            int wight = GridSizeWidth - itemToInsert.ItemData.Width + 1; 
            
            for (int y = 0; y < heght; y++)
            {
                for (int x = 0; x < wight ; x++)
                {
                    if(CheckAvailabeSpace(x, y, itemToInsert.ItemData.Width, itemToInsert.ItemData.Height) == true)
                    {
                        return new Vector2Int(x, y); 
                    }
                }
            }

            return null;
        }

        //метод установить итем в слот
        public bool PlaceItem(UIInventoryItem inventoryItem, int posX, int posY, ref UIInventoryItem overlapItem, IInventorySystem owner)
        {
            // не можем расположить итем если он хотябы частично за сеткой
            if (BoundryCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT) == false)
            {
                return false;
            }
            
            if(isSingle)
            {
                GetFrestOverLap(inventoryItem.WIDTH, inventoryItem.HEIGHT, ref overlapItem);
            }
            else
            {
                if (OverLapTheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT, ref overlapItem) == false)
                {
                    overlapItem = null;

                    return false;
                }
            }

            if (overlapItem != null)
            {
                Debug.Log($"Есть перекрытие! Совместить {CanBeCombinedItems(inventoryItem, overlapItem)}");
                if(CanBeCombinedItems(inventoryItem, overlapItem))
                {
                    Debug.Log("Можно совместить ");
                    if(CombineSlotIsNotFree(inventoryItem, overlapItem))
                    {
                        Debug.Log("Слот свободен, Вставляю");
                        
                        if(overlapItem.InventoryItem.ItemData.TypeItem == ItemType.Оружие && inventoryItem.InventoryItem.ItemData.TypeItem == ItemType.Обойма_патронов)
                        {
                            overlapItem.InventoryItem.combinedItems.Add(inventoryItem.InventoryItem.ItemData.TypeItem, inventoryItem.InventoryItem);
                            
                            overlapItem.UpdateAmount(inventoryItem.InventoryItem.Amount); 

                            inventoryItem.DestructSelf();
                        }
                        else 
                        {
                            TransferAmount(inventoryItem, overlapItem);
                        }

                        if(inventoryItem.InventoryItem.ItemData.TypeItem == ItemType.Патроны && inventoryItem.InventoryItem.Amount == 0 ) TransferAmount(inventoryItem, overlapItem);

                        CleanGridReference(inventoryItem);

                        overlapItem = null;

                        return false;
                    }
                }
                else
                if(inventoryItem.InventoryItem.ItemData.TypeItem == ItemType.Коробка_патронов && overlapItem.InventoryItem.ItemData.TypeItem == ItemType.Оружие)
                {
                    Debug.Log("пытаюсь вставить коробку патронов в оружие");
                    if(CombineSlotIsFree(overlapItem, ItemType.Обойма_патронов))
                    {
                        Debug.Log("у оружия нет обоймы патронов");
                    }
                    else
                    {
                        Debug.Log("у оружия есть обойма патронов " + overlapItem.InventoryItem.combinedItems[ItemType.Обойма_патронов]);

                        TransferAmount(inventoryItem, overlapItem);

                        overlapItem = null;
                        
                        return false;
                    }
                }

                CleanGridReference(overlapItem);
            }

            PlaceItem(inventoryItem, isSingle ? 0 : posX, isSingle ? 0 : posY);

            return true;
        }

        //переложить амоунт из одного итема в другой, ограничение на максимальное количество 
        private void TransferAmount(UIInventoryItem from, UIInventoryItem to)
        {
            int amount = from.InventoryItem.Amount;

            if(to.InventoryItem.ItemData.TypeItem == ItemType.Оружие && from.InventoryItem.ItemData.TypeItem == ItemType.Коробка_патронов)
            {
                if(!CombineSlotIsFree(to, ItemType.Обойма_патронов))
                {
                    to.InventoryItem.ItemData.MaxAmount = to.InventoryItem.combinedItems[ItemType.Обойма_патронов].ItemData.MaxAmount;
                    
                    Debug.Log(to.InventoryItem.combinedItems[ItemType.Обойма_патронов].ItemData.MaxAmount + " ! " + to.InventoryItem.Amount);
                    
                    //если у оружия есть обойма патронов, то надо расчитать сколько патронов можно вставить и сколько останется, в случае если патронов больше чем влезет в обойму
                    if(to.InventoryItem.combinedItems[ItemType.Обойма_патронов].ItemData.MaxAmount <= to.InventoryItem.Amount)
                    {
                        return;
                    }
                    else
                    if(to.InventoryItem.combinedItems[ItemType.Обойма_патронов].ItemData.MaxAmount < to.InventoryItem.combinedItems[ItemType.Обойма_патронов].Amount + amount)
                    {
                        amount = to.InventoryItem.combinedItems[ItemType.Обойма_патронов].ItemData.MaxAmount - to.InventoryItem.combinedItems[ItemType.Обойма_патронов].Amount;          
                    }
                }
            }
            else
            if(to.InventoryItem.ItemData.MaxAmount < to.InventoryItem.Amount + amount)
            {
                amount = to.InventoryItem.ItemData.MaxAmount - to.InventoryItem.Amount;
            }

            from.UpdateAmount(-amount);

            to.UpdateAmount(amount);            
        }

        //существует ли и занят ли слот у overlapItem. Слот который соответствует типу itemType. true значит занят
        private bool CombineSlotIsFree(UIInventoryItem overlapItem, ItemType itemType) => !overlapItem.InventoryItem.combinedItems.Keys.Contains(itemType);

        //занят ли слот у overlapItem. Слот который соответствует типу inventoryItem. true значит занят
        private bool CombineSlotIsNotFree(UIInventoryItem inventoryItem, UIInventoryItem overlapItem) => !overlapItem.InventoryItem.combinedItems.Keys.Contains(inventoryItem.InventoryItem.ItemData.TypeItem);
        
        // можно ли в overlapItem поместить inventoryItem
        private bool CanBeCombinedItems(UIInventoryItem inventoryItem, UIInventoryItem overlapItem)
        {
            if(overlapItem.InventoryItem.ItemData.CanBeCombined == null) return false;

            foreach (ItemData item in overlapItem.InventoryItem.ItemData.CanBeCombined)
            {
                if(inventoryItem.InventoryItem.ItemData == item)
                {
                    return true;
                }
            }

            return false;
        }

        public void PlaceItem(UIInventoryItem inventoryItem, int posX, int posY)
        {
            RectTransform rectTransform = inventoryItem.rectTransform;

            rectTransform.SetParent(this.rectTransform);

            for (int x = 0; x < inventoryItem.WIDTH; x++)
            {
                for (int y = 0; y < inventoryItem.HEIGHT; y++)
                {
                    inventoryItemSlot[posX + x, posY + y] = inventoryItem;
                }
            }

            inventoryItem.onGridPositionX = posX;
            inventoryItem.onGridPositionY = posY;
            Vector2 positionItem = CalculatePositionOnGrid(inventoryItem, posX, posY);

            rectTransform.localPosition = positionItem;
        }

        public Vector2 CalculatePositionOnGrid(UIInventoryItem inventoryItem, int posX, int posY)
        {
            Vector2 positionItem = new Vector2();
            positionItem.x = posX * titleSizeWidth + titleSizeWidth * inventoryItem.WIDTH / 2;
            positionItem.y = -(posY * titleSizeHeight + titleSizeHeight * inventoryItem.HEIGHT / 2);
            return positionItem;
        }

        private bool CheckAvailabeSpace(int posX, int posY, int width, int height)
        {        
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if(inventoryItemSlot[posX + x, posY + y] != null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void GetFrestOverLap(int width, int height, ref UIInventoryItem overlapItem)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if(inventoryItemSlot[x, y] != null)
                    {
                        overlapItem = inventoryItemSlot[x, y];
                    }
                }
            }
        }

        private bool OverLapTheck(int posX, int posY, int width, int height, ref UIInventoryItem overlapItem)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if(inventoryItemSlot[posX + x, posY + y] != null)
                    {
                        if(overlapItem == null)
                        {
                            overlapItem = inventoryItemSlot[posX + x, posY + y];
                        }
                        else
                        {
                            if(overlapItem != inventoryItemSlot[posX + x, posY + y])
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        //метод поднять итем
        public UIInventoryItem SelectIteme(int x, int y)
        {
            UIInventoryItem toReturn = inventoryItemSlot[x, y];

            if (toReturn == null) { return null; }

            CleanGridReference(toReturn);

            return toReturn;
        }

        //очистить сылки на итем в сетке
        public void CleanGridReference(UIInventoryItem toReturn)
        {
            for (int ix = 0; ix < toReturn.WIDTH; ix++)
            {
                for (int iy = 0; iy < toReturn.HEIGHT; iy++)
                {
                    inventoryItemSlot[toReturn.onGridPositionX + ix, toReturn.onGridPositionY + iy] = null;
                }
            }
        }

        //метод проверка на позицию итема что все его части внутри сетки
        private bool PositionCheck(int posX, int posY)
        {
            if(posX < 0|| posY < 0)
            {
                return false;
            }

            if(posX >= GridSizeWidth || posY >= GridSizeHeight)
            {
                return false;
            }

            return true;
        }

        //проверка границ сетки, если позиция итема + его самая дальяя часть за сеткой то фалс
        public bool BoundryCheck(int posX, int posY, int width, int height)
        {
            if(PositionCheck(posX, posY) == false) {return false;}

            posX += width - 1;
            posY += height - 1;

            if(PositionCheck(posX, posY) == false) {return false;}

            return true;
        }

        internal IEnumerable<UIInventoryItem> GetItems()
        {
            foreach (var item in inventoryItemSlot)
            {
                if(item != null)
                {
                    yield return item;
                }
            }
        }
    }
}