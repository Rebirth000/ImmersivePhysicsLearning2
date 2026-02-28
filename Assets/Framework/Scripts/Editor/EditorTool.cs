using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 封装的 GUI 绘制类
/// </summary>
public static class EditorTool
{
    /// <summary>
    /// 封装方法，用于画文本内容，即：标题 + 内容
    /// </summary>
    /// <param name="label"></param>
    /// <param name="text"></param>
    /// <param name="labelWidth"></param>
    /// <param name="textWidth"></param>
    /// <param name="height"></param>
    /// <param name="cursor"></param>
    public static string GUITextHorizontal(string      label,
                                           string      text,
                                           float       labelWidth,
                                           float       textWidth,
                                           float       height,
                                           MouseCursor cursor = MouseCursor.Text) {
        EditorGUILayout.BeginHorizontal();

        GUILayout.Label(label,
                        GUILayout.Width(labelWidth),
                        GUILayout.Height(height));
        string str = GUILayout.TextField(text,
                                         GUILayout.Width(textWidth),
                                         GUILayout.Height(height));

        EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), cursor);

        EditorGUILayout.EndHorizontal();

        return str;
    }

    /// <summary>
    /// 封装方法，用于画文本内容，即：标题 + 内容
    /// </summary>
    /// <param name="label"></param>
    /// <param name="text"></param>
    /// <param name="leftWidth"></param>
    /// <param name="rightWidth"></param>
    /// <param name="height"></param>
    /// <param name="cursor"></param>
    public static void GUILabelHorizontal(string      label,
                                          string      text,
                                          float       leftWidth,
                                          float       rightWidth,
                                          float       height,
                                          MouseCursor cursor = MouseCursor.Text) {
        EditorGUILayout.BeginHorizontal();

        GUILayout.Label(label,
                        GUILayout.Width(leftWidth),
                        GUILayout.Height(height));
        GUILayout.Label(text,
                        GUILayout.Width(rightWidth),
                        GUILayout.Height(height));

        EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), cursor);

        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 封装方法，用于绘制 bool 选项，即：标题 + bool
    /// </summary>
    /// <param name="label"></param>
    /// <param name="toggle"></param>
    /// <param name="labelWidth"></param>
    /// <param name="toggleWidth"></param>
    /// <param name="height"></param>
    /// <param name="cursor"></param>
    public static bool GUIToggleHorizontal(string      label,
                                           bool        toggle,
                                           float       labelWidth,
                                           float       toggleWidth,
                                           float       height,
                                           MouseCursor cursor = MouseCursor.Link) {
        EditorGUILayout.BeginHorizontal();

        GUILayout.Label(label,
                        GUILayout.Width(labelWidth),
                        GUILayout.Height(height));
        bool rt = EditorGUILayout.Toggle(toggle,
                                         GUILayout.Width(toggleWidth),
                                         GUILayout.Height(height));

        EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), cursor);

        EditorGUILayout.EndHorizontal();

        return rt;
    }

    /// <summary>
    /// 封装方法，用于绘制 Slider 选项，即：标题 + Slider
    /// </summary>
    /// <param name="label"></param>
    /// <param name="value"></param>
    /// <param name="leftValue"></param>
    /// <param name="rightValue"></param>
    /// <param name="labelWidth"></param>
    /// <param name="toggleWidth"></param>
    /// <param name="height"></param>
    /// <param name="cursor"></param>
    public static float GUISliderHorizontal(string      label,
                                            float       value,
                                            float       leftValue,
                                            float       rightValue,
                                            float       labelWidth,
                                            float       toggleWidth,
                                            float       height,
                                            MouseCursor cursor = MouseCursor.Link) {
        EditorGUILayout.BeginHorizontal();

        GUILayout.Label(label,
                        GUILayout.Width(labelWidth),
                        GUILayout.Height(height));
        float sld = EditorGUILayout.Slider(value, leftValue, rightValue,
                                           GUILayout.Width(toggleWidth),
                                           GUILayout.Height(height));

        EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), cursor);

        EditorGUILayout.EndHorizontal();

        return sld;
    }

    /// <summary>
    /// 封装方法，用于绘制 Enum 选项，即：标题 + Enum
    /// </summary>
    /// <param name="label"></param>
    /// <param name="enumer"></param>
    /// <param name="labelWidth"></param>
    /// <param name="toggleWidth"></param>
    /// <param name="height"></param>
    /// <param name="cursor"></param>
    public static Enum GUIEnumHorizontal(string      label,
                                         Enum        enumer,
                                         float       labelWidth,
                                         float       toggleWidth,
                                         float       height,
                                         MouseCursor cursor = MouseCursor.Link) {
        EditorGUILayout.BeginHorizontal();

        GUILayout.Label(label,
                        GUILayout.Width(labelWidth),
                        GUILayout.Height(height));
        Enum em = EditorGUILayout.EnumPopup(enumer,
                                            GUILayout.Width(toggleWidth),
                                            GUILayout.Height(height));

        EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), cursor);

        EditorGUILayout.EndHorizontal();

        return em;
    }

    /// <summary>
    /// 封装方法，用于画 Object 内容，即：标题 + Object
    /// </summary>
    /// <param name="label"></param>
    /// <param name="obj"></param>
    /// <param name="type"></param>
    /// <param name="labelWidth"></param>
    /// <param name="textWidth"></param>
    /// <param name="height"></param>
    /// <param name="cursor"></param>
    public static Object GUIObjectHorizontal(string      label,
                                             Object      obj,
                                             Type        type,
                                             float       labelWidth,
                                             float       textWidth,
                                             float       height,
                                             MouseCursor cursor = MouseCursor.Link) {
        EditorGUILayout.BeginHorizontal();

        GUILayout.Label(label,
                        GUILayout.Width(labelWidth),
                        GUILayout.Height(height));
        Object returnObj = EditorGUILayout.ObjectField(obj, type, true,
                                                       GUILayout.Width(textWidth),
                                                       GUILayout.Height(height));

        EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), cursor);

        EditorGUILayout.EndHorizontal();

        return returnObj;
    }

    /// <summary>
    /// 封装方法，用于画 Object 内容，即：标题 + Object
    /// </summary>
    /// <param name="label"></param>
    /// <param name="obj"></param>
    /// <param name="type"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="cursor"></param>
    public static Object GUIObjectVertical(string      label,
                                           Object      obj,
                                           Type        type,
                                           float       width,
                                           float       height,
                                           MouseCursor cursor = MouseCursor.Link) {
        EditorGUILayout.BeginVertical();

        GUILayout.Label(label,
                        GUILayout.Width(width),
                        GUILayout.Height(height / 2));
        Object returnObj = EditorGUILayout.ObjectField(obj, type, true,
                                                       GUILayout.Width(width),
                                                       GUILayout.Height(height / 2));

        EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), cursor);

        EditorGUILayout.EndVertical();
        
        return returnObj;
    }

    /// <summary>
    /// 是鼠标能够水平拖拽指定区域
    /// </summary>
    /// <param name="splitRect">哪个区域</param>
    /// <param name="isDragging">是否正在拖动</param>
    /// <param name="ratio">水平分割占比，左宽 / 右宽</param>
    /// <param name="totalWidth">总宽</param>
    /// <param name="minSideRatio">左右移动的最小比例</param>
    public static void MouseDrag(ref Rect splitRect, ref bool isDragging, ref float ratio, float totalWidth, float minSideRatio) {
        Event ent = Event.current;

        EditorGUIUtility.AddCursorRect(splitRect, MouseCursor.ResizeHorizontal); // 拖拽时改变鼠标光标
        switch (Event.current.rawType) {
            // 开始拖拽分割线
            case EventType.MouseDown:
                isDragging = splitRect.Contains(Event.current.mousePosition);
                break;
            case EventType.MouseDrag:
                if (isDragging) {
                    ratio += ent.delta.x / totalWidth;
                    // 限制其最大最小值
                    ratio = Mathf.Clamp(ratio, minSideRatio, 1 - minSideRatio);
                    ent.Use();
                }
            
                break;
            // 结束拖拽分割线
            case EventType.MouseUp:
                if (isDragging) isDragging = false;
                break;
        }
    }

    /// <summary>
    /// 将 foldList 长度与 collection 保持一致
    /// </summary>
    /// <param name="foldList"></param>
    /// <param name="collection"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    public static void UpdateLength<T, K>(List<T> foldList, ICollection<K> collection) {
        int n = collection.Count - foldList.Count;

        if (collection.Count == 0) foldList.Clear();
        else if (n > 0)
            foldList.AddRange(new T[n]);
        else if (n < 0)
            foldList.RemoveRange(foldList.Count + n, -n);
    }

    /// <summary>
    /// 封装方法，画竖线
    /// </summary>
    /// <param name="x"></param>
    /// <param name="startY"></param>
    /// <param name="endY"></param>
    /// <param name="lineWidth"></param>
    /// <param name="lineColor"></param>
    public static void GUILineVertical(float x, float startY, float endY, float lineWidth, Color lineColor) {
        if (startY >= endY) return;
        Color defaultColor = GUI.backgroundColor;
        GUI.backgroundColor = lineColor;
        GUI.Box(new Rect(x, startY, lineWidth, endY - startY),
                ""); // 中间分割线
        GUI.backgroundColor = defaultColor;
    }

    /// <summary>
    /// 获取 property 的所有子属性
    /// </summary>
    /// <param name="property"></param>
    /// <param name="recursive">是否递归寻找</param>
    /// <returns></returns>
    public static IEnumerable<SerializedProperty> GetPropertyChildren(SerializedProperty property, bool recursive = false) {
        if (!property.hasVisibleChildren) { // 如果属性不可见，则返回属性本身
            yield return property;
            yield break;
        }

        SerializedProperty end = property.GetEndProperty(); // 获取属性的结束标志
        property.NextVisible(true);                         // 移动到可见的第一个子属性

        do { // 如果还有可见的子属性，则循环返回子属性，直到属性内容与结束标志相等
            yield return property;
        } while (property.NextVisible(recursive) && !SerializedProperty.EqualContents(property, end));
    }

    /// <summary>
    /// 判断 property 是否有 drawer
    /// </summary>
    /// <param name="property"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool HasDrawerForProperty(SerializedProperty property, Type type) {
        // 获取 SerializedProperty 类型的所在 assembly
        Type attributeUtilityType = typeof(SerializedProperty).Assembly.GetType("UnityEditor.ScriptAttributeUtility");
        // 如果获取失败，则返回 false
        if (attributeUtilityType == null)
            return false;
        // 获取 GetDrawerTypeForPropertyAndType 方法
        var getDrawerMethod = attributeUtilityType.GetMethod("GetDrawerTypeForPropertyAndType",
                                                             BindingFlags.Static | BindingFlags.NonPublic);
        // 如果获取失败，则返回 false
        if (getDrawerMethod == null)
            return false;
        // 调用 GetDrawerTypeForPropertyAndType 方法，传入参数 property 和 type，返回值不为 null，则返回 true
        return getDrawerMethod.Invoke(null, new object[] { property, type }) != null;
    }
}