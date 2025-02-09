using UnityEditor;
using UnityEngine;

public class MultiSelectExample : EditorWindow
{
    private string[] options = { "Option 1", "Option 2", "Option 3", "Option 4" };
    private bool[] selectedOptions;

    [MenuItem("Window/MultiSelect Example")]
    public static void ShowWindow()
    {
        GetWindow<MultiSelectExample>("MultiSelect Example");
    }

    private void OnEnable()
    {
        selectedOptions = new bool[options.Length]; // Инициализация массива для хранения состояния выбора
    }

    private void OnGUI()
    {
        GUILayout.Label("Select Multiple Options", EditorStyles.boldLabel);

        for (int i = 0; i < options.Length; i++)
        {
            selectedOptions[i] = EditorGUILayout.Toggle(options[i], selectedOptions[i]);
        }

        if (GUILayout.Button("Submit"))
        {
            // Обработка выбранных опций
            for (int i = 0; i < options.Length; i++)
            {
                if (selectedOptions[i])
                {
                    Debug.Log("Selected: " + options[i]);
                }
            }
        }
    }
}