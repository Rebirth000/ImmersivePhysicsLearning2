using System;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SerializedDic.Editor
{
    public class SerializedDictionaryInstanceDrawer
    {
        private bool _isDragging;                      // 记录是否正在拖拽
        private bool _propertyDisplayStyleInitialized; // 记录属性显示样式是否已初始化

        private FieldInfo _fieldInfo;      // 字段信息
        private FieldInfo _keyFieldInfo;   // 键字段信息
        private FieldInfo _valueFieldInfo; // 值字段信息

        private Rect     _headerRect;    // 标题栏 Rect
        private GUIStyle _GUITextCenter; // 居中显示的文本样式，用于标题栏

        private DisplayType _keyDisplayType;   // 键的显示样式
        private DisplayType _valueDisplayType; // 值的显示样式

        private ReorderableList    _reorderableList; // 用于显示的 reorderableList
        private SerializedProperty _listProperty;    // SerializedDictionary 中 _serializedList 对应的属性

        private float KeyValueWidthRatio = 0.3f; // 键值对所占宽度比例
        private float LabelWidthRatio    = 0.3f; // 标签所占宽度比例

        private Color _keyValueSplitLineColor = Color.black; // 键值对分割线颜色（竖线）
        private Color _itemSplitLineColor     = Color.grey;  // 每个 item 竖直方向上分割线颜色（横线）

        private const float MinSideRatio         = 0.1f; // 拖拽 _keyValueSplitLine 时，最小允许的侧边比例
        private const float HeaderSplitLineWidth = 2f;   // header 中的分割线宽度
        private const float ItemSplitLineWidth   = 0.5f; // item 分割线宽度
        private const float LineSideSpace        = 5f;   // 分割线两边的间距
        private const float ListTriangleSpace    = 12f;  // 下拉列表左侧三角形的宽度，用于缩进列表
        private const float ItemElemSpace        = 2f;   // 每个 item 内部的间距
        private const float ItemSpace            = 5f;   // 每个 item 之间的间距
        private const float CountNumWidth        = 45f;  // SerializedDictionary 右侧计数数字宽度

        private SerializedDictionaryAttribute _dictionaryAttribute; // SerializedDictionary 特性信息

        public SerializedDictionaryInstanceDrawer(SerializedProperty property, FieldInfo fieldInfo) {
            // 序列化列表属性
            _listProperty = property.FindPropertyRelative(SerializedDictionaryDrawer.SerializedListName);

            // 创建可排序列表
            _reorderableList = new ReorderableList(_listProperty.serializedObject, _listProperty, true, true, true, true) {
                drawElementCallback = OnDrawElement,
                elementHeightCallback = OnGetElementHeight,
                drawHeaderCallback = OnDrawHeader,
                drawNoneElementCallback = OnDrawNoneElement,
            };

            _GUITextCenter = new GUIStyle() { alignment = TextAnchor.MiddleCenter }; // 居中样式

            _fieldInfo = fieldInfo;
            _dictionaryAttribute = _fieldInfo.GetCustomAttribute<SerializedDictionaryAttribute>(); // 获取特性

            // 获取序列化列表的字段
            var listField = _fieldInfo.FieldType.GetField(SerializedDictionaryDrawer.SerializedListName,
                                                          BindingFlags.Instance | BindingFlags.NonPublic);

            if (listField != null) {
                // 获取键字段
                _keyFieldInfo = listField.FieldType.GetGenericArguments()[0].GetField(SerializedDictionaryDrawer.DefaultKeyName);
                // 获取值字段
                _valueFieldInfo = listField.FieldType.GetGenericArguments()[0].GetField(SerializedDictionaryDrawer.DefaultValueName);
            }
        }

        /// <summary>
        /// _reorderableList 绘制每个元素时执行的方法
        /// </summary>
        /// <param name="rect">绘制区域</param>
        /// <param name="index">元素下标位置</param>
        /// <param name="isActive">元素是否被激活</param>
        /// <param name="isFocused">元素是否被选中</param>
        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            var itemProperty  = _listProperty.GetArrayElementAtIndex(index); // item 属性，即 SerializedKeyValuePair 对应的序列化属性
            var keyProperty   = itemProperty.FindPropertyRelative(SerializedDictionaryDrawer.DefaultKeyName);
            var valueProperty = itemProperty.FindPropertyRelative(SerializedDictionaryDrawer.DefaultValueName);

            var defaultLabelWidth = EditorGUIUtility.labelWidth; // 记录原始 labelWidth，更改后用于还原

            var headerValueRectWidth = _headerRect.width * (1 - KeyValueWidthRatio); // 分割线的 x 位置

            if (index != 0) { // 从第 2 个元素开始，绘制上分割线
                // 宽度为 SplitLineWidth，下移 ItemElemSpace
                EditorGUI.DrawRect(rect.CutToUp(ItemSplitLineWidth).Move(0, ItemElemSpace), _itemSplitLineColor);
            }

            rect = rect.CutUp(ItemSpace); // item 绘制区域下移 ItemSpace 高度，留下 ItemSpace - ItemElemSpace 的空白间距

            if (!_propertyDisplayStyleInitialized) { // DisplayType 未初始化，则初始化
                // 初始化后置，因为需要提供 keyProperty 和 valueProperty，无法在构造器中直接初始化
                _keyDisplayType = GetPropertyDisplayType(keyProperty, _keyFieldInfo.FieldType);
                _valueDisplayType = GetPropertyDisplayType(valueProperty, _valueFieldInfo.FieldType);
                _propertyDisplayStyleInitialized = true; // DisplayType 已经初始化
            }

            // 绘制 key 属性
            var keyRect = rect.CutRight(headerValueRectWidth + LineSideSpace);
            EditorGUIUtility.labelWidth = LabelWidthRatio * keyRect.width;
            DrawPropertyWithDisplayType(keyRect, keyProperty, _keyDisplayType);

            // 绘制中间分割线
            // var splitLineRect = new Rect(headerKeyRectWidth, rect.y, ItemSplitLineWidth, rect.height);
            var splitLineRect = rect.CutRight(headerValueRectWidth - ItemSplitLineWidth / 2).CutToRight(ItemSplitLineWidth);
            EditorGUI.DrawRect(splitLineRect, _keyValueSplitLineColor);

            // 绘制 value 属性
            // var valueRect = new Rect(headerRectWidth + LineSideSpace, rect.y, _headerRect.xMax - headerRectWidth - LineSideSpace, rect.height);
            var valueRect = rect.CutToRight(headerValueRectWidth - LineSideSpace);
            EditorGUIUtility.labelWidth = LabelWidthRatio * valueRect.width;
            DrawPropertyWithDisplayType(valueRect, valueProperty, _valueDisplayType);

            EditorGUIUtility.labelWidth = defaultLabelWidth; // 还原 labelWidth
        }

        /// <summary>
        /// _reorderableList 获取每个元素的高度
        /// </summary>
        /// <param name="index">元素下标位置</param>
        /// <returns></returns>
        private float OnGetElementHeight(int index) {
            var itemProperty  = _listProperty.GetArrayElementAtIndex(index);
            var keyProperty   = itemProperty.FindPropertyRelative(SerializedDictionaryDrawer.DefaultKeyName);
            var valueProperty = itemProperty.FindPropertyRelative(SerializedDictionaryDrawer.DefaultValueName);
            return Mathf.Max(GetPropertyHeightWithDisplayType(keyProperty, _keyDisplayType), // key、value 的最大高度，加上 item 之间的间距 ItemSpace
                             GetPropertyHeightWithDisplayType(valueProperty, _valueDisplayType)) + ItemSpace;
        }

        /// <summary>
        /// _reorderableList 绘制 header 执行的方法
        /// </summary>
        /// <param name="rect"></param>
        private void OnDrawHeader(Rect rect) {
            _headerRect = rect; // 记录 header 所在的 Rect，用于计算分割线位置

            var keyRect             = rect.CutToLeft(KeyValueWidthRatio, true);
            var valueRect           = rect.CutLeft(KeyValueWidthRatio, true);
            var headerSplitLineRect = keyRect.CutToRight(HeaderSplitLineWidth);

            // 如果有 SerializedDictionary 特性，则使用特性中的标题名称
            var keyName   = _dictionaryAttribute?.KeyName ?? SerializedDictionaryDrawer.DefaultKeyName;
            var valueName = _dictionaryAttribute?.ValueName ?? SerializedDictionaryDrawer.DefaultValueName;

            // 绘制
            EditorGUI.LabelField(keyRect, keyName, _GUITextCenter);
            EditorGUI.LabelField(valueRect, valueName, _GUITextCenter);
            EditorGUI.DrawRect(headerSplitLineRect, Color.gray);

            // 实现拖拽 headerSplitLineRect 移动分割线
            EditorTool.MouseDrag(ref headerSplitLineRect, ref _isDragging, ref KeyValueWidthRatio, rect.width, MinSideRatio);
        }

        /// <summary>
        /// _reorderableList 中没有元素时执行的绘制方法
        /// </summary>
        /// <param name="rect"></param>
        private void OnDrawNoneElement(Rect rect) {
            EditorGUI.LabelField(rect, "Dic is Empty!");
        }

        /// <summary>
        /// 计算整个 dic 的显示高度
        /// </summary>
        /// <returns></returns>
        public float GetPropertyHeight() {
            return EditorGUIUtility.singleLineHeight + ItemElemSpace +            // 标题高度
                   (_listProperty.isExpanded ? _reorderableList.GetHeight() : 0); // reorderableList 高度
        }

        /// <summary>
        /// 绘制 dic 方法
        /// </summary>
        /// <param name="position"></param>
        /// <param name="label"></param>
        public void OnGUI(Rect position, GUIContent label) {
            EditorGUI.BeginChangeCheck();

            DoList(position, label); // 绘制

            if (EditorGUI.EndChangeCheck()) {
                _listProperty.serializedObject.ApplyModifiedProperties(); // 应用更改
            }
        }

        /// <summary>
        /// 封装方法，绘制 label 标题和 _reorderableList
        /// </summary>
        /// <param name="position"></param>
        /// <param name="label"></param>
        private void DoList(Rect position, GUIContent label) {
            // 绘制标题
            var rect = position.Resize(y: EditorGUIUtility.singleLineHeight);
            _listProperty.isExpanded =
                EditorGUI.BeginFoldoutHeaderGroup(rect.CutRight(CountNumWidth + LineSideSpace), _listProperty.isExpanded, label); // 左侧折叠区域
            EditorGUI.TextField(rect.CutToRight(CountNumWidth), $"{_reorderableList.count}");                                     // 右侧计数数字区域
            EditorGUI.EndFoldoutHeaderGroup();

            if (_listProperty.isExpanded) // 如果折叠区域展开，绘制 _reorderableList
                // 绘制区域需要下移 EditorGUIUtility.singleLineHeight + ItemElemSpace
                _reorderableList.DoList(position.Move(0, EditorGUIUtility.singleLineHeight + ItemElemSpace));
        }

        /// <summary>
        /// 依据 property 和 type 决定绘制类型
        /// </summary>
        /// <param name="property"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private DisplayType GetPropertyDisplayType(SerializedProperty property, Type type) {
            var hasCustomEditor = EditorTool.HasDrawerForProperty(property, type); // 检查是否存在自定义编辑器
            if (hasCustomEditor)
                return DisplayType.Property; // 显示默认绘制

            var isArray = property.isArray && property.propertyType != SerializedPropertyType.String; // 检查属性是否为数组，并且不是字符串
            if (isArray)
                return DisplayType.List; // 列表绘制，需要向右移动一段距离

            var isGenericWithChildren = property.propertyType == SerializedPropertyType.Generic // struct or class
                                     && property.hasVisibleChildren;
            if (isGenericWithChildren) 
                return DisplayType.Children; // 显示子属性

            return DisplayType.PropertyNoLabel; // 无标签绘制
        }

        /// <summary>
        /// 依据绘制类型进行绘制
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="property"></param>
        /// <param name="type"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void DrawPropertyWithDisplayType(Rect rect, SerializedProperty property, DisplayType type) {
            switch (type) {
                case DisplayType.Property: // 默认绘制
                    EditorGUI.PropertyField(rect, property, true);
                    break;
                case DisplayType.List: // 列表绘制，需要向右移动一段距离
                    EditorGUI.PropertyField(rect.CutLeft(ListTriangleSpace), property, true);
                    break;
                case DisplayType.PropertyNoLabel: // 无标签绘制
                    EditorGUI.PropertyField(rect, property, GUIContent.none, true);
                    break;
                case DisplayType.Children: // 绘制子属性
                    var childRect = new Rect(rect.x, rect.y, rect.width, 0);

                    // 遍历所有子属性
                    foreach (var child in EditorTool.GetPropertyChildren(property, false)) {
                        var childHeight = EditorGUI.GetPropertyHeight(child, true);
                        childRect.height = childHeight;
                        EditorGUI.PropertyField(childRect, child, true);
                        childRect.y += childHeight + ItemElemSpace; // 间隔高度为 ItemElemSpace
                    }

                    break;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        /// <summary>
        /// 依据绘制类型获取高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private float GetPropertyHeightWithDisplayType(SerializedProperty property, DisplayType type) {
            switch (type) {
                case DisplayType.Property:
                case DisplayType.PropertyNoLabel:
                case DisplayType.List:
                    return EditorGUI.GetPropertyHeight(property, true);
                case DisplayType.Children:
                    var height = 0f;

                    foreach (var child in EditorTool.GetPropertyChildren(property, false)) {
                        var childHeight = EditorGUI.GetPropertyHeight(child, true);
                        height += childHeight + ItemElemSpace;
                    }

                    return height;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }

    /// <summary>
    /// 属性显示样式
    /// </summary>
    internal enum DisplayType
    {
        Property,
        PropertyNoLabel,
        Children,
        List
    }
}