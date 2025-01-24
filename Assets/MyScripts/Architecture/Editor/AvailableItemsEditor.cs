using UnityEditor;
using UnityEngine;
using InventoryDiablo;
using System.Collections.Generic;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace ModularEventArchitecture
{
    public class AvailableItemsEditor : EditorWindow
    {
        private AvailableItems targetAsset;
        private Vector2 scrollPosition;
        private float cellSize = 32f;
        private InventoryItem selectedItem;
        private bool isDragging;
        private Vector2 mousePosition;
        private Vector2 dragStartPosition;
        private Vector2 dragOffset;
        private GridData2 activeGrid;
        private Color validPlacementColor = new Color(0, 1, 0, 0.3f);
        private Color invalidPlacementColor = new Color(1, 0, 0, 0.3f);

        private bool needToRemoveGrid = false;
        private int gridIndexToRemove = -1;

        [MenuItem("Tools/Items Editor")]
        public static void ShowWindow()
        {
            GetWindow<AvailableItemsEditor>("Available Items Editor");
        }

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
            
            // Правая панель - редактор выбранного предмета
            DrawSelectedItemEditor();
            
            EditorGUILayout.EndHorizontal();

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


        private void DrawAssetSelection()
        {
            
            EditorGUILayout.HelpBox("Select AvailableItems Asset", MessageType.Info);
            targetAsset = EditorGUILayout.ObjectField("Asset", targetAsset, typeof(AvailableItems), false) as AvailableItems;
            
            if (GUILayout.Button("Create New Asset"))
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
            EditorGUILayout.LabelField("Available Items", EditorStyles.boldLabel);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            for (int i = 0; i < targetAsset.items.Count; i++)
            {
                var item = targetAsset.items[i];
                EditorGUILayout.BeginHorizontal();
                
                GUI.backgroundColor = selectedItem == item ? Color.cyan : Color.white;
                if (GUILayout.Button(item.ItemData.Title))
                {
                    selectedItem = item;
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawSelectedItemEditor()
        {
            if (selectedItem == null)
                return;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical("box", GUILayout.Width(400));
            
            // Основные параметры предмета
            EditorGUILayout.LabelField("Параметры итема", EditorStyles.boldLabel);
            selectedItem.ItemData.Title = EditorGUILayout.TextField("Title", selectedItem.ItemData.Title);
            selectedItem.ItemData.ItemIcon = EditorGUILayout.ObjectField("Icon", selectedItem.ItemData.ItemIcon, typeof(Sprite), false) as Sprite;
            
            DrawGridEditor();
            EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical("box"/* , GUILayout.Width(600) */);

                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Редактор сеток предмета
            foreach (var grid in selectedItem.Grids2)
            {
                
                // Отрисовка сетки
                Rect gridRect = new Rect(
                     grid.Position.x,
                    grid.Position.y,
                    grid.GridSize.x * cellSize,
                    grid.GridSize.y * cellSize
                );


                DrawGrid(gridRect, grid);
                HandleDragAndDrop(gridRect, grid);
            }
            
                    EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }        

        private void DrawGridEditor()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Grid Editor", EditorStyles.boldLabel);

            if (GUILayout.Button("Добавить сетку"))
            {
                AddNewGrid();
            }

            // Отображаем все сетки
            for (int i = 0; i < selectedItem.Grids2.Length; i++)
            {
                EditorGUILayout.BeginVertical("box");
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Сетка {i}", GUILayout.Width(100));
                
                if (GUILayout.Button("Удалить", GUILayout.Width(100)))
                {
                    gridIndexToRemove = i;
                    needToRemoveGrid = true;
                }

                EditorGUILayout.EndHorizontal();

                var grid = selectedItem.Grids2[i];

                // Размеры сетки
                EditorGUILayout.BeginHorizontal();
                grid.GridSize = EditorGUILayout.Vector2IntField("Grid Size", grid.GridSize);
                EditorGUILayout.EndHorizontal();

                // Позиция сетки

                EditorGUILayout.EndVertical();
                grid.Position = EditorGUILayout.Vector2Field("Position", grid.Position);
                
            }

        }

        private void AddNewGrid()
        {
            GridData2 newGrid = new GridData2
            {
                GridSize = new Vector2Int(5, 5),
                Position = new Vector2(0, selectedItem.Grids2.Length * 200)
            };
            Array.Resize(ref selectedItem.Grids2, selectedItem.Grids2.Length + 1);
            selectedItem.Grids2[selectedItem.Grids2.Length - 1] = newGrid;
        }

        private void RemoveGrid(int index)
        {
            var tempList = selectedItem.Grids2.ToList();
            tempList.RemoveAt(index);
            selectedItem.Grids2 = tempList.ToArray();
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
