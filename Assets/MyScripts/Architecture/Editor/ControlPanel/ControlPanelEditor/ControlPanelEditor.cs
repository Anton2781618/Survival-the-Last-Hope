using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ControlPanelEditor : EditorWindow
{
    private MainScreenView treeView;

    [MenuItem("Tools/Панель управления")]
    public static void OpenWindow()
    {
        ControlPanelEditor wnd = GetWindow<ControlPanelEditor>();
        wnd.titleContent = new GUIContent("ControlPanelEditor");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/MyScripts/Architecture/Editor/ControlPanel/ControlPanelEditor/DesignMainScreen.uxml");
        visualTree.CloneTree(root);

        // получить ссылку на дерево         
        treeView = root.Q<MainScreenView>();
        
        Debug.Log(treeView);

        // treeView.OnNodeSelected = OnNodeSelectionChanged;
        Button populateBUtton = root.Q<Button>("populateBUtton");
        populateBUtton.clicked += populateBUttonClick;

    }

    public void OnNodeSelectionChanged(NodeView nodeView)
    {
    }

    private void populateBUttonClick()
    {
        Debug.Log("!!!!");
        treeView.CreateNode(typeof(WnidowNode));
    }
}