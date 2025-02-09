using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ControlPanelEditor : EditorWindow
{
    private MainScreenView _treeView;
    private ControlPanelView _сontrolPanelView;
    private BehavioureTree _target;

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
        _treeView = root.Q<MainScreenView>();

        _сontrolPanelView = root.Q<ControlPanelView>();

        // treeView.OnNodeSelected = OnNodeSelectionChanged;
        Button populateBUtton = root.Q<Button>("populateBUtton");
        populateBUtton.clicked += populateBUttonClick;

        SetupControlPanel();

        
        OnSelectionChange();
    }

    public void SetupControlPanel() => _сontrolPanelView.Setup(this);
    

    public void OnNodeSelectionChanged(NodeView nodeView)
    {
    }

    public void SelectTrget(BehavioureTree target)
    {
        _target = target;
        OnSelectionChange();
    }

    private void OnSelectionChange()
    {
        if(Application.isPlaying)
        {
            if(_target)
            {
                _treeView.PopulateView(_target);
            }
        }
        else
        {
            if(_target && AssetDatabase.CanOpenAssetInEditor(_target.GetInstanceID()))
            {
                _treeView.PopulateView(_target);
            }
        }
    }

    private void populateBUttonClick()
    {
        SetupControlPanel();
    }
}