using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleInput : SensorInputBase<AngleInput>
{
    public float tolerance = 0.5f; // 灵敏度

    public override float Delta { // 角度的变化量
        get {
            float delta = _nowValue - _lastValue;
            if (Mathf.Abs(delta) < tolerance) return 0;
            if (Mathf.Abs(delta) > 180) return delta - Mathf.Sign(delta) * 360; // 严密推理
            return delta;
        }
    }

    public override float Value { // 当前角度值
        get => _nowValue;
    }

    protected override string ProcessString(string input) {
        Debug.Log(input);

        return input.Substring(6, input.Length - 6); // 去掉 “Angle:12.222” 前面的 “Angle:”
    }
    
    /// <summary>
    /// 判断是否为有效字符串
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    protected override bool IsValidString(string input) {
        Debug.Log(input);
        if (input == null || input.Length < 6) return false;
        return input.StartsWith("Angle:");
    }

    #region testing

    // public float delta;
    // public float value;

    #endregion
    
    // private void OnEnable()
    // {
    //     // 先寻找文件中存储的端口
    //     TryOpenPort("COM6");
    // }

    protected override void Update() {
        base.Update();
        #region testing

        // delta = Delta;
        // value = Value;

        #endregion
    }
}