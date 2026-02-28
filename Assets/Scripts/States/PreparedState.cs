using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparedState : BaseState
{
    public PreparedState(Main main)
    {
        this.main = main;
    }

    public override void Enter()
    {
        if (main.statusType != StateType.Prepared)
        {
            main.ExistCurrentStatus();
            main.statusType = StateType.Prepared; // 更改当前状态
        }

        // couple：重置位置、停止移动、取消弹簧连接
        DataSetting.Instance.couple.StopMove();
        DataSetting.Instance.couple.DisableJoint();
        DataSetting.Instance.couple.ResetPos();

        // graph：高亮点显示
        DataSetting.Instance.graphMgr.HideHighlightPoints();

        // graph：重置绘制
        // DataSetting.Instance.graphMgr.Reset();

        // couple：显示箭头数据
        DataSetting.Instance.couple.HideArrows();

        // baffle：重置位置
        DataSetting.Instance.baffleMove.MoveTo(main.baffleTargetPos, 5, BaffleMove.EMoveType.Lerp);

        // 设置参数
        main.moveTime = 0;                                      // 运动时间清零
        DataSetting.Instance.couple.SetMoveTime(main.moveTime); // 应用 moveTime
        DataSetting.Instance.couple.HideArrows();               // 准备阶段不显示箭头数据
        DataSetting.Instance.panel.SetStartSpeed(0);            // 更新初速度

        // 激活交互
        main.blockBEffect.highlighted = true;
        main.menuEffect.highlighted   = true;

        if (main.interactionType == InteractionType.VirtualHands)
        {
            main.cubeGrab.enabled             = true;
            main.cubeGrabEvent.enabled        = true;
            main.cubeGrabInteractable.enabled = true;
        }

        // 进入准备阶段，重置 textPullVelocity 显示和 IsPulling
        main.textPullVelocity.enabled = false;
        PullInput.Instance.IsPulling  = false;
    }

    public override void Update()
    {
        SwitchState();
    }

    public override void Exist()
    {
        // 取消交互
        main.blockBEffect.highlighted = false;
        main.menuEffect.highlighted   = false;

        DataSetting.Instance.graphMgr.Reset();
    }

    // 检查切换状态的方法
    public override void SwitchState()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // couple：给 block 设置初速度
            DataSetting.Instance.couple.SetPreSpeed(main.block, main.maxSpeed);

            // panel：更新面板信息
            DataSetting.Instance.panel.SetStartSpeed(main.maxSpeed);

            // DataSetting.Instance.couple.SetPreSpeed(BlockSpringCouple.EBlock.B, 10);
            DataSetting.Instance.couple.ShowArrows();

            // Prepared --> Running
            EventMgr.Instance.EventTrigger(nameof(MainEventType.EnterRunningStatus));
        }

        if (PullInput.Instance.IsPulling)
        {
            // EventMgr.Instance.EventTrigger(nameof(EEventType.EnterPullingStatus));
            main.OnSpringPullEnter();
        }
    }
}