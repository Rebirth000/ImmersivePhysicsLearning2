using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SerializedDic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;


/// <summary>
/// 事件管理器
/// </summary>
public sealed class EventMgr : Singleton<EventMgr>
{
    private EventMgr() { }

    [SerializedDictionary("EventName", "Functions")]
    private SerializedDictionary<string, BEvent> _eventDic = new SerializedDictionary<string, BEvent>(); // 记录所有事件

    public SerializedDictionary<string, BEvent> EventDic => _eventDic;
    
    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="name">事件名</param>
    /// <param name="filePath">访问文件路径</param>
    /// <param name="line">代码所在行数</param>
    /// <param name="callingMember">调用成员名称</param>
    public void EventTrigger(string                    name,
                             [CallerFilePath]   string filePath      = "",
                             [CallerLineNumber] int    line          = 0,
                             [CallerMemberName] string callingMember = "") {
        if (_eventDic.TryGetValue(name, out BEvent bEnt)) { // 存在对应的事件才执行
            if (bEnt is SerializedEvent ent) ent.Invoke();
            else
                Debug.LogWarning($"Method \"{name}\" exists in _idc of EventManager, but can't execute successfully, maybe params don't match.\n" +
                                 $"File: \"{filePath}\".\n" +
                                 $"Line: {line}.\n" +
                                 $"Called From: \"{callingMember}\"");
        }
        else
            Debug.LogWarning($"EventManager can't find Method \"{name}\".\n" +
                             $"File: \"{filePath}\".\n" +
                             $"Line: {line}.\n" +
                             $"Called From: \"{callingMember}\"");
    }

    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="name">事件名</param>
    /// <param name="func">待添加的委托函数</param>
    /// <param name="adder">添加者，通常为 this</param>
    /// <param name="adderType">添加者的类型，通常为 GetType()</param>
    /// <param name="filePath">访问文件路径</param>
    /// <param name="line">代码所在行数</param>
    /// <param name="callingMember">调用成员名称</param>
    public void AddListener(string                    name,
                            UnityAction               func,
                            Object                    adder,
                            Type                      adderType,
                            [CallerFilePath]   string filePath      = "",
                            [CallerLineNumber] int    line          = 0,
                            [CallerMemberName] string callingMember = "") {
        if (!_eventDic.TryGetValue(name, out BEvent bEnt)) { // 不存在对应的事件，则创建
            bEnt = new SerializedEvent();
            _eventDic.Add(name, bEnt);
        }

        (bEnt as SerializedEvent)?.AddListener(func, adder, adderType, filePath, line, callingMember); // 添加事件
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="name">事件名</param>
    /// <param name="param">参数</param>
    /// <param name="filePath">访问文件路径</param>
    /// <param name="line">代码所在行数</param>
    /// <param name="callingMember">调用成员名称</param>
    public void EventTrigger<T>(string                    name,
                                T                         param,
                                [CallerFilePath]   string filePath      = "",
                                [CallerLineNumber] int    line          = 0,
                                [CallerMemberName] string callingMember = "") {
        if (_eventDic.TryGetValue(name, out BEvent bEnt)) { // 存在对应的事件才执行
            if (bEnt is SerializedEvent<T> ent) ent.Invoke(param);
            else
                Debug.LogWarning($"Method \"{name}\" exists in _dic of EventMgr, but can't execute successfully, maybe params don't match.\n" +
                                 $"File: \"{filePath}\".\n" +
                                 $"Line: {line}.\n" +
                                 $"Called From: \"{callingMember}\".");
        }
        else
            Debug.LogWarning($"EventMgr can't find ({name}).\n" +
                             $"File: \"{filePath}\".\n" +
                             $"Line: {line}.\n" +
                             $"Called From: \"{callingMember}\".");
    }

    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="name">事件名</param>
    /// <param name="func">待添加的委托函数</param>
    /// <param name="adder">添加者，通常为 this</param>
    /// <param name="adderType">添加者的类型，通常为 GetType()</param>
    /// <param name="filePath">访问文件路径</param>
    /// <param name="line">代码所在行数</param>
    /// <param name="callingMember">调用成员名称</param>
    public void AddListener<T>(string                    name,
                               UnityAction<T>            func,
                               Object                    adder,
                               Type                      adderType,
                               [CallerFilePath]   string filePath      = "",
                               [CallerLineNumber] int    line          = 0,
                               [CallerMemberName] string callingMember = "") {
        if (!_eventDic.TryGetValue(name, out BEvent bEnt)) { // 不存在对应的事件，则创建
            bEnt = new SerializedEvent<T>();
            _eventDic.Add(name, bEnt);
        }

        (bEnt as SerializedEvent<T>)?.AddListener(func, adder, adderType, filePath, line, callingMember); // 添加事件
    }
    
    /// <summary>
    /// 移除事件监听
    /// </summary>
    /// <param name="name">事件名</param>
    /// <param name="func">待移除的委托函数</param>
    public void RemoveListener(string name, UnityAction func) {
        if (_eventDic.TryGetValue(name, out BEvent bEnt)) {
            (bEnt as SerializedEvent)?.RemoveListener(func);
            if (bEnt.FuncCount() == 0) { // 如果没有事件，则移除
                _eventDic.Remove(name);
            }
        }
    }

    /// <summary>
    /// 移除事件监听
    /// </summary>
    /// <param name="name">事件名</param>
    /// <param name="func">待移除的委托函数</param>
    public void RemoveListener<T>(string name, UnityAction<T> func) {
        if (_eventDic.TryGetValue(name, out BEvent bEnt)) {
            (bEnt as SerializedEvent<T>)?.RemoveListener(func);
            if (bEnt.FuncCount() == 0) { // 如果没有事件，则移除
                _eventDic.Remove(name);
            }
        }
    }

    /// <summary>
    /// 移除事件监听
    /// </summary>
    /// <param name="name">事件名</param>
    public void RemoveAllListener(string name) {
        if (_eventDic.ContainsKey(name)) 
            _eventDic.Remove(name);
    }

    /// <summary>
    /// 移除所有事件
    /// </summary>
    public void Clear() {
        _eventDic.Clear();
    }

    /// <summary>
    /// 移除指定的事件
    /// </summary>
    /// <param name="name">事件名</param>
    public void Clear(string name) {
        if (_eventDic.ContainsKey(name)) {
            _eventDic.Remove(name);
        }
    }
}