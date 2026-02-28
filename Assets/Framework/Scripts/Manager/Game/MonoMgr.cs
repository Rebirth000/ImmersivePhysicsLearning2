using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

/// <summary>
/// 公共 Mono 管理器
/// </summary>
public sealed partial class MonoMgr : SingletonMono<MonoMgr>
{
    [SerializeField]
    private SerializedEvent _updateEvent = new SerializedEvent();

    [SerializeField]
    private SerializedEvent _fixedUpdateEvent = new SerializedEvent();

    [SerializeField]
    private SerializedEvent _lateUpdateEvent = new SerializedEvent();

    public SerializedEvent UpdateEvent      => _updateEvent;
    public SerializedEvent FixedUpdateEvent => _fixedUpdateEvent;
    public SerializedEvent LateUpdateEvent  => _lateUpdateEvent;

    /// <summary>
    /// 添加 Update 监听函数（持久化监听器）
    /// </summary>
    /// <param name="updateFunc">更新函数</param>
    /// <param name="adder">添加者，通常为 this</param>
    /// <param name="adderType">添加者的类型，通常为 GetType()</param>
    /// <param name="type">添加到哪个更新函数中</param>
    /// <param name="filePath">访问文件路径</param>
    /// <param name="line">代码所在行数</param>
    /// <param name="callingMember">调用成员名称</param>
    public void AddUpdateFunc(UnityAction               updateFunc,
                              Object                    adder,
                              Type                      adderType,
                              EUpdateType               type          = EUpdateType.EUpdateEvent,
                              [CallerFilePath]   string filePath      = "",
                              [CallerLineNumber] int    line          = 0,
                              [CallerMemberName] string callingMember = "") {
        SerializedEvent ent = type switch {
            EUpdateType.EUpdateEvent      => _updateEvent,
            EUpdateType.EFixedUpdateEvent => _fixedUpdateEvent,
            EUpdateType.ELateUpdateEvent  => _lateUpdateEvent,
            _                             => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        ent.AddListener(updateFunc, adder, adderType, filePath, line, callingMember);
    }

    /// <summary>
    /// 移除 Update 监听函数（持久化监听器）
    /// </summary>
    /// <param name="updateFunc">更新函数</param>
    /// <param name="type">添加到哪个更新函数中</param>
    public void RemoveUpdateFunc(UnityAction updateFunc, EUpdateType type = EUpdateType.EUpdateEvent) {
        SerializedEvent ent = type switch {
            EUpdateType.EUpdateEvent      => _updateEvent,
            EUpdateType.EFixedUpdateEvent => _fixedUpdateEvent,
            EUpdateType.ELateUpdateEvent  => _lateUpdateEvent,
            _                             => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        ent.RemoveListener(updateFunc);
    }

    private void Update() {
        _updateEvent?.Invoke();
    }

    private void FixedUpdate() {
        _fixedUpdateEvent?.Invoke();
    }

    private void LateUpdate() {
        _lateUpdateEvent?.Invoke();
    }
}

public sealed partial class MonoMgr
{
    public enum EUpdateType
    {
        EUpdateEvent,      // 对应 Update 函数
        EFixedUpdateEvent, // 对应 FixedUpdate 函数
        ELateUpdateEvent   // 对应 LateUpdate 函数
    }
}