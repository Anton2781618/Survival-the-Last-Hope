using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

//класс для сохранения данных о дереве в скриптовый объект

[CreateAssetMenu()]
public class BehavioureTree : ScriptableObject
{
    public Node rootNode;
    public List<Node> nodes = new List<Node>();
    public List<Group> groups = new List<Group>();

    public void SaveTree()
    {
        // Undo.RegisterCreatedObjectUndo(node, "Behavior Tree (CreateNode)");
        
        // сохранить изменения
        AssetDatabase.SaveAssets();
    }
    
    
    //записать данные о созданной ноде в скриптовый объект
    public Node CreateNode(System.Type type)
    {
        //создать ноду
        Node node = ScriptableObject.CreateInstance(type) as Node;
        
        //присвоить ей имя
        node.name = type.Name;
        
        // присвоить ей уникальный идентификатор
        node.guid = GUID.Generate().ToString();

        // Undo.RecordObject(this, "Behavior Tree (CreateNode)");
        
        // добавить ее в список нод
        nodes.Add(node);

        // добавить ее в корень скриптового объекта
        AssetDatabase.AddObjectToAsset(node, this);

        // Undo.RegisterCreatedObjectUndo(node, "Behavior Tree (CreateNode)");
        
        // сохранить изменения
        // AssetDatabase.SaveAssets();

        return node;
    }
    

    // удалить ноду из скриптового объекта
    public void DeleteNode(Node node)
    {
        Undo.RecordObject(this, "Behavior Tree (DeleteNode)");

        // удалить ноду из списка нод
        nodes.Remove(node);

        // удалить ноду из корня скриптового объекта
        // AssetDatabase.RemoveObjectFromAsset(node);

        Undo.DestroyObjectImmediate(node);
        
        // сохранить изменения
        AssetDatabase.SaveAssets();
    }

    public void AddChild(Node parent, Node child)
    {
        Undo.RecordObject(parent, "Behavior Tree (AddChild)");

        parent.childrens.Add(child);

        child.parents.Add(parent);
    }
    public void AddDependencie(Node parent, Node child)
    {
        Undo.RecordObject(parent, "Behavior Tree (AddChild)");

        parent.childrenDependencies.Add(child);

        child.parentsDependencies.Add(parent);
    }

    public void RemoveChild(Node parent, Node child)
    {
        Undo.RecordObject(parent, "Behavior Tree (RemoveChild)");

        parent.childrens.Remove(child);

        child.parents.Remove(parent);
    }
    public void RemoveDependencie(Node parent, Node child)
    {
        Undo.RecordObject(parent, "Behavior Tree (RemoveChild)");

        parent.childrenDependencies.Remove(child);

        child.parentsDependencies.Remove(parent);
    }

    public List<Node> GetChildren(Node parent) => parent.childrenDependencies;
    public List<Node> GetDerivatives(Node parent) => parent.childrens;

}
