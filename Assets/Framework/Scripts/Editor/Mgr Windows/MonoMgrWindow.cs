using System;
using System.Collections;
using System.Collections.Generic;
using SerializedDic;
using UnityEditor;
using UnityEngine;

public class MonoMgrWindow : MgrWindowBase<MonoMgrWindow>
{
    [SerializeField]
    private SerializedEvent _updateEvent;

    [SerializeField]
    private SerializedEvent _fixedUpdateEvent;

    [SerializeField]
    private SerializedEvent _lateUpdateEvent;

    private SerializedProperty _updateEventProperty;
    private SerializedProperty _fixedUpdateEventProperty;
    private SerializedProperty _lateUpdateEventProperty;

    [MenuItem("Framework/Windows/" + nameof(MonoMgrWindow))]
    private static void ShowWindow() {
        MonoMgrWindow win = GetWindow<MonoMgrWindow>();
        win.Show();
    }

    /// <summary>
    /// 初始化成员引用
    /// </summary>
    protected override void Init() {
        base.Init();
        _updateEvent = MonoMgr.Instance.UpdateEvent;
        _fixedUpdateEvent = MonoMgr.Instance.FixedUpdateEvent;
        _lateUpdateEvent = MonoMgr.Instance.LateUpdateEvent;
        _updateEventProperty = _serializedObject.FindProperty(nameof(_updateEvent));
        _fixedUpdateEventProperty = _serializedObject.FindProperty(nameof(_fixedUpdateEvent));
        _lateUpdateEventProperty = _serializedObject.FindProperty(nameof(_lateUpdateEvent));
    }

    protected override void OnGUIWhenOnPlay() {
        EditorGUILayout.PropertyField(_updateEventProperty);
        EditorGUILayout.PropertyField(_fixedUpdateEventProperty);
        EditorGUILayout.PropertyField(_lateUpdateEventProperty);
    }
}