
using System;
using System.Linq;
using InventoryDiablo;
using UnityEngine;
using static InventoryDiablo.ItemData;

[Serializable]
public class GridData
{
    public const float titleSizeWidth = 32;
    public const float titleSizeHeight = 32;

    //это ссылка на того чей инвентарь
    public Inventory owner {get; set;}
    
    [NonSerialized] public InventoryItem[,] InventoryItems;
    
    [Header("Размер сетки")]
    public int GridSizeWidth = 20; 
    public int GridSizeHeight = 10; 

    [Header("Зона определений возиможных предметов")]
    [SerializeField] private ItemType gridForItemsType;

    //поле определяет сетка для одного предмета или нет
    public bool isSingle = false;
    // имя сетки
    public GridInfo GridName = GridInfo.BackpackGrid;
    public enum GridInfo
    {
        BackpackSlot,
        BackpackGrid,
        PistolSlot,
        RifleSlot,
        UnloadingSlot,
        UnloadingGrid,
        HelmetSlot,
    }

    //устанавлмвает начальный размер сетки
    public void Init(int width, int height)
    {
        InventoryItems = new InventoryItem[width, height];
    }

    public void Init()
    {
        InventoryItems = new InventoryItem[GridSizeWidth, GridSizeHeight];
    }

    public ItemType GetGridForItemsType()
    {
        return gridForItemsType;
    }

    public InventoryItem GetItem(int x, int y)
    {
        return InventoryItems[x, y];
    }

    //находит свободное место на сетке для объекта
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

    //переложить амоунт из одного итема в другой, ограничение на максимальное количество 
    private void TransferAmount(UIInventoryItem from, UIInventoryItem to)
    {
        int amount = from.InventoryItem.Amount;

        if(to.InventoryItem.ItemData.TypeItem == ItemType.Оружие && from.InventoryItem.ItemData.TypeItem == ItemType.Коробка_патронов)
        {
            if(!CombineSlotIsFree(to, ItemType.Обойма_патронов))
            {
                to.InventoryItem.ItemData.MaxAmount = to.InventoryItem.CombinedItems[ItemType.Обойма_патронов].ItemData.MaxAmount;
                
                Debug.Log(to.InventoryItem.CombinedItems[ItemType.Обойма_патронов].ItemData.MaxAmount + " ! " + to.InventoryItem.Amount);
                
                //если у оружия есть обойма патронов, то надо расчитать сколько патронов можно вставить и сколько останется, в случае если патронов больше чем влезет в обойму
                if(to.InventoryItem.CombinedItems[ItemType.Обойма_патронов].ItemData.MaxAmount <= to.InventoryItem.Amount)
                {
                    return;
                }
                else
                if(to.InventoryItem.CombinedItems[ItemType.Обойма_патронов].ItemData.MaxAmount < to.InventoryItem.CombinedItems[ItemType.Обойма_патронов].Amount + amount)
                {
                    amount = to.InventoryItem.CombinedItems[ItemType.Обойма_патронов].ItemData.MaxAmount - to.InventoryItem.CombinedItems[ItemType.Обойма_патронов].Amount;          
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

    public void PlaceItem(InventoryItem inventoryItem, int posX, int posY)
    {
        for (int x = 0; x < inventoryItem.WIDTH; x++)
        {
            for (int y = 0; y < inventoryItem.HEIGHT; y++)
            {
                InventoryItems[posX + x, posY + y] = inventoryItem;
            }
        }

        inventoryItem.onGridPositionX = posX;
        inventoryItem.onGridPositionY = posY;

        inventoryItem.GridName = GridName;
    }

    private bool CheckAvailabeSpace(int posX, int posY, int width, int height)
    {        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(InventoryItems[posX + x, posY + y] != null)
                {
                    return false;
                }
            }
        }
        return true;
    }    

    //метод поднять итем
    public InventoryItem SelectIteme(int x, int y)
    {
        InventoryItem toReturn = InventoryItems[x, y];

        if (toReturn == null) { return null; }

        CleanGridReference(toReturn);

        return toReturn;
    }

    //очистить сылки на итем в сетке
    public void CleanGridReference(InventoryItem toReturn)
    {
        for (int ix = 0; ix < toReturn.WIDTH; ix++)
        {
            for (int iy = 0; iy < toReturn.HEIGHT; iy++)
            {
                InventoryItems[toReturn.onGridPositionX + ix, toReturn.onGridPositionY + iy] = null;
            }
        }
    }
    
    //полностьб очистить сетку
    public void Clear()
    {
        for (int x = 0; x < GridSizeWidth; x++)
        {
            for (int y = 0; y < GridSizeHeight; y++)
            {
                InventoryItems[x, y] = null;
            }
        }
    }
}
