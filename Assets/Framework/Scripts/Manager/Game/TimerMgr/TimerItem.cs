using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 计时器
/// </summary>
[Serializable]
public class TimerItem : IPoolData
{
    public int         KeyID;            // 唯一 ID
    public UnityAction OverCallBack;     // 结束回调
    public UnityAction IntervalCallBack; // 每间隔一定时间执行的回调

    public int CurOverTime;     // 当前总时间（毫秒）
    public int DefaultOverTime; // 默认总时间（毫秒），用于重置计时器

    public int CurIntervalTime;     // 当前间隔执行回调的时间（毫秒）
    public int DefaultIntervalTime; // 默认间隔执行回调的时间（毫秒），用于重置计时器

    public bool IsRunning; // 是否正在计时

    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="keyId">唯一 ID</param>
    /// <param name="overTime">总计时时间</param>
    /// <param name="overCallBack">总计时结束后的回调</param>
    /// <param name="intervalTime">间隔执行回调的时间，默认为 0</param>
    /// <param name="intervalCallBack">间隔时间结束后执行的回调</param>
    public void InitInfo(int keyId, int overTime, UnityAction overCallBack, int intervalTime = 0, UnityAction intervalCallBack = null) {
        KeyID = keyId;
        DefaultOverTime = CurOverTime = overTime;
        DefaultIntervalTime = CurIntervalTime = intervalTime;
        OverCallBack = overCallBack;
        IntervalCallBack = intervalCallBack;
        IsRunning = true;
    }

    /// <summary>
    /// 重置总计时
    /// </summary>
    public void ResetOverTime() {
        CurOverTime = DefaultOverTime;
    }

    /// <summary>
    /// 重置间隔计时
    /// </summary>
    public void ResetIntervalTime() {
        CurIntervalTime = DefaultIntervalTime;
    }

    /// <summary>
    /// 重置计时器
    /// </summary>
    public void ResetTime() {
        ResetOverTime();
        ResetIntervalTime();
        IsRunning = true;
    }

    /// <summary>
    /// 缓存池回收时，清除相关引用数据
    /// </summary>
    public void ResetData() {
        OverCallBack = null;
        IntervalCallBack = null;
    }
}