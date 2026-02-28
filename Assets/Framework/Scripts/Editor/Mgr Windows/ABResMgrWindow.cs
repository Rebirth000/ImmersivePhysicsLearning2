using System;
using System.Collections;
using System.Collections.Generic;
using SerializedDic;
using UnityEditor;
using UnityEngine;

public class ABResMgrWindow : MgrWindowBase<ABResMgrWindow>
{
    [SerializeField]
    [SerializedDictionary("ABName", "AssetBundle")]
    private SerializedDictionary<string, AssetBundle> _abDic;

    private SerializedProperty _abDicProperty;

    private const float LabelWidth = 100; // 成员名称显示宽度
    private const float Height     = 20f; // 每行的高度
    private const float SpaceWidth = 4f;  // GUILayout 自动布局的间隔宽度

    [MenuItem("Framework/Windows/" + nameof(ABResMgrWindow))]
    private static void ShowWindow() {
        ABResMgrWindow win = GetWindow<ABResMgrWindow>();
        win.Show();
    }

    protected override void Init() {
        base.Init();
        _abDic = ABMgr.Instance.ABDic;
        _abDicProperty = _serializedObject.FindProperty(nameof(_abDic));
    }

    protected override void OnGUIWhenOnPlay() {
        ABResMgr.Instance.IsDebug = EditorTool.GUIToggleHorizontal("Is Debug", ABResMgr.Instance.IsDebug,
                                                                   LabelWidth, position.width - LabelWidth - SpaceWidth * 3, Height);

        if (ABResMgr.Instance.IsDebug) {
            EditorResMgr.Instance.RootPath = EditorTool.GUITextHorizontal("RootPath", EditorResMgr.Instance.RootPath,
                                                                          LabelWidth, position.width - LabelWidth - SpaceWidth * 3, Height);
        }
        else {
            EditorTool.GUIObjectHorizontal("MainAB", ABMgr.Instance.MainAB, typeof(AssetBundle),
                                           LabelWidth, position.width - LabelWidth - SpaceWidth * 3, Height, MouseCursor.Arrow);
            EditorTool.GUIObjectHorizontal("Manifest", ABMgr.Instance.Manifest, typeof(AssetBundleManifest),
                                           LabelWidth, position.width - LabelWidth - SpaceWidth * 3, Height, MouseCursor.Arrow);
            EditorTool.GUITextHorizontal("ABPath", ABMgr.Instance.ABPath,
                                         LabelWidth, position.width - LabelWidth - SpaceWidth * 3, Height, MouseCursor.Arrow);
            EditorTool.GUITextHorizontal("MainABName", ABMgr.Instance.MainABName,
                                         LabelWidth, position.width - LabelWidth - SpaceWidth * 3, Height, MouseCursor.Arrow);
            
            EditorGUILayout.PropertyField(_abDicProperty, new GUIContent("AB Dic"));
        }
    }
}