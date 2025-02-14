
using System;
using System.Collections.Generic;
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

    public void PlaceItem(InventoryItem inventoryItem, int posX, int posY)
    {
        for (int x = 0; x < inventoryItem.WIDTH; x++)
        {
            for (int y = 0; y < inventoryItem.HEIGHT; y++)
            {
                InventoryItems[posX + x, posY + y] = inventoryItem;
            }
        }

        inventoryItem.OnGridPositionX = posX;
        inventoryItem.OnGridPositionY = posY;

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
                InventoryItems[toReturn.OnGridPositionX + ix, toReturn.OnGridPositionY + iy] = null;
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

[Serializable]
public class GridData2 : ISerializationCallbackReceiver
{
    //-------------------------------------------------------------------------------------
    [Header("Размер сетки")]
    public Vector2Int GridSize = new Vector2Int(5, 5);
    
    //-------------------------------------------------------------------------------------
    [Header("Максимальный размер стека")]
    public int MaxStackSize; // Максимальный размер стека для предметов на этой сетке

    //-------------------------------------------------------------------------------------
    [Header("Позиция сетки")]
    public Vector2 Position;

    //-------------------------------------------------------------------------------------
    [Header("Блок совместимости с предметами")]
    
    //поле указывает какие методы проверки совместимости необходимо исользовать при расположении итема на сетку
    public Compatibility CompatibilityGridMod = Compatibility.Access_public; 
    //в случае если сетка предназначена только для конкретного итема
    public List<ItemData> specificItemCombined = new List<ItemData>(); 

    //в случае если сетка предназначена для группы итемов
    public string CompatibleGroup = "default";

    public enum Compatibility
    {
        Access_public, //сетка публичная
        Access_by_item_groups,//дать доступ к сетке только выбранной группе итемов
        Access_by_specific_item //дать доступ к сетке только конкретному итему
    }

    //-------------------------------------------------------------------------------------
    [Header("Блок итемов размещенных на сетке")]
    [SerializeField] public List<InventoryItem> activeItems = new List<InventoryItem>();

    //-------------------------------------------------------------------------------------
        //настройки сериалиации
        // Указываем максимальную глубину сериализации
        private const int MaxDepth = 3;
        //поле для хранения текущей глубины
        [NonSerialized] private int currentDepth;
        
    //-------------------------------------------------------------------------------------

    
    
    
    public bool TryPlaceItem(InventoryItem item)
    {
        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                if (CheckAvailableSpace(x, y, item.WIDTH, item.HEIGHT))
                {
                    item.OnGridPositionX = x;
                    item.OnGridPositionY = y;
                    // activeItems.Add(new ItemPosition { Item = item, X = x, Y = y });
                    activeItems.Add(item);
                    return true;
                }
            }
        }
        return false;
    }

    public bool CheckAvailableSpace(int posX, int posY, int width, int height)
    {
        // Проверка выхода за границы сетки
        if (posX + width > GridSize.x || posY + height > GridSize.y)
        {
            return false;
        }

        foreach (var items in activeItems)
        {
            if (DoRectsIntersect(posX, posY, width, height, items.OnGridPositionX, items.OnGridPositionY, items.WIDTH, items.HEIGHT))
            {
                return false;
            }
        }
        return true;
    }

    private bool DoRectsIntersect(int x1, int y1, int w1, int h1, int x2, int y2, int w2, int h2)
    {
        return x1 < x2 + w2 && x1 + w1 > x2 &&
               y1 < y2 + h2 && y1 + h1 > y2;
    }

    public bool ValidateItemsPosition()
    {
        List<InventoryItem> tempItems = new List<InventoryItem>(activeItems);
        activeItems.Clear();

        foreach (var item in tempItems)
        {
            // Проверка размеров предмета
            if (item.WIDTH > GridSize.x || item.HEIGHT > GridSize.y)
            {
                activeItems.Clear();
                activeItems.AddRange(tempItems);
                return false;
            }

            bool found = false;
            for (int x = 0; x < GridSize.x && !found; x++)
            {
                for (int y = 0; y < GridSize.y && !found; y++)
                {
                    if (CheckAvailableSpace(x, y, item.WIDTH, item.HEIGHT))
                    {
                        activeItems.Add(item);
                        found = true;
                    }
                }
            }
            
            if (!found)
            {
                activeItems.Clear();
                activeItems.AddRange(tempItems);
                return false;
            }
        }
        return true;
    }

    public void PlaceItem(int x, int y, InventoryItem item)
    {
        item.OnGridPositionX = x;
        item.OnGridPositionY = y;
        activeItems.Add(item);
    }

    //вызывается перед сериализацией
    public void OnBeforeSerialize()
    {
        if (currentDepth >= MaxDepth)
        {
            // activeItems = null;
        }
        else 
        {
            // Увеличиваем текущую глубину
            currentDepth++;
        }
    }

    //вызывается после десериализации
    public void OnAfterDeserialize()
    {
        // Сбрасываем текущую глубину
        currentDepth = 0;
    }
}