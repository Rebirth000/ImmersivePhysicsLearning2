using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PullInput : SensorInputBase<PullInput>
{
    // 20 10 20 15 60 50 100 200 330...
    // 30 20 60 50 100 200 ...
    public float maxPullLimit = 800; // 最大拉力阈值

    public float pullTolerance = 0.05f; // 开始拉动的灵敏度
    public float releaseTolerance = 0.05f; // 开始释放的灵敏度

    private float _maxValue = 0; // 记录一次拉动时的最大拉力

    private List<float> _lastValues = new List<float>(new float[5]); // 记录前 5 帧的拉力值，用于松手的判断

    [SerializeField]
    private bool _isPulling = false; // 是否正在拉动

    public override float Delta { // 拉力的变化量
        get => Value - Mathf.Clamp(_lastValue, 0, maxPullLimit);
    }

    public override float Value { // 拉力值
        get => Mathf.Clamp(_nowValue, 0, maxPullLimit);
    }

    public float NormedDelta { // 归一后的拉力值变化量
        get => NormedValue - Mathf.Clamp((_lastValue - _startValue) / (maxPullLimit - _startValue), 0, 1);
    }

    public float NormedValue { // 归一后的拉力值
        get => Mathf.Clamp((_nowValue - _startValue) / (maxPullLimit - _startValue), 0, 1);
    }

    public float NormedMaxValue { // 归一后的最大拉力值
        get => Mathf.Clamp((_maxValue - _startValue) / (maxPullLimit - _startValue), 0, 1);
    }

    public bool IsPulling { // 是否正在拉动
        get => _isPulling;
        set => _isPulling = value;
    }

    public UnityEvent onPulled; // 监听事件，开始拉动时将执行该事件

    public UnityEvent onReleased; // 监听事件，释放滑块时将执行该事件

    protected override string ProcessString(string input) {
        return input.Substring(5, input.Length - 5); // 去掉 “Pull:12” 前面的 “Pull:”
    }
    
    /// <summary>
    /// 判断是否为有效字符串
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    protected override bool IsValidString(string input) {
        if (input == null || input.Length < 5) return false;
        return input.StartsWith("Pull:");
    }
    
    #region testing

    public float delta;
    public float value;
    public float normedDelta;
    public float normedValue;
    public float normedMaxValue;

    #endregion

    protected override void Update() {
        base.Update();
        
        #region testing

        delta = Delta;
        value = Value;
        normedValue = NormedValue;
        normedDelta = NormedDelta;
        normedMaxValue = NormedMaxValue;

        #endregion
        
        // 更新前 5 帧的 normedValue
        for (int i = 0; i < _lastValues.Count - 1; i++) {
            _lastValues[i] = _lastValues[i + 1];
        }
        _lastValues[^1] = normedValue;

        if (!_isPulling && NormedValue > pullTolerance) { // 开始拉动
            _isPulling = true;
            onPulled?.Invoke(); // 执行监听事件
        }

        if (_isPulling && _nowValue > _maxValue)
            _maxValue = _nowValue; // 更新拉力最大值

        if (_isPulling && _lastValues.Max() - NormedValue > releaseTolerance) { // 当前拉力 < 前 5 帧的最大拉力 - tolerance，即结束拉动
            _isPulling = false;
            onReleased?.Invoke();    // 执行监听事件
            _maxValue = _startValue; // 重置拉力最大值
        }
    }
}