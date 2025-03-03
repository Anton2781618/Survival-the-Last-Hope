using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ModularEventArchitecture
{
    [CreateAssetMenu(fileName = "ButtonsEditors", menuName = "Inventory/Buttons Editors")]
    public class ButtonsEditors : ScriptableObject
    {
        [Tools.Button("Редактор Инвентарей")]
        public void ShowInventoryEditor()
        {
            EditorWindow.GetWindow<InventoryEditor>("Inventory Editor");
        }
        [Tools.Button("Редактор Предметов")]
        public void ShowAvailableItemsEditor()
        {
            EditorWindow.GetWindow<ItemsEditor>("Available Items Editor");
        }
    }
}
