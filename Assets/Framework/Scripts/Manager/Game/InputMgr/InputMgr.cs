using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SerializedDic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Internal;

/// <summary>
/// 输入监听管理器
/// </summary>
public class InputMgr : Singleton<InputMgr>
{
    private InputMgr()
    {
        MonoMgr.Instance.AddUpdateFunc(InputUpdate, null, null); // 使用 MonoMgr 执行监听函数
    }

    [SerializedDictionary("InputName", "InputInfo")]
    private SerializedDictionary<string, InputInfo> _inputDic = new SerializedDictionary<string, InputInfo>(); // 存储输入监听信息

    public SerializedDictionary<string, InputInfo> InputDic => _inputDic;

    private bool _isBeginCheckInput = false; // 是否开始检测输入

    private readonly string InputNamePrefix = "InputMgr --- "; // 输入名称前缀

    private UnityAction<InputInfo> _getInputInfoCallBack; // 获取输入信息委托

    public bool IsStart { get; set; } = false;

    /// <summary>
    /// 输入监听函数
    /// </summary>
    private void InputUpdate()
    {
        // 委托不为空，则进行输入判断
        bool anyInputDown = (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) ||
                            (Mouse.current != null && (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame || Mouse.current.middleButton.wasPressedThisFrame));

        if (_isBeginCheckInput && anyInputDown)
        {
            InputInfo inputInfo = null;

            // 判断键盘输入
            if (Keyboard.current != null)
            {
                foreach (Key key in Enum.GetValues(typeof(Key)))
                {
                    if (key != Key.None && Keyboard.current[key].wasPressedThisFrame)
                    {
                        // 尝试将新版 Key 映射为旧版 KeyCode (如果有遗留枚举需求)暂且保留兼容性
                        if (Enum.TryParse(key.ToString(), true, out KeyCode kc))
                        {
                            inputInfo = new InputInfo(kc, InputInfo.InputType.Down, null);
                            break;
                        }
                    }
                }
            }

            // 判断鼠标输入
            if (Mouse.current != null)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame)
                    inputInfo = new InputInfo(0, InputInfo.InputType.Down, null);
                else if (Mouse.current.rightButton.wasPressedThisFrame)
                    inputInfo = new InputInfo(1, InputInfo.InputType.Down, null);
                else if (Mouse.current.middleButton.wasPressedThisFrame)
                    inputInfo = new InputInfo(2, InputInfo.InputType.Down, null);
            }

            if (inputInfo != null)
            {
                _getInputInfoCallBack?.Invoke(inputInfo); // 传入输入信息
                _getInputInfoCallBack = null;            // 置空
                _isBeginCheckInput = false;              // 停止检测
            }
        }

        if (!IsStart) return; // 未开启监听，则直接返回，不执行下面的按键监听

        foreach (KeyValuePair<string, InputInfo> pair in _inputDic)
        {
            if (pair.Value.Source == InputInfo.InputSource.KeyBoard)
            { // 监听键盘
                if (Keyboard.current == null) continue;
                if (!Enum.TryParse(pair.Value.Key.ToString(), true, out Key newKey)) continue;

                bool input = pair.Value.Type switch
                {
                    InputInfo.InputType.Down => Keyboard.current[newKey].wasPressedThisFrame,
                    InputInfo.InputType.Up => Keyboard.current[newKey].wasReleasedThisFrame,
                    InputInfo.InputType.Keep => Keyboard.current[newKey].isPressed,
                    _ => false
                };

                if (input)
                    EventMgr.Instance.EventTrigger(ProcessInputName(pair.Key)); // 触发事件
            }
            else if (pair.Value.Source == InputInfo.InputSource.Mouse)
            { // 监听鼠标
                if (Mouse.current == null) continue;

                var btnControl = pair.Value.MouseID switch
                {
                    0 => Mouse.current.leftButton,
                    1 => Mouse.current.rightButton,
                    2 => Mouse.current.middleButton,
                    _ => null
                };

                if (btnControl == null) continue;

                bool input = pair.Value.Type switch
                {
                    InputInfo.InputType.Down => btnControl.wasPressedThisFrame,
                    InputInfo.InputType.Up => btnControl.wasReleasedThisFrame,
                    InputInfo.InputType.Keep => btnControl.isPressed,
                    _ => false
                };

                if (input)
                    EventMgr.Instance.EventTrigger(ProcessInputName(pair.Key)); // 触发事件
            }
            else if (pair.Value.Source == InputInfo.InputSource.Axis)
            { // 监听轴
                float value = 0f;
                // 暂时使用旧的获取名称的方法来适配新版的硬编码键位。
                // 这可能需要配置 Input System 的 Action Map 以获得最佳效果。
                if (Keyboard.current != null)
                {
                    if (pair.Value.AxisName is "Horizontal")
                    {
                        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) value = 1f;
                        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) value = -1f;
                    }
                    else if (pair.Value.AxisName is "Vertical")
                    {
                        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) value = 1f;
                        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) value = -1f;
                    }
                }

                EventMgr.Instance.EventTrigger(ProcessInputName(pair.Key), value); // 触发事件
            }
        }
    }

    /// <summary>
    /// 开始检测输入的协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator BeginCheckInput()
    {
        yield return null;         // 等待一帧再执行
        _isBeginCheckInput = true; // 准备开始检测
    }

    /// <summary>
    /// 获取下一次输入的信息
    /// </summary>
    /// <param name="callback"></param>
    public void GetInputInfo(UnityAction<InputInfo> callback)
    {
        _getInputInfoCallBack = callback;                   // 设置委托
        MonoMgr.Instance.StartCoroutine(BeginCheckInput()); // 开始准备检测输入
    }

    /// <summary>
    /// 添加键盘输入监听
    /// </summary>
    /// <param name="inputName">存储名称，可填 null，此时为内部规定的名称</param>
    /// <param name="key">键盘按键</param>
    /// <param name="action">监听的委托</param>
    /// <param name="type">输入类型</param>
    /// <param name="allowOverride">是否允许覆盖监听</param>
    public void AddKeyBoardListener(string inputName,
                                    KeyCode key,
                                    UnityAction action,
                                    InputInfo.InputType type = InputInfo.InputType.Down,
                                    bool allowOverride = false)
    {
        InputInfo info = new InputInfo(key, type, action);

        if (inputName == null) inputName = info.GetFullName(); // 默认不填则为内部规定的名称

        if (_inputDic.TryGetValue(inputName, out InputInfo oldInfo))
        {            // 已经存储过
            if (allowOverride)
            {                                                  // 允许覆盖
                EventMgr.Instance.RemoveAllListener(ProcessInputName(inputName)); // 在 EventMgr 中移除旧监听

                // 覆盖更改信息
                oldInfo.Source = InputInfo.InputSource.KeyBoard;
                oldInfo.Key = key;
                oldInfo.Type = type;
                EventMgr.Instance.AddListener(ProcessInputName(inputName), action, null, null); // 添加新监听
            }
            else // 不允许覆盖，则不予处理并警告
                Debug.LogWarning($"{inputName} has already exist!");
        }
        else
        { // 未存储，则直接添加
            EventMgr.Instance.AddListener(ProcessInputName(inputName), action, null, null);
            _inputDic.Add(inputName, info);
        }
    }

    /// <summary>
    /// 添加鼠标输入监听
    /// </summary>
    /// <param name="inputName">存储名称，可填 null，此时为内部规定的名称</param>
    /// <param name="mouseID">鼠标按键，输入 0,1,2</param>
    /// <param name="action">监听的委托</param>
    /// <param name="type">输入类型</param>
    /// <param name="allowOverride">是否允许覆盖监听</param>
    public void AddMouseListener(string inputName,
                                 int mouseID,
                                 UnityAction action,
                                 InputInfo.InputType type = InputInfo.InputType.Down,
                                 bool allowOverride = false)
    {
        InputInfo info = new InputInfo(mouseID, type, action);

        if (inputName == null) inputName = info.GetFullName(); // 默认不填则为内部规定的名称

        if (_inputDic.TryGetValue(inputName, out InputInfo oldInfo))
        {            // 已经存储过
            if (allowOverride)
            {                                                  // 允许覆盖
                EventMgr.Instance.RemoveAllListener(ProcessInputName(inputName)); // 在 EventMgr 中移除旧监听

                // 覆盖更改信息
                oldInfo.Source = InputInfo.InputSource.Mouse;
                oldInfo.MouseID = mouseID;
                oldInfo.Type = type;
                EventMgr.Instance.AddListener(ProcessInputName(inputName), action, null, null); // 添加新监听
            }
            else // 不允许覆盖，则不予处理并警告
                Debug.LogWarning($"{inputName} has already exist!");
        }
        else
        { // 未存储，则直接添加
            EventMgr.Instance.AddListener(ProcessInputName(inputName), action, null, null);
            _inputDic.Add(inputName, info);
        }
    }

    /// <summary>
    /// 添加轴输入监听
    /// </summary>
    /// <param name="inputName">存储名称，可填 null，此时为内部规定的名称</param>
    /// <param name="axisName">轴名称</param>
    /// <param name="action">监听的委托</param>
    /// <param name="isRaw">是否为 Raw 输入</param>
    /// <param name="allowOverride">是否允许覆盖监听</param>
    public void AddAxisListener(string inputName,
                                string axisName,
                                UnityAction<float> action,
                                bool isRaw = false,
                                bool allowOverride = false)
    {
        InputInfo info = new InputInfo(axisName, isRaw, action);

        if (inputName == null) inputName = info.GetFullName(); // 默认不填则为内部规定的名称

        if (_inputDic.TryGetValue(inputName, out InputInfo oldInfo))
        {            // 已经存储过
            if (allowOverride)
            {                                                  // 允许覆盖
                EventMgr.Instance.RemoveAllListener(ProcessInputName(inputName)); // 在 EventMgr 中移除旧监听

                // 覆盖更改信息
                oldInfo.Source = InputInfo.InputSource.Axis;
                oldInfo.AxisName = axisName;
                oldInfo.IsRaw = isRaw;
                EventMgr.Instance.AddListener(ProcessInputName(inputName), action, null, null); // 添加新监听
            }
            else // 不允许覆盖，则不予处理并警告
                Debug.LogWarning($"{inputName} has already exist!");
        }
        else
        { // 未存储，则直接添加
            EventMgr.Instance.AddListener(ProcessInputName(inputName), action, null, null);
            _inputDic.Add(inputName, info);
        }
    }

    /// <summary>
    /// 获取 InputInfo 信息
    /// </summary>
    /// <param name="inputName"></param>
    /// <returns></returns>
    public InputInfo GetInputInfo(string inputName)
    {
        return _inputDic.GetValueOrDefault(inputName, null);
    }

    /// <summary>
    /// 移除指定监听
    /// </summary>
    /// <param name="inputName">存储在 InputMgr 中的名称</param>
    public void RemoveListener(string inputName)
    {
        if (_inputDic.ContainsKey(inputName))
        {
            _inputDic.Remove(inputName);
            EventMgr.Instance.RemoveAllListener(ProcessInputName(inputName));
        }
    }

    /// <summary>
    /// 设置是否开启输入监听
    /// </summary>
    /// <param name="isStart"></param>
    public void EnableInput(bool isStart)
    {
        IsStart = isStart;
    }

    /// <summary>
    /// 对 inputName 进行特殊处理，用于添加到 EventMgr 中，表示该监听由 InputMgr 添加
    /// </summary>
    /// <param name="inputName"></param>
    /// <returns></returns>
    private string ProcessInputName(string inputName)
    {
        return InputNamePrefix + inputName;
    }
}