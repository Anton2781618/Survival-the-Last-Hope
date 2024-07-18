using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;
using ModestTree;
using System.Reflection;
using System;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using System.Collections.Generic;

public class BehavioureTreeEdtor : EditorWindow
{
    BehaviorTreeView treeView;
    InspectorView inpectorView;

    [MenuItem("Window/BehavioureTreeEdtor/Editor...")]
    public static void OpenWindow()
    {
        BehavioureTreeEdtor wnd = GetWindow<BehavioureTreeEdtor>();
        wnd.titleContent = new GUIContent("BehavioureTreeEdtor");
    }

    // метод открывает редактор по двойному щелчку на BehavioureTree
    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        if(Selection.activeObject is BehavioureTree)
        {
            OpenWindow();
            return true;
        }

        return false;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Graph/Edtor/BehavioureTreeEdtor.uxml");
        visualTree.CloneTree(root);
    
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Graph/Edtor/BehavioureTreeEdtor.uss");
        root.styleSheets.Add(styleSheet);
        
        // получить ссылку на дерево         
        treeView = root.Q<BehaviorTreeView>();

        inpectorView = root.Q<InspectorView>();

        Button populateBUtton = root.Q<Button>("populateBUtton");

        populateBUtton.clicked += populateBUttonClick;

        treeView.OnNodeSelected = OnNodeSelectionChanged;

        //перезаполнить дерево
        OnSelectionChange();
    }

    private void populateBUttonClick()
    {
        // Assembly[] assemblie = AppDomain.CurrentDomain.GetAssemblies().Where(s => s.GetName().Name.StartsWith("_")).ToArray();
        Assembly[] assemblie = AppDomain.CurrentDomain.GetAssemblies().Where(s => s.GetName().Name == "Assembly-CSharp" || s.GetName().Name.StartsWith("_")).ToArray();


        // foreach (var item in assemblie)
        // {
        //     foreach (var Mytype in item.GetTypes())
        //     {
        //         Debug.Log(Mytype.Assembly + " | " +  Mytype.FullName); 
                
        //     }
        // }


        // return;

        List<Type> allclassesX = new List<Type>();
        List<Type> allclassesY = new List<Type>();

        List<Node> nodes = new List<Node>();
        for (int i1 = 0; i1 < assemblie.Length; i1++)
        {
            Assembly item = assemblie[i1];

            // Type[] classesArray = item.GetTypes().Where(s => s.Namespace == "MyProject").ToArray();
            Type[] classesArray = item.GetTypes();
            
            allclassesX.AddRange(classesArray);
            allclassesY.AddRange(classesArray);
            
            Group group = treeView.CreteGroup($"{item.GetName().Name}");

            group.SetPosition(new Rect(group.contentRect.x + (i1 * 400), group.contentRect.y , group.contentRect.width, group.contentRect.height ));

            
            for (int i = 0; i < classesArray.Length; i++)
            {
                Node node = treeView.CreateNode(typeof(LogNode));

                node.group = group;

                node.name = classesArray[i].Name;

                node.nodeName = classesArray[i].FullName.ToString(); 

                node.position = new Vector2(0, i * 120); 

                nodes.Add(node);
            }

        }

            for (int i = 0; i < nodes.Count; i++)
            {
                var fields = allclassesX[i].GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

                foreach (FieldInfo field in fields)
                {
                    for (int ii = 0; ii < nodes.Count; ii++)
                    {
                        if (field.FieldType.ToString() == nodes[ii].nodeName) 
                        {
                            treeView.treeModel.AddDependencie(nodes[i], nodes[ii]);
                        }
                    }
                }
                
            }
        


        OnSelectionChange();
    }

    //перезаполнить дерево каждый раз когда мы его выделяем или открываем
    private void OnSelectionChange()
    {
        BehavioureTree tree = Selection.activeObject as BehavioureTree;

        if(!tree)
        {
            if(Selection.activeGameObject)
            {
                TreeController treeRunner = Selection.activeGameObject.GetComponent<TreeController>();

                if(treeRunner)
                {
                    tree = treeRunner.tree;
                }
            }
        }

        if(Application.isPlaying)
        {
            if(tree)
            {
                treeView.PopulateView(tree);
            }
        }
        else
        {
            if(tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
            {
                treeView.PopulateView(tree);
            }
        }
    }

    //заполнить инспектор данными
    private void OnNodeSelectionChanged(NodeView node) => inpectorView.UpdateSelection(node);

}
