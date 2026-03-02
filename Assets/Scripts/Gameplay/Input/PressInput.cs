using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PressInput : SensorInputBase<PressInput>
{
    public float maxPressLimit = 800; // 最大压力阈值

    public float tolerance = 0.05f; // 灵敏度

    private float _maxValue = 0; // 记录一次按压时的最大压力

    private bool _isPressing = false; // 是否正在按压

    public override float Delta { // 压力的变化量
        get => Value - Mathf.Clamp(_lastValue, 0, maxPressLimit);
    }

    public override float Value { // 压力值
        get => Mathf.Clamp(_nowValue, 0, maxPressLimit);
    }

    public float NormedDelta { // 归一后的压力值变化量
        get => NormedValue - Mathf.Clamp((_lastValue - _startValue) / (maxPressLimit - _startValue), 0, 1);
    }

    public float NormedValue { // 归一后的压力值
        get => Mathf.Clamp((_nowValue - _startValue) / (maxPressLimit - _startValue), 0, 1);
    }

    public float NormedMaxValue { // 归一后的最大压力值
        get => Mathf.Clamp((_maxValue - _startValue) / (maxPressLimit - _startValue), 0, 1);
    }

    public bool IsPressing => _isPressing; // 是否正在按压

    public UnityEvent onPressed; // 监听事件，开始按压时将执行该事件

    public UnityEvent onReleased; // 监听事件，释放按钮时将执行该事件
    
    protected override string ProcessString(string input) {
        return input.Substring(6, input.Length - 6);// 去掉 “Press:12” 前面的 “Press:”
    }

    /// <summary>
    /// 判断是否为有效字符串
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    protected override bool IsValidString(string input) {
        if (input == null || input.Length < 6) return false;
        return input.StartsWith("Press:");
    }
    
    #region testing

    // public float delta;
    // public float value;
    // public float normedDelta;
    // public float normedValue;
    // public float normedMaxValue;

    #endregion

    protected override void Update() {
        base.Update();

        #region testing

        // delta = Delta;
        // value = Value;
        // normedValue = NormedValue;
        // normedDelta = NormedDelta;
        // normedMaxValue = NormedMaxValue;

        #endregion

        if (!_isPressing && NormedValue > tolerance) { // 开始按压
            _isPressing = true;
            onPressed?.Invoke(); // 执行监听事件
        }

        if (_isPressing && _nowValue > _maxValue)
            _maxValue = _nowValue; // 更新压力最大值

        if (_isPressing && NormedValue < tolerance) { // 结束按压，判定为压力几乎为 0 时结束
            _isPressing = false;
            _maxValue = _startValue; // 重置压力最大值
            onReleased?.Invoke();    // 执行监听事件
        }
    }
}