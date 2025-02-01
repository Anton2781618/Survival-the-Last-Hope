using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

//класс для отображения ноды в редакторе
public class WindowNodeView : UnityEditor.Experimental.GraphView.Node
{
    public Action<WindowNodeView> OnNodeSelected;
    //ссылка на ноду в скриптовом объекте
    public Node node;

    // public WindowNodeView(Node node) : base("Assets/MyScripts/Architecture/Editor/ControlPanel/Node/WindowNodeView.uxml")
    public WindowNodeView(Node node) : base(Path.Combine(GetScriptPath(), "WindowNodeView.uxml"))
    {
        this.node = node;

        this.title = node.name;

        //установить ключ для сохранения данных
        this.viewDataKey = node.guid;

        //установить стиль
        SetupStyleNode(node);

    }

    private static string GetScriptPath()
    {
        string scriptGUID = AssetDatabase.FindAssets($"t:Script {nameof(WindowNodeView)}")[0];
        string scriptPath = AssetDatabase.GUIDToAssetPath(scriptGUID);
        string directoryPath = Path.GetDirectoryName(scriptPath);
        return directoryPath;
    }

    private void SetupStyleNode(Node node)
    {
        style.left = node.position.x;

        style.top = node.position.y;
    }
}
