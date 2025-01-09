using System.Collections;
using System.Collections.Generic;
using InventoryDiablo;
using ModularEventArchitecture;
using UnityEditor;
using UnityEngine;

// [CustomPropertyDrawer(typeof(InventorySlot))]
public class InventoryPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, label, true);

        if (property.isExpanded)
        {
            var target = GetTargetObjectOfProperty(property) as InventorySlot;
            if (target != null)
            {
                var methods = target.GetType().GetMethods();
                foreach (var method in methods)
                {
                    var buttonAttr = method.GetCustomAttributes(typeof(ButtonAttribute), true);
                    if (buttonAttr.Length > 0)
                    {
                        var ba = buttonAttr[0] as ButtonAttribute;
                        if (ba != null)
                        {
                            GUI.enabled = ba.mode == ButtonMode.AlwaysEnabled
                                || (EditorApplication.isPlaying ? ba.mode == ButtonMode.EnabledInPlayMode 
                                : ba.mode == ButtonMode.DisabledInPlayMode);

                            if (GUILayout.Button(ObjectNames.NicifyVariableName(method.Name)))
                            {
                                method.Invoke(target, null);
                            }

                            GUI.enabled = true;
                        }
                    }
                }
            }
        }
    }

    private static object GetTargetObjectOfProperty(SerializedProperty prop)
    {
        var path = prop.propertyPath.Replace(".Array.data[", "[");
        object obj = prop.serializedObject.targetObject;
        var elements = path.Split('.');
        
        for (int i = 0; i < elements.Length - 1; i++)
        {
            string element = elements[i];
            if (element.Contains("["))
            {
                string elementName = element.Substring(0, element.IndexOf("["));
                int index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue_Imp(obj, elementName, index);
            }
            else
            {
                obj = GetValue_Imp(obj, element);
            }
        }
        
        return obj;
    }

    private static object GetValue_Imp(object source, string name)
    {
        if (source == null)
            return null;
        var type = source.GetType();

        while (type != null)
        {
            var f = type.GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (f != null)
                return f.GetValue(source);

            var p = type.GetProperty(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
            if (p != null)
                return p.GetValue(source, null);

            type = type.BaseType;
        }
        return null;
    }

    private static object GetValue_Imp(object source, string name, int index)
    {
        var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
        if (enumerable == null)
            return null;
        var enm = enumerable.GetEnumerator();

        for (int i = 0; i <= index; i++)
        {
            if (!enm.MoveNext())
                return null;
        }
        return enm.Current;
    }
}
