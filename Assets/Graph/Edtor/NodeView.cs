using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System;

//класс для отображения ноды в редакторе
public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Action<NodeView> OnNodeSelected;
    //ссылка на ноду в скриптовом объекте
    public Node node;
    public Port inputPort;
    public Port outputPort;
    public Port ParentPort;
    public Port childPort;

    public NodeView(Node node) : base("Assets/Graph/Edtor/NodeView.uxml")
    {
        this.node = node;

        this.title = node.name;

        //установить ключ для сохранения данных
        this.viewDataKey = node.guid;

        //установить стиль
        StyleNode(node);

        CreateInputPorts();
        CreateOutputPorts();
        SetupClasses();
    }

    //добавить в классы метки стилей
    private void SetupClasses()
    {
        if(node is RootNode)
        {
            AddToClassList("root");
        }
        else
        if(node is ActionNode)
        {
            AddToClassList("ActionNode");
        }
    }

    private void CreateOutputPorts()
    {
        outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
        childPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));

        if(outputPort != null && childPort != null)
        {
            outputPort.portName = "выход";
            outputContainer.Add(outputPort);

            childPort.portName = "дочерние";
            mainContainer.Add(childPort);
        }
    }

    private void CreateInputPorts()
    {
        if(node is ActionNode)
        {
            inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));

            ParentPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(bool));
        }

        if(inputPort != null && ParentPort != null)
        {
            inputPort.portName = "вход";
            inputContainer.Add(inputPort);

            ParentPort.portName = "предки";
            titleContainer.Add(ParentPort);
        }
        
    }

    private void StyleNode(Node node)
    {
        style.left = node.position.x;

        style.top = node.position.y;
    }

    //переопределить метод установки позиции ноды
    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);

        // для отмены действия
        Undo.RecordObject(node, "Behvior Tree (Set Position)");
        
        node.position = newPos.position;
        // node.position.x = newPos.xMin;

        // node.position.y = newPos.yMin;
        
        // сохранить изменения
        EditorUtility.SetDirty(node);
    }

    // выделить ноду
    public override void OnSelected()
    {
        base.OnSelected();

        OnNodeSelected?.Invoke(this);
    }
}
