using System.Collections.Generic;
using System.Linq;
using InventoryDiablo;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ModularEventArchitecture
{
    //настройка на сетке предмета
    public class ItemGridEditorWindow : EditorWindow
    {
        //-----------------------------------------------------------
        private InventoryItem targetItem;
        private Vector2 scrollPosition;
        private float cellSize = 32f;
        private InventoryItem selectedItem;
        private bool isDragging;
        private Vector2 mousePosition;
        private InventoryItem draggedItem;
        private InventoryItem originalPosition;

        private List<ItemData> _currCombinedItems;
        private Vector2 itemListScroll;
        private Vector2 dragOffset;
        //!-----------------------------------------------------------

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
            var availableItems = AssetDatabase.LoadAssetAtPath<AvailableItems>("Assets/MyAssets/ScriptableObjects/AvailableItems.asset");
            
            // var availableItems = AssetDatabase.FindAssets("t:InventoryItem")
            //     .Select(guid => AssetDatabase.LoadAssetAtPath<InventoryItem>(AssetDatabase.GUIDToAssetPath(guid)))
            //     .Where(item => item.ItemData.CanBeCombined);
            
            foreach (InventoryItem item in availableItems.items)
            {
                GUI.backgroundColor = selectedItem != null && selectedItem.ItemData.Title == item.ItemData.Title ? Color.cyan : Color.white;

                if(_currCombinedItems != null)
                {
                    foreach (var combineditem in _currCombinedItems)
                    {
                        if(item.ItemData == combineditem)
                        {

                            // if (GUILayout.Button(item.ItemData.Title))
                            // {
                            //     // InventoryItem newItem = new InventoryItem(item);

                            //     // selectedItem = newItem;
                            // }
                        }
                    }
                }
                else
                {
                    if (GUILayout.Button(item.ItemData.Title))
                    {
                        InventoryItem clone = item.Clone();

                        selectedItem = clone;
                    }

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

        private void DrawItemGrids()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            foreach (var grid in targetItem.Grids)
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
                _currCombinedItems = grid.SpecificItemCombined;
            }
            EditorGUILayout.EndHorizontal();

            // Создаем область для сетки с учетом позиции
            Rect gridRect = GUILayoutUtility.GetRect(
                grid.Size.x * cellSize,
                grid.Size.y * cellSize
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
            for (int x = 0; x < grid.Size.x; x++)
            {
                for (int y = 0; y < grid.Size.y; y++)
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
                                    gridRect.x + item.OnGridPosition.x * cellSize,
                                    gridRect.y + item.OnGridPosition.y * cellSize,
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
                        foreach (var item in grid.ActiveItems)
                        {
                            Rect itemRect = new Rect(
                                gridRect.x + item.OnGridPosition.x * cellSize,
                                gridRect.y + item.OnGridPosition.y * cellSize,
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
                                
                                grid.ActiveItems.Remove(item);
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
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene()); // Помечаем сцену как измененную
                    }
                    break;

                case EventType.MouseUp:
                if (isDragging && draggedItem != null)
                {
                    if (gridRect.Contains(mousePosition))
                    {
                        // Проверяем, не отпустили ли мы предмет над другим предметом
                        InventoryItem targetItem = null;
                        foreach (var item in grid.ActiveItems)
                        {
                            Rect itemRect = new Rect(
                                gridRect.x + item.OnGridPosition.x * cellSize,
                                gridRect.y + item.OnGridPosition.y * cellSize,
                                item.WIDTH * cellSize,
                                item.HEIGHT * cellSize
                            );

                            if (itemRect.Contains(mousePosition))
                            {
                                targetItem = item;
                                break;
                            }
                        }

                        // Проверяем можно ли вставить предмет в целевой предмет
                        if (targetItem != null && CanCombineItems(targetItem, draggedItem))
                        {
                            // Пытаемся добавить предмет в первую подходящую сетку
                            foreach (var targetGrid in targetItem.Grids)
                            {
                                // Проверяем есть ли место в сетке
                                if (targetGrid.CheckAvailableSpace(0, 0, draggedItem.WIDTH, draggedItem.HEIGHT))
                                {
                                    targetGrid.PlaceItem(draggedItem, 0, 0);
                                    if (draggedItem == selectedItem)
                                    {
                                        selectedItem = null;
                                    }
                                    isDragging = false;
                                    draggedItem = null;
                                    originalPosition = null;
                                    dragOffset = Vector2.zero;
                                    currentEvent.Use();
                                }
                            }
                        }
                        else
                        {
                            // Если предмет не может быть вставлен в целевой предмет, то проверяем сетки предмета на наличие предметов которые подходят для комбинации с целевым предметом
                            foreach (var draggedItemGrid in draggedItem.Grids)
                            {
                                foreach (var draggedActiveItem in draggedItemGrid.ActiveItems)
                                {
                                    if (targetItem != null && CanCombineItems(targetItem, draggedActiveItem))
                                    {
                                        // Пытаемся добавить предмет в первую подходящую сетку
                                        foreach (var targetGrid in targetItem.Grids)
                                        {
                                            // Проверяем есть ли место в сетке
                                            if (targetGrid.CheckAvailableSpace(0, 0, draggedActiveItem.WIDTH, draggedActiveItem.HEIGHT))
                                            {
                                                InventoryItem cloneItem = draggedActiveItem.Clone();
                                                //вычитаем из 
                                                
                                                draggedActiveItem.Amount -= targetGrid.MaxStackSize;
                                                cloneItem.Amount = targetGrid.MaxStackSize;
                                                
                                                // cloneItem.Amount = diff;
                                                targetGrid.PlaceItem(cloneItem, 0, 0);
                                                if (draggedActiveItem == selectedItem)
                                                {
                                                    selectedItem = null;
                                                }

                                                // Возвращаем предмет на исходную позицию
                                                grid.ActiveItems.Add(originalPosition);
                                                isDragging = false;
                                                originalPosition = null;
                                                draggedItem = null;
                                                dragOffset = Vector2.zero;
                                                currentEvent.Use();
                                            }
                                            else
                                            {
                                                Debug.Log("Нет места в сетке");
                                                foreach (var targetActiveItem in targetGrid.ActiveItems)
                                                {
                                                    if(targetActiveItem.ItemData == draggedActiveItem.ItemData)
                                                    {
                                                        int diff = math.abs(targetActiveItem.Amount - targetGrid.MaxStackSize); 

                                                        if(diff > 0)
                                                        {
                                                            if(draggedActiveItem.Amount > diff)
                                                            {
                                                                Debug.Log("1");
                                                                targetActiveItem.Amount += diff;
                                                                draggedActiveItem.Amount -= diff;
                                                            }
                                                            else
                                                            {
                                                                Debug.Log("2");
                                                                Debug.Log($"target {targetActiveItem.Amount} : eragg {draggedActiveItem.Amount}" );
                                                                targetActiveItem.Amount += draggedActiveItem.Amount;
                                                                
                                                                draggedItemGrid.ActiveItems.Remove(draggedActiveItem);
                                                                grid.ActiveItems.Add(originalPosition);
                                                                isDragging = false;
                                                                originalPosition = null;
                                                                draggedItem = null;
                                                                dragOffset = Vector2.zero;
                                                                currentEvent.Use();
                                                                return;
                                                            }
                                                            
                                                        }

                                                    }
                                                    
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }                        
                                       
                        Vector2 newPos = GetGridPosition(gridRect, mousePosition - dragOffset);
                        if (CanPlaceItem(grid, (int)newPos.x, (int)newPos.y))
                        {
                            grid.PlaceItem(draggedItem, (int)newPos.x, (int)newPos.y);
                            if (draggedItem == selectedItem)
                            {
                                selectedItem = null;
                            }
                        }
                        else if (originalPosition != null)
                        {
                            // Возвращаем предмет на исходную позицию
                            grid.ActiveItems.Add(originalPosition);
                        }
                        isDragging = false;
                        draggedItem = null;
                        originalPosition = null;
                        dragOffset = Vector2.zero;
                        currentEvent.Use();
                    }
                }
                break;
            }
            
            if (isDragging && draggedItem != null)
            {
                DrawDragPreview(gridRect, grid);
            }
        }

        // Добавляем метод проверки возможности комбинации
        private bool CanCombineItems(InventoryItem targetItem, InventoryItem draggedItem)
        {
            if (targetItem == null || draggedItem == null) return false;
            
            // Проверяем есть ли у целевого предмета сетка
            if (targetItem.Grids == null || targetItem.Grids.Length == 0) return false;
            
            // Проверяем разрешенные типы предметов для комбинации
            foreach (var grid in targetItem.Grids)
            {
                if(grid.CompatibilityGridMod == GridData2.Compatibility.Access_public)
                {
                    return true;
                }
                else
                if(grid.CompatibilityGridMod == GridData2.Compatibility.Access_by_specific_item)
                {
                    if (grid.SpecificItemCombined.Contains(draggedItem.ItemData))
                    {
                        return true;
                    }
                }
                else
                if(grid.CompatibilityGridMod == GridData2.Compatibility.Access_by_item_groups)
                {
                    if(draggedItem.ItemData.ItemGroup == grid.CompatibleGroup)
                    {
                        return true;
                    }
                }
            }
            
            return false;
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
        }

        private bool CanPlaceItem(GridData2 grid, int x, int y)
        {
            if (draggedItem == null) return false;
            if (x < 0 || y < 0) return false;
            if (x + draggedItem.WIDTH > grid.Size.x) return false;
            if (y + draggedItem.HEIGHT > grid.Size.y) return false;

            return grid.CheckAvailableSpace(x, y, draggedItem.WIDTH, draggedItem.HEIGHT);
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

            if (item.ItemData.MaxStackSize > 0)
            {
                menu.AddItem(new GUIContent("Добавить 5 штук"), false, () => {
                    // Проверяем максимальное количество, вычеслиь сколько недастает до максимального стека и если оно больше 0, то добавляем 
                    if(item.Amount < item.ItemData.MaxStackSize)
                    {
                        Debug.Log(grid.MaxStackSize);
                        item.Amount += 5;
                        if(item.Amount > grid.MaxStackSize)
                        {
                            item.Amount = grid.MaxStackSize;
                        }
                        else
                        if(item.Amount > item.ItemData.MaxStackSize)
                        {
                            item.Amount = item.ItemData.MaxStackSize;
                        }
                    }
                });
            }

            if (item.ItemData.MaxStackSize > 0 && item.Amount > 0)
            {
                menu.AddItem(new GUIContent("Отнять 5 штук"), false, () => {
                    item.Amount -= 5;
                    if(item.Amount < 0)item.Amount = 0;
                });
            }

            // Добавляем пункт только если предмет имеет сетку и поддерживает комбинирование
            if (item.Grids != null && item.Grids.Length > 0)
            {
                menu.AddItem(new GUIContent("Открыть сетку"), false, () => 
                {
                    OpenItemGridEditor(item);
                });
            }
            
            // пройтись по всем сеткам предмета и если есть сетка с предметами, то добавить пункт меню
            foreach (var itemGrid in item.Grids)
            {
                foreach (var activeItem in itemGrid.ActiveItems)
                {
                    menu.AddItem(new GUIContent($"Извлечь {activeItem.ItemData.Title}"), false, () => 
                    {
                        itemGrid.ActiveItems.Remove(activeItem);
                        // Добавляем activeItem в целевую сетку или в выбранные предметы
                        grid.ActiveItems.Add(activeItem); // Пример добавления в первую сетку
                    });
                    
                }
            }

            


            menu.ShowAsContext();
        }

        private void OpenItemGridEditor(InventoryItem item)
        {
            ItemGridEditorWindow window = EditorWindow.GetWindow<ItemGridEditorWindow>("Item Grid Editor");
            window.Initialize(item);
            window.Show();
        }

        //----------------------------------
        // Изменяем метод DrawItems
        private void DrawItems(Rect gridRect, GridData2 grid)
        {
            foreach (var item in grid.ActiveItems)
            {
                // Вычисляем прямоугольник для предмета
                Rect itemRect = new Rect(
                    gridRect.x + item.OnGridPosition.x * cellSize,
                    gridRect.y + item.OnGridPosition.y * cellSize,
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

                // Проверяем и отображаем количество
                int amount = FindDeepAmount(item);
                if (amount > 0)
                {
                    // Создаем прямоугольник для текста в правом нижнем углу
                    Rect amountRect = new Rect(
                        itemRect.x + itemRect.width - 20,
                        itemRect.y + itemRect.height - 15,
                        20,
                        15
                    );

                    // Настраиваем стиль для текста
                    GUIStyle amountStyle = new GUIStyle(EditorStyles.boldLabel);
                    amountStyle.normal.textColor = Color.white;
                    amountStyle.alignment = TextAnchor.LowerRight;

                    // Рисуем фон для текста
                    EditorGUI.DrawRect(amountRect, new Color(0, 0, 0, 0.5f));
                    
                    // Отображаем количество
                    EditorGUI.LabelField(amountRect, amount.ToString(), amountStyle);
                }
            }
        }

        // Добавляем метод поиска вложенных предметов с amount
        private int FindDeepAmount(InventoryItem item)
        {
            if (item == null || item.Grids == null) return 0;
            
            int totalAmount = item.Amount;
            
            foreach (var grid in item.Grids)
            {
                foreach (var activeItem in grid.ActiveItems)
                {
                    int deepAmount = FindDeepAmount(activeItem);
                    if (deepAmount > 0)
                    {
                        return deepAmount; // Возвращаем первое найденное количество
                    }
                }
            }
            
            return totalAmount;
        }
    }
}
