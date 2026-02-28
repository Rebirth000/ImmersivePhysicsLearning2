using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    protected Main main;

    public abstract void Enter();

    public abstract void Update();

    public abstract void Exist();
    
    // 检查切换状态的方法
    public abstract void SwitchState();
}

/// <summary>
/// 程序状态
/// </summary>
public enum StateType
{
    Prepared,    // 准备状态：block 等待发射
    Pulling,     // 拉动状态：block 正在发射
    Running,     // 运行状态：block 进行无干预运动
    Controlling, // 用户操作状态：用户使用 slider 控制 block 运动
    Paused,      // 暂停状态：暂未实现
    Finished,    // 结束状态：block 运动到终点
}