using UnityEditor;
using UnityEngine;
using InventoryDiablo;
using System;
using static InventoryDiablo.ItemData;
using Unity.VisualScripting;
using Tool;
using System.Collections.Generic;
using static GridData2;

namespace ModularEventArchitecture
{
    //Эдитор инвентаря
    public class InventoryEditor : EditorWindow
    {
        //---------------------------------------------------
        // private InventoryModule targetModule;
        private Inventory _targetInventory;

        //---------------------------------------------------
        //скроллы
        private Vector2 _scrollPosition;
        private Vector2 _slotScroll;

        //---------------------------------------------------
        public int ItemGroupIndex = 0;
        //---------------------------------------------------
        private float _cellSize = 32f;
        private float _gridCellSize = 10f;
        private InventoryItem _selectedItem;
        private AvailableItems _availableItems;
        private Vector2 _itemListScroll;

        //---------------------------------------------------
        // Добавьте в класс InventoryEditor
        private static Dictionary<ItemType, Vector2Int> slotSizes = new Dictionary<ItemType, Vector2Int>
        {
            { ItemType.Шлем, new Vector2Int(144, 184) },
            { ItemType.Оружие, new Vector2Int(320, 100) },
            { ItemType.Разгрузка, new Vector2Int(480, 180) },
            { ItemType.Рюкзак, new Vector2Int(800, 270) },
            { ItemType.Ремень, new Vector2Int(150, 180) },
            // Добавьте другие типы предметов и их размеры
        };

        //---------------------------------------------------
        private bool isDragging = false;
        private Vector2 mousePosition;

        private InventoryItem draggedItem;
        private InventoryItem originalPosition;

        // private bool isDraggingGrid = false;
        private bool editSlotGrid = false;
        private Vector2 dragStartPosition;
        private Vector2 initialGridPosition;
        
        //---------------------------------------------------
        private GridData2 draggedGrid;
        private InventoryContainer _currentInventoryContainer;
        private Vector2 dragOffset;
        private InventoryModule _inventoryModule;

        //---------------------------------------------------
        private int draggedSlotIndex = -1;
        private bool isDraggingSlot = false;
        private Vector2 slotDragOffset;

        //---------------------------------------------------
        bool open = false;
        //---------------------------------------------------
        private HierarchyItemTypesBuilder _asset;
        private HierarchyItemTypesBuilder Asset
        {
            get
            {
                if (_asset == null)
                {
                    string[] guids = AssetDatabase.FindAssets("t:HierarchyItemTypesBuilder");
                    if (guids.Length > 0)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                        _asset = AssetDatabase.LoadAssetAtPath<HierarchyItemTypesBuilder>(path);
                    }
                }
                return _asset;
            }
        }

        //!---------------------------------------------------


        [MenuItem("Tools/Редактор Инвентарей")]
        public static void ShowWindow() => GetWindow<InventoryEditor>("Inventory Editor");

        private void OnGUI()
        {
            if (_targetInventory == null)
            {
                Debug.Log("Target inventory is null");
                DrawModuleSelectionBlock();
                return;
            }

            EditorGUILayout.BeginHorizontal();
            
            // Левая панель - список предметов
            if (editSlotGrid)
            {
                if (GUILayout.Button("Назад", GUILayout.Width(80), GUILayout.Height(50))) 
                {
                    _currentInventoryContainer = null;

                    editSlotGrid = false;

                    GUIUtility.ExitGUI();
                }
                
                //рисуем окно редактирования слота
                DrawSlotEditorWindow();
            }
            else
            {
                
                //рисуем окно инвентаря
                DrawInventoryWindow();
            }

            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawModuleSelectionBlock()
        {
            EditorGUILayout.HelpBox("Выберите инвентарь", MessageType.Info);
            _inventoryModule = EditorGUILayout.ObjectField("Module", _inventoryModule, typeof(InventoryModule), true) as InventoryModule;
            
            if (_inventoryModule != null) _targetInventory = _inventoryModule.Inventory;
        }

        public void SetTartget(InventoryModule inventoryModule)
        {
            _inventoryModule = inventoryModule;
            _targetInventory = inventoryModule.Inventory;
        }

        //рисуем окно редактирования слота
        private void DrawSlotEditorWindow()
        {
            DrawItemsList();

            DrawSlotGridInfo();

            DrawInventoryGrid();            
        }

        //рисуем итемы которые можн 
        private void DrawItemsList()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.Width(210));
            
            if (_availableItems == null)
            {
                _availableItems = AssetDatabase.LoadAssetAtPath<AvailableItems>("Assets/MyAssets/ScriptableObjects/AvailableItems.asset");
                if (_availableItems == null)
                {
                    EditorGUILayout.HelpBox("Create AvailableItems asset", MessageType.Warning);
                    
                    if (GUILayout.Button("Создать"))
                    {
                        _availableItems = CreateInstance<AvailableItems>();
                        AssetDatabase.CreateAsset(_availableItems, "Assets/Data/AvailableItems.asset");
                    }
                    
                    EditorGUILayout.EndVertical();
                    return;
                }
            }

            EditorGUILayout.LabelField("Список предметов", EditorStyles.boldLabel);
            
                _itemListScroll = EditorGUILayout.BeginScrollView(_itemListScroll, "Box");
                
                    foreach (var item in _availableItems.items)
                    {
                        if (Helper.AreHasFlag(item.ItemData.TypeItem, _currentInventoryContainer.TypeItem)) continue;

                        GUI.backgroundColor = _selectedItem != null && _selectedItem.ItemData.Title == item.ItemData.Title ? Color.cyan : Color.white;

                        if (GUILayout.Button(item.ItemData.Title))
                        {
                            _selectedItem = item.Clone();
                        }

                        GUI.backgroundColor = Color.white;
                    }
                    
                EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();
        }
        
        //рисуем окно инвентаря
        private void DrawInventoryWindow()
        {
            EditorGUILayout.BeginHorizontal();

                DrawInventoryBackround();

                DrawContainerOnBackground();

                _slotScroll = EditorGUILayout.BeginScrollView(_slotScroll, "Box", GUILayout.Width(330));

                DrawContainerInfo();

                EditorGUILayout.EndScrollView();


            EditorGUILayout.EndHorizontal();
        }

        //создать задний фон инвентаря
        private void DrawInventoryBackround()
        {
            Rect cellRect = new Rect(0, 0, _targetInventory.InventoryWindowSize.x, _targetInventory.InventoryWindowSize.y);

            EditorGUI.DrawRect(cellRect, Color.gray);
        }

        private void DrawContainerInfo()
        {
            if (GUILayout.Button("Сохранить")) 
            {  
                EditorUtility.SetDirty(_inventoryModule);

                // Сохраняем ассеты
                AssetDatabase.SaveAssets();
                
                // Обновляем базу ассетов
                AssetDatabase.Refresh();
                
            }
            if (GUILayout.Button("Добавить Контейнер инвентаря"))            
            {
                _targetInventory.InventoryContainers.Add(new InventoryContainer());
            }

            _targetInventory.InventoryWindowSize = EditorGUILayout.Vector2Field("Размеры", _targetInventory.InventoryWindowSize);

            for (int i = 0; i < _targetInventory.InventoryContainers.Count; i++)
            {
                var container = _targetInventory.InventoryContainers[i];

                // Используем сохраненную позицию слота или вычисляем базовую
                
                Vector2 Position = new Vector2(5, i * 120 + 100);

                // Создаем прямоугольник слота на основе его сохраненной позиции
                Rect conteinerRect = new Rect(Position.x, Position.y, 300, 115);

                // Отрисовка фона контейнера
                EditorGUI.DrawRect(conteinerRect, new Color(0.3f, 0.3f, 0.3f));

                // Группа для содержимого
                GUI.BeginGroup(conteinerRect);
                
                // Содержимое слота относительно группы
                EditorGUI.LabelField(new Rect(5, 5, 200, 20), $"Слот №{i}", EditorStyles.boldLabel);
                
                if (GUI.Button(new Rect(5, 30, 70, 60), "Сетка"))
                {
                    _currentInventoryContainer = container;
                    editSlotGrid = true;
                }

                if (GUI.Button(new Rect(265, 5, 30, 30),"Х"))            
                {
                    _targetInventory.InventoryContainers.Remove(container);
                    
                    GUIUtility.ExitGUI();
                }

                EditorGUI.LabelField(new Rect(150, 5, 200, 20), container.Position.ToSafeString());
                // container.Icon = EditorGUI.ObjectField(new Rect(80, 30, 60, 60), container.Icon, typeof(Sprite), false) as Sprite;
                container.TypeItem = (ItemType)EditorGUI.EnumFlagsField(new Rect(5, 95, 290, 20), "Слот для предмета", container.TypeItem);

                GUI.EndGroup();
            }            
        }

        private void DrawContainerOnBackground()
        {
            // Создаем область для контейнеров
            Rect totalArea = GUILayoutUtility.GetRect(300, _targetInventory.InventoryContainers.Count * 120);

            for (int i = 0; i < _targetInventory.InventoryContainers.Count; i++)
            {
                InventoryContainer container = _targetInventory.InventoryContainers[i];
        
                // Используем сохраненную позицию слота или вычисляем базовую
                if (container.Position == Vector2.zero)
                {
                     container.Position = new Vector2(totalArea.x, totalArea.y + (i * _gridCellSize));
                }

                // Получаем размер слота на основе типа предмета
                container.Size = GetContainerSize(container.TypeItem);

                // Создаем прямоугольник слота на основе его сохраненной позиции
                Rect slotRect = new Rect(container.Position.x, container.Position.y, container.Size.x, container.Size.y);

                // Если этот слот перетаскивается, обновляем его позицию
                if (isDraggingSlot && draggedSlotIndex == i)
                {
                    Vector2 snappedPosition = new Vector2(
                        Mathf.Floor((Event.current.mousePosition.x - slotDragOffset.x) / _gridCellSize) * _gridCellSize,
                        Mathf.Floor((Event.current.mousePosition.y - slotDragOffset.y) / _gridCellSize) * _gridCellSize
                    );
                    
                    // Обновляем только позицию текущего слота
                    slotRect.position = snappedPosition;
                    container.Position = snappedPosition;
                }

                // Отрисовка фона слота
                EditorGUI.DrawRect(slotRect, new Color(0.3f, 0.3f, 0.3f));

                // Группа для содержимого
                GUI.BeginGroup(slotRect);
                
                EditorGUI.LabelField(new Rect(5, 5, 200, 20), $"Слот №{i} {container.TypeItem}", EditorStyles.boldLabel);


                GUI.EndGroup();

                // Обработка событий
                Event currentEvent = Event.current;
                switch (currentEvent.type)
                {
                    case EventType.MouseDown:
                        if (slotRect.Contains(currentEvent.mousePosition) && currentEvent.button == 0)
                        {
                            isDraggingSlot = true;
                            draggedSlotIndex = i;
                            slotDragOffset = currentEvent.mousePosition - slotRect.position;
                            currentEvent.Use();
                        }
                        break;

                    case EventType.MouseDrag:
                        if (isDraggingSlot && draggedSlotIndex == i)
                        {
                            GUI.changed = true;
                            Repaint();
                            currentEvent.Use();
                        }
                        break;

                    case EventType.MouseUp:
                        if (isDraggingSlot && draggedSlotIndex == i)
                        {
                            // Фиксируем конечную позицию с привязкой к сетке
                            Vector2 finalPosition = new Vector2(
                                Mathf.Floor(slotRect.x / _gridCellSize) * _gridCellSize,
                                Mathf.Floor(slotRect.y / _gridCellSize) * _gridCellSize
                            );
                            // Сохраняем позицию только для перетаскиваемого слота
                            _targetInventory.InventoryContainers[i].Position = finalPosition;
                            
                            isDraggingSlot = false;
                            draggedSlotIndex = -1;
                            currentEvent.Use();
                        }
                        break;
                }
            }        
        }

        public static Vector2Int GetContainerSize(ItemType type)
        {
            // Проверяем каждый флаг в enum
            foreach (ItemType flagValue in Enum.GetValues(typeof(ItemType)))
            {
                if (type.HasFlag(flagValue) && slotSizes.ContainsKey(flagValue))
                {
                    return slotSizes[flagValue];
                }
            }
            
            // Возвращаем размер по умолчанию, если тип не найден
            return new Vector2Int(300, 115);
        }

        private void DrawSlotGridInfo()
        {
            EditorGUILayout.BeginVertical("Box", GUILayout.Width(300));

                if (GUILayout.Button("добавить слот"))            
                {
                    _currentInventoryContainer.Slots.Add(new InventorySlot());
                }


                foreach (InventorySlot slot in _currentInventoryContainer.Slots)
                {
                    // slot.SlotObject = EditorGUILayout.ObjectField("Объект для спавна", slot.SlotObject, typeof(GameObject), false) as GameObject;
                    slot.PlaceToSpawnObject = (Transform)EditorGUILayout.ObjectField("Точка крепления", slot.PlaceToSpawnObject, typeof(Transform), true);
                    EditorGUILayout.Space(10);
                    
                    var gridRect = EditorGUILayout.BeginVertical();
                    
                    EditorGUI.DrawRect(gridRect, new Color(0.2627451f, 0.2627451f, 0.2627451f));
                    
                    slot.SlotGrid.Size = EditorGUILayout.Vector2IntField("Размер: ", slot.SlotGrid.Size);

                    slot.SlotGrid.Position = EditorGUILayout.Vector2Field("Позиция слота", slot.SlotGrid.Position);
                    
                    slot.Icon = EditorGUILayout.ObjectField("Иконка", slot.Icon, typeof(Sprite), false) as Sprite;

                    slot.ShowGrid = EditorGUILayout.Toggle("Показывать сетку", slot.ShowGrid);

                    slot.SlotGrid.CompatibilityGridMod = (Compatibility)EditorGUILayout.EnumPopup("Тип совместимости", slot.SlotGrid.CompatibilityGridMod);

                    if(slot.SlotGrid.CompatibilityGridMod == Compatibility.Access_by_item_groups)
                    {
                        ItemGroupIndex = EditorGUILayout.Popup("Совместима с группой", Array.IndexOf(Asset.HierarchyItemsTypes.ToArray(), slot.SlotGrid.CompatibleGroup), Asset.HierarchyItemsTypes.ToArray());

                        slot.SlotGrid.CompatibleGroup = Asset.HierarchyItemsTypes[ItemGroupIndex];
                    }
                    else
                    if(slot.SlotGrid.CompatibilityGridMod == Compatibility.Access_by_specific_item)
                    {
                        open = EditorGUILayout.BeginFoldoutHeaderGroup(open, "Итемы которые можно вставить в слот");
                            if(open)
                            {
                                for (int i1 = 0; i1 < slot.SlotGrid.SpecificItemCombined.Count; i1++)
                                {
                                    ItemData item = slot.SlotGrid.SpecificItemCombined[i1];
                                    
                                    EditorGUILayout.BeginHorizontal();

                                        slot.SlotGrid.SpecificItemCombined[i1] = (ItemData)EditorGUILayout.ObjectField("Element " + i1, item, typeof(ItemData), false);
                                        if (GUILayout.Button("-", GUILayout.Width(20)))
                                        {
                                            slot.SlotGrid.SpecificItemCombined.RemoveAt(i1);
                                        }

                                    EditorGUILayout.EndHorizontal();
                                }

                                EditorGUILayout.BeginHorizontal();
                                if (GUILayout.Button("+", GUILayout.Width(20)))
                                {
                                    slot.SlotGrid.SpecificItemCombined.Add(new ItemData());
                                }
                                if (GUILayout.Button("-", GUILayout.Width(20)))
                                {
                                    slot.SlotGrid.SpecificItemCombined.RemoveAt(slot.SlotGrid.SpecificItemCombined.Count - 1);
                                }
                                EditorGUILayout.EndHorizontal();
                            }

                        EditorGUILayout.EndFoldoutHeaderGroup();
                    }


                    if (GUILayout.Button("Х"))            
                    {
                        _currentInventoryContainer.Slots.Remove(slot);

                        GUIUtility.ExitGUI();
                    }
                    EditorGUILayout.EndVertical();
                }

            EditorGUILayout.EndVertical();
            // Отрисовка позиции сетки
        }

        private void DrawInventoryGrid()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
    
            // Создаем общий контейнер для всех сеток
            EditorGUI.DrawRect(new Rect(0, 0, GetContainerSize(_currentInventoryContainer.TypeItem).x, GetContainerSize(_currentInventoryContainer.TypeItem).y), new Color(0.1803922f, 0.1803922f, 0.1803922f));

            Rect gridRect = new Rect(
                0, 0, _currentInventoryContainer.Size.x * _cellSize, _currentInventoryContainer.Size.y * _cellSize
            );

            foreach (var slot in _currentInventoryContainer.Slots)
            {
                DrawSlotGridData(gridRect, slot);
            }
        
            EditorGUILayout.EndScrollView();
        }

        private void DrawSlotGridData(Rect gridRect, InventorySlot slot)
        {
                // Применяем позицию из объекта slotGrid
            Rect adjustedGridRect = new Rect(
                gridRect.x + slot.SlotGrid.Position.x, 
                gridRect.y + slot.SlotGrid.Position.y, 
                slot.SlotGrid.Size.x * _cellSize, 
                slot.SlotGrid.Size.y * _cellSize
            );

            // Рисуем сетку с учетом её позиции
            DrawSlotGrid(adjustedGridRect, slot);
            
            // Рисуем предметы с учетом позиции сетки
            DrawItems(adjustedGridRect, slot.SlotGrid);
            
            // Обрабатываем перетаскивание с учетом позиции сетки
            HandleDragAndDrop(adjustedGridRect, slot.SlotGrid);
        }

        private void DrawSlotGrid(Rect gridRect, InventorySlot slot)
        {
            for (int x = 0; x < slot.SlotGrid.Size.x; x++)
            {
                for (int y = 0; y < slot.SlotGrid.Size.y; y++)
                {
                    Rect cellRect = new Rect(
                        gridRect.x + x * _cellSize, gridRect.y + y * _cellSize, _cellSize, _cellSize
                    );
                    EditorGUI.DrawRect(cellRect, Color.gray);

                }
            }
            // Отображаем иконку предмета при перетаскивании
            if(slot.Icon)
            {
                GUIStyle slotStyle = new GUIStyle();
                slotStyle.normal.background = slot.Icon.texture;
                slotStyle.stretchWidth = true;
                slotStyle.stretchHeight = true;
                GUI.Box(gridRect, "", slotStyle);
            }
            else
            {
                EditorGUI.DrawRect(gridRect, Color.gray);
                GUI.Box(gridRect, "", EditorStyles.helpBox);
            }
        }

       private void HandleDragAndDrop(Rect gridRect, GridData2 grid)
        {
            Event currentEvent = Event.current;
            mousePosition = currentEvent.mousePosition;

            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    if (gridRect.Contains(mousePosition))
                    {
                        // Обработка правого клика
                        if (currentEvent.button == 1)
                        {
                            foreach (var item in grid.ActiveItems)
                            {
                                Rect itemRect = new Rect(
                                    gridRect.x + item.OnGridPosition.x * _cellSize,
                                    gridRect.y + item.OnGridPosition.y * _cellSize,
                                    item.WIDTH * _cellSize,
                                    item.HEIGHT * _cellSize
                                );

                                if (itemRect.Contains(mousePosition))
                                {
                                    ShowContextMenu(grid, item);
                                    currentEvent.Use();
                                    return;
                                }
                            }
                        }

                        // Проверяем клик по существующему предмету
                        Vector2 gridPos = GetGridPosition(gridRect, mousePosition);
                        bool clickedEmptyCell = true;
                        
                        foreach (var item in grid.ActiveItems)
                        {
                            Rect itemRect = new Rect(
                                gridRect.x + item.OnGridPosition.x * _cellSize,
                                gridRect.y + item.OnGridPosition.y * _cellSize,
                                item.WIDTH * _cellSize,
                                item.HEIGHT * _cellSize
                            );

                            if (itemRect.Contains(mousePosition))
                            {
                                isDragging = true;
                                draggedItem = item;
                                originalPosition = item;
                                // Вычисляем смещение между позицией мыши и левым верхним углом предмета
                                dragOffset = new Vector2((mousePosition.x - itemRect.x) - (_cellSize / 2), (mousePosition.y - itemRect.y) - (_cellSize / 2));
                                grid.ActiveItems.Remove(item);
                                currentEvent.Use();
                                return;
                            }

                            if (itemRect.Contains(mousePosition))
                            {
                                clickedEmptyCell = false;
                                break;
                            }
                        }

                        // Если кликнули в пустую область и нет выбранного предмета, начинаем перетаскивать сетку
                        if (clickedEmptyCell && _selectedItem == null)
                        {
                            isDraggingSlot = true;
                            draggedGrid = grid;
                            dragStartPosition = mousePosition;
                            initialGridPosition = grid.Position;
                            slotDragOffset = mousePosition - gridRect.position;
                            currentEvent.Use();
                            return;
                        }

                        // Если не нашли существующий предмет и есть выбранный предмет
                        if (_selectedItem != null)
                        {
                            isDragging = true;
                            draggedItem = _selectedItem;
                            currentEvent.Use();
                        }
                    }
                    break;

                case EventType.MouseDrag:
                    if (isDragging)
                    {
                        Repaint();
                        currentEvent.Use();
                    }
                    else if (isDraggingSlot && draggedGrid == grid)
                    {
                        // Рассчитываем новую позицию с привязкой к сетке
                        Vector2 delta = mousePosition - dragStartPosition;
                        Vector2 newPosition = new Vector2(
                            initialGridPosition.x + Mathf.Round(delta.x / _gridCellSize) * _gridCellSize,
                            initialGridPosition.y + Mathf.Round(delta.y / _gridCellSize) * _gridCellSize
                        );
                        
                        // Обновляем позицию сетки
                        grid.Position = newPosition;
                        
                        Repaint();
                        currentEvent.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (isDragging && gridRect.Contains(mousePosition))
                    {
                        Vector2 newPos = GetGridPosition(gridRect, mousePosition - dragOffset);
                        if (CanPlaceItem(grid, (int)newPos.x, (int)newPos.y))
                        {
                            grid.PlaceItem(draggedItem, (int)newPos.x, (int)newPos.y);
                            if (draggedItem == _selectedItem)
                            {
                                _selectedItem = null;
                            }
                        }
                        else if (originalPosition != null)
                        {
                            // Возвращаем предмет на исходную позицию
                            grid.ActiveItems.Add(originalPosition);
                        }

                        isDragging = false;
                        draggedItem = null;
                        originalPosition = new InventoryItem();
                        
                        currentEvent.Use();
                    }
                    else if (isDraggingSlot)
                    {
                        // Фиксируем конечную позицию с привязкой к сетке
                        Vector2 finalPosition = new Vector2(
                            Mathf.Round(grid.Position.x / _gridCellSize) * _gridCellSize,
                            Mathf.Round(grid.Position.y / _gridCellSize) * _gridCellSize
                        );
                        
                        grid.Position = finalPosition;
                        // EditorUtility.SetDirty(_currentInventoryContainer);
                        
                        isDraggingSlot = false;
                        draggedGrid = null;
                        currentEvent.Use();
                    }
                    break;
            }

            if (isDragging && draggedItem != null)
            {
                DrawDragPreview(gridRect, grid);
            }
        }

        private void DrawDragPreview(Rect gridRect, GridData2 grid)
        {
            Vector2 gridPosition = GetGridPosition(gridRect, mousePosition - dragOffset);
            bool canPlace = CanPlaceItem(grid, (int)gridPosition.x, (int)gridPosition.y);

            Rect previewRect = new Rect(
                gridRect.x + gridPosition.x * _cellSize,
                gridRect.y + gridPosition.y * _cellSize,
                draggedItem.WIDTH * _cellSize,
                draggedItem.HEIGHT * _cellSize
            );

            Color previewColor = canPlace ? new Color(0, 1, 0, 0.3f) : new Color(1, 0, 0, 0.3f);
            EditorGUI.DrawRect(previewRect, previewColor);

            // Отображаем иконку предмета при перетаскивании
            GUIStyle itemStyle = new GUIStyle();
            itemStyle.normal.background = draggedItem.ItemData.ItemIcon.texture;
            itemStyle.stretchWidth = true;
            itemStyle.stretchHeight = true;
            GUI.Box(previewRect, "", itemStyle);
        }

        private void ShowContextMenu(GridData2 grid, InventoryItem item)
        {
            GenericMenu menu = new GenericMenu();
            
            menu.AddItem(new GUIContent("Удалить"), false, () => {
                grid.ActiveItems.Remove(item);
            });
            
            menu.AddItem(new GUIContent("Информация"), false, () => {
                EditorUtility.DisplayDialog("Информация о предмете", 
                    $"Название: {item.ItemData.Title}\n" +
                    $"Размер: {item.WIDTH}x{item.HEIGHT}\n" +
                    $"Позиция: ({item.OnGridPosition.x}, {item.OnGridPosition.y})", 
                    "OK");
            });

            // Добавляем пункт только если предмет имеет сетку и поддерживает комбинирование
            if (item.Grids != null && item.Grids.Length > 0)
            {
                menu.AddItem(new GUIContent("Открыть сетку"), false, () => {
                    OpenItemGridEditor(item);
                });
            }

            menu.ShowAsContext();
        }

        private void OpenItemGridEditor(InventoryItem item)
        {
            ItemGridEditorWindow window = EditorWindow.GetWindow<ItemGridEditorWindow>($"Редактор Предмета {item.ItemData.Title}");
            window.Initialize(item);
            window.Show();
        }

        private Vector2 GetGridPosition(Rect gridRect, Vector2 mousePos)
        {
            return new Vector2(
                Mathf.Floor((mousePos.x - gridRect.x) / _cellSize),
                Mathf.Floor((mousePos.y - gridRect.y) / _cellSize)
            );
        }

        private bool CanPlaceItem(GridData2 grid, int x, int y)
        {
            if (draggedItem == null) return false;
            if (x < 0 || y < 0) return false;
            if (x + draggedItem.WIDTH > grid.Size.x) return false;
            if (y + draggedItem.HEIGHT > grid.Size.y) return false;

            return grid.CheckAvailableSpace(x, y, draggedItem.WIDTH, draggedItem.HEIGHT);
        }

        private void PlaceItemInGrid(Rect gridRect, GridData2 grid)
        {
            Vector2 gridPosition = GetGridPosition(gridRect, mousePosition - dragOffset);
            if (CanPlaceItem(grid, (int)gridPosition.x, (int)gridPosition.y))
            {
                grid.PlaceItem(_selectedItem, (int)gridPosition.x, (int)gridPosition.y);
                _selectedItem = null;
            }
        }        

        private void DrawItems(Rect gridRect, GridData2 grid)
        {
            foreach (var item in grid.ActiveItems)
            {
                // Вычисляем прямоугольник для предмета
                Rect itemRect = new Rect(
                    gridRect.x + item.OnGridPosition.x * _cellSize,
                    gridRect.y + item.OnGridPosition.y * _cellSize,
                    item.WIDTH * _cellSize,
                    item.HEIGHT * _cellSize 
                );

                // Создаем стиль с текстурой
                GUIStyle itemStyle = new GUIStyle();
                itemStyle.normal.background = item.ItemData.ItemIcon.texture;
                itemStyle.stretchWidth = true;
                itemStyle.stretchHeight = true;

                // Рисуем предмет с текстурой
                GUI.Box(itemRect, "", itemStyle);

                // Рисуем рамку
                GUI.Box(itemRect, "", EditorStyles.helpBox);

                // Отображаем название предмета (опционально)
                // GUI.Label(itemRect, itemPos.Item.ItemData.Title, EditorStyles.centeredGreyMiniLabel);
            }
        }
    }
}