using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InputInfo), true)]
public class InputInfoDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty sourceProperty   = property.FindPropertyRelative(nameof(InputInfo.Source));
        SerializedProperty typeProperty     = property.FindPropertyRelative(nameof(InputInfo.Type));
        SerializedProperty keyProperty      = property.FindPropertyRelative(nameof(InputInfo.Key));
        SerializedProperty mouseIDProperty  = property.FindPropertyRelative(nameof(InputInfo.MouseID));
        SerializedProperty axisNameProperty = property.FindPropertyRelative(nameof(InputInfo.AxisName));
        SerializedProperty isRawProperty    = property.FindPropertyRelative(nameof(InputInfo.IsRaw));

        float nextHeight = EditorGUIUtility.singleLineHeight + 2f; // 2f 为间距
        Rect  start      = position.CutToUp(EditorGUIUtility.singleLineHeight);

        EditorGUI.PropertyField(start, sourceProperty);
        EditorGUI.PropertyField(start.Move(0, nextHeight), typeProperty);

        switch (sourceProperty.enumValueIndex) {
            case 0: // KeyBoard
                EditorGUI.PropertyField(start.Move(0, nextHeight * 2), keyProperty);
                break;
            case 1: // Mouse
                EditorGUI.PropertyField(start.Move(0, nextHeight * 2), mouseIDProperty);
                break;
            case 2: // Axis
                EditorGUI.PropertyField(start.Move(0, nextHeight * 2), axisNameProperty);
                EditorGUI.PropertyField(start.Move(0, nextHeight * 3), isRawProperty);
                break;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        SerializedProperty sourceProperty = property.FindPropertyRelative(nameof(InputInfo.Source));

        float nextHeight = EditorGUIUtility.singleLineHeight + 2f; // 2f 为间距

        int count = sourceProperty.enumValueIndex switch {
            0 or 1 => 1,
            2      => 2, 
            _ => throw new ArgumentOutOfRangeException()
        };

        return EditorGUIUtility.singleLineHeight + nextHeight * (1 + count);
    }
}