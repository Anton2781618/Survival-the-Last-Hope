using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System;
using System.Linq;
using UnityEngine;
using System.Net;

//класс для отображения дерева в редакторе 
//переопределяет методы для создания нод и контекстного меню
//подключает файл стилей
//переопределяет метод создания ноды
//
public class BehaviorTreeView : GraphView
{
    //событие выбора ноды
    public Action<NodeView> OnNodeSelected;
    public new class UxmlFactory : UxmlFactory<BehaviorTreeView, GraphView.UxmlTraits> { }
    public BehavioureTree treeModel;

    public BehaviorTreeView()
    {
        // добавить бэкграунд
        Insert(0, new GridBackground());

        // Add a minimap
        // Add(new MiniMap { anchored = true });

        // добавить группы нод
        // AddElement(new Group { title = "Group", autoUpdateGeometry = true }); 

        // добавить манипуляторы
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        
        //подключить файл стилей
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Graph/Edtor/BehavioureTreeEdtor.uss");
        styleSheets.Add(styleSheet);

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    // метод для проверки отмены действия
    private void OnUndoRedo()
    {
        PopulateView(treeModel);

        AssetDatabase.SaveAssets();
    }

    //перезаполнить дерево каждый раз когда мы его выделяем или открываем
    internal void PopulateView(BehavioureTree tree)
    {
        this.treeModel = tree;
        
        //отписаться от события изменения графа для того чтобы не вызывалось при удалении нод
        graphViewChanged -= OnGraphViewChanged;
     
        //удалить все элементы
        DeleteElements(graphElements);
        
        graphViewChanged += OnGraphViewChanged;

        //создать ноды
        tree.nodes.ForEach(node => CreateNodeView(node));
        
        //создать связи между нодами(ребра)
        tree.nodes.ForEach(node => СreateСonnections(tree, node));
        
        
        for (int i = 0; i < tree.groups.Count; i++)
        {
            Debug.Log("Группа " + i + " | " + tree.groups[i].title);
            
            // tree.groups[i].SetPosition(new Rect(tree.groups[i].contentRect.x + (i * 500), tree.groups[i].contentRect.y , tree.groups[i].contentRect.width, tree.groups[i].contentRect.height ) );
        }
    }

    // метод сохранить дерево
    private void SaveTree(BehavioureTree tree)
    {
        // сохранить изменения иначе данные удалятся после перезагрузки
        EditorUtility.SetDirty(tree);
            
        // сохранить изменения скриптбл обджектов
        AssetDatabase.SaveAssets();
    }
    
    private void СreateСonnections(BehavioureTree tree, Node node)
    {
        //получить детей ноды
        var children = tree.GetChildren(node);
        
        //пройтись по всем детям
        children.ForEach(child =>
        {
            //получить ноды для соединения ребра
            NodeView parentView = FindNodeView(node);

            NodeView childView = FindNodeView(child);


            Edge edge = parentView.outputPort.ConnectTo(childView.inputPort);

            AddElement(edge);
        });
        
        //получить детей ноды
        var derivatives = tree.GetDerivatives(node);
        
        //пройтись по всем детям
        derivatives.ForEach(child =>
        {
            //получить ноды для соединения ребра
            NodeView parentView = FindNodeView(node);

            NodeView childView = FindNodeView(child);

            Edge edge = parentView.childPort.ConnectTo(childView.ParentPort);

            AddElement(edge);
        });
    }


    //получить порт ноды по имени порта 
    private NodeView FindNodeView(Node node) => GetNodeByGuid(node.guid) as NodeView;

    //переопределить метод получения совместимых портов для того чтобы нельзя было соединять ноды с одинаковыми портами типа вход к входу
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(EndPoint => EndPoint.direction != startPort.direction && EndPoint.node != startPort.node).ToList();
    }

    //метод изменения графа для того чтобы удалять ноды из модели дерева
    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        //если есть элементы для удаления
        if(graphViewChange.elementsToRemove != null)
        {
            //пройтись по всем элементам
            graphViewChange.elementsToRemove.ForEach(element =>
            {
                //получить ноду
                NodeView nodeView = element as NodeView;
                
                //если нода не пустая
                if(nodeView != null)
                {
                    //удалить ноду из модели дерева 
                    treeModel.DeleteNode(nodeView.node);
                }

                Edge edge = element as Edge;

                if(edge != null)
                {
                    //получить ноды
                    NodeView childView = edge.input.node as NodeView;
                    NodeView parentView = edge.output.node as NodeView;

                    //если ноды не пустые
                    if(childView != null && parentView != null)
                    {
                        if(edge.input.portName == "предки" && edge.output.portName == "дочерние")
                        {
                            treeModel.RemoveChild(parentView.node, childView.node);
                        }
                        else
                        {
                            //удалить связь между нодами
                            treeModel.RemoveDependencie(parentView.node, childView.node);
                        }
                    }
                }

            });
        }

        if(graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                //получить ноды
                NodeView childView = edge.input.node as NodeView;
                NodeView parentView = edge.output.node as NodeView;
                
                //если ноды не пустые
                if(childView != null && parentView != null)
                {
                    if(edge.input.portName == "предки" && edge.output.portName == "дочерние")
                    {
                        treeModel.AddChild(parentView.node, childView.node);
                    }
                    else
                    {
                        //добавить связь между нодами
                        treeModel.AddDependencie(parentView.node, childView.node);
                    }
                }
            });
        }

        //вернуть изменения
        return graphViewChange;
    }

    //переопределить метод создания контекстного меню
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        base.BuildContextualMenu(evt);
        {
            //получить все типы наследуемые от ActionNode
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();

            foreach (var type in types)
            {
                // Добавление действия в контекстное меню
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type)); 
                
                evt.menu.AppendAction($"Создавть группу", (a) => CreteGroup("новая группа"));
            }
        }
        
    }

    public Group CreteGroup(string title)
    {
        Group group = new Group();

        treeModel.groups.Add(group);

        group.title = title;

        return group;
    }

    //создать ноду
    public Node CreateNode(System.Type type)
    {
        Node node = treeModel.CreateNode(type);

        CreateNodeView(node);

        return node;
    }
    

    //переопределить метод создания ноды
    public void CreateNodeView(Node node)
    {
        NodeView nodeView = new NodeView(node);

        nodeView.OnNodeSelected = OnNodeSelected;

        AddElement(nodeView);
        
        if(node.group != null)
        {
            node.group.AddElement(nodeView);

            AddElement(node.group);
        }
    }
}
