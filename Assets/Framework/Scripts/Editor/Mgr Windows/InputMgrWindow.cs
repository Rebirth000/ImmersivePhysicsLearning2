using System;
using System.Collections;
using System.Collections.Generic;
using SerializedDic;
using UnityEditor;
using UnityEngine;

public class InputMgrWindow : MgrWindowBase<InputMgrWindow>
{
    [SerializeField]
    [SerializedDictionary("InputName", "InputInfo")]
    private SerializedDictionary<string, InputInfo> _inputDic;

    private SerializedProperty _inputDicProperty;

    private const float LabelWidth = 100; // 成员名称显示宽度
    private const float Height     = 20f; // 每行的高度
    private const float SpaceWidth = 4f;  // GUILayout 自动布局的间隔宽度
    
    [MenuItem("Framework/Windows/" + nameof(InputMgrWindow))]
    private static void ShowWindow() {
        InputMgrWindow win = GetWindow<InputMgrWindow>();
        win.Show();
    }

    /// <summary>
    /// 初始化成员引用
    /// </summary>
    protected override void Init() {
        base.Init();
        _inputDic = InputMgr.Instance.InputDic;
        _inputDicProperty = _serializedObject.FindProperty(nameof(_inputDic));
    }

    protected override void OnGUIWhenOnPlay() {
        InputMgr.Instance.IsStart = EditorTool.GUIToggleHorizontal("IsStart", InputMgr.Instance.IsStart,
                                                                   LabelWidth, position.width - LabelWidth - SpaceWidth * 3, Height);

        EditorGUILayout.PropertyField(_inputDicProperty, new GUIContent("Input Dic"));
    }
}