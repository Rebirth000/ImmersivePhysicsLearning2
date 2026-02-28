using System;
using System.Collections;
using System.Collections.Generic;
using SerializedDic;
using UnityEditor;
using UnityEngine;

public class PoolMgrWindow : MgrWindowBase<PoolMgrWindow>
{
    [SerializeField]
    [SerializedDictionary("Container", "Objects")]
    private SerializedDictionary<string, PoolMgr.PoolMonoContainer> _monoDic;

    [SerializeField]
    [SerializedDictionary("Container", "Objects")]
    private SerializedDictionary<string, PoolMgr.PoolDataContainerBase> _dataDic;

    private SerializedProperty _monoDicProperty;
    private SerializedProperty _dataDicProperty;

    private const float LabelWidth = 100;  // 成员名称显示宽度
    private const float Height     = 20f;  // 每行的高度
    private const float NumWidth   = 25;   // key 中第一个数字序号的宽度
    private const float NameWidth  = 100f; // value 中名称的宽度
    private const float SpaceWidth = 4f;   // GUILayout 自动布局的间隔宽度
    private const float SplitWidth = 4f;   // key、value 两列中间分隔宽度
    
    private bool _dataFold = true;


    [MenuItem("Framework/Windows/" + nameof(PoolMgrWindow))]
    private static void ShowWindow() {
        PoolMgrWindow win = GetWindow<PoolMgrWindow>();
        win.Show();
    }

    /// <summary>
    /// 初始化成员引用
    /// </summary>
    protected override void Init() {
        base.Init();
        _monoDic = PoolMgr.Instance.MonoDic;
        _dataDic = PoolMgr.Instance.DataDic;
        _monoDicProperty = _serializedObject.FindProperty(nameof(_monoDic));
        _dataDicProperty = _serializedObject.FindProperty(nameof(_dataDic));
    }

    protected override void OnEnable() {
        base.OnEnable();
        _StopPlayingDo = PoolMgr.Instance.ClearAllPool;
    }

    /// <summary>
    /// 运行状态下显示的内容
    /// </summary>
    protected override void OnGUIWhenOnPlay() {
        PoolMgr.Instance.Pool = (GameObject) EditorTool.GUIObjectHorizontal("Pool", PoolMgr.Instance.Pool, typeof(GameObject),
                                                                            LabelWidth, position.width - LabelWidth - SpaceWidth * 3, Height);

        EditorGUILayout.PropertyField(_monoDicProperty, new GUIContent("Mono Dic"));
        // EditorGUILayout.PropertyField(_dataDicProperty, new GUIContent("Data Dic"));
        
        int i = 0;

        _dataFold = EditorGUILayout.BeginFoldoutHeaderGroup(_dataFold, "Data Dic");
        EditorGUILayout.EndFoldoutHeaderGroup();
        
        if (_dataFold) {
            foreach (var pair in PoolMgr.Instance.DataDic) { // 例举每个 pair 元素
                EditorGUILayout.BeginHorizontal();

                EditorTool.GUITextHorizontal($"{++i}:", pair.Key,
                                             NumWidth, position.width * 0.3f - SplitWidth / 2 - SpaceWidth * 3 - NumWidth, Height);

                EditorTool.GUITextHorizontal("Count: ", pair.Value.Count.ToString(),
                                             NameWidth, position.width * (1 - 0.3f) - SplitWidth / 2 - SpaceWidth * 3 - NameWidth, Height);
                
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}