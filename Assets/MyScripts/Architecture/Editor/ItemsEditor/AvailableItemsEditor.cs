using UnityEditor;
using UnityEngine;
using InventoryDiablo;
using System;
using System.Linq;
using static GridData2;

namespace ModularEventArchitecture
{
    // Редактор настрока итемов
    public class AvailableItemsEditor : EditorWindow
    {
        
        private AvailableItems targetAsset;
        private Vector2 scrollPosition;
        private float cellSize = 32f;
        private InventoryItem selectedItem;
        private bool isDragging;
        private Vector2 mousePosition;
        private Vector2 dragStartPosition;
        private GridData2 activeGrid;

        public int ItemsIndexTypes = 0;

        private bool needToRemoveGrid = false;
        private int gridIndexToRemove = -1;
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

        [MenuItem("Tools/Редактор предметов")]
        public static void ShowWindow() => GetWindow<AvailableItemsEditor>("Available Items Editor");

        private void OnGUI()
        {
            if (targetAsset == null)
            {
                DrawAssetSelection();
                return;
            }
            
            EditorGUILayout.BeginHorizontal();
            
                DrawConfigSelection();
            
                // Левая панель - список предметов
                DrawItemsList();

                if(selectedItem != null)
                {
                    DrawItemInfo();
                
                    // Правая панель - редактор выбранного предмета
                    DrawSelectedItemEditor();
                    

                    if (Event.current.type == EventType.Layout)
                    {
                        if (needToRemoveGrid)
                        {
                            RemoveGrid(gridIndexToRemove);
                            needToRemoveGrid = false;
                            Repaint();
                        }
                    }
                }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawConfigSelection()
        {
            // Добавляем кнопку сохранения внизу окна
            EditorGUILayout.BeginVertical("box", GUILayout.Width(100));
            if (GUILayout.Button("Сохранить изменения", GUILayout.Height(30)))
            {
                EditorUtility.SetDirty(targetAsset);
                AssetDatabase.SaveAssets();
            }

            EditorGUILayout.Space(20);

            if (GUILayout.Button("Выбрать другой ассет", GUILayout.Height(30)))
            {
                targetAsset = null;

                GUIUtility.ExitGUI();
            }
            EditorGUILayout.EndVertical();
        }
        
        //блок и информацией по сеткам
        private void DrawItemInfo()
        {
            if (selectedItem == null) return;
            
            EditorGUILayout.BeginVertical("box", GUILayout.Width(400));
            
                // Основные параметры предмета
                EditorGUILayout.LabelField("Параметры итема", EditorStyles.boldLabel);

                selectedItem.ItemData.Title = EditorGUILayout.TextField("Title", selectedItem.ItemData.Title);

                selectedItem.ItemData.ItemIcon = EditorGUILayout.ObjectField("Icon", selectedItem.ItemData.ItemIcon, typeof(Sprite), false) as Sprite;
                
                DrawGridEditor();
            EditorGUILayout.EndVertical();
        }

        private void DrawAssetSelection()
        {
            EditorGUILayout.HelpBox("Select AvailableItems Asset", MessageType.Info);
            
            targetAsset = EditorGUILayout.ObjectField("Asset", targetAsset, typeof(AvailableItems), false) as AvailableItems;
            
            if (GUILayout.Button("Создать новый ассет"))
            {
                string path = EditorUtility.SaveFilePanelInProject("Create AvailableItems","AvailableItems","asset","Create a new AvailableItems asset");
                
                if (!string.IsNullOrEmpty(path))
                {
                    targetAsset = CreateInstance<AvailableItems>();
                    AssetDatabase.CreateAsset(targetAsset, path);
                    AssetDatabase.SaveAssets();
                }
            }
        }

        private void DrawItemsList()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.Width(200));
                EditorGUILayout.LabelField("Список итемов:", EditorStyles.boldLabel);

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                
                    for (int i = 0; i < targetAsset.items.Count; i++)
                    {
                        var item = targetAsset.items[i];
                        
                        GUI.backgroundColor = selectedItem != null && selectedItem.ItemData.Title == item.ItemData.Title ? Color.cyan : Color.white;
                        
                        if (GUILayout.Button(item.ItemData.Title))
                        {
                            selectedItem = item;
                        }

                        GUI.backgroundColor = Color.white;
                    }
                
                EditorGUILayout.EndScrollView();

                if (selectedItem != null)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Выбранный итем:", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"Название: {selectedItem.ItemData.Title}");
                    EditorGUILayout.LabelField($"Размер: {selectedItem.WIDTH}x{selectedItem.HEIGHT}");
                }

            EditorGUILayout.EndVertical();
        }

        private void DrawSelectedItemEditor()
        {
            EditorGUILayout.BeginVertical("box"/* , GUILayout.Width(600) */);

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                    // Редактор сеток предмета
                    foreach (var grid in selectedItem.Grids)
                    {
                        // Отрисовка сетки
                        Rect gridRect = new Rect(grid.Position.x, grid.Position.y, grid.GridSize.x * cellSize, grid.GridSize.y * cellSize);

                        DrawGrid(gridRect, grid);

                        HandleDragAndDrop(gridRect, grid);
                    }
                
                EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }        

            bool open = false;
        private void DrawGridEditor()
        {
            // EditorGUILayout.Space(150);

            EditorGUILayout.LabelField("Настройки сетки", EditorStyles.boldLabel);

            if (GUILayout.Button("Добавить сетку"))
            {
                AddNewGrid();
            }


            // Отображаем все сетки
            for (int i = 0; i < selectedItem.Grids.Length; i++)
            {
                EditorGUILayout.BeginVertical("box");
                
                    EditorGUILayout.LabelField($"Сетка {i}", GUILayout.Width(100));

                    if (GUILayout.Button("Удалить", GUILayout.Width(100)))
                    {
                        gridIndexToRemove = i;
                        needToRemoveGrid = true;
                    }

                    var grid = selectedItem.Grids[i];
                    grid.Position = EditorGUILayout.Vector2Field("Позиция сетки", grid.Position);
                    grid.GridSize = EditorGUILayout.Vector2IntField("Размер сетки", grid.GridSize);

                    selectedItem.Grids[i].CompatibilityGridMod = (Compatibility)EditorGUILayout.EnumPopup("Тип совместимости", selectedItem.Grids[i].CompatibilityGridMod);

                    if(selectedItem.Grids[i].CompatibilityGridMod == Compatibility.Access_by_item_groups)
                    {
                        ItemsIndexTypes = EditorGUILayout.Popup("Совместима с группой", ItemsIndexTypes, Asset.HierarchyItemsTypes.ToArray());
                        selectedItem.Grids[i].CompatibleGroup = Asset.HierarchyItemsTypes[ItemsIndexTypes];
                    }
                    else
                    if(selectedItem.Grids[i].CompatibilityGridMod == Compatibility.Access_by_specific_item)
                    {
                        open = EditorGUILayout.BeginFoldoutHeaderGroup(open, "Итемы которые можно расположить на сетку");
                            if(open)
                            {
                                for (int i1 = 0; i1 < selectedItem.Grids[i].specificItemCombined.Count; i1++)
                                {
                                    ItemData item = selectedItem.Grids[i].specificItemCombined[i1];
                                    
                                    EditorGUILayout.BeginHorizontal();

                                        selectedItem.Grids[i].specificItemCombined[i1] = (ItemData)EditorGUILayout.ObjectField("Element " + i1, item, typeof(ItemData), false);
                                        if (GUILayout.Button("-", GUILayout.Width(20)))
                                        {
                                            selectedItem.Grids[i].specificItemCombined.RemoveAt(i1);
                                        }

                                    EditorGUILayout.EndHorizontal();
                                }

                                EditorGUILayout.BeginHorizontal();
                                if (GUILayout.Button("+", GUILayout.Width(20)))
                                {
                                    selectedItem.Grids[i].specificItemCombined.Add(new ItemData());
                                }
                                if (GUILayout.Button("-", GUILayout.Width(20)))
                                {
                                    selectedItem.Grids[i].specificItemCombined.RemoveAt(selectedItem.Grids[i].specificItemCombined.Count - 1);
                                }
                                EditorGUILayout.EndHorizontal();
                            }

                        EditorGUILayout.EndFoldoutHeaderGroup();
                    }
                        

                EditorGUILayout.EndVertical();
            }
        }

        private void AddNewGrid()
        {
            GridData2 newGrid = new GridData2
            {
                GridSize = new Vector2Int(5, 5),
                Position = new Vector2(0, selectedItem.Grids.Length * 200)
            };
            Array.Resize(ref selectedItem.Grids, selectedItem.Grids.Length + 1);
            selectedItem.Grids[selectedItem.Grids.Length - 1] = newGrid;
        }

        private void RemoveGrid(int index)
        {
            var tempList = selectedItem.Grids.ToList();
            tempList.RemoveAt(index);
            selectedItem.Grids = tempList.ToArray();
        }

        private void DrawGrid(Rect gridRect, GridData2 grid)
        {            
            // Рисуем ячейки с учетом смещения
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

                    // Рисуем фон ячейки
                    EditorGUI.DrawRect(cellRect, new Color(0.3f, 0.3f, 0.3f));
                    
                    // Рисуем рамку ячейки
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
                        isDragging = true;
                        dragStartPosition = mousePosition;
                        activeGrid = grid;
                        currentEvent.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (isDragging && activeGrid == grid)
                    {
                        Vector2 delta = mousePosition - dragStartPosition;
                        grid.Position += delta;
                        dragStartPosition = mousePosition;
                        Repaint();
                        currentEvent.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (isDragging)
                    {
                        isDragging = false;
                        activeGrid = null;
                        currentEvent.Use();
                    }
                    break;
            }
        }
    }
}
