using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

/// <summary>
/// UI 管理器，默认使用 ResourceMgr 加载 <br/>
/// 规定：面板预制体名称必须和面板脚本名称一致
/// </summary>
public partial class UIMgr : Singleton<UIMgr>
{
    public readonly string UICameraPath    = "Prefab/UI/UI_Camera";   // UICamera 预制体的存储路径
    public readonly string UICanvasPath    = "Prefab/UI/Canvas";      // Canvas 预制体的存储路径
    public readonly string EventSystemPath = "Prefab/UI/EventSystem"; // EventSystem 预制体的存储路径
    public readonly string PanelPath       = "Prefab/UI/Panel/";      // UI 面板预制体的存储路径

    private Camera      _uiCamera;
    private Canvas      _uiCanvas;
    private EventSystem _eventSystem;

    private Transform _bottomLayer;
    private Transform _middleLayer;
    private Transform _topLayer;
    private Transform _systemLayer;

    private Dictionary<string, BaseUIPanelInfo> _panelDic = new Dictionary<string, BaseUIPanelInfo>(); // 用于存储所有面板信息

    private UIMgr() {
        // 加载 UI 摄像机
        _uiCamera = Object.Instantiate(ResourceMgr.Instance.Load<GameObject>(UICameraPath)).GetComponent<Camera>();
        Object.DontDestroyOnLoad(_uiCamera.gameObject); // UI 摄像机过场景不移除

        // 加载 Canvas
        _uiCanvas = Object.Instantiate(ResourceMgr.Instance.Load<GameObject>(UICanvasPath)).GetComponent<Canvas>();
        _uiCanvas.worldCamera = _uiCamera;
        Object.DontDestroyOnLoad(_uiCanvas.gameObject);

        // 加载 EventSystem
        _eventSystem = Object.Instantiate(ResourceMgr.Instance.Load<GameObject>(EventSystemPath)).GetComponent<EventSystem>();
        Object.DontDestroyOnLoad(_eventSystem.gameObject);

        // 加载 UI 层级
        _bottomLayer = _uiCanvas.transform.Find("Bottom");
        _middleLayer = _uiCanvas.transform.Find("Middle");
        _topLayer = _uiCanvas.transform.Find("Top");
        _systemLayer = _uiCanvas.transform.Find("System");
    }

    /// <summary>
    /// 获取层级的 Transform
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Transform GetLayerTransform(UILayer layer) {
        return layer switch {
            UILayer.EBottom => _bottomLayer,
            UILayer.EMiddle => _middleLayer,
            UILayer.ETop    => _topLayer,
            UILayer.ESystem => _systemLayer,
            _               => throw new ArgumentOutOfRangeException(nameof(layer), layer, null)
        };
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <param name="layer">面板显示的层级</param>
    /// <param name="callBack">显示完成后的回调函数</param>
    /// <param name="isAsync">是否异步加载</param>
    /// <typeparam name="T"></typeparam>
    public void ShowPanel<T>(UILayer layer = UILayer.EMiddle, UnityAction<T> callBack = null, bool isAsync = true) where T : BasePanel {
        string panelName = typeof(T).Name; // 预制体名称必须和面板脚本名称一致

        UIPanelInfo<T> info = null;
        if (_panelDic.TryGetValue(panelName, out BaseUIPanelInfo bInfo)) {
            info = (UIPanelInfo<T>) bInfo;
            if (info.Panel == null) { // 面板异步加载中
                info.IsHide = false;
                info.CallBack += callBack;
            }
            else { // 面板完成异步加载
                if (!info.Panel.gameObject.activeSelf)
                    info.Panel.gameObject.SetActive(true); // 激活面板附属的对象

                info.Panel.Show();                 // 显示面板
                info.CallBack?.Invoke(info.Panel); // 进行回调
            }
        }
        else { // 不存在面板，则进行加载
            info = new UIPanelInfo<T>(callBack);
            if (isAsync) {                      // 异步加载
                _panelDic.Add(panelName, info); // 先存放空信息进行占位
                ResourceMgr.Instance.LoadAsync<GameObject>(PanelPath + panelName, res => {
                    info = (UIPanelInfo<T>) _panelDic[panelName]; // 异步加载需要更新下一帧的 info
                    if (info.IsHide) {                            // 异步加载结束前，被标记为隐藏，则不进行显示
                        _panelDic.Remove(panelName);
                        return;
                    }

                    Transform  layerTf  = GetLayerTransform(layer);                // 找到对应的层级对象
                    GameObject panelObj = Object.Instantiate(res, layerTf, false); // 实例化对象
                    info.Panel = panelObj.GetComponent<T>();                       // 获取面板脚本
                    info.Panel.Show();                                             // 显示面板
                    info.CallBack?.Invoke(info.Panel);                             // 进行回调
                    info.CallBack = null;                                          // 清空回调，避免内存泄漏
                });
            }
            else { // 同步加载
                GameObject res      = ResourceMgr.Instance.Load<GameObject>(PanelPath + panelName);
                Transform  layerTf  = GetLayerTransform(layer);                // 找到对应的层级对象
                GameObject panelObj = Object.Instantiate(res, layerTf, false); // 实例化对象
                info.Panel = panelObj.GetComponent<T>();                       // 获取面板脚本
                info.Panel.Show();                                             // 显示面板
                info.CallBack?.Invoke(info.Panel);                             // 进行回调
                info.CallBack = null;                                          // 清空回调，避免内存泄漏
                _panelDic.Add(panelName, info);
            }
        }
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <param name="isDestroy">隐藏后是否销毁面板</param>
    /// <typeparam name="T"></typeparam>
    public void HidePanel<T>(bool isDestroy = false) where T : BasePanel {
        string panelName = typeof(T).Name; // 预制体名称必须和面板脚本名称一致

        if (_panelDic.TryGetValue(panelName, out BaseUIPanelInfo bInfo)) {
            UIPanelInfo<T> info = (UIPanelInfo<T>) bInfo;
            if (info.Panel == null) { // 面板异步加载中
                info.IsHide = true;   // 标记为隐藏
                info.CallBack = null; // 清除回调
            }
            else { // 面板完成异步加载
                info.Panel.Hide();
                if (isDestroy) {
                    Object.Destroy(info.Panel.gameObject); // 销毁面板
                    _panelDic.Remove(panelName);
                }
                else {
                    info.Panel.gameObject.SetActive(false); // 失活面板附属的对象
                }
            }
        }
    }

    /// <summary>
    /// 获取面板
    /// </summary>
    /// <param name="callback">获取后执行的回调函数</param>
    /// <typeparam name="T"></typeparam>
    public void GetPanel<T>(UnityAction<T> callback) where T : BasePanel {
        string panelName = typeof(T).Name; // 预制体名称必须和面板脚本名称一致

        if (_panelDic.TryGetValue(panelName, out BaseUIPanelInfo bInfo)) {
            UIPanelInfo<T> info = (UIPanelInfo<T>) bInfo;
            if (info.Panel == null) {      // 面板异步加载中
                info.CallBack += callback; // 添加到回调列表
            }
            else {                       // 面板完成异步加载
                if (info.IsHide) return; // 隐藏的面板不执行操作
                callback?.Invoke(info.Panel);
            }
        }
    }

    /// <summary>
    /// 为 UI 控件添加自定义事件监听
    /// </summary>
    /// <param name="uiElement">ui 控件</param>
    /// <param name="type">事件监听类型</param>
    /// <param name="callback">监听的回调函数</param>
    public static void AddCustomEventListener(UIBehaviour uiElement, EventTriggerType type, UnityAction<BaseEventData> callback) {
        EventTrigger trigger = uiElement.GetComponent<EventTrigger>();
        if (trigger == null) trigger = uiElement.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(callback);

        trigger.triggers.Add(entry);
    }
}

public partial class UIMgr
{
    public enum UILayer
    {
        EBottom,
        EMiddle,
        ETop,
        ESystem
    }

    private class UIPanelInfo<T> : BaseUIPanelInfo where T : BasePanel
    {
        public T              Panel;    // 面板
        public UnityAction<T> CallBack; // 面板加载完成后的回调函数
        public bool           IsHide;   // 是否隐藏

        public UIPanelInfo(UnityAction<T> callBack) {
            CallBack = callBack;
        }
    }

    private abstract class BaseUIPanelInfo { }
}