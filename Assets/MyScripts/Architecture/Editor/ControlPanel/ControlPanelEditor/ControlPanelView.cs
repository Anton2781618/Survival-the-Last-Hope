using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;

//класс для отображения ноды в редакторе
public class ControlPanelView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ControlPanelView, VisualElement.UxmlTraits> { }
    private BehavioureTree _targetAsset;
    private ScrollView _scrollView;
    private Button _selectedButton;

    private ControlPanelEditor _controlPanelEditor;

    public ControlPanelView()
    {
        
    }

    internal void Setup(ControlPanelEditor controlPanelEditor)
    {
        Clear();
        _controlPanelEditor = controlPanelEditor;
        
        // Создаем базовый контейнер
        var container = new VisualElement();
        
        
        // Создаем кнопку создания нового ассета
        var createButton = new Button(() =>
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create BehaviourTree",
                "NewBehaviourTree",
                "asset",
                "Create a new BehaviourTree asset"
            );
            
            if (!string.IsNullOrEmpty(path))
            {
                var newAsset = ScriptableObject.CreateInstance<BehavioureTree>();
                AssetDatabase.CreateAsset(newAsset, path);
                AssetDatabase.SaveAssets();

                _controlPanelEditor.SetupControlPanel();
            }
        })
        {
            text = "+ Создать новую доску"
        };
        
        // Создаем скролл для списка
        _scrollView = new ScrollView();
        
        // Добавляем элементы в контейнер
        container.Add(createButton);
        container.Add(new HelpBox("Выберите доску из списка", HelpBoxMessageType.None));
        container.Add(_scrollView);
        DrawItemsList(_scrollView);
        
        Add(container);
    }

    private void RefreshItemsList()
    {
        _scrollView.Clear();
        DrawItemsList(_scrollView);
    }

    private void DrawAssetSelection()
    {
        Debug.Log("DrawAssetSelection");
        EditorGUILayout.HelpBox("Выберите ассет", MessageType.Info);
        
        // _targetAsset = EditorGUILayout.ObjectField("Asset", _targetAsset, typeof(BehavioureTree), false) as BehavioureTree;
        
        if (GUILayout.Button("Создать новый ассет"))
        {
            string path = EditorUtility.SaveFilePanelInProject("Create AvailableItems","AvailableItems","asset","Create a new AvailableItems asset");
            
            if (!string.IsNullOrEmpty(path))
            {
                _targetAsset = EditorWindow.CreateInstance<BehavioureTree>();
                AssetDatabase.CreateAsset(_targetAsset, path);
                AssetDatabase.SaveAssets();
            }
        }
    }

    private void DrawItemsList(VisualElement container)
    {
        
        // EditorGUILayout.LabelField("Список досок:", EditorStyles.boldLabel);

        //получить список ассеты BehavioureTree        
        string[] guids = AssetDatabase.FindAssets("t:BehavioureTree");

        // _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
    
        foreach (var item in guids)
        {
            BehavioureTree asset = AssetDatabase.LoadAssetAtPath<BehavioureTree>(AssetDatabase.GUIDToAssetPath(item));

            Button newButton = new Button(); 

            newButton.text = asset.name;

            newButton.clicked += () =>
            {
                if (_selectedButton != null)
                {
                    _selectedButton.style.backgroundColor = new Color(0.345098f, 0.345098f, 0.345098f);
                }
                
                _controlPanelEditor.SelectTrget(asset);
                newButton.style.backgroundColor = new Color(0.136837f, 0.3867925f, 0.1702243f);
                _selectedButton = newButton;
            };
            
            container.Add(newButton);
        }
    }
}
