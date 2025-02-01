using InventoryDiablo;
using MyProject;
using UnityEditor;
using UnityEngine;

namespace ModularEventArchitecture
{
    public abstract class BaseWindowDrawer : EditorWindow
    {
        protected AvailableItems _targetAsset;
        protected InventoryItem _selectedItem;
        protected InventorySlot _currentSlot;
        protected Vector2 _itemListScroll;

        protected void DrawConfigSelection()
        {
            // Добавляем кнопку сохранения внизу окна
            EditorGUILayout.BeginVertical("box", GUILayout.Width(100));
                
                if (GUILayout.Button("Сохранить изменения", GUILayout.Height(30)))
                {
                    EditorUtility.SetDirty(_targetAsset);
                    AssetDatabase.SaveAssets();
                }

                EditorGUILayout.Space(20);

                if (GUILayout.Button("Выбрать другой ассет", GUILayout.Height(30)))
                {
                    _targetAsset = null;

                    GUIUtility.ExitGUI();
                }

            EditorGUILayout.EndVertical();
        }

        protected void DrawBlock_CreateNewAsset()
        {

            EditorGUILayout.BeginVertical("box", GUILayout.Width(200));

                if (_targetAsset == null)
                {
                    _targetAsset = AssetDatabase.LoadAssetAtPath<AvailableItems>("Assets/MyScripts/Architecture/Editor/AvailableItems.asset");
                    if (_targetAsset == null)
                    {
                        EditorGUILayout.HelpBox("Create AvailableItems asset", MessageType.Warning);
                        
                        if (GUILayout.Button("Create"))
                        {
                            _targetAsset = CreateInstance<AvailableItems>();
                            
                            AssetDatabase.CreateAsset(_targetAsset, "Assets/Data/AvailableItems.asset");
                        }
                        
                        EditorGUILayout.EndVertical();
                        return;
                    }
                }

            EditorGUILayout.EndVertical();
        }

        //отобразить список итемов
        protected void DrawBlock_ItemsList()
        {            
            EditorGUILayout.BeginVertical("box", GUILayout.Width(200));
            

                EditorGUILayout.LabelField("Предметы", EditorStyles.boldLabel);
                
                _itemListScroll = EditorGUILayout.BeginScrollView(_itemListScroll);
                
                    foreach (var item in _targetAsset.items)
                    {
                        if (Helper.AreHasFlag(item.ItemData.TypeItem, _currentSlot.TypeItem)) continue;

                        GUI.backgroundColor = _selectedItem != null && _selectedItem.ItemData.Title == item.ItemData.Title ? Color.cyan : Color.white;

                        if (GUILayout.Button(item.ItemData.Title))
                        {
                            InventoryItem newItem = new InventoryItem(item);

                            _selectedItem = newItem;
                        }

                        GUI.backgroundColor = Color.white;
                    }
                
                EditorGUILayout.EndScrollView();
                
                if (_selectedItem != null)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Выбран итем:", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"Имя: {_selectedItem.ItemData.Title}");
                    EditorGUILayout.LabelField($"размер: {_selectedItem.WIDTH}x{_selectedItem.HEIGHT}");
                }
            
            EditorGUILayout.EndVertical();
        }
    }
}