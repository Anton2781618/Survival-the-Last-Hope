using InventoryDiablo;
using ModularEventArchitecture;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Inventory))]
public class InventoryPropertyDrawer : PropertyDrawer
{
    public GUISkin customSkin;
    private void LoadSkin()
    {
        
        if (customSkin == null)
        {
            customSkin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/MyScripts/Architecture/Editor/InventoryEditor/GUISkin.guiskin");
        }
        
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        LoadSkin();
        
        EditorGUILayout.BeginVertical("Box");

        if (GUILayout.Button("Редактировать инвентарь", GUILayout.Width(170), GUILayout.Height(40)))
        {
            OpenEditor(property);
        }

        EditorGUILayout.EndVertical();
        
    }

    private void OpenEditor(SerializedProperty property)
    {
        object inventory = property.serializedObject.targetObject;
            
            InventoryModule inventory1 = inventory as InventoryModule;
            
            var editor = EditorWindow.GetWindow<InventoryEditor>();
            
            editor.SetTartget(inventory1.Inventory);

            editor.Show();
    }
}
