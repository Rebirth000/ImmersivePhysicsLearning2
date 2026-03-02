using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllingState : BaseState
{
    public ControllingState(Main main) {
        this.main = main;
    }
    
    public override void Enter() {
        if (main.statusType != StateType.Controlling) {
            main.ExistCurrentStatus();
            main.statusType = StateType.Controlling; // 更改当前状态
        }

        // 设置交互
        main.buttonEffect.highlighted = true;
    }
    
    public override void Update() {
        float deltaAngle = main.interactionType switch {
            InteractionType.VirtualHands => main.knobAngle.Delta,
            InteractionType.RealObjects  => -AngleInput.Instance.Delta, // 负号保证角度旋转方向和物块运动方向一致
            _                            => throw new ArgumentOutOfRangeException()
        } * main.angleRatio;
        if (deltaAngle != 0) {
            // 两帧之间的时间变化
            float lastTime = main.moveTime;
            float nowTime  = main.moveTime + deltaAngle / 360; // 表示转一圈代表 1 s
            main.moveTime = nowTime <= 0 ? 0 : nowTime;

            DataSetting.Instance.couple.SetMoveTime(main.moveTime); // 确保 nowTime >= 0
            
            DataSetting.Instance.graphMgr.SetHighlightPoint(main.moveTime);

            if (lastTime >= 0 || nowTime >= 0) {
                // 确保 lastTime - nowTime 中间有 >=0 的点需要绘制
                // graph：进行绘制
                float startTime = (int) (lastTime / main.xAxisUnit) * main.xAxisUnit;
                float endTime   = (int) (nowTime / main.xAxisUnit) * main.xAxisUnit;
                float deltaTime = Mathf.Sign(nowTime - lastTime) * main.xAxisUnit;
                for (float t = startTime;
                     t >= 0 && ((deltaAngle > 0 && t < endTime) || (deltaAngle < 0 && t > endTime));
                     t += deltaTime) {
                    // 线性采样，绘制图像
                    DataSetting.Instance.graphMgr.AddTime(t);
                }

                // DataSetting.Instance.velocityGraph.DrawGraph();
                
                DataSetting.Instance.graphMgr.DrawGraph();
            }
        }
        
        SwitchState();
    }

    public override void Exist() {
        // 取消交互
        main.buttonEffect.highlighted = false;
    }
    
    // 检查切换状态的方法
    public override void SwitchState() {
        
    }
}
