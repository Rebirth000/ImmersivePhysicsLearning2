using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SerializedDic.Editor
{
    [CustomPropertyDrawer(typeof(SerializedDictionary<,>), true)]
    public class SerializedDictionaryDrawer : PropertyDrawer
    {
        public const string SerializedListName = "_serializedList";                              // 序列化列表的名称，供外部获取
        public const string DefaultKeyName     = nameof(SerializedKeyValuePair<int, int>.Key);   // 如果字典没有特性，则标题使用这个键名
        public const string DefaultValueName   = nameof(SerializedKeyValuePair<int, int>.Value); // 如果字典没有特性，则标题使用这个值名

        // 存储所有的实例化 drawer
        private Dictionary<string, SerializedDictionaryInstanceDrawer> _drawers = new Dictionary<string, SerializedDictionaryInstanceDrawer>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            if (!_drawers.TryGetValue(property.propertyPath, out var drawer)) { // 寻找对应的实例化 drawer，没有则新建添加
                drawer = new SerializedDictionaryInstanceDrawer(property, fieldInfo);
                _drawers.Add(property.propertyPath, drawer);
            }

            drawer.OnGUI(position, label); // 使用实例化 drawer 绘制

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if (!_drawers.TryGetValue(property.propertyPath, out var drawer)) {// 寻找对应的实例化 drawer，没有则新建添加
                drawer = new SerializedDictionaryInstanceDrawer(property, fieldInfo);
                _drawers.Add(property.propertyPath, drawer);
            }

            return drawer.GetPropertyHeight(); // 返回实例化 drawer 的高度
        }
    }
}