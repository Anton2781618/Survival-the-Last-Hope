using ModularEventArchitecture;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    [CustomPropertyDrawer(typeof(PopupAttribute))]
    public class PopupDrawer : PropertyDrawer
    {
        int selectedIndex = 0;
        private HierarchyItemTypesBuilder _asset;
        private HierarchyItemTypesBuilder Asset
        {
            get
            {
                if (_asset == null)
                {
                    string[] guids = AssetDatabase.FindAssets("t:HierarchyItemTypesBuilder");
                    if (guids.Length > 0)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                        _asset = AssetDatabase.LoadAssetAtPath<HierarchyItemTypesBuilder>(path);
                    }
                }
                return _asset;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Получаем атрибут
            PopupAttribute popupAttribute = attribute as PopupAttribute;
            
            // Отрисовываем popup с полученными опциями
            selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, Asset.HierarchyItemsTypes.ToArray());
        }
    }
}