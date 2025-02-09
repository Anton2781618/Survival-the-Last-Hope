using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

//класс для отображения ноды в скриптовом объекте
public abstract class Node : ScriptableObject
{
    public string NodeName;
    public Type nodeType;
    public string guid;
    public Vector2 Position;
    public Color ColorNode = new Color(0.2627451f , 0.2627451f, 0.2627451f);

    public Group group;
    public List<Node> parentsDependencies = new List<Node>();
    public List<Node> childrenDependencies = new List<Node>();
    public List<Node> parents = new List<Node>();
    public List<Node> childrens = new List<Node>();

}
