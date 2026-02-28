using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishedState : BaseState
{
    public FinishedState(Main main) {
        this.main = main;
    }
    
    public override void Enter() {
        if (main.statusType != StateType.Finished) {
            main.ExistCurrentStatus();
            main.statusType = StateType.Finished; // 更改当前状态
        }
        
        // graph：暂停绘制
        DataSetting.Instance.graphMgr.Freeze();

        // 设置交互
        main.buttonEffect.highlighted = true;
    }
    
    public override void Update() {
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
