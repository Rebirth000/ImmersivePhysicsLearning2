using System;
using System.Collections;
using System.Collections.Generic;
using SerializedDic;
using UnityEditor;
using UnityEngine;

public class ResourceMgrWindow : MgrWindowBase<ResourceMgrWindow>
{
    [SerializeField]
    [SerializedDictionary("ResName", "Resources")]
    private SerializedDictionary<string, ResourceMgr.BResInfo> _resDic;

    private SerializedProperty _resDicProperty;

    [MenuItem("Framework/Windows/" + nameof(ResourceMgrWindow))]
    private static void ShowWindow() {
        ResourceMgrWindow win = GetWindow<ResourceMgrWindow>();
        win.Show();
    }

    protected override void Init() {
        base.Init();
        _resDic = ResourceMgr.Instance.ResDic;
        _resDicProperty = _serializedObject.FindProperty(nameof(_resDic));
    }

    protected override void OnGUIWhenOnPlay() {
        EditorGUILayout.PropertyField(_resDicProperty, new GUIContent("Resource Dic"));
    }
}