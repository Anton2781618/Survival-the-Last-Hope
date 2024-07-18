using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModestTree;
using MyProject;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    public BehavioureTree tree;

    [ContextMenu("Refletion")]
    private void Refletion()
    {
        // запросить текущую сборку без указания типа
        Assembly assembly = Assembly.GetExecutingAssembly();
        
        tree.rootNode.nodeName = assembly.GetName().ToString(); 

        tree.rootNode.name = tree.rootNode.nodeName;

        // List<Type> classes = assembly.GetTypes().Where(a => a.Namespace == "MyProject").ToList();
        Type[] classes = assembly.GetTypes();

        List<Node> nodes = new List<Node>();

        foreach (var item in classes)
        {
            Debug.Log(item);
            Node node = tree.CreateNode(typeof(LogNode));
            
            node.name = item.Name.ToString(); 

            node.nodeName = item.FullName.ToString(); 
            
            node.nodeType = item; 

            nodes.Add(node);
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            var members = classes[i].GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            foreach (FieldInfo  property in members)
            {
                for (int i1 = 0; i1 < nodes.Count; i1++)
                {
                    Debug.Log(property.FieldType.ToString() == nodes[i1].nodeName);

                    if (property.FieldType.ToString() == nodes[i1].nodeName)
                    {
                        tree.AddDependencie(nodes[i], nodes[i1]);
                    }
                }
            }
            
        }

        SetPositin();
    }

    [ContextMenu("SetPos")]
    private void SetPositin()
    {
        for (int i = 0; i < tree.nodes.Count; i++)
        {
            Node node = tree.nodes[i];
            
            // если у ноды есть родители и нет детей
            if(node.parentsDependencies.Count > 0 && node.childrenDependencies.Count == 0)
            {
                if(node.parentsDependencies.Count > 0 && node.parentsDependencies.Count < 2)
                {
                    node.position = new Vector2(1000, node.parentsDependencies[0].position.y);
                }
                else
                {
                    node.position = new Vector2(1000, i * 50);
                }
            }
            else
            // если у ноды есть родители и есть дети
            if(node.parentsDependencies.Count > 0 && node.childrenDependencies.Count > 0)
            {
                if(node.parentsDependencies.Count > 0 && node.parentsDependencies.Count < 2)
                {
                    node.position = new Vector2(500, node.parentsDependencies[0].position.y);
                }
                else
                {
                    node.position = new Vector2(500, i * 50);
                }

            }
            else
            {
                node.position = new Vector2(0, i * 50);
            }
        }
    }

    [ContextMenu("найти все классы defenitions")]
    private void FindClassesDefenitions()
    {
        // UnityEditor.Compilation.Assembly[] playerAssemblies = UnityEditor.Compilation.CompilationPipeline.GetAssemblies(UnityEditor.Compilation.AssembliesType.Player);
        // UnityEditor.Compilation.Assembly[] playerAssemblies = UnityEditor.Compilation.CompilationPipeline.GetPrecompiledAssemblyPaths(UnityEditor.Compilation.CompilationPipeline.PrecompiledAssemblySources.UserAssembly);
        string[] playerAssemblies = UnityEditor.Compilation.CompilationPipeline.GetPrecompiledAssemblyPaths(UnityEditor.Compilation.CompilationPipeline.PrecompiledAssemblySources.UserAssembly);

        foreach (var assembly in playerAssemblies)
        {
                // Debug.Log(assembly.name);
            foreach (var item in assembly)
            {
                Debug.Log(item);
                
            }
        }
    }

    [ContextMenu("найти все классы")]
    private void FindClasses()
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        Assembly assemblie = AppDomain.CurrentDomain.Load("_Inventory");
        
        foreach (var item in assemblie.GetTypes())
        {
            Debug.Log(item);
        }
        
        // Type[] classes = assembly.GetTypes();

        // foreach (var item in classes)
        // {
        //     Debug.Log(item);
        // }
        
        // string[] assetPaths;
        // Найти все скрипты, исключая библиотек dll, в проекте в папке Assets
        // assetPaths = AssetDatabase.FindAssets("t:Script", new[] {"Assets/"});

        // foreach (var item in assetPaths)
        // {
        //     Debug.Log(item.);
        // }
    }
}
