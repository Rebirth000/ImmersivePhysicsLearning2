using System.Collections;
using System.Collections.Generic;
using SerializedDic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 计时器管理器
/// </summary>
public class TimerMgr : Singleton<TimerMgr>
{
    private TimerMgr() {
        Start(); // 创建时，默认开启
    }

    private SerializedDictionary<int, TimerItem> _scaledTimerDic = new SerializedDictionary<int, TimerItem>(); // 受到 Time.timeScale 影响的计时器
    public  SerializedDictionary<int, TimerItem> ScaledTimerDic => _scaledTimerDic;

    private SerializedDictionary<int, TimerItem> _realTimerDic = new SerializedDictionary<int, TimerItem>(); // 不受到 Time.timeScale 影响的计时器
    public  SerializedDictionary<int, TimerItem> RealTimerDic => _realTimerDic;

    private List<TimerItem> _delList = new List<TimerItem>(); // 待移除的计时器

    private Coroutine _scaledTimer;
    private Coroutine _realTimer;

    private const float IntervalTime = 0.1f; // 计时间隔时间

    private int TIMER_KEY = 0; // 当前将要创建的计时器 ID

    private bool _isRunning = true;
    public  bool IsRunning => _isRunning;

    private WaitForSecondsRealtime _waitForSecondsRealtime = new WaitForSecondsRealtime(IntervalTime);
    private WaitForSeconds         _waitForSeconds         = new WaitForSeconds(IntervalTime);

    /// <summary>
    /// 开启计时
    /// </summary>
    public void Start() {
        _isRunning = true;
        _scaledTimer = MonoMgr.Instance.StartCoroutine(StartTiming(false));
        _realTimer = MonoMgr.Instance.StartCoroutine(StartTiming(true));
    }

    /// <summary>
    /// 关闭计时
    /// </summary>
    public void Stop() {
        _isRunning = false;
        MonoMgr.Instance.StopCoroutine(_scaledTimer);
        MonoMgr.Instance.StopCoroutine(_realTimer);
    }

    private IEnumerator StartTiming(bool isRealTime) {
        while (_isRunning) {
            // 每隔 IntervalTime 进行一次计时
            if (isRealTime)                           // 区分是否受到 Time.timeScale 影响
                yield return _waitForSecondsRealtime; // 使用成员变量，避免每次都 new WaitForSecondsRealtime()
            else
                yield return _waitForSeconds;

            var timerDic = isRealTime ? _realTimerDic : _scaledTimerDic; // 选择对应的 Dic

            foreach (var item in timerDic.Values) {
                if (!item.IsRunning) continue;

                int pastTime = (int) (IntervalTime * 1000); // 转换为 ms

                // 处理间隔计时
                if (item.IntervalCallBack != null) {
                    item.CurIntervalTime -= pastTime;   // 计时
                    if (item.CurIntervalTime <= 0) {    // 达到规定时间
                        item.IntervalCallBack.Invoke(); // 执行回调
                        item.ResetIntervalTime();       // 重置计时时间
                    }
                }

                // 处理总计时
                item.CurOverTime -= pastTime;   // 计时
                if (item.CurOverTime <= 0) {    // 达到规定时间
                    item.OverCallBack.Invoke(); // 执行回调
                    _delList.Add(item);         // 添加移除记录
                }
            }

            // 移除已完成的计时器
            foreach (var item in _delList) {
                timerDic.Remove(item.KeyID);                // 移除字典中的计时器
                PoolMgr.Instance.PushData<TimerItem>(item); // 回收计时器，放入缓存池中
            }

            _delList.Clear();
        }
    }

    /// <summary>
    /// 创建单个计时器
    /// </summary>
    /// <param name="overTime">总计时时间（秒）</param>
    /// <param name="overCallBack">总计时结束后的回调</param>
    /// <param name="intervalTime">间隔执行回调的时间，默认为 0（秒）</param>
    /// <param name="intervalCallBack">间隔时间结束后执行的回调</param>
    /// <param name="isRealTime">是否受到 Time.timeScale 的影响</param>
    /// <returns></returns>
    public int CreateTimer(float overTime, UnityAction overCallBack, float intervalTime = 0, UnityAction intervalCallBack = null,
                           bool  isRealTime = false) {
        var item     = PoolMgr.Instance.PopData<TimerItem>();
        var timerDic = isRealTime ? _realTimerDic : _scaledTimerDic; // 选择对应的 Dic
        int keyID    = item.KeyID == 0 || timerDic.ContainsKey(item.KeyID) ? ++TIMER_KEY : item.KeyID; // 避免添加时 keyID 不连续
        item.InitInfo(keyID, (int) (overTime * 1000), overCallBack, (int) (intervalTime * 1000), intervalCallBack);
        timerDic.Add(keyID, item);
        return keyID;
    }

    /// <summary>
    /// 移除单个计时器
    /// </summary>
    /// <param name="keyID">计时器 ID</param>
    public void RemoveTimer(int keyID) {
        if (_scaledTimerDic.TryGetValue(keyID, out var item)) {
            PoolMgr.Instance.PushData<TimerItem>(item);
            _scaledTimerDic.Remove(keyID);
        }
        else if (_realTimerDic.TryGetValue(keyID, out var realItem)) {
            PoolMgr.Instance.PushData<TimerItem>(realItem);
            _realTimerDic.Remove(keyID);
        }
    }

    /// <summary>
    /// 重置计时器
    /// </summary>
    /// <param name="keyID">计时器 ID</param>
    public void ResetTimer(int keyID) {
        if (_scaledTimerDic.TryGetValue(keyID, out var item)) {
            item.ResetTime();
        }
        else if (_realTimerDic.TryGetValue(keyID, out var realItem)) {
            realItem.ResetTime();
        }
    }

    /// <summary>
    /// 开启计时器
    /// </summary>
    /// <param name="keyID">计时器 ID</param>
    public void StartTimer(int keyID) {
        if (_scaledTimerDic.TryGetValue(keyID, out var item)) {
            item.IsRunning = true;
        }
        else if (_realTimerDic.TryGetValue(keyID, out var realItem)) {
            realItem.IsRunning = true;
        }
    }

    /// <summary>
    /// 停止计时器
    /// </summary>
    /// <param name="keyID">计时器 ID</param>
    public void StopTimer(int keyID) {
        if (_scaledTimerDic.TryGetValue(keyID, out var item)) {
            item.IsRunning = false;
        }
        else if (_realTimerDic.TryGetValue(keyID, out var realItem)) {
            realItem.IsRunning = false;
        }
    }
}