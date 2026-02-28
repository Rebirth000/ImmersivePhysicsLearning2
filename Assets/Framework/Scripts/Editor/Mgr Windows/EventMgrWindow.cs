using System;
using System.Collections;
using System.Collections.Generic;
using SerializedDic;
using UnityEditor;
using UnityEngine;

public class EventMgrWindow : MgrWindowBase<EventMgrWindow>
{
    [SerializeField]
    [SerializedDictionary("EventName", "Functions")]
    private SerializedDictionary<string, BEvent> _eventDic;

    private SerializedProperty _eventDicProperty;
    
    [MenuItem("Framework/Windows/" + nameof(EventMgrWindow))]
    private static void ShowWindow() {
        EventMgrWindow win = GetWindow<EventMgrWindow>();
        win.Show();
    }

    protected override void Init() {
        base.Init();
        _eventDic = EventMgr.Instance.EventDic;
        _eventDicProperty = _serializedObject.FindProperty(nameof(_eventDic));
    }

    protected override void OnGUIWhenOnPlay() {
        EditorGUILayout.PropertyField(_eventDicProperty, new GUIContent("Event Dic"));
    }
}