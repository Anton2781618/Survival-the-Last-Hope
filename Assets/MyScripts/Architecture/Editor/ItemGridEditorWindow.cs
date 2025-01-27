using System.Collections.Generic;
using System.Linq;
using InventoryDiablo;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace ModularEventArchitecture
{
    public class ItemGridEditorWindow : EditorWindow
    {
        private InventoryItem targetItem;
        private Vector2 scrollPosition;
        private float cellSize = 32f;
        private InventoryItem selectedItem;
        private bool isDragging;
        private Vector2 mousePosition;
        private InventoryItem draggedItem;
        private InventoryItem originalPosition;

        private List<ItemData> _currCombinedItems;
        private GridData2 activeGrid;
        private Vector2 itemListScroll;

        public void Initialize(InventoryItem item)
        {
            targetItem = item;
        }

        private void OnGUI()
        {
            if (targetItem == null) return;

            EditorGUILayout.BeginHorizontal();
            
            
            // Левая панель - доступные предметы
            DrawAvailableItems();
            
            // Правая панель - сетка предмета
            DrawItemGrids();
            
            EditorGUILayout.EndHorizontal();
        }

        private void DrawAvailableItems()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.Width(230));

            if (GUILayout.Button("Сбросить фильтр", GUILayout.Height(30)))
            {
                _currCombinedItems = null;
            }

            itemListScroll = EditorGUILayout.BeginScrollView(itemListScroll);

            EditorGUILayout.LabelField("Предметы", EditorStyles.boldLabel);
            
            // Фильтруем предметы которые можно комбинировать
            var availableItems = AssetDatabase.LoadAssetAtPath<AvailableItems>("Assets/MyScripts/Architecture/Editor/AvailableItems.asset");
            
            // var availableItems = AssetDatabase.FindAssets("t:InventoryItem")
            //     .Select(guid => AssetDatabase.LoadAssetAtPath<InventoryItem>(AssetDatabase.GUIDToAssetPath(guid)))
            //     .Where(item => item.ItemData.CanBeCombined);
            
            foreach (InventoryItem item in availableItems.items)
            {
                // foreach (var combinedItemDatas in targetItem.ItemData.CanBeCombined)
                // {
                GUI.backgroundColor = selectedItem != null && selectedItem.ItemData.Title == item.ItemData.Title ? Color.cyan : Color.white;
                if(_currCombinedItems != null)
                {
                    foreach (var combineditem in _currCombinedItems)
                    {
                        if(item.ItemData == combineditem)
                        {

                            if (GUILayout.Button(item.ItemData.Title))
                            {
                                InventoryItem newItem = new InventoryItem(item);

                                selectedItem = newItem;
                            }
                        }
                    }
                }
                else
                {
                    if (GUILayout.Button(item.ItemData.Title))
                    {
                        InventoryItem newItem = new InventoryItem(item);

                        selectedItem = newItem;
                    }

                }
                    // if (_currCombinedItems != null && item.ItemData.CanBeCombined != _currCombinedItems) continue;
                    // {
                    // }
                // }
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

        private void DrawItemGrids()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            foreach (var grid in targetItem.Grids2)
            {
                EditorGUILayout.BeginVertical("box");
                DrawGridData(grid);
                EditorGUILayout.EndVertical();
            }
            // EditorGUILayout.LabelField($"Итем: {selectedItem.ItemData.Title}");
            
            EditorGUILayout.EndScrollView();
        }

        private void DrawGridData(GridData2 grid)
        {
            EditorGUILayout.BeginVertical("box");
    
            // Добавляем поля для редактирования позиции сетки
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Отсеять предметы по типу сетки",GUILayout.Width(220), GUILayout.Height(44))) 
            {
                _currCombinedItems = grid.CanBeCombined;
            }
            EditorGUILayout.EndHorizontal();

            // Создаем область для сетки с учетом позиции
            Rect gridRect = GUILayoutUtility.GetRect(
                grid.GridSize.x * cellSize,
                grid.GridSize.y * cellSize
            );
            // gridRect.x += grid.Position.x;
            // gridRect.y += grid.Position.y;

            DrawGrid(gridRect, grid);
            DrawItems(gridRect, grid);
            HandleDragAndDrop(gridRect, grid);

            EditorGUILayout.EndVertical();
        }

        private void DrawGrid(Rect gridRect, GridData2 grid)
        {
            for (int x = 0; x < grid.GridSize.x; x++)
            {
                for (int y = 0; y < grid.GridSize.y; y++)
                {
                    Rect cellRect = new Rect(
                        gridRect.x + x * cellSize,
                        gridRect.y + y * cellSize,
                        cellSize,
                        cellSize
                    );
                    EditorGUI.DrawRect(cellRect, Color.gray);
                    GUI.Box(cellRect, "", EditorStyles.helpBox);
                }
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

                        activeGrid = grid;
                        // Проверяем клик по существующему предмету
                        Vector2 gridPos = GetGridPosition(gridRect, mousePosition);
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
                                grid.activeItems.Remove(item);
                                currentEvent.Use();
                                return;
                            }
                        }

                        // Если не нашли существующий предмет и есть выбранный предмет
                        if (selectedItem != null)
                        {
                            isDragging = true;
                            draggedItem = selectedItem;
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
                    break;

                case EventType.MouseUp:
                    if (isDragging && gridRect.Contains(mousePosition)/* && grid == activeGrid */)
                    {
                        Vector2 newPos = GetGridPosition(gridRect, mousePosition);
                        if (CanPlaceItem(grid, (int)newPos.x, (int)newPos.y))
                        {
                            grid.PlaceItem((int)newPos.x, (int)newPos.y, draggedItem);
                            if (draggedItem == selectedItem)
                            {
                                selectedItem = null;
                            }
                        }
                        else if (originalPosition != null/* && grid == activeGrid */)
                        {
                            // Возвращаем предмет на исходную позицию
                            grid.activeItems.Add(originalPosition);
                        }
                        isDragging = false;
                        draggedItem = null;
                        originalPosition = null;
                        currentEvent.Use();
                    }
                    break;
            }
            
            if (isDragging && draggedItem != null /* && grid == activeGrid */)
            {
                DrawDragPreview(gridRect, grid);
            }
        }

        private Vector2 GetGridPosition(Rect gridRect, Vector2 mousePos)
        {
            return new Vector2(
                Mathf.Floor((mousePos.x - gridRect.x) / cellSize),
                Mathf.Floor((mousePos.y - gridRect.y) / cellSize)
            );
        }

        private void DrawDragPreview(Rect gridRect, GridData2 grid)
        {            
            if (gridRect.Contains(mousePosition))
            {
                Vector2 gridPosition = GetGridPosition(gridRect, mousePosition);
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
        }

        private bool CanPlaceItem(GridData2 grid, int x, int y)
        {
            if (draggedItem == null) return false;
            if (x < 0 || y < 0) return false;
            if (x + draggedItem.WIDTH > grid.GridSize.x) return false;
            if (y + draggedItem.HEIGHT > grid.GridSize.y) return false;

            return grid.CheckAvailableSpace(x, y, draggedItem.WIDTH, draggedItem.HEIGHT);
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
            if (item.Grids2 != null && item.Grids2.Length > 0 && 
            item.ItemData.CanBeCombined != null && item.ItemData.CanBeCombined.Length > 0)
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
    }
}