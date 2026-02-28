using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningState : BaseState
{
    public RunningState(Main main) {
        this.main = main;
    }
    
    public override void Enter() {
        if (main.statusType != StateType.Running) {
            main.ExistCurrentStatus();
            main.statusType = StateType.Running; // 更改当前状态
        }

        // 取消拉动速度的显示
        main.textPullVelocity.enabled = false;

        // 设置交互
        main.buttonEffect.highlighted = true;
        main.knobEffect.highlighted = true;
    }
    
    public override void Update() {
        float lastTime = main.moveTime;
        float nowTime  = main.moveTime + Time.deltaTime;

        // 更新 couple 位置并计时
        DataSetting.Instance.couple.SetMoveTime(nowTime);
        main.moveTime = nowTime;

        DataSetting.Instance.graphMgr.SetHighlightPoint(main.moveTime);
        
        // graph：进行绘制
        float startTime = (int) (lastTime / main.xAxisUnit) * main.xAxisUnit;
        float endTime   = (int) (nowTime / main.xAxisUnit) * main.xAxisUnit;
        for (float t = startTime; t < endTime; t += main.xAxisUnit) {
            DataSetting.Instance.graphMgr.AddTime(t);
        }

        DataSetting.Instance.graphMgr.DrawGraph();
        
        SwitchState();
    }

    public override void Exist() {
        // 取消交互
        main.buttonEffect.highlighted = false;
        main.knobEffect.highlighted = false;
    }
    
    // 检查切换状态的方法
    public override void SwitchState() {
        if (AngleInput.Instance.Delta != 0) {
            // Running --> Controlling
            EventMgr.Instance.EventTrigger(nameof(MainEventType.EnterControllingStatus));
        }
    }
}
