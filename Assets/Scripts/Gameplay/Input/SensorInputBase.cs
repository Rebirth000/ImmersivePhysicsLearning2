using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public abstract class SensorInputBase<T> : MonoBehaviour where T : SensorInputBase<T>
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                Type type = typeof(T); // 获取参数 T 的类型信息
                GameObject obj = GameObject.Find(type.Name);
                if (obj == null)
                {
                    obj = new GameObject(type.Name); // 创建名称相同的 obj
                    _instance = obj.AddComponent<T>(); // 添加 _instance
                }
                else
                    _instance = obj.GetComponent<T>(); // 获取 _instance

                DontDestroyOnLoad(obj.transform.root); // 过场景不移除
            }

            return _instance;
        }
    }

    public string portName = "COM3";
    public int baudRate = 9600;

    private SerialPort _serialPort; // 传感器端口
    private Thread _sensorListener; // 监听线程

    [SerializeField] private bool _isRunning = false; // 线程是否运行

    public bool IsRunning => _isRunning;

    [SerializeField] protected float _nowValue, _lastValue; // 传感器读取的当前值和上一次的值

    [SerializeField] protected float _startValue; // 设置传感器初始值，供其他逻辑使用

    private volatile bool _connectThreadDone = false; // 后台线程是否完成
    private Thread _connectThread; // 后台连接线程

    private void OnEnable()
    {
        Debug.Log(this.GetType().Name);
        // 所有串口操作在后台线程中执行，主线程最多等待 3 秒
        StartCoroutine(InitSensorWithTimeout(3f));
    }

    /// <summary>
    /// 在后台线程中尝试连接传感器，主线程等待最多 maxWaitTime 秒
    /// </summary>
    private IEnumerator InitSensorWithTimeout(float maxWaitTime)
    {
        string filePath = typeof(T).Name + ".txt";
        string savedPort = TxtMgr.Instance.Load(filePath);

        _connectThreadDone = false;
        bool portFound = false;

        // 在后台线程中执行所有阻塞的串口操作
        _connectThread = new Thread(() =>
        {
            try
            {
                // 先尝试文件中保存的端口
                if (savedPort != default && TryConnectPort(savedPort))
                {
                    portFound = true;
                    _connectThreadDone = true;
                    return;
                }

                // 扫描 COM1-COM15
                for (int i = 1; i <= 15; i++)
                {
                    if (_connectThreadDone) break; // 被主线程超时中止
                    if (TryConnectPort($"COM{i}"))
                    {
                        portFound = true;
                        break;
                    }
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception e)
            {
                Debug.LogWarning($"{typeof(T).Name}: 连接异常: {e.Message}");
            }
            _connectThreadDone = true;
        });
        _connectThread.IsBackground = true;
        _connectThread.Start();

        // 主线程等待，最多 maxWaitTime 秒
        float elapsed = 0f;
        while (!_connectThreadDone && elapsed < maxWaitTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!_connectThreadDone)
        {
            // 超时，中止后台线程
            _connectThreadDone = true; // 通知线程停止循环
            try { _connectThread.Abort(); } catch { }
            try { if (_serialPort != null && _serialPort.IsOpen) _serialPort.Close(); } catch { }
            Debug.LogWarning($"{typeof(T).Name}: 传感器连接超时（{maxWaitTime}秒），已停止尝试连接");
            yield break;
        }

        if (portFound && _serialPort != null && _serialPort.IsOpen)
        {
            // 连接成功，在主线程上完成 Unity 相关设置
            portName = _serialPort.PortName;
            TxtMgr.Instance.Save(filePath, portName);
            _isRunning = true;

            _sensorListener = new Thread(SensorListener) { IsBackground = true };
            _sensorListener.Start();
            Debug.Log($"{typeof(T).Name}: {portName} has opened!");

            Invoke(nameof(SetStartValueDelay), 1f);
        }
        else
        {
            Debug.LogWarning($"{typeof(T).Name}: No port found!");
        }
    }

    /// <summary>
    /// 在后台线程中尝试连接指定端口（仅做连接验证，不执行 Unity 操作）
    /// </summary>
    private bool TryConnectPort(string port)
    {
        const int maxRetries = 3;
        const int readTimeoutMs = 500; // 每次读取最多等 500ms

        try
        {
            var sp = new SerialPort(port, baudRate);
            sp.ReadTimeout = readTimeoutMs;

            if (sp.IsOpen) return false;

            sp.Open();

            string validData = null;
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    var input = sp.ReadLine();
                    if (IsValidString(input))
                    {
                        validData = input;
                        break;
                    }
                }
                catch (TimeoutException) { }
            }

            if (validData == null)
            {
                sp.Close();
                return false;
            }

            _serialPort = sp;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private void OnDisable()
    {
        _isRunning = false;
        _connectThreadDone = true; // 确保后台连接线程也停止

        try { _connectThread?.Abort(); } catch { }
        try { if (_serialPort != null && _serialPort.IsOpen) _serialPort.Close(); } catch { }

        _sensorListener?.Abort();
    }

    protected virtual void Update()
    {
    }

    /// <summary>
    /// 端口监听事件
    /// </summary>
    void SensorListener()
    {
        while (_isRunning)
        {
            // 如果线程可以运行
            try
            {
                // 尝试从端口获取值
                string input = ProcessString(_serialPort.ReadLine()); // 数据预处理
                _lastValue = _nowValue; // 更新上次的值
                _nowValue = Convert.ToSingle(input); // 读数
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }

    /// <summary>
    /// 用于延时记录传感器初始读数
    /// </summary>
    protected void SetStartValueDelay()
    {
        _startValue = _nowValue;
    }

    /// <summary>
    /// 处理传感器读取的字符串，变成全为数字的字符串（由子类实现）
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    protected abstract string ProcessString(string input);

    protected abstract bool IsValidString(string input);

    public abstract float Delta { get; }
    public abstract float Value { get; }
}