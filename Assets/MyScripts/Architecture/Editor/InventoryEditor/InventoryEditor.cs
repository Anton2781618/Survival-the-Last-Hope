using UnityEditor;
using UnityEngine;
using InventoryDiablo;
using System;
using static InventoryDiablo.ItemData;
using Unity.VisualScripting;
using UnityEditor.Graphs;
using MyProject;

namespace ModularEventArchitecture
{
    public class InventoryEditor : EditorWindow
    {
        // private InventoryModule targetModule;
        private Inventory targetInventory;
        private Vector2 scrollPosition;
        private float cellSize = 32f;
        private InventoryItem selectedItem;
        private AvailableItems availableItems;
        private Vector2 itemListScroll;

        private bool isDragging = false;
        private Vector2 mousePosition;

        private InventoryItem draggedItem;
        private InventoryItem originalPosition;

        private bool isDraggingGrid = false;
        private bool editSlotGrid = false;
        private Vector2 dragStartPosition;
        private Vector2 initialGridPosition;
        private GridData2 draggedGrid;
        private InventorySlot _currentSlot;
        private Vector2 dragOffset;
        

        [MenuItem("Tools/Inventory Editor")]
        public static void ShowWindow()
        {
            GetWindow<InventoryEditor>("Inventory Editor");
        }

        private void OnGUI()
        {
            if (targetInventory == null)
            {
                Debug.Log("Target inventory is null");
                DrawModuleSelection();
                return;
            }

            EditorGUILayout.BeginHorizontal();
            
            // Левая панель - список предметов
            if (editSlotGrid)
            {

                if (GUILayout.Button("Назад", GUILayout.Width(80), GUILayout.Height(50))) 
                {
                    _currentSlot = null;

                    editSlotGrid = false;

                    GUIUtility.ExitGUI();
                }

                DrawItemsList();

                DrawGridInfo();

                DrawInventoryGrid();
            }
            else
            {
                DrawSlotsInfo();
            }
            

            EditorGUILayout.EndHorizontal();
        }
        private InventoryModule inventoryModule;
        private void DrawModuleSelection()
        {
            EditorGUILayout.HelpBox("Выберите инвентарь", MessageType.Info);
            inventoryModule = EditorGUILayout.ObjectField("Module", inventoryModule, typeof(InventoryModule), true) as InventoryModule;
            
            if (inventoryModule) 
            {
                targetInventory = inventoryModule.Inventory;
            }
        }

        public void SetTartget(Inventory inventory)
        {
            targetInventory = inventory;
        }

        private void DrawItemsList()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.Width(200));
            
            if (availableItems == null)
            {
                availableItems = AssetDatabase.LoadAssetAtPath<AvailableItems>("Assets/MyScripts/Architecture/Editor/AvailableItems.asset");
                if (availableItems == null)
                {
                    EditorGUILayout.HelpBox("Create AvailableItems asset", MessageType.Warning);
                    
                    if (GUILayout.Button("Create"))
                    {
                        availableItems = CreateInstance<AvailableItems>();
                        AssetDatabase.CreateAsset(availableItems, "Assets/Data/AvailableItems.asset");
                    }
                    
                    EditorGUILayout.EndVertical();
                    return;
                }
            }

            EditorGUILayout.LabelField("Предметы", EditorStyles.boldLabel);
            
            itemListScroll = EditorGUILayout.BeginScrollView(itemListScroll);
            
            foreach (var item in availableItems.items)
            {
                if (Helper.AreHasFlag(item.ItemData.TypeItem, _currentSlot.TypeItem)) continue;
                // if (!item.ItemData.TypeItem.HasFlag(_currentSlot.TypeItem)) continue;
                // if (_currentSlot.TypeItem != item.ItemData.TypeItem) continue;

                GUI.backgroundColor = selectedItem != null && selectedItem.ItemData.Title == item.ItemData.Title ? Color.cyan : Color.white;

                if (GUILayout.Button(item.ItemData.Title))
                {
                    InventoryItem newItem = new InventoryItem(item);

                    selectedItem = newItem;
                }

                GUI.backgroundColor = Color.white;
            }
            
            EditorGUILayout.EndScrollView();
            
            if (selectedItem != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Selected Item:", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Name: {selectedItem.ItemData.Title}");
                EditorGUILayout.LabelField($"Size: {selectedItem.WIDTH}x{selectedItem.HEIGHT}");
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawSlotsInfo()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.Width(300));
            
                if (GUILayout.Button("Добавить слот инвентаря"))            
                {
                    targetInventory.Slots.Add(new InventorySlot());
                }

                foreach (var slot in targetInventory.Slots)
                {
                    // Основные параметры предмета
                    EditorGUILayout.LabelField("Параметры слота", EditorStyles.boldLabel);

                    EditorGUILayout.BeginHorizontal();

                        if (GUILayout.Button("Cетка", GUILayout.Width(70), GUILayout.Height(60)))
                        {
                            _currentSlot = slot;
                            editSlotGrid = true;
                        }
                        
                        slot.Icon = EditorGUILayout.ObjectField(" ", slot.Icon, typeof(Sprite), false) as Sprite;

                    EditorGUILayout.EndHorizontal();
                    
                    slot.TypeItem = (ItemType)EditorGUILayout.EnumFlagsField("Слот для предмета", slot.TypeItem);

                    if (GUILayout.Button("Удалиь слот"))            
                    {
                        targetInventory.Slots.Remove(slot);
                        
                        GUIUtility.ExitGUI();
                    }
                }

            EditorGUILayout.EndVertical();
        }

        private void DrawGridInfo()
        {
            EditorGUILayout.BeginVertical("Box");
            // if (GUILayout.Button("Добавить сетку"))
            // {
            //     foreach (var slot in targetInventory.Slots)
            //     {
            //         // Создаем новую сетку
            //         GridData2 newGrid = new GridData2
            //         {
            //             GridSize = new Vector2Int(5, 5),
            //             Position = new Vector2(0 ,  0)
            //         };

            //         // Добавляем сетку в массив
            //         Array.Resize(ref slot.Grids2, slot.Grids2.Length + 1);
            //         slot.Grids2[slot.Grids2.Length - 1] = newGrid;
            //     }
            // }

            // foreach (var slot in targetInventory.Slots)
            // {
                // for (int i = 0; i < slot.Grids2.Length; i++)
                // {
                //     EditorGUILayout.BeginVertical("box");
            

                //     EditorGUILayout.BeginHorizontal("box", GUILayout.Width(300));
                    
                //     if (GUILayout.Button("Удалить", GUILayout.Width(100)))
                //     {
                //         // Создаем временный массив без удаляемой сетки
                //         var tempGrids = new GridData2[slot.Grids2.Length - 1];
                //         Array.Copy(slot.Grids2, 0, tempGrids, 0, i);
                //         Array.Copy(slot.Grids2, i + 1, tempGrids, i, slot.Grids2.Length - i - 1);
                //         slot.Grids2 = tempGrids;
                        
                //         GUIUtility.ExitGUI();
                //     }

                //     EditorGUILayout.EndHorizontal();

                //     GridData2 grid = slot.Grids2[i];

                //     grid.Position = EditorGUILayout.Vector2Field("Сетка номер: " + i, grid.Position);
                    
                //     EditorGUILayout.EndVertical();
                // }

            // }
            _currentSlot.Grids2.Position = EditorGUILayout.Vector2Field("Позиция ", _currentSlot.Grids2.Position);
            _currentSlot.Grids2.GridSize = EditorGUILayout.Vector2IntField("Размер: ", _currentSlot.Grids2.GridSize);

            EditorGUILayout.EndVertical();
            // Отрисовка позиции сетки
        }

        private void DrawInventoryGrid()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
    
            // Создаем общий контейнер для всех сеток
            Rect totalRect = GUILayoutUtility.GetRect(
                1000, // Достаточно большая ширина для всех сеток
                1000  // Достаточно большая высота для всех сеток
            );

            Rect gridRect = new Rect(
                totalRect.x + _currentSlot.Grids2.Position.x,totalRect.y + _currentSlot.Grids2.Position.y,
                _currentSlot.Grids2.GridSize.x * cellSize, _currentSlot.Grids2.GridSize.y * cellSize
            );

            DrawGridData(gridRect, _currentSlot.Grids2);
        
            EditorGUILayout.EndScrollView();
        }

        private void DrawGridData(Rect gridRect, GridData2 grid)
        {

            DrawGrid(gridRect, grid);
            DrawItems(gridRect, grid);
            HandleDragAndDrop(gridRect, grid);
        }

        private void DrawGrid(Rect gridRect, GridData2 grid)
        {
            for (int x = 0; x < grid.GridSize.x; x++)
            {
                for (int y = 0; y < grid.GridSize.y; y++)
                {
                    Rect cellRect = new Rect(
                        gridRect.x + x * cellSize, gridRect.y + y * cellSize, cellSize, cellSize
                    );
                    EditorGUI.DrawRect(cellRect, Color.gray);
                    GUI.Box(cellRect, "", EditorStyles.helpBox);
                }
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
                            foreach (var item in grid.activeItems)
                            {
                                Rect itemRect = new Rect(
                                    gridRect.x + item.OnGridPositionX * cellSize,
                                    gridRect.y + item.OnGridPositionY * cellSize,
                                    item.WIDTH * cellSize,
                                    item.HEIGHT * cellSize
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
                        
                        foreach (var item in grid.activeItems)
                        {
                            Rect itemRect = new Rect(
                                gridRect.x + item.OnGridPositionX * cellSize,
                                gridRect.y + item.OnGridPositionY * cellSize,
                                item.WIDTH * cellSize,
                                item.HEIGHT * cellSize
                            );

                            if (itemRect.Contains(mousePosition))
                            {
                                isDragging = true;
                                draggedItem = item;
                                originalPosition = item;
                                // Вычисляем смещение между позицией мыши и левым верхним углом предмета
                                dragOffset = new Vector2((mousePosition.x - itemRect.x) - (cellSize / 2), (mousePosition.y - itemRect.y) - (cellSize / 2));
                                grid.activeItems.Remove(item);
                                currentEvent.Use();
                                return;
                            }

                            if (itemRect.Contains(mousePosition))
                            {
                                clickedEmptyCell = false;
                                break;
                            }
                        }

                        if (clickedEmptyCell && selectedItem == null)
                        {

                            isDraggingGrid = true;
                            draggedGrid = grid;
                            dragStartPosition = mousePosition;
                            initialGridPosition = new Vector2(grid.Position.x, grid.Position.y);
                            currentEvent.Use();
                            return;
                        }

                        // Если не нашли существующий предмет и есть выбранный предмет
                        if (selectedItem != null)
                        {
                            isDragging = true;
                            draggedItem = selectedItem;
                            // dragStartPosition = mousePosition;
                            // initialGridPosition = new Vector2(selectedItem.OnGridPositionX, selectedItem.OnGridPositionY);
                            currentEvent.Use();
                        }
                    }
                    break;

                case EventType.MouseDrag:
                    if (isDragging)
                    {
                        // Vector2 delta = mousePosition - dragStartPosition;
                        // draggedItem.OnGridPositionX = (int)(initialGridPosition.x + delta.x);
                        // draggedItem.OnGridPositionY = (int)(initialGridPosition.y + delta.y);
                        Repaint();
                        currentEvent.Use();
                    }

                    if (isDraggingGrid)
                    {
                        Vector2 delta = mousePosition - dragStartPosition;
                        draggedGrid.Position.x = initialGridPosition.x + delta.x;
                        draggedGrid.Position.y = initialGridPosition.y + delta.y;
                        Repaint();
                        currentEvent.Use();
                    }
                    break;

                case EventType.MouseUp:

                    if (isDraggingGrid)
                    {
                        isDraggingGrid = false;
                        draggedGrid = null;
                        currentEvent.Use();
                    }

                    if (isDragging && gridRect.Contains(mousePosition))
                    {
                        Vector2 newPos = GetGridPosition(gridRect, mousePosition - dragOffset);
                        if (CanPlaceItem(grid, (int)newPos.x, (int)newPos.y))
                        {
                            grid.PlaceItem((int)newPos.x, (int)newPos.y, draggedItem);
                            if (draggedItem == selectedItem)
                            {
                                selectedItem = null;
                            }
                        }
                        else if (originalPosition!= null)
                        {
                            // Возвращаем предмет на исходную позицию
                            grid.activeItems.Add(originalPosition);
                        }

                        isDragging = false;
                        draggedItem = null;
                        originalPosition = new InventoryItem();
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
                gridRect.x + gridPosition.x * cellSize,
                gridRect.y + gridPosition.y * cellSize,
                draggedItem.WIDTH * cellSize,
                draggedItem.HEIGHT * cellSize
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
                grid.activeItems.Remove(item);
            });
            
            menu.AddItem(new GUIContent("Информация"), false, () => {
                EditorUtility.DisplayDialog("Информация о предмете", 
                    $"Название: {item.ItemData.Title}\n" +
                    $"Размер: {item.WIDTH}x{item.HEIGHT}\n" +
                    $"Позиция: ({item.OnGridPositionX}, {item.OnGridPositionY})", 
                    "OK");
            });

            // Добавляем пункт только если предмет имеет сетку и поддерживает комбинирование
            // if (item.Grids2 != null && item.Grids2.Length > 0 && item.ItemData.CanBeCombined != null && item.ItemData.CanBeCombined.Length > 0)
            if (item.Grids2 != null && item.Grids2.Length > 0)
            {
                menu.AddItem(new GUIContent("Добавить предмет на сетку"), false, () => {
                    OpenItemGridEditor(item);
                });
            }

            menu.ShowAsContext();
        }

        private void OpenItemGridEditor(InventoryItem item)
        {
            ItemGridEditorWindow window = EditorWindow.GetWindow<ItemGridEditorWindow>("Item Grid Editor");
            window.Initialize(item);
            window.Show();
        }

        private Vector2 GetGridPosition(Rect gridRect, Vector2 mousePos)
        {
            return new Vector2(
                Mathf.Floor((mousePos.x - gridRect.x) / cellSize),
                Mathf.Floor((mousePos.y - gridRect.y) / cellSize)
            );
        }

        private bool CanPlaceItem(GridData2 grid, int x, int y)
        {
            if (draggedItem == null) return false;
            if (x < 0 || y < 0) return false;
            if (x + draggedItem.WIDTH > grid.GridSize.x) return false;
            if (y + draggedItem.HEIGHT > grid.GridSize.y) return false;

            return grid.CheckAvailableSpace(x, y, draggedItem.WIDTH, draggedItem.HEIGHT);
        }

        private void PlaceItemInGrid(Rect gridRect, GridData2 grid)
        {
            Vector2 gridPosition = GetGridPosition(gridRect, mousePosition - dragOffset);
            if (CanPlaceItem(grid, (int)gridPosition.x, (int)gridPosition.y))
            {
                grid.PlaceItem((int)gridPosition.x, (int)gridPosition.y, selectedItem);
                selectedItem = null;
            }
        }        

        private void DrawItems(Rect gridRect, GridData2 grid)
        {
            foreach (var item in grid.activeItems)
            {
                // Вычисляем прямоугольник для предмета
                Rect itemRect = new Rect(
                    gridRect.x + item.OnGridPositionX * cellSize,
                    gridRect.y + item.OnGridPositionY * cellSize,
                    item.WIDTH * cellSize,
                    item.HEIGHT * cellSize 
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