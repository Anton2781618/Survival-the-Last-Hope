using UnityEditor;
using UnityEngine;
using InventoryDiablo;
using System;
using System.Linq;
using static GridData2;
using static InventoryDiablo.ItemData;

namespace ModularEventArchitecture
{
    // Редактор настрока итемов
    public class ItemsEditor : EditorWindow
    {
        //-----------------------------------------------------------
        private AvailableItems _targetAsset;
        private Vector2 _centrScrollPosition;
        private Vector2 _scrollPosition;

        //-----------------------------------------------------------
        private float _cellSize = 32f;
        private float _gridCellSize = 16f;
        private Vector2 initialGridPosition;
        
        //-----------------------------------------------------------
        private InventoryItem _selectedItem;
        private bool _isDragging;
        private Vector2 _mousePosition;
        private Vector2 _dragStartPosition;
        private GridData2 _activeGrid;
        
        //-----------------------------------------------------------
        public int ItemsIndexTypes = 0;
        public int ItemGroupIndex = 0;
        
        //-----------------------------------------------------------
        private bool _needToRemoveGrid = false;
        private int _gridIndexToRemove = -1;

        //-----------------------------------------------------------
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

        //!-----------------------------------------------------------

        [MenuItem("Tools/Редактор предметов")]
        public static void ShowWindow() => GetWindow<ItemsEditor>("Available Items Editor");

        private void OnGUI()
        {
            if (_targetAsset == null)
            {
                DrawAssetSelection();
                return;
            }
            
            EditorGUILayout.BeginHorizontal();
            
                DrawConfigSelection();
            
                // Левая панель - список предметов
                DrawItemsList();

                if(_selectedItem != null)
                {
                    //центральная часть - информация о выбранном предмете
                    DrawItemInfo();
                
                    // Правая панель - редактор выбранного предмета
                    DrawSelectedItemEditor();
                    

                    if (Event.current.type == EventType.Layout)
                    {
                        if (_needToRemoveGrid)
                        {
                            RemoveGrid(_gridIndexToRemove);
                            _needToRemoveGrid = false;
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
                // Помечаем все измененные объекты как "грязные"
                EditorUtility.SetDirty(_targetAsset);
                EditorUtility.SetDirty(_selectedItem.ItemData);
                
                // Сохраняем все изменения
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            EditorGUILayout.Space(20);

            if (GUILayout.Button("Выбрать другой ассет", GUILayout.Height(30)))
            {
                _targetAsset = null;

                GUIUtility.ExitGUI();
            }
            EditorGUILayout.EndVertical();
        }
        
        //блок и информацией по сеткам
        private void DrawItemInfo()
        {
            if (_selectedItem == null) return;
            _centrScrollPosition = EditorGUILayout.BeginScrollView(_centrScrollPosition, GUILayout.Width(450));
            
                EditorGUILayout.BeginVertical(GUILayout.Width(400));
                    EditorGUILayout.BeginVertical("box", GUILayout.Width(400));
                    
                        EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(_selectedItem.ItemData.name, EditorStyles.boldLabel);
                            if (GUILayout.Button("Обновиь название ассета", GUILayout.Width(200)))
                            {
                                //сменить название итема на название ассета
                                string assetPath = AssetDatabase.GetAssetPath(_selectedItem.ItemData);
                                _selectedItem.ItemData.name = _selectedItem.ItemData.Title;
                                AssetDatabase.RenameAsset(assetPath, _selectedItem.ItemData.name);
                                EditorUtility.SetDirty(_selectedItem.ItemData);
                                AssetDatabase.SaveAssets();
                            }

                            //удалить ассет
                            if (GUILayout.Button("X", GUILayout.Width(20)))
                            {
                                _targetAsset.items.Remove(_selectedItem);
                                string assetPath = AssetDatabase.GetAssetPath(_selectedItem.ItemData);
                                AssetDatabase.DeleteAsset(assetPath);

                                _selectedItem = null;
                                GUIUtility.ExitGUI();
                            }
                        EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space(5);
                
                    // Основные параметры предмета
                    EditorGUILayout.LabelField("Параметры итема", EditorStyles.boldLabel);

                    _selectedItem.ItemData.ItemIcon = EditorGUILayout.ObjectField("Иконка", _selectedItem.ItemData.ItemIcon, typeof(Sprite), false) as Sprite;

                    _selectedItem.ItemData.Title = EditorGUILayout.TextField("Название", _selectedItem.ItemData.Title);
                    _selectedItem.ItemData.Description = EditorGUILayout.TextField("Описание", _selectedItem.ItemData.Description, GUILayout.Height(50));

                    _selectedItem.ItemData.TypeItem = (ItemType)EditorGUILayout.EnumFlagsField("Тип предмета", _selectedItem.ItemData.TypeItem);

                    //префаб
                    _selectedItem.ItemData.Prefab = EditorGUILayout.ObjectField("Префаб", _selectedItem.ItemData.Prefab, typeof(ItemOnstreet), false) as ItemOnstreet;

                    EditorGUILayout.LabelField("размеры", EditorStyles.boldLabel);
                    _selectedItem.ItemData.Width = EditorGUILayout.IntField("Ширина", _selectedItem.ItemData.Width);
                    _selectedItem.ItemData.Height = EditorGUILayout.IntField("Высота", _selectedItem.ItemData.Height);

                    ItemGroupIndex = EditorGUILayout.Popup("группа итема", Array.IndexOf(Asset.HierarchyItemsTypes.ToArray(), _selectedItem.ItemData.ItemGroup), Asset.HierarchyItemsTypes.ToArray());
                    _selectedItem.ItemData.ItemGroup = Asset.HierarchyItemsTypes[ItemGroupIndex];

                    _selectedItem.ItemData.MaxStackSize = EditorGUILayout.IntField("Максимальный стак", _selectedItem.ItemData.MaxStackSize);
                    _selectedItem.Amount = EditorGUILayout.IntField("Количество", _selectedItem.Amount);
                    
                    EditorGUILayout.EndVertical();
                    DrawGridEditor();
                EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();
        }

        private void DrawAssetSelection()
        {
            EditorGUILayout.HelpBox("Select AvailableItems Asset", MessageType.Info);
            
            _targetAsset = EditorGUILayout.ObjectField("Asset", _targetAsset, typeof(AvailableItems), false) as AvailableItems;
            
            if (GUILayout.Button("Создать новый ассет"))
            {
                string path = EditorUtility.SaveFilePanelInProject("Create AvailableItems","AvailableItems","asset","Create a new AvailableItems asset");
                
                if (!string.IsNullOrEmpty(path))
                {
                    _targetAsset = CreateInstance<AvailableItems>();
                    AssetDatabase.CreateAsset(_targetAsset, path);
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
                
                _targetAsset.items.Add(newItem);
            }

                EditorGUILayout.LabelField("Список итемов:", EditorStyles.boldLabel);

                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                
                    for (int i = 0; i < _targetAsset.items.Count; i++)
                    {
                        var item = _targetAsset.items[i];
                        
                        GUI.backgroundColor = _selectedItem != null && _selectedItem.ItemData.Title == item.ItemData.Title ? Color.cyan : Color.white;
                        
                        if (GUILayout.Button(item.ItemData.Title))
                        {
                            _selectedItem = item;
                        }

                        GUI.backgroundColor = Color.white;
                    }
                
                EditorGUILayout.EndScrollView();

                if (_selectedItem != null)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Выбранный итем:", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"Название: {_selectedItem.ItemData.Title}");
                    EditorGUILayout.LabelField($"Размер: {_selectedItem.WIDTH}x{_selectedItem.HEIGHT}");
                }

            EditorGUILayout.EndVertical();
        }

        private void DrawSelectedItemEditor()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            Vector2 sizeItem = InventoryEditor.GetContainerSize(_selectedItem.ItemData.TypeItem);

            EditorGUI.DrawRect(new Rect(0, 0, sizeItem.x, sizeItem.y), new Color(0.1803922f, 0.1803922f, 0.1803922f));

            if(_selectedItem.Grids == null) return;

            // Редактор сеток предмета
            for (int i = 0; i < _selectedItem.Grids.Length; i++)
            {
                GridData2 grid = _selectedItem.Grids[i];
                // Отрисовка сетки
                Rect gridRect = new Rect(grid.Position.x, grid.Position.y, grid.Size.x * _cellSize, grid.Size.y * _cellSize);

                DrawGrid(gridRect, grid);
                // Рисуем текст с номером сетки посередине сетки белого цвета
                Handles.Label(gridRect.min + new Vector2(5, 5), $"Сетка №{i}", new GUIStyle { normal = new GUIStyleState { textColor = Color.white} });

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
            for (int i = 0; i < _selectedItem.Grids.Length; i++)
            {
                EditorGUILayout.Space(5);
                var grid = _selectedItem.Grids[i];

                var gridRect = EditorGUILayout.BeginVertical();

                    EditorGUI.DrawRect(gridRect, new Color(0.2627451f, 0.2627451f, 0.2627451f));
                
                    EditorGUILayout.LabelField($"Сетка {i}", GUILayout.Width(100));

                    if (GUILayout.Button("Удалить", GUILayout.Width(100)))
                    {
                        _gridIndexToRemove = i;
                        _needToRemoveGrid = true;
                    }

                    grid.Position = EditorGUILayout.Vector2Field("Позиция сетки", grid.Position);
                    grid.Size = EditorGUILayout.Vector2IntField("Размер сетки", grid.Size);

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
                Size = new Vector2Int(5, 5),
                Position = new Vector2(0, 0)
            };
            Array.Resize(ref _selectedItem.Grids, _selectedItem.Grids.Length + 1);
            _selectedItem.Grids[_selectedItem.Grids.Length - 1] = newGrid;
        }

        private void RemoveGrid(int index)
        {
            var tempList = _selectedItem.Grids.ToList();
            tempList.RemoveAt(index);
            _selectedItem.Grids = tempList.ToArray();
        }

        private void DrawGrid(Rect gridRect, GridData2 grid)
        {            
            // Рисуем ячейки с учетом смещения
            for (int x = 0; x < grid.Size.x; x++)
            {
                for (int y = 0; y < grid.Size.y; y++)
                {
                    Rect cellRect = new Rect(
                        gridRect.x + x * _cellSize,
                        gridRect.y + y * _cellSize,
                        _cellSize,
                        _cellSize
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
            _mousePosition = currentEvent.mousePosition;

            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    if (gridRect.Contains(_mousePosition))
                    {
                        _isDragging = true;
                        _dragStartPosition = _mousePosition;
                        _activeGrid = grid;
                        // Сохраняем начальную позицию сетки
                        initialGridPosition = grid.Position;
                        currentEvent.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (_isDragging && _activeGrid == grid)
                    {
                        // Вычисляем дельту с учетом размера ячейки
                        Vector2 delta = _mousePosition - _dragStartPosition;
                        
                        // Привязываем позицию к сетке
                        Vector2 snappedPosition = new Vector2(
                            initialGridPosition.x + Mathf.Round(delta.x / _gridCellSize) * _gridCellSize,
                            initialGridPosition.y + Mathf.Round(delta.y / _gridCellSize) * _gridCellSize
                        );
                        
                        grid.Position = snappedPosition;
                        Repaint();
                        currentEvent.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (_isDragging)
                    {
                                // Финальная привязка к сетке
                        Vector2 finalPosition = new Vector2(
                            Mathf.Round(grid.Position.x / _gridCellSize) * _gridCellSize,
                            Mathf.Round(grid.Position.y / _gridCellSize) * _gridCellSize
                        );
                        
                        grid.Position = finalPosition;
                        _isDragging = false;
                        _activeGrid = null;
                        currentEvent.Use();
                    }
                    break;
            }
        }
    }
}
