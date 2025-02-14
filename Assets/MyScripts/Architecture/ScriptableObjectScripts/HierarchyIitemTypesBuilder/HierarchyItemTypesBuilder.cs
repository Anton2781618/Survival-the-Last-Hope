using System;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

namespace ModularEventArchitecture
{

    [CreateAssetMenu(fileName = "HierarchyIitemTypesBuilder", menuName = "HierarchyIitemTypesBuilder", order = 0)]
    public class HierarchyItemTypesBuilder : ScriptableObject 
    {
        public List<string> HierarchyItemsTypes;
        public HierarchyItem[] hierarchyItems; 

        [Tools.Button("Построить иерархию")]
        public void BuildHierarchy()
        {
            HierarchyItemsTypes.Clear();
            
            foreach (var hierarchyUnit in hierarchyItems)
            {
                foreach (var item in hierarchyUnit.HierarchyItemtype)
                {
                    HierarchyItemsTypes.Add(hierarchyUnit.NameHierarchy + "/" + item);
                }
            }
        }
    }

    [Serializable]
    public class HierarchyItem
    {
        public string NameHierarchy = "";
        public string[] HierarchyItemtype;
    }
}