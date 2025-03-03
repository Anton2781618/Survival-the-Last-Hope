using InventoryDiablo;
using UnityEngine;
using static GridData2;

namespace ModularEventArchitecture
{
    [CompatibleUnit(typeof(InventoryManager))]
    public class UIInteractionGridModule : ModuleBase
    {
        //---------------------------------------------------
        //класс является системой управления всех ивентарей, основной функцианал инвентарей находится тут
        [SerializeField] private UIContextMenu contextMenu;

        //---------------------------------------------------
        private UIItemGrid selectedGrid;
        public UIItemGrid SelectedGrid 
        {
            get => selectedGrid; 
            
            set 
            {
                selectedGrid = value;

                inventoryIHighLight.SetParent(value);
            }
        }

        //---------------------------------------------------
        private UIInventoryItem UIselectedItem;
        private UIInventoryItem overlapItem;
        
        //---------------------------------------------------
        private RectTransform itemRectTransform;
        
        //---------------------------------------------------
        [SerializeField] private Canvas canvas;

        //---------------------------------------------------
        [SerializeField] private UIInventoryItem itemPrefab;
        [SerializeField] private AvailableItems availableItems;

        //---------------------------------------------------
        private Vector2Int _oldPosition;
        private UIInventoryItem _itemToHighLight;
        //---------------------------------------------------
        [SerializeField] private InventoryIHighLight inventoryIHighLight;

        //!---------------------------------------------------

        public override void Initialize()
        {
            Entity.Globalevents.Add((EventsInventory.SeletGrid, (data) => SelectedGrid = ((SelectGridEventData)data).ItemGrid));
            
            Entity.Globalevents.Add((EventsInventory.CreateAndInsertItem, (data) => OnCreateAndInsertItem(((CreateAndInsertItemEventData)data).InventoryItem, ((CreateAndInsertItemEventData)data).ItemGrid)));
            
            Entity.Globalevents.Add((EventsInventory.Item_Spawned_InHand, (data) => CreateRandomItem(((UIItemGridEvent)data).grid)));
        }

        public override void UpdateMe() 
        {
            ItemIconDrag();

            if(Input.GetKeyDown(KeyCode.M))
            {
                if(UIselectedItem == null)
                {
                    CreateRandomItem(selectedGrid);
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

            if(SelectedGrid == null)
            {
                inventoryIHighLight.Show(false);

                if(Input.GetMouseButtonDown(0))
                {
                    if(UIselectedItem) DropItem(UIselectedItem.InventoryItem);
                }

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

        private void RotateItem()
        {
            if(UIselectedItem == null) {return;}

            UIselectedItem.Rotated();
        }

        [ContextMenu("InsertRandomItem")]
        public void InsertRandomItem()
        {
            if(selectedGrid == null) 
            {
                Debug.LogError("Не выбрана сетка для вставки предмета");

                return;
            }

            CreateRandomItem(selectedGrid);

            UIInventoryItem itemToInsert = UIselectedItem;

            UIselectedItem = null;

            
            InsertItemOnGrid(itemToInsert, selectedGrid);
        }

        private void InsertItemOnGrid(UIInventoryItem itemToInsert, UIItemGrid grid)
        {
            // Debug.Log($"Вставка предмета {itemToInsert.InventoryItem.ItemData.Title} на сетку {grid}");
            Debug.Log(grid);
            Vector2Int? posOnGrid = grid.GridDataInfo.FindSpaceForObject(itemToInsert.InventoryItem);
            
            if(posOnGrid == null) 
            {
                Debug.Log($"На сетке {grid} нет места для {itemToInsert.InventoryItem.ItemData.Title}");
                Debug.Log("ВНИМАНИЕ! Надо переделать так что бы сначало проверялась сетка на наличие места, а потом ставился итем");
                Destroy(itemToInsert.gameObject);

                return;
            }

            grid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
        }

        //!!!Этот метод создает итем и устанавливает его на сетку ОБРАЩАТЬСЯ ЧЕРЕЗ НЕГО
        //создать физически итем и установить его на сетку 
        public void OnCreateAndInsertItem(InventoryItem inventoryItem, UIItemGrid grid)
        {
            Debug.Log($"Создан предмет {inventoryItem.ItemData.Title}");
            
            CreateItem(inventoryItem);
            
            UIInventoryItem itemToInsert = UIselectedItem;
            
            UIselectedItem = null;
            
            InsertItemOnGrid(itemToInsert, grid);
        }

        //метод подсветки предмета
        private void HandleHighlight()
        {
            Vector2Int positionOnGrid = GetTitleGridPosition();

            if(_oldPosition == positionOnGrid){return;}
            
            _oldPosition = positionOnGrid;
            if(UIselectedItem == null)
            {
                _itemToHighLight = SelectedGrid.GetUIItem(positionOnGrid.x, positionOnGrid.y);
                
                if(_itemToHighLight != null)
                {
                    inventoryIHighLight.Show(true);
                    inventoryIHighLight.SetSize(_itemToHighLight);
                    inventoryIHighLight.SetPosition(SelectedGrid, _itemToHighLight);
                }
                else
                {
                    inventoryIHighLight.Show(false);
                }
            }
            else
            {
                if(ValidateItem())
                {
                    inventoryIHighLight.Show(SelectedGrid.BoundryCheck(positionOnGrid.x, positionOnGrid.y, UIselectedItem.InventoryItem.WIDTH, UIselectedItem.InventoryItem.HEIGHT));
                    inventoryIHighLight.SetSize(UIselectedItem);
                    inventoryIHighLight.SetPosition(SelectedGrid, UIselectedItem, positionOnGrid.x, positionOnGrid.y);            
                }
            }
        }
    
        //проверка выделеного предмета и выделеной сетки на валидность для установки на сетку
        private bool ValidateItem()
        {
            if(SelectedGrid.GridDataInfo.CompatibilityGridMod == Compatibility.Access_public)
            {
                return true;
            }
            else
            if(SelectedGrid.GridDataInfo.CompatibilityGridMod == Compatibility.Access_by_item_groups)
            {
                //если группы не совпадают то не подсвечивать
                if(UIselectedItem.InventoryItem.ItemData.ItemGroup == SelectedGrid.GridDataInfo.CompatibleGroup) 
                {
                    return true;
                }
            }
            else
            if(SelectedGrid.GridDataInfo.CompatibilityGridMod == Compatibility.Access_by_specific_item)
            {
                foreach (var SpecificItemData in SelectedGrid.GridDataInfo.SpecificItemCombined)
                {
                    if(UIselectedItem.InventoryItem.ItemData == SpecificItemData) 
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        //создать случайный итем
        public void CreateRandomItem(UIItemGrid itemGrid)
        {
            UIInventoryItem newUIinventoryItem = Instantiate(itemPrefab);
            UIselectedItem = newUIinventoryItem;

            itemRectTransform = newUIinventoryItem.rectTransform;
            itemRectTransform.SetParent(canvas.transform);
            itemRectTransform.SetAsLastSibling();
            
            int selectedItemID = UnityEngine.Random.Range(0, availableItems.items.Count);

            UIselectedItem.Setup(availableItems.items[selectedItemID].Clone());
        }

        private void CreateItem(InventoryItem inventoryItem)
        {
            UIInventoryItem uiInventoryItem = Instantiate(itemPrefab);
            UIselectedItem = uiInventoryItem;

            itemRectTransform = uiInventoryItem.rectTransform;
            itemRectTransform.SetParent(canvas.transform);
            itemRectTransform.SetAsLastSibling();

            UIselectedItem.Setup(inventoryItem);
        }

        //метод перемещает итем в след за мышкой
        private void ItemIconDrag()
        {
            if(UIselectedItem)
            {
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
        }

        //открыть контектсное меню
        private void RightMouseButtonPress()
        {
            if (UIselectedItem != null) return;

            Vector2Int titleGridPosition = GetTitleGridPosition();
            UIInventoryItem uiInventoryItem = SelectedGrid.GetUIItem(titleGridPosition.x, titleGridPosition.y);
            
            if(!uiInventoryItem)return;
            
            contextMenu.Setup(uiInventoryItem);
        }

        //тут мы устанавливаем итем на сетку со смещением. Это для того что бы распологать итем по центру а не с краю мышки 
        private Vector2Int GetTitleGridPosition()
        {
            Vector2 position = Input.mousePosition;
            
            if (UIselectedItem != null)
            {
                position.x -= (UIselectedItem.InventoryItem.WIDTH - 1) * GridData.titleSizeWidth / 2;
                position.y += (UIselectedItem.InventoryItem.HEIGHT - 1) * GridData.titleSizeHeight / 2;
            }

            return SelectedGrid.GetTitleGridPosition(position);
        }

        //поднять предмет с сетки
        private void PickUpItem(Vector2Int titleGridPosition)
        {
            UIselectedItem = SelectedGrid.SelectIteme(titleGridPosition.x, titleGridPosition.y);

            if (UIselectedItem)
            {
                UIselectedItem.transform.SetParent(canvas.transform);
                itemRectTransform = UIselectedItem.rectTransform;
                itemRectTransform.SetAsLastSibling();
            }
        }

        //расположить итем на сетке
        private void PlaceItem(Vector2Int titleGridPosition)
        {
            bool complete = ValidateItem() ? SelectedGrid.PlaceItem(UIselectedItem, titleGridPosition.x, titleGridPosition.y, ref overlapItem) : false;
            
            if(complete)
            {
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

        //Выкинуть предмет
        public void DropItem(InventoryItem item)
        {
            GlobalEventBus.Instance.Publish(EventsSpawner.SpawnUnitOnStreet, new EventDataUnit {Item = UIselectedItem.InventoryItem} );
                
            UIselectedItem.DestructSelf();

            UIselectedItem = null;
        }
    }
}