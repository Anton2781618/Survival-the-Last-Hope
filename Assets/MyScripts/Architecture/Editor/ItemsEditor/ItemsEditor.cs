using UnityEditor;
using UnityEngine;
using InventoryDiablo;
using System;
using System.Linq;
using static GridData2;
using System.Reflection.Emit;
using NUnit.Framework;

namespace ModularEventArchitecture
{
    // Редактор настрока итемов
    public class ItemsEditor : EditorWindow
    {
        
        private AvailableItems targetAsset;
        private Vector2 CentrScrollPosition;
        private Vector2 scrollPosition;
        private float cellSize = 32f;
        private InventoryItem selectedItem;
        private bool isDragging;
        private Vector2 mousePosition;
        private Vector2 dragStartPosition;
        private GridData2 activeGrid;
        
        //-----------------------------------------------------------
        public int ItemsIndexTypes = 0;
        public int ItemGroupIndex = 0;
        

        //-----------------------------------------------------------
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
        public static void ShowWindow() => GetWindow<ItemsEditor>("Available Items Editor");

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
                    //центральная часть - информация о выбранном предмете
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
            CentrScrollPosition = EditorGUILayout.BeginScrollView(CentrScrollPosition, GUILayout.Width(450));
            
                EditorGUILayout.BeginVertical(GUILayout.Width(400));
                    EditorGUILayout.BeginVertical("box", GUILayout.Width(400));
                    
                        EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(selectedItem.ItemData.name, EditorStyles.boldLabel);
                            if (GUILayout.Button("Обновиь название ассета", GUILayout.Width(200)))
                            {
                                //сменить название итема на название ассета
                                string assetPath = AssetDatabase.GetAssetPath(selectedItem.ItemData);
                                selectedItem.ItemData.name = selectedItem.ItemData.Title;
                                AssetDatabase.RenameAsset(assetPath, selectedItem.ItemData.name);
                                EditorUtility.SetDirty(selectedItem.ItemData);
                                AssetDatabase.SaveAssets();
                            }

                            //удалить ассет
                            if (GUILayout.Button("X", GUILayout.Width(20)))
                            {
                                targetAsset.items.Remove(selectedItem);
                                string assetPath = AssetDatabase.GetAssetPath(selectedItem.ItemData);
                                AssetDatabase.DeleteAsset(assetPath);

                                selectedItem = null;
                                GUIUtility.ExitGUI();
                            }
                        EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space(5);
                
                    // Основные параметры предмета
                    EditorGUILayout.LabelField("Параметры итема", EditorStyles.boldLabel);

                    selectedItem.ItemData.ItemIcon = EditorGUILayout.ObjectField("Иконка", selectedItem.ItemData.ItemIcon, typeof(Sprite), false) as Sprite;

                    selectedItem.ItemData.Title = EditorGUILayout.TextField("Название", selectedItem.ItemData.Title);
                    selectedItem.ItemData.Description = EditorGUILayout.TextField("Описание", selectedItem.ItemData.Description, GUILayout.Height(50));

                    //префаб
                    selectedItem.ItemData.Prefab = EditorGUILayout.ObjectField("Префаб", selectedItem.ItemData.Prefab, typeof(ItemOnstreet), false) as ItemOnstreet;

                    EditorGUILayout.LabelField("размеры", EditorStyles.boldLabel);
                    selectedItem.ItemData.Width = EditorGUILayout.IntField("Ширина", selectedItem.ItemData.Width);
                    selectedItem.ItemData.Height = EditorGUILayout.IntField("Высота", selectedItem.ItemData.Height);

                    ItemGroupIndex = EditorGUILayout.Popup("группа итема", Array.IndexOf(Asset.HierarchyItemsTypes.ToArray(), selectedItem.ItemData.ItemGroup), Asset.HierarchyItemsTypes.ToArray());
                    selectedItem.ItemData.ItemGroup = Asset.HierarchyItemsTypes[ItemGroupIndex];

                    selectedItem.ItemData.MaxStackSize = EditorGUILayout.IntField("Максимальный стак", selectedItem.ItemData.MaxStackSize);
                    selectedItem.Amount = EditorGUILayout.IntField("Количество", selectedItem.Amount);
                    
                    EditorGUILayout.EndVertical();
                    DrawGridEditor();
                EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();
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
            if (GUILayout.Button("Создать модель"))
            {
                //сохранить itemData как ассет. адресс папки взять из первого экземпляра в списке выбранного AvailableItems 
                string path = "Assets/MyAssets/ScriptableObjects/Items";
                //создать новый itemData 
                ItemData itemData = CreateInstance<ItemData>();
                itemData.Title = "New Item";
                // разместиь itemData в папке path
                AssetDatabase.CreateAsset(itemData, path + "/New Item.asset");
                
                InventoryItem newItem = new InventoryItem();
                newItem.ItemData = itemData;
                
                targetAsset.items.Add(newItem);
            }

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
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUI.DrawRect(new Rect(0, 0, 650, 265), new Color(0.1803922f, 0.1803922f, 0.1803922f));

            if(selectedItem.Grids == null) return;

            // Редактор сеток предмета
            for (int i = 0; i < selectedItem.Grids.Length; i++)
            {
                GridData2 grid = selectedItem.Grids[i];
                // Отрисовка сетки
                Rect gridRect = new Rect(grid.Position.x, grid.Position.y, grid.GridSize.x * cellSize, grid.GridSize.y * cellSize);

                DrawGrid(gridRect, grid);
                // Рисуем текст с номером сетки посередине сетки белого цвета
                Handles.Label(gridRect.min + new Vector2(5, 5), $"Сетка №{i} {grid.Position}", new GUIStyle { normal = new GUIStyleState { textColor = Color.white} });

                HandleDragAndDrop(gridRect, grid);
            }
            
            EditorGUILayout.EndScrollView();
        }        

            bool open = false;
        private void DrawGridEditor()
        {
            EditorGUILayout.LabelField("Настройки сетки", EditorStyles.boldLabel);

            if (GUILayout.Button("Добавить сетку"))
            {
                AddNewGrid();
            }

            // Отображаем все сетки
            for (int i = 0; i < selectedItem.Grids.Length; i++)
            {
                EditorGUILayout.Space(5);
                var grid = selectedItem.Grids[i];

                var gridRect = EditorGUILayout.BeginVertical();

                    EditorGUI.DrawRect(gridRect, new Color(0.3f, 0.3f, 0.3f));
                
                    EditorGUILayout.LabelField($"Сетка {i}", GUILayout.Width(100));

                    if (GUILayout.Button("Удалить", GUILayout.Width(100)))
                    {
                        gridIndexToRemove = i;
                        needToRemoveGrid = true;
                    }

                    grid.Position = EditorGUILayout.Vector2Field("Позиция сетки", grid.Position);
                    grid.GridSize = EditorGUILayout.Vector2IntField("Размер сетки", grid.GridSize);

                    grid.MaxStackSize = EditorGUILayout.IntField("Максимальный стак итемов", grid.MaxStackSize);

                    grid.CompatibilityGridMod = (Compatibility)EditorGUILayout.EnumPopup("Тип совместимости", grid.CompatibilityGridMod);

                    if(grid.CompatibilityGridMod == Compatibility.Access_by_item_groups)
                    {
                        ItemsIndexTypes = EditorGUILayout.Popup("Совместима с группой", Array.IndexOf(Asset.HierarchyItemsTypes.ToArray(), grid.CompatibleGroup), Asset.HierarchyItemsTypes.ToArray());
                        grid.CompatibleGroup = Asset.HierarchyItemsTypes[ItemsIndexTypes];
                    }
                    else
                    if(grid.CompatibilityGridMod == Compatibility.Access_by_specific_item)
                    {
                        open = EditorGUILayout.BeginFoldoutHeaderGroup(open, "Итемы которые можно расположить на сетку");
                            if(open)
                            {
                                for (int i1 = 0; i1 < grid.specificItemCombined.Count; i1++)
                                {
                                    ItemData item = grid.specificItemCombined[i1];
                                    
                                    EditorGUILayout.BeginHorizontal();

                                        grid.specificItemCombined[i1] = (ItemData)EditorGUILayout.ObjectField("Element " + i1, item, typeof(ItemData), false);
                                        if (GUILayout.Button("-", GUILayout.Width(20)))
                                        {
                                            grid.specificItemCombined.RemoveAt(i1);
                                        }

                                    EditorGUILayout.EndHorizontal();
                                }

                                EditorGUILayout.BeginHorizontal();
                                if (GUILayout.Button("+", GUILayout.Width(20)))
                                {
                                    grid.specificItemCombined.Add(new ItemData());
                                }
                                if (GUILayout.Button("-", GUILayout.Width(20)))
                                {
                                    grid.specificItemCombined.RemoveAt(grid.specificItemCombined.Count - 1);
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
