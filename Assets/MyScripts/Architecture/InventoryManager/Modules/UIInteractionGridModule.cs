using InventoryDiablo;
using UnityEngine;

namespace ModularEventArchitecture
{
    [CompatibleUnit(typeof(InventoryManager))]
    public class UIInteractionGridModule : ModuleBase
    {
        //---------------------------------------------------
        //класс является системой управления всех ивентарей, основной функцианал инвентарей находится тут
        [SerializeField] private UIContextMenu _contextMenu;

        //---------------------------------------------------
        private UIItemGrid _selectedGrid;
        public UIItemGrid SelectedGrid 
        {
            get => _selectedGrid; 
            
            set 
            {
                _selectedGrid = value;

                _inventoryIHighLight.SetParent(value);
            }
        }

        //---------------------------------------------------
        private UIInventoryItem _uiSelectedItem;
        private UIInventoryItem _overlapItem;
        
        //---------------------------------------------------
        private RectTransform _itemRectTransform;
        
        //---------------------------------------------------
        [SerializeField] private Canvas _canvas;

        //---------------------------------------------------
        [SerializeField] private UIInventoryItem _itemPrefab;
        [SerializeField] private AvailableItems _availableItems;

        //---------------------------------------------------
        private Vector2Int _oldPosition;
        private UIInventoryItem _itemToHighLight;
        //---------------------------------------------------
        [SerializeField] private InventoryIHighLight _inventoryIHighLight;

        //!---------------------------------------------------

        public override void Initialize()
        {
            Entity.Globalevents.Add((EventsInventory.Select_Grid, (data) => SelectedGrid = ((SelectGridEventData)data).ItemGrid));
            
            Entity.Globalevents.Add((EventsInventory.Item_Spawned_On_Cursor, (data) => CreateRandomItem()));
        }

        public override void UpdateMe() 
        {
            ItemIconDrag();

            if(Input.GetKeyDown(KeyCode.M))
            {
                if(_uiSelectedItem == null)
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

            if(SelectedGrid == null)
            {
                _inventoryIHighLight.Show(false);

                if(Input.GetMouseButtonDown(0))
                {
                    if(_uiSelectedItem) DropItem(_uiSelectedItem.InventoryItem);
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
            if(_uiSelectedItem == null) {return;}

            _uiSelectedItem.Rotated();
        }

        [ContextMenu("InsertRandomItem")]
        public void InsertRandomItem()
        {
            if(SelectedGrid == null) 
            {
                Debug.LogError("Не выбрана сетка для вставки предмета");

                return;
            }

            CreateRandomItem();

            UIInventoryItem itemToInsert = _uiSelectedItem;

            _uiSelectedItem = null;
            
            InsertItemOnGrid(itemToInsert, SelectedGrid);
        }

        private void InsertItemOnGrid(UIInventoryItem itemToInsert, UIItemGrid grid)
        {
            // Debug.Log($"Вставка предмета {itemToInsert.InventoryItem.ItemData.Title} на сетку {grid}");
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

        //метод подсветки предмета
        private void HandleHighlight()
        {
            Vector2Int positionOnGrid = GetTitleGridPosition();

            if(_oldPosition == positionOnGrid){return;}
            
            _oldPosition = positionOnGrid;
            if(_uiSelectedItem == null)
            {
                _itemToHighLight = SelectedGrid.GetUIItem(positionOnGrid.x, positionOnGrid.y);
                
                if(_itemToHighLight != null)
                {
                    _inventoryIHighLight.Show(true);
                    _inventoryIHighLight.SetSize(_itemToHighLight);
                    _inventoryIHighLight.SetPosition(SelectedGrid, _itemToHighLight);
                }
                else
                {
                    _inventoryIHighLight.Show(false);
                }
            }
            else
            {
                if(SelectedGrid.GridDataInfo.ValidateItem(_uiSelectedItem.InventoryItem))
                {
                    _inventoryIHighLight.Show(SelectedGrid.BoundryCheck(positionOnGrid.x, positionOnGrid.y, _uiSelectedItem.InventoryItem.WIDTH, _uiSelectedItem.InventoryItem.HEIGHT));
                    _inventoryIHighLight.SetSize(_uiSelectedItem);
                    _inventoryIHighLight.SetPosition(SelectedGrid, _uiSelectedItem, positionOnGrid.x, positionOnGrid.y);            
                }
            }
        }

        //создать случайный итем
        public void CreateRandomItem(/* UIItemGrid itemGrid */)
        {
            UIInventoryItem newUIinventoryItem = Instantiate(_itemPrefab);
            _uiSelectedItem = newUIinventoryItem;

            _itemRectTransform = newUIinventoryItem.rectTransform;
            _itemRectTransform.SetParent(_canvas.transform);
            _itemRectTransform.SetAsLastSibling();
            
            int selectedItemID = UnityEngine.Random.Range(0, _availableItems.items.Count);

            _uiSelectedItem.Setup(_availableItems.items[selectedItemID].Clone());
        }

        private void CreateItem(InventoryItem inventoryItem)
        {
            UIInventoryItem uiInventoryItem = Instantiate(_itemPrefab);
            _uiSelectedItem = uiInventoryItem;

            _itemRectTransform = uiInventoryItem.rectTransform;
            _itemRectTransform.SetParent(_canvas.transform);
            _itemRectTransform.SetAsLastSibling();

            _uiSelectedItem.Setup(inventoryItem);
        }

        //метод перемещает итем в след за мышкой
        private void ItemIconDrag()
        {
            if(_uiSelectedItem)
            {
                //расчитать позицию итема с учетом изменения размера итема
                _itemRectTransform.position = Input.mousePosition - new Vector3((_itemRectTransform.sizeDelta.x * _itemRectTransform.localScale.x) / (_itemRectTransform.lossyScale.x * 10), 0);
            }
        }

        //метод выделяет итем и станавлевает его в ячейку
        private void LeftMouseButtonPress()
        {
            Vector2Int titleGridPosition = GetTitleGridPosition();
            
            if (_uiSelectedItem == null)
            {
                PickUpItem(titleGridPosition);
            }
            else
            {
                PlaceItem(titleGridPosition);
            }

            _contextMenu.Show(false);
        }

        //открыть контектсное меню
        private void RightMouseButtonPress()
        {
            if (_uiSelectedItem != null) return;

            Vector2Int titleGridPosition = GetTitleGridPosition();
            UIInventoryItem uiInventoryItem = SelectedGrid.GetUIItem(titleGridPosition.x, titleGridPosition.y);
            
            if(!uiInventoryItem)return;
            
            _contextMenu.Setup(uiInventoryItem);
        }

        //тут мы устанавливаем итем на сетку со смещением. Это для того что бы распологать итем по центру а не с краю мышки 
        private Vector2Int GetTitleGridPosition()
        {
            Vector2 position = Input.mousePosition;
            
            if (_uiSelectedItem != null)
            {
                position.x -= (_uiSelectedItem.InventoryItem.WIDTH - 1) * GridData.titleSizeWidth / 2;
                position.y += (_uiSelectedItem.InventoryItem.HEIGHT - 1) * GridData.titleSizeHeight / 2;
            }

            return SelectedGrid.GetTitleGridPosition(position);
        }

        //поднять предмет с сетки
        private void PickUpItem(Vector2Int titleGridPosition)
        {
            _uiSelectedItem = SelectedGrid.SelectIteme(titleGridPosition.x, titleGridPosition.y);

            if (_uiSelectedItem)
            {
                _uiSelectedItem.transform.SetParent(_canvas.transform);
                _itemRectTransform = _uiSelectedItem.rectTransform;
                _itemRectTransform.SetAsLastSibling();
            }
        }

        //расположить итем на сетке
        private void PlaceItem(Vector2Int titleGridPosition)
        {
            bool complete = SelectedGrid.GridDataInfo.ValidateItem(_uiSelectedItem.InventoryItem) ? SelectedGrid.PlaceItem(_uiSelectedItem, titleGridPosition.x, titleGridPosition.y, ref _overlapItem) : false;
            
            if(complete)
            {
                _uiSelectedItem = null;
                
                if(_overlapItem != null)
                {
                    _uiSelectedItem = _overlapItem;
                
                    _overlapItem = null;
                
                    _itemRectTransform = _uiSelectedItem.rectTransform;
                
                    _itemRectTransform.SetAsLastSibling();
                }
            }
        }

        //Выкинуть предмет
        public void DropItem(InventoryItem item)
        {
            GlobalEventBus.Instance.Publish(EventsSpawner.SpawnUnitOnStreet, new EventDataUnit {Item = _uiSelectedItem.InventoryItem} );
                
            _uiSelectedItem.DestructSelf();

            _uiSelectedItem = null;
        }
    }
}