using System;
using System.Collections;
using System.Collections.Generic;
using SerializedDic;
using UnityEditor;
using UnityEngine;

public class TimerMgrWindow : MgrWindowBase<TimerMgrWindow>
{
    [SerializeField]
    [SerializedDictionary("TimerKey", "TimerInfo")]
    private SerializedDictionary<int, TimerItem> _scaledTimerDic;
    
    [SerializeField]
    [SerializedDictionary("TimerKey", "TimerInfo")]
    private SerializedDictionary<int, TimerItem> _realTimerDic;

    private SerializedProperty _scaledTimerDicProperty;
    private SerializedProperty _realTimerDicProperty;
    
    [MenuItem("Framework/Windows/" + nameof(TimerMgrWindow))]
    private static void ShowWindow() {
        TimerMgrWindow win = GetWindow<TimerMgrWindow>();
        win.Show();
    }

    protected override void Init() {
        base.Init();
        _scaledTimerDic = TimerMgr.Instance.ScaledTimerDic;
        _realTimerDic = TimerMgr.Instance.RealTimerDic;
        _scaledTimerDicProperty = _serializedObject.FindProperty(nameof(_scaledTimerDic));
        _realTimerDicProperty = _serializedObject.FindProperty(nameof(_realTimerDic));
    }

    protected override void OnGUIWhenOnPlay() {
        EditorGUILayout.PropertyField(_scaledTimerDicProperty, new GUIContent("Scaled Timer Dic"));
        EditorGUILayout.PropertyField(_realTimerDicProperty, new GUIContent("Real Timer Dic"));
    }
}