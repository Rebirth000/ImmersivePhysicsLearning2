using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

/// <summary>
/// 可序列化事件（无参数）
/// </summary>
[Serializable]
public class SerializedEvent : BEvent
{
    private UnityEvent _ent = new UnityEvent();

    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="func">监听函数</param>
    /// <param name="adder">添加者，通常为 this</param>
    /// <param name="adderType">添加者的类型，通常为 GetType()</param>
    /// <param name="filePath">访问文件路径</param>
    /// <param name="line">代码所在行数</param>
    /// <param name="callingMember">调用成员名称</param>
    public void AddListener(UnityAction               func,
                            Object                    adder,
                            Type                      adderType,
                            [CallerFilePath]   string filePath      = "",
                            [CallerLineNumber] int    line          = 0,
                            [CallerMemberName] string callingMember = "") {
        _ent.AddListener(func);
        _funcList.Add(new MethodInfo() { // 添加序列化信息
                          funcFullName = StringUtil.GetVoidMethodFullName(func.Method),
                          callerFile = filePath,
                          callerLine = line,
                          callerMember = callingMember,
                          caller = adder,
                          callerType = adderType
                      });
    }

    public void RemoveListener(UnityAction func) {
        _ent.RemoveListener(func);
        _funcList.RemoveAll(x => x.funcFullName == StringUtil.GetVoidMethodFullName(func.Method)); // 将所有与该函数名相同的函数名从列表中移除
    }

    public void Invoke() {
        _ent?.Invoke();
    }

    public void RemoveAllListeners() {
        _ent.RemoveAllListeners();
        _funcList.Clear();
    }
}

/// <summary>
/// 可序列化事件（有参数）
/// </summary>
/// <typeparam name="T">参数类型</typeparam>
[Serializable]
public class SerializedEvent<T> : BEvent
{
    private UnityEvent<T> _ent = new UnityEvent<T>();

    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="func">监听函数</param>
    /// <param name="adder">添加者，通常为 this</param>
    /// <param name="adderType">添加者的类型，通常为 GetType()</param>
    /// <param name="filePath">访问文件路径</param>
    /// <param name="line">代码所在行数</param>
    /// <param name="callingMember">调用成员名称</param>
    public void AddListener(UnityAction<T>            func,
                            Object                    adder,
                            Type                      adderType,
                            [CallerFilePath]   string filePath      = "",
                            [CallerLineNumber] int    line          = 0,
                            [CallerMemberName] string callingMember = "") {
        _ent.AddListener(func);
        _funcList.Add(new MethodInfo() { // 添加序列化信息
                          funcFullName = StringUtil.GetVoidMethodFullName(func.Method),
                          callerFile = filePath,
                          callerLine = line,
                          callerMember = callingMember,
                          caller = adder,
                          callerType = adderType
                      });
    }

    public void RemoveListener(UnityAction<T> func) {
        _ent.RemoveListener(func);
        _funcList.RemoveAll(x => x.funcFullName == StringUtil.GetVoidMethodFullName(func.Method)); // 将所有与该函数名相同的函数名从列表中移除
    }

    public void Invoke(T para) {
        _ent?.Invoke(para);
    }

    public void RemoveAllListeners() {
        _ent.RemoveAllListeners();
        _funcList.Clear();
    }
}


/// <summary>
/// 事件基类，通过 List 记录添加的 func 以实现 Inspector 面板序列化（在 Inspector 面板上显示 func 信息）
/// </summary>
[Serializable]
public class BEvent
{
    /// <summary>
    /// 函数记录信息类
    /// </summary>
    [Serializable]
    public class MethodInfo
    {
        public string funcFullName;
        public string callerFile;
        public Object caller;
        public int    callerLine;
        public string callerMember;
        public Type   callerType;
    }

    [SerializeField]
    protected List<MethodInfo> _funcList = new List<MethodInfo>(); // 函数信息

    public List<MethodInfo> FuncList => _funcList;

    /// <summary>
    /// Event 中委托函数的个数（持久化监听函数）
    /// </summary>
    /// <returns></returns>
    public int FuncCount() => _funcList.Count;
}