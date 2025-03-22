using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class UMLClassDiagramWindow : EditorWindow
{
    private MonoScript targetScript;
    private Type rootType;
    private Vector2 scrollPosition;
    private Dictionary<Type, Rect> classRects = new Dictionary<Type, Rect>();
    private Dictionary<Type, bool> expandedClasses = new Dictionary<Type, bool>();
    private Dictionary<Type, Color> classColors = new Dictionary<Type, Color>();
    private float zoom = 1.0f;
    private Vector2 panOffset = Vector2.zero;
    private bool isDragging = false;
    private Vector2 lastMousePosition;
    
    // Настройки отображения
    private float classBoxWidth = 250f;
    private float headerHeight = 30f;
    private float memberHeight = 20f;
    private float relationshipArrowSize = 10f;
    private float horizontalSpacing = 50f;
    private float verticalSpacing = 100f;
    
    // Стили GUI
    private GUIStyle classHeaderStyle;
    private GUIStyle classMemberStyle;
    private GUIStyle classBoxStyle;
    private GUIStyle interfaceBoxStyle;
    private GUIStyle abstractClassBoxStyle;
    
    [MenuItem("Window/UML Class Diagram")]
    public static void ShowWindow()
    {
        GetWindow<UMLClassDiagramWindow>("UML Class Diagram");
    }
    
    private void OnEnable()
    {
        InitializeStyles();
    }
    
    private void InitializeStyles()
    {
        classHeaderStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 12
        };
        
        classMemberStyle = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleLeft,
            fontSize = 11,
            richText = true
        };
        
        classBoxStyle = new GUIStyle(EditorStyles.helpBox)
        {
            padding = new RectOffset(10, 10, 10, 10)
        };
        
        interfaceBoxStyle = new GUIStyle(classBoxStyle);
        abstractClassBoxStyle = new GUIStyle(classBoxStyle);
    }
    
    private void OnGUI()
    {
        DrawToolbar();
        
        if (rootType == null)
        {
            EditorGUILayout.HelpBox("Выберите класс для отображения UML-диаграммы", MessageType.Info);
            return;
        }
        
        // Обработка ввода для панорамирования и масштабирования
        HandleInput();
        
        // Зона для рисования диаграммы
        Rect diagramRect = new Rect(0, EditorGUIUtility.singleLineHeight * 2, position.width, position.height - EditorGUIUtility.singleLineHeight * 2);
        
        // Начинаем скролл-область
        scrollPosition = GUI.BeginScrollView(
            diagramRect, 
            scrollPosition, 
            new Rect(0, 0, position.width * 3, position.height * 3)
        );
        
        // Создаем матрицу для масштабирования и панорамирования
        Matrix4x4 originalMatrix = GUI.matrix;
        GUI.matrix = Matrix4x4.TRS(
            panOffset, 
            Quaternion.identity, 
            new Vector3(zoom, zoom, 1)
        );
        
        // Рисуем UML-диаграмму
        DrawUMLDiagram(rootType);
        
        // Восстанавливаем исходную матрицу
        GUI.matrix = originalMatrix;
        
        // Завершаем скролл-область
        GUI.EndScrollView();
    }
    
    private void HandleInput()
    {
        // Обработка нажатия средней кнопки мыши для панорамирования
        if (Event.current.type == EventType.MouseDown && Event.current.button == 2)
        {
            isDragging = true;
            lastMousePosition = Event.current.mousePosition;
            Event.current.Use();
        }
        else if (Event.current.type == EventType.MouseUp && Event.current.button == 2)
        {
            isDragging = false;
            Event.current.Use();
        }
        else if (Event.current.type == EventType.MouseDrag && isDragging)
        {
            panOffset += (Event.current.mousePosition - lastMousePosition);
            lastMousePosition = Event.current.mousePosition;
            Repaint();
            Event.current.Use();
        }
        
        // Обработка колеса мыши для масштабирования
        if (Event.current.type == EventType.ScrollWheel)
        {
            float zoomDelta = -Event.current.delta.y * 0.01f;
            zoom = Mathf.Clamp(zoom + zoomDelta, 0.5f, 2.0f);
            Repaint();
            Event.current.Use();
        }
    }
    
    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        
        // Поле выбора скрипта
        EditorGUI.BeginChangeCheck();
        targetScript = EditorGUILayout.ObjectField("Target Script", targetScript, typeof(MonoScript), false) as MonoScript;
        if (EditorGUI.EndChangeCheck() && targetScript != null)
        {
            rootType = targetScript.GetClass();
            classRects.Clear();
            expandedClasses.Clear();
            classColors.Clear();
        }
        
        if (GUILayout.Button("Reset View", EditorStyles.toolbarButton, GUILayout.Width(80)))
        {
            zoom = 1.0f;
            panOffset = Vector2.zero;
            scrollPosition = Vector2.zero;
        }
        
        if (GUILayout.Button("Export PNG", EditorStyles.toolbarButton, GUILayout.Width(80)))
        {
            ExportDiagramAsPNG();
        }
        
        EditorGUILayout.EndHorizontal();
        
        // Дополнительные настройки
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        zoom = EditorGUILayout.Slider("Zoom", zoom, 0.5f, 2.0f, GUILayout.Width(200));
        classBoxWidth = EditorGUILayout.FloatField("Class Width", classBoxWidth, GUILayout.Width(150));
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawUMLDiagram(Type rootType)
    {
        if (rootType == null) return;
        
        // Сбрасываем данные о расположении классов
        if (classRects.Count == 0)
        {
            LayoutClasses(rootType, 50, 50);
        }
        
        // Рисуем отношения между классами
        DrawRelationships();
        
        // Рисуем классы
        foreach (var entry in classRects)
        {
            Type classType = entry.Key;
            Rect classRect = entry.Value;
            
            // Выбор цвета для класса
            Color classColor = GetClassColor(classType);
            
            // Рисуем бокс класса
            DrawClassBox(classType, classRect, classColor);
        }
    }
    
    private Color GetClassColor(Type type)
    {
        if (!classColors.ContainsKey(type))
        {
            if (type.IsInterface)
            {
                classColors[type] = new Color(0.5f, 0.8f, 1.0f);
            }
            else if (type.IsAbstract)
            {
                classColors[type] = new Color(1.0f, 0.9f, 0.7f);
            }
            else
            {
                classColors[type] = new Color(0.9f, 0.9f, 0.9f);
            }
        }
        return classColors[type];
    }
    
    private void DrawClassBox(Type classType, Rect rect, Color color)
    {
        bool isExpanded = !expandedClasses.ContainsKey(classType) || expandedClasses[classType];
        
        // Определяем высоту блока на основе количества членов
        float boxHeight = CalculateClassBoxHeight(classType, isExpanded);
        
        // Рисуем фон класса
        EditorGUI.DrawRect(rect, color);
        
        // Заголовок класса
        string classPrefix = classType.IsInterface ? "«interface»" : (classType.IsAbstract ? "«abstract»" : "");
        string className = classType.Name;
        if (!string.IsNullOrEmpty(classPrefix))
        {
            className = $"{classPrefix}\n{className}";
        }
        
        Rect headerRect = new Rect(rect.x, rect.y, rect.width, headerHeight * (classPrefix != "" ? 2 : 1));
        GUI.Box(headerRect, "", classBoxStyle);
        EditorGUI.LabelField(headerRect, className, classHeaderStyle);
        
        // Разделительная линия
        Rect lineRect = new Rect(rect.x, rect.y + headerRect.height, rect.width, 1);
        EditorGUI.DrawRect(lineRect, Color.gray);
        
        if (isExpanded)
        {
            // Свойства и поля
            DrawClassMembers(classType, rect, headerRect.height);
        }
        
        // Обработка клика по заголовку для сворачивания/разворачивания
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && headerRect.Contains(Event.current.mousePosition / zoom - panOffset / zoom))
        {
            expandedClasses[classType] = !isExpanded;
            Event.current.Use();
            Repaint();
        }
    }
    
    private void DrawClassMembers(Type classType, Rect classRect, float startY)
    {
        float currentY = startY;
        
        // Перечисляем публичные поля и свойства
        var members = GetClassMembers(classType);
        
        foreach (var member in members)
        {
            string memberType = "";
            string memberName = "";
            string accessModifier = "";
            
            if (member is FieldInfo field)
            {
                accessModifier = GetAccessModifierSymbol(field);
                memberType = GetTypeNameForDisplay(field.FieldType);
                memberName = field.Name;
            }
            else if (member is PropertyInfo property)
            {
                accessModifier = GetAccessModifierSymbol(property);
                memberType = GetTypeNameForDisplay(property.PropertyType);
                memberName = property.Name;
            }
            else if (member is MethodInfo method)
            {
                if (method.IsSpecialName) continue; // Пропускаем авто-сгенерированные методы
                accessModifier = GetAccessModifierSymbol(method);
                memberType = GetTypeNameForDisplay(method.ReturnType);
                memberName = method.Name + "()";
            }
            
            Rect memberRect = new Rect(classRect.x, classRect.y + currentY, classRect.width, memberHeight);
            string memberText = $"{accessModifier} {memberName}: <color=#666666>{memberType}</color>";
            
            EditorGUI.LabelField(memberRect, memberText, classMemberStyle);
            currentY += memberHeight;
        }
    }
    
    private string GetAccessModifierSymbol(MemberInfo member)
    {
        if (member is FieldInfo field)
        {
            if (field.IsPublic) return "+";
            if (field.IsPrivate) return "-";
            if (field.IsFamily) return "#"; // protected
            return "~"; // internal/package
        }
        else if (member is PropertyInfo property)
        {
            var getMethod = property.GetGetMethod(true);
            if (getMethod != null)
            {
                if (getMethod.IsPublic) return "+";
                if (getMethod.IsPrivate) return "-";
                if (getMethod.IsFamily) return "#";
                return "~";
            }
            return "-";
        }
        else if (member is MethodInfo method)
        {
            if (method.IsPublic) return "+";
            if (method.IsPrivate) return "-";
            if (method.IsFamily) return "#";
            return "~";
        }
        
        return "";
    }
    
    private string GetTypeNameForDisplay(Type type)
    {
        if (type == typeof(void)) return "void";
        if (type == typeof(int)) return "int";
        if (type == typeof(float)) return "float";
        if (type == typeof(double)) return "double";
        if (type == typeof(string)) return "string";
        if (type == typeof(bool)) return "bool";
        
        if (type.IsGenericType)
        {
            var genericArgs = type.GetGenericArguments();
            string baseType = type.Name.Split('`')[0];
            string args = string.Join(", ", genericArgs.Select(t => GetTypeNameForDisplay(t)));
            return $"{baseType}<{args}>";
        }
        
        return type.Name;
    }
    
    private MemberInfo[] GetClassMembers(Type classType)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        
        List<MemberInfo> members = new List<MemberInfo>();
        
        // Добавляем поля
        members.AddRange(classType.GetFields(flags));
        
        // Добавляем свойства
        members.AddRange(classType.GetProperties(flags));
        
        // Добавляем методы (исключая конструкторы и специальные методы)
        members.AddRange(classType.GetMethods(flags).Where(m => !m.IsSpecialName));
        
        return members.ToArray();
    }
    
    private void DrawRelationships()
    {
        // Рисуем наследование
        foreach (var entry in classRects)
        {
            Type childType = entry.Key;
            Rect childRect = entry.Value;
            
            // Наследование (от базового класса)
            Type baseType = childType.BaseType;
            if (baseType != null && baseType != typeof(object) && classRects.ContainsKey(baseType))
            {
                Rect parentRect = classRects[baseType];
                DrawInheritanceRelationship(childRect, parentRect);
            }
            
            // Реализация интерфейса
            foreach (Type interfaceType in childType.GetInterfaces())
            {
                if (classRects.ContainsKey(interfaceType))
                {
                    Rect interfaceRect = classRects[interfaceType];
                    DrawImplementationRelationship(childRect, interfaceRect);
                }
            }
            
            // Ассоциации (поля и свойства, указывающие на другие классы)
            DrawAssociationRelationships(childType, childRect);
        }
    }
    
    private void DrawInheritanceRelationship(Rect childRect, Rect parentRect)
    {
        Vector2 start = new Vector2(childRect.x + childRect.width / 2, childRect.y);
        Vector2 end = new Vector2(parentRect.x + parentRect.width / 2, parentRect.y + parentRect.height);
        DrawArrow(start, end, Color.black, true);
    }
    
    private void DrawImplementationRelationship(Rect childRect, Rect interfaceRect)
    {
        Vector2 start = new Vector2(childRect.x + childRect.width / 2, childRect.y);
        Vector2 end = new Vector2(interfaceRect.x + interfaceRect.width / 2, interfaceRect.y + interfaceRect.height);
        DrawArrow(start, end, Color.black, true, true);
    }
    
    private void DrawAssociationRelationships(Type classType, Rect classRect)
    {
        // Получаем все поля и свойства
        var fields = classType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var properties = classType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        
        // Проверяем каждое поле на ассоциацию с другими классами
        foreach (var field in fields)
        {
            Type fieldType = field.FieldType;
            // Если это коллекция, получаем тип элементов
            if (fieldType.IsGenericType && (
                fieldType.GetGenericTypeDefinition() == typeof(List<>) ||
                fieldType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                fieldType.GetGenericTypeDefinition() == typeof(ICollection<>)))
            {
                fieldType = fieldType.GetGenericArguments()[0];
            }
            
            // Проверяем, есть ли указанный тип на диаграмме
            if (classRects.ContainsKey(fieldType) && fieldType != classType)
            {
                Rect targetRect = classRects[fieldType];
                DrawAssociationRelationship(classRect, targetRect, field.Name);
            }
        }
        
        // Аналогично для свойств
        foreach (var property in properties)
        {
            Type propertyType = property.PropertyType;
            if (propertyType.IsGenericType && (
                propertyType.GetGenericTypeDefinition() == typeof(List<>) ||
                propertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                propertyType.GetGenericTypeDefinition() == typeof(ICollection<>)))
            {
                propertyType = propertyType.GetGenericArguments()[0];
            }
            
            if (classRects.ContainsKey(propertyType) && propertyType != classType)
            {
                Rect targetRect = classRects[propertyType];
                DrawAssociationRelationship(classRect, targetRect, property.Name);
            }
        }
    }
    
    private void DrawAssociationRelationship(Rect sourceRect, Rect targetRect, string label)
    {
        Vector2 start = new Vector2(sourceRect.x + sourceRect.width, sourceRect.y + sourceRect.height / 2);
        Vector2 end = new Vector2(targetRect.x, targetRect.y + targetRect.height / 2);
        DrawArrow(start, end, Color.blue, false);
        
        // Показываем имя поля/свойства
        Vector2 labelPos = Vector2.Lerp(start, end, 0.5f);
        Rect labelRect = new Rect(labelPos.x - 50, labelPos.y - 10, 100, 20);
        GUI.Label(labelRect, label, classMemberStyle);
    }
    
    private void DrawArrow(Vector2 start, Vector2 end, Color color, bool isInheritance = false, bool isDashed = false)
    {
        Handles.color = color;
        
        // Рисуем линию
        if (isDashed)
        {
            Handles.DrawDottedLine(start, end, 4f);
        }
        else
        {
            Handles.DrawLine(start, end);
        }
        
        // Рисуем стрелку
        if (isInheritance)
        {
            // Для наследования рисуем треугольную стрелку
            Vector2 direction = (start - end).normalized;
            Vector2 perpendicular = new Vector2(-direction.y, direction.x) * relationshipArrowSize;
            
            Vector3[] trianglePoints = new Vector3[3];
            trianglePoints[0] = end;
            trianglePoints[1] = end + direction * relationshipArrowSize + perpendicular;
            trianglePoints[2] = end + direction * relationshipArrowSize - perpendicular;
            
            Handles.DrawAAConvexPolygon(trianglePoints);
        }
        else
        {
            // Для ассоциации рисуем простую стрелку
            Vector2 direction = (end - start).normalized;
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);
            
            Vector2 arrowEnd = end;
            Vector2 arrowStart1 = arrowEnd - direction * relationshipArrowSize + perpendicular * relationshipArrowSize * 0.5f;
            Vector2 arrowStart2 = arrowEnd - direction * relationshipArrowSize - perpendicular * relationshipArrowSize * 0.5f;
            
            Handles.DrawLine(arrowEnd, arrowStart1);
            Handles.DrawLine(arrowEnd, arrowStart2);
        }
    }
    
    private float CalculateClassBoxHeight(Type classType, bool isExpanded)
    {
        if (!isExpanded) return headerHeight;
        
        var members = GetClassMembers(classType);
        return headerHeight + memberHeight * members.Length;
    }
    
    private void LayoutClasses(Type rootType, float startX, float startY)
    {
        // Список типов для отображения
        var typesToDisplay = new HashSet<Type>();
        
        // Добавляем корневой тип
        typesToDisplay.Add(rootType);
        
        // Добавляем базовые типы (исключая Object)
        Type baseType = rootType.BaseType;
        while (baseType != null && baseType != typeof(object))
        {
            typesToDisplay.Add(baseType);
            baseType = baseType.BaseType;
        }
        
        // Добавляем интерфейсы
        foreach (Type interfaceType in rootType.GetInterfaces())
        {
            typesToDisplay.Add(interfaceType);
        }
        
        // Добавляем типы-ассоциации (поля, свойства)
        AddRelatedTypes(rootType, typesToDisplay);
        
        // Размещаем классы в сетке
        int numClasses = typesToDisplay.Count;
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(numClasses));
        
        float x = startX;
        float y = startY;
        int count = 0;
        
        foreach (Type type in typesToDisplay)
        {
            // Вычисляем высоту класса
            float classBoxHeight = CalculateClassBoxHeight(type, true);
            
            // Создаем прямоугольник для класса
            Rect classRect = new Rect(x, y, classBoxWidth, classBoxHeight);
            classRects[type] = classRect;
            
            // Обновляем позицию для следующего класса
            count++;
            if (count % gridSize == 0)
            {
                x = startX;
                y += classBoxHeight + verticalSpacing;
            }
            else
            {
                x += classBoxWidth + horizontalSpacing;
            }
        }
    }
    
    private void AddRelatedTypes(Type type, HashSet<Type> typesToDisplay)
    {
        // Добавляем типы полей
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            Type fieldType = field.FieldType;
            if (fieldType.IsGenericType)
            {
                foreach (var argument in fieldType.GetGenericArguments())
                {
                    if (argument.IsClass && argument != typeof(string) && !argument.IsValueType)
                    {
                        typesToDisplay.Add(argument);
                    }
                }
            }
            else if (fieldType.IsClass && fieldType != typeof(string) && !fieldType.IsValueType)
            {
                typesToDisplay.Add(fieldType);
            }
        }
        
        // Добавляем типы свойств
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            Type propertyType = property.PropertyType;
            if (propertyType.IsGenericType)
            {
                foreach (var argument in propertyType.GetGenericArguments())
                {
                    if (argument.IsClass && argument != typeof(string) && !argument.IsValueType)
                    {
                        typesToDisplay.Add(argument);
                    }
                }
            }
            else if (propertyType.IsClass && propertyType != typeof(string) && !propertyType.IsValueType)
            {
                typesToDisplay.Add(propertyType);
            }
        }
    }
    
    private void ExportDiagramAsPNG()
    {
        if (rootType == null) return;
        
        string path = EditorUtility.SaveFilePanel("Save UML Diagram", "", rootType.Name + "_UML", "png");
        if (string.IsNullOrEmpty(path)) return;
        
        try
        {
            // Вычисляем размеры изображения
            float maxX = 0;
            float maxY = 0;
            
            foreach (var rect in classRects.Values)
            {
                maxX = Mathf.Max(maxX, rect.x + rect.width);
                maxY = Mathf.Max(maxY, rect.y + rect.height);
            }
            
            // Создаем текстуру
            int width = Mathf.CeilToInt(maxX + 50);
            int height = Mathf.CeilToInt(maxY + 50);
            
            Texture2D texture = new Texture2D(width, height);
            
            // Заполняем текстуру белым цветом
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }
            texture.SetPixels(pixels);
            texture.Apply();
            
            // Сохраняем текстуру в файл
            byte[] bytes = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
            
            // Уведомляем пользователя
            Debug.Log("UML diagram exported to: " + path);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to export UML diagram: " + ex.Message);
        }
    }
}