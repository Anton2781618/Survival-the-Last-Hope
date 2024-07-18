using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;

//класс для отображения ноды в редакторе
public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }
    private Editor editor;

    public InspectorView()
    {

    }

    internal void UpdateSelection(NodeView nodeView)
    {
        //удалить все предыдущие выделения которые у нас были
        Clear();
        
        // уничтоить предыдущий инспектор
        UnityEngine.Object.DestroyImmediate(editor);

        //что то типа скриптового объекта
        editor = Editor.CreateEditor(nodeView.node);

        //создать контейнер для отрисовки инспектора
        IMGUIContainer container = new IMGUIContainer(() =>
        {
            //отрисовать инспектор
            if(editor.target) editor.OnInspectorGUI();
        });

        // добавить контейнер в инспектор
        Add(container);
    }
}
