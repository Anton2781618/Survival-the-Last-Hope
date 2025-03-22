using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyEditor
{
    //класс для сохранения данных о дереве в скриптовый объект
    [CreateAssetMenu()]
    public class BehavioureTree : ScriptableObject
    {
        public Node rootNode;
        public List<Node> nodes = new List<Node>();

    #if UNITY_EDITOR
        public List<UnityEditor.Experimental.GraphView.Group> groups = new List<UnityEditor.Experimental.GraphView.Group>();
    #endif

        public void SaveAssets()
        {
            // Undo.RegisterCreatedObjectUndo(node, "Behavior Tree (CreateNode)");
            #if UNITY_EDITOR
            // сохранить изменения
            AssetDatabase.SaveAssets();
            #endif
        }
        
        
        //записать данные о созданной ноде в скриптовый объект
        public Node CreateNode(System.Type type)
        {
            //создать ноду
            Node node = ScriptableObject.CreateInstance(type) as Node;
            
            //присвоить ей имя
            node.name = type.Name;
            
            #if UNITY_EDITOR
            // присвоить ей уникальный идентификатор
            node.guid = GUID.Generate().ToString();
            // Undo.RecordObject(this, "Behavior Tree (CreateNode)");
            
            // добавить ее в список нод
            nodes.Add(node);

            // добавить ее в корень скриптового объекта
            AssetDatabase.AddObjectToAsset(node, this);
            #endif

            // Undo.RegisterCreatedObjectUndo(node, "Behavior Tree (CreateNode)");
            // AssetDatabase.SaveAssets();

            return node;
        }

        // удалить ноду из скриптового объекта
        public void DeleteNode(Node node)
        {
            #if UNITY_EDITOR
            Undo.RecordObject(this, "Behavior Tree (DeleteNode)");

            // удалить ноду из списка нод
            nodes.Remove(node);

            // удалить ноду из корня скриптового объекта
            AssetDatabase.RemoveObjectFromAsset(node);

            Undo.DestroyObjectImmediate(node);
            
            // сохранить изменения
            AssetDatabase.SaveAssets();
            #endif
        }

        //добавить дочернюю ноду
        public void AddChild(Node parent, Node child)
        {
            #if UNITY_EDITOR
            Undo.RecordObject(parent, "Behavior Tree (AddChild)");
            #endif
            parent.childrens.Add(child);

            child.parents.Add(parent);
        }

        //добавить зависимость между нодами
        public void AddDependencie(Node parent, Node child)
        {
            #if UNITY_EDITOR
            Undo.RecordObject(parent, "Behavior Tree (AddChild)");
            #endif

            parent.childrenDependencies.Add(child);

            child.parentsDependencies.Add(parent);
        }

        public void RemoveChild(Node parent, Node child)
        {
            #if UNITY_EDITOR
            Undo.RecordObject(parent, "Behavior Tree (RemoveChild)");
            #endif

            parent.childrens.Remove(child);

            child.parents.Remove(parent);
        }
        public void RemoveDependencie(Node parent, Node child)
        {
            #if UNITY_EDITOR
            Undo.RecordObject(parent, "Behavior Tree (RemoveChild)");
            #endif
            parent.childrenDependencies.Remove(child);

            child.parentsDependencies.Remove(parent);
        }

        public List<Node> GetChildren(Node parent) => parent.childrenDependencies;
        public List<Node> GetDerivatives(Node parent) => parent.childrens;

    }
}