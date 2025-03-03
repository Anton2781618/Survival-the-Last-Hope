
using System;
using System.Collections.Generic;
using System.Linq;
using ModularEventArchitecture;
using UnityEngine;
using UnityEngine.EventSystems;
using static InventoryDiablo.ItemData;


namespace InventoryDiablo
{
    //класс представляет из себя сетку с ячейками и данные о размерах
    //устанавливается на UI сетки
    public class UIItemGrid : MonoBehaviour
    {
        //-----------------------------------------------------------
        public GridData2 GridDataInfo;

        //-----------------------------------------------------------
        public RectTransform RectTransform;

        //-----------------------------------------------------------
        private Vector2 _mousePositionOnTheGrid = new Vector2();
        private Vector2Int _titeGridPosition = new Vector2Int();

        //-----------------------------------------------------------
        //префаб итема на сетки
        [SerializeField] private UIInventoryItem _itemPrefab;

        //-----------------------------------------------------------
        //пулл UI итемов 
        private List<UIInventoryItem> _uIInventoryItemsList = new List<UIInventoryItem>();
        private List<UIInventoryItem> _uIInventoryItemPool = new List<UIInventoryItem>();

        //!-----------------------------------------------------------

        [Tools.Button("Обновить сетку")] 
        public void UpdateSizeGrid()
        {
            Vector2 size = new Vector2(GridDataInfo.Size.x * global::GridData.titleSizeWidth, GridDataInfo.Size.y * global::GridData.titleSizeHeight);
            
            RectTransform.sizeDelta = size;
        }

        public void Setup(GridData2 gridData)
        {
            GridDataInfo = gridData;

            Vector2 size = new Vector2(gridData.Size.x * global::GridData.titleSizeWidth, gridData.Size.y * global::GridData.titleSizeHeight);
            
            RectTransform.sizeDelta = size;

            CreateItemsOnGrids();
        }

        //создать итемы на сетке. вызывается только когда предмет с сеткой размещаетсяя в слот
        private void CreateItemsOnGrids()
        {
            Tool.Helper.ResetCards(_uIInventoryItemsList, _uIInventoryItemPool);

            foreach (InventoryItem item in GridDataInfo.activeItems)
            {
                UIInventoryItem newUIInventoryItem = Tool.Helper.GetFreeCard(_itemPrefab, _uIInventoryItemPool);

                _uIInventoryItemsList.Add(newUIInventoryItem);

                newUIInventoryItem.transform.SetParent(transform, false);

                GridDataInfo.SetItemPosition(item, item.OnGridPosition);

                newUIInventoryItem.Setup(item);

                newUIInventoryItem.gameObject.SetActive(true);
            }
        }

        //выключить сетку, предварительно выключив все итемы на ней
        public void DeactiveGrid()
        {
            Tool.Helper.ResetCards(_uIInventoryItemsList, _uIInventoryItemPool);

            gameObject.SetActive(false);
        }

        //метод найти итем в сетке по координатам
        public UIInventoryItem GetUIItem(int x, int y)
        {
            var InventoryItem = GridDataInfo.GetItem(x, y);

            foreach (var item in _uIInventoryItemsList)
            {
                if(item.InventoryItem == InventoryItem)
                {
                    return item;
                }
                
            }
            return null;
        }

        //найти итем в списке по InventoryItem
        public UIInventoryItem GetUIItem(InventoryItem inventoryItem)
        {
            foreach (var item in _uIInventoryItemsList)
            {
                if(item.InventoryItem == inventoryItem)
                {
                    return item;
                }
            }
            return null;
        }
    
        //извлеч итем из сетки по координатам
        public virtual UIInventoryItem SelectIteme(int x, int y)
        {
            InventoryItem item = GridDataInfo.SelectIteme(x, y);

            if (item == null) { return null; }

            UIInventoryItem toReturn = GetUIItem(item);

            _uIInventoryItemsList.Remove(toReturn);

            if (toReturn == null) { return null; }
            
            return toReturn;
        }

        //метод возвращает координаты ячейки на сетке над которой находится мышь
        public Vector2Int GetTitleGridPosition(Vector2 mousePosition)
        {
            _mousePositionOnTheGrid.x = mousePosition.x - RectTransform.position.x;
            _mousePositionOnTheGrid.y = RectTransform.position.y - mousePosition.y;

            _titeGridPosition.x = (int)((_mousePositionOnTheGrid.x / global::GridData.titleSizeWidth) / transform.localScale.x);
            _titeGridPosition.y = (int)((_mousePositionOnTheGrid.y / global::GridData.titleSizeHeight) / transform.localScale.y );

            return _titeGridPosition;
        }

        //метод установить итем в слот
        public bool PlaceItem(UIInventoryItem inventoryItem, int posX, int posY, ref UIInventoryItem overlapItem)
        {
            // не можем расположить итем если он хотябы частично за сеткой
            if (BoundryCheck(posX, posY, inventoryItem.InventoryItem.WIDTH, inventoryItem.InventoryItem.HEIGHT) == false)
            {
                return false;
            }
            
            if(GridDataInfo.isSingle)
            {
                GetFrestOverLap(inventoryItem.InventoryItem.WIDTH, inventoryItem.InventoryItem.HEIGHT, ref overlapItem);
            }
            else
            {
                if (OverLapTheck(posX, posY, inventoryItem.InventoryItem.WIDTH, inventoryItem.InventoryItem.HEIGHT, ref overlapItem) == false)
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
                            overlapItem.InventoryItem.CombinedItems.Add(inventoryItem.InventoryItem.ItemData.TypeItem, inventoryItem.InventoryItem);
                            
                            overlapItem.UpdateAmount(inventoryItem.InventoryItem.Amount); 

                            inventoryItem.DestructSelf();
                        }
                        else 
                        {
                            TransferAmount(inventoryItem, overlapItem);
                        }

                        if(inventoryItem.InventoryItem.ItemData.TypeItem == ItemType.Патроны && inventoryItem.InventoryItem.Amount == 0 ) TransferAmount(inventoryItem, overlapItem);

                        // CleanGridReference(inventoryItem);

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
                        Debug.Log("у оружия есть обойма патронов " + overlapItem.InventoryItem.CombinedItems[ItemType.Обойма_патронов]);

                        TransferAmount(inventoryItem, overlapItem);

                        overlapItem = null;
                        
                        return false;
                    }
                }

                GridDataInfo.CleanGridReference(overlapItem.InventoryItem);
            }

            PlaceItem(inventoryItem, GridDataInfo.isSingle ? 0 : posX, GridDataInfo.isSingle ? 0 : posY);

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
                    to.InventoryItem.ItemData.MaxStackSize = to.InventoryItem.CombinedItems[ItemType.Обойма_патронов].ItemData.MaxStackSize;
                    
                    Debug.Log(to.InventoryItem.CombinedItems[ItemType.Обойма_патронов].ItemData.MaxStackSize + " ! " + to.InventoryItem.Amount);
                    
                    //если у оружия есть обойма патронов, то надо расчитать сколько патронов можно вставить и сколько останется, в случае если патронов больше чем влезет в обойму
                    if(to.InventoryItem.CombinedItems[ItemType.Обойма_патронов].ItemData.MaxStackSize <= to.InventoryItem.Amount)
                    {
                        return;
                    }
                    else
                    if(to.InventoryItem.CombinedItems[ItemType.Обойма_патронов].ItemData.MaxStackSize < to.InventoryItem.CombinedItems[ItemType.Обойма_патронов].Amount + amount)
                    {
                        amount = to.InventoryItem.CombinedItems[ItemType.Обойма_патронов].ItemData.MaxStackSize - to.InventoryItem.CombinedItems[ItemType.Обойма_патронов].Amount;          
                    }
                }
            }
            else
            if(to.InventoryItem.ItemData.MaxStackSize < to.InventoryItem.Amount + amount)
            {
                amount = to.InventoryItem.ItemData.MaxStackSize - to.InventoryItem.Amount;
            }

            from.UpdateAmount(-amount);

            to.UpdateAmount(amount);            
        }

        //существует ли и занят ли слот у overlapItem. Слот который соответствует типу itemType. true значит занят
        private bool CombineSlotIsFree(UIInventoryItem overlapItem, ItemType itemType) => !overlapItem.InventoryItem.CombinedItems.Keys.Contains(itemType);

        //занят ли слот у overlapItem. Слот который соответствует типу inventoryItem. true значит занят
        private bool CombineSlotIsNotFree(UIInventoryItem inventoryItem, UIInventoryItem overlapItem) => !overlapItem.InventoryItem.CombinedItems.Keys.Contains(inventoryItem.InventoryItem.ItemData.TypeItem);
        
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

        public virtual void PlaceItem(UIInventoryItem inventoryItem, int posX, int posY)
        {
            RectTransform rectTransform = inventoryItem.rectTransform;

            rectTransform.SetParent(this.RectTransform);

            _uIInventoryItemsList.Add(inventoryItem);

            GridDataInfo.PlaceItem(inventoryItem.InventoryItem, posX, posY);

            Vector2 positionItem = CalculatePositionOnGrid(inventoryItem, posX, posY);

            rectTransform.localPosition = positionItem;
        }

        public Vector2 CalculatePositionOnGrid(UIInventoryItem InventoryItemUI, int posX, int posY)
        {
            Vector2 positionItem = new Vector2();
            positionItem.x = posX * global::GridData.titleSizeWidth + global::GridData.titleSizeWidth * InventoryItemUI.InventoryItem.WIDTH / 2;
            positionItem.y = -(posY * global::GridData.titleSizeHeight + global::GridData.titleSizeHeight * InventoryItemUI.InventoryItem.HEIGHT / 2);
            return positionItem;
        }

        private void GetFrestOverLap(int width, int height, ref UIInventoryItem overlapItem)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // if(GridDataInfo.InventoryItems[x, y] != null)
                    if(GridDataInfo.GetItem(x, y) != null)
                    {
                        overlapItem = GetUIItem(x, y);
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
                    // if(GridDataInfo.InventoryItems[posX + x, posY + y] != null)
                    if(GridDataInfo.GetItem(posX + x, posY + y) != null)
                    {
                        if(overlapItem == null)
                        {
                            overlapItem = GetUIItem(posX + x, posY + y);
                        }
                        else
                        {
                            if(overlapItem != GetUIItem(posX + x, posY + y))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        //метод проверка на позицию итема что все его части внутри сетки
        private bool PositionCheck(int posX, int posY)
        {
            if(posX < 0|| posY < 0)
            {
                return false;
            }

            if(posX >= GridDataInfo.Size.x || posY >= GridDataInfo.Size.y)
            {
                return false;
            }

            return true;
        }

        //проверка границ сетки, если позиция итема + его самая дальяя часть за сеткой то фалс
        public bool BoundryCheck(int posX, int posY, int width, int height)
        {
            // Debug.Log("BoundryCheck " + posX + " " + posY + " " + width + " " + height);
            if(PositionCheck(posX, posY) == false) {return false;}

            posX += width - 1;
            posY += height - 1;

            if(PositionCheck(posX, posY) == false) {return false;}

            return true;
        }

        public IEnumerable<UIInventoryItem> GetItems()
        {
            foreach (var item in _uIInventoryItemsList)
            {
                if(item != null)
                {
                    yield return item;
                }
            }
        }
    }
}