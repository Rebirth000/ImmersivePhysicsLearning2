using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

[ExecuteAlways]
public class DataSetting : MonoBehaviour
{
    private static DataSetting _instance;

    public static DataSetting Instance {
        get {
            if (_instance == null) {
                GameObject obj = GameObject.Find(nameof(DataSetting));
                if (obj == null) {
                    obj       = new GameObject(nameof(DataSetting)); // 创建名称相同的 obj
                    _instance = obj.AddComponent<DataSetting>();     // 添加 _instance
                }
                else
                    _instance = obj.GetComponent<DataSetting>();
            }

            return _instance;
        }
    }

    public Main              main;
    public BlockSpringCouple couple;
    public BlockMove         blockA;
    public BlockMove         blockB;
    public SpringMove        springMove;
    public GraphMgr          graphMgr;
    public DataPanel         panel;
    public BaffleMove        baffleMove;
    public ButtonMenu        buttonMenu;
    public TMP_FontAsset     englishFont;
    public TMP_FontAsset     chineseFont;

    private void Awake() {
        main       = GetComponent<Main>("Main");
        couple     = GetComponent<BlockSpringCouple>("Objects/LabTable/Track/Couple");
        blockA     = GetComponent<BlockMove>("Objects/LabTable/Track/Couple/BlockA");
        blockB     = GetComponent<BlockMove>("Objects/LabTable/Track/Couple/BlockB");
        springMove = GetComponent<SpringMove>("Objects/LabTable/Track/Couple/Spring");
        graphMgr   = GetComponent<GraphMgr>("DisplayBoard_2D/GraphDrawers");
        panel      = GetComponent<DataPanel>("DisplayBoard_2D/DataPanel");
        baffleMove = GetComponent<BaffleMove>("Objects/LabTable/Track/MonitorTrigger/Baffle");
        buttonMenu = GetComponent<ButtonMenu>("ButtonMenu");

        // englishFont = ResourceMgr.Instance.Load<TMP_FontAsset>("Fonts/TIMES SDF");
        // chineseFont = ResourceMgr.Instance.Load<TMP_FontAsset>("Fonts/STZHONGS SDF");
    }

    /// <summary>
    /// 依据绝对路径获取组件
    /// </summary>
    /// <param name="path">子物体路径</param>
    /// <typeparam name="T">组件类型</typeparam>
    public static T GetComponent<T>(string path) where T : Object {
        T comp = GameObject.Find(path).GetComponent<T>();

        if (comp == null)
            Debug.LogError($"Script DataSetting.cs: \"{path}\" is null");

        return comp;
    }

    /// <summary>
    /// 获取 trans 上的组件
    /// </summary>
    /// <param name="trans">物体 transform</param>
    /// <typeparam name="T">组件类型</typeparam>
    public static T GetComponent<T>(Transform trans) where T : Object {
        T comp = trans.GetComponent<T>();

        if (comp == null)
            Debug.LogError($"Script DataSetting.cs: \"{trans.name}\" has no component \"{typeof(T)}\"");

        return comp;
    }

    /// <summary>
    /// 从子物体中获取组件
    /// </summary>
    /// <param name="path">子物体路径</param>
    /// <param name="parent">父物体</param>
    /// <typeparam name="T">组件类型</typeparam>
    public static T GetComponentFromChild<T>(Transform parent, string path) where T : Object {
        Transform transPath = parent.Find(path);

        if (transPath == null)
            Debug.LogError($"Script DataSetting.cs: \"{parent.name}: {path}\" is null");

        T comp = transPath.GetComponent<T>();

        if (comp == null)
            Debug.LogError($"Script DataSetting.cs: \"{transPath.name}\" has no component \"{typeof(T)}\"");

        return comp;
    }
}