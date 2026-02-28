using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class InputInfo
{
    public enum InputType
    {
        Down, Up, Keep
    }

    public enum InputSource
    {
        KeyBoard, Mouse, Axis
    }

    public InputSource Source;
    public InputType   Type;
    public KeyCode     Key;
    public int         MouseID;
    public string      AxisName;
    public bool        IsRaw;

    public InputInfo(KeyCode key, InputType type, UnityAction action) { // 键盘按键对应的初始化
        Key = key;
        Type = type;
        Source = InputSource.KeyBoard;
    }

    public InputInfo(int mouseID, InputType type, UnityAction action) { // 鼠标按键对应的初始化
        MouseID = mouseID;
        Type = type;
        Source = InputSource.Mouse;
    }

    public InputInfo(string axisName, bool isRaw, UnityAction<float> action) { // 轴按键对应的初始化
        AxisName = axisName;
        IsRaw = isRaw;
        Source = InputSource.Axis;
    }

    /// <summary>
    /// 获取完整名称
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public string GetFullName() {
        return Source switch {
            InputSource.KeyBoard => $"KeyBoard_{Key}: {Type}",
            InputSource.Mouse    => $"Mouse_{MouseID}: {Type}",
            InputSource.Axis     => $"Axis{(IsRaw ? "Raw" : "")}: {AxisName}",
            _                     => throw new ArgumentOutOfRangeException()
        };
    }
}