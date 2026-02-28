using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullingState : BaseState
{
    public PullingState(Main main) {
        this.main = main;
    }

    public override void Enter() {
        if (main.statusType != StateType.Pulling) {
            main.ExistCurrentStatus();
            main.statusType = StateType.Pulling; // 更改当前状态
        }

        // 进入拉动状态时，先记录手的初始位置
        main.pullHandStartPos = main.interactionType switch {
            InteractionType.VirtualHands => DataSetting.Instance.blockB.transform.position,
            InteractionType.RealObjects  => main.rightHandAnchorDetached.position,
            _                            => throw new ArgumentOutOfRangeException()
        };
        main.pullHandLastPos = main.pullHandStartPos;
    }

    public override void Update() {
        // 让物块随手移动
        main.pullHandNowPos = main.interactionType switch {
            InteractionType.VirtualHands => DataSetting.Instance.blockB.transform.position,
            InteractionType.RealObjects  => main.rightHandAnchorDetached.position,
            _                            => throw new ArgumentOutOfRangeException()
        };
        DataSetting.Instance.blockB.transform.Translate(
            new Vector3(main.pullHandNowPos.x - main.pullHandLastPos.x, 0, 0), 
            Space.World);
        main.pullHandLastPos = main.pullHandNowPos;

        //TODO 小 bug：第一次拉动到最大值时会显示 0
        // float moveDis = DataSetting.Instance.blockB.transform.InverseTransformVector(main.pullHandNowPos - main.pullHandStartPos).x;
        float moveDis = main.pullHandNowPos.x - main.pullHandStartPos.x;
        float speed = main.interactionType switch {
            InteractionType.VirtualHands => moveDis / main.blockBTranslateTransformer.Constraints.MaxZ.Value * main.maxSpeed,
            InteractionType.RealObjects  => PullInput.Instance.NormedMaxValue * main.maxSpeed,
            _                            => throw new ArgumentOutOfRangeException()
        };
        main.textPullVelocity.text = $"{speed:F2}m/s";

        SwitchState();
    }

    public override void Exist() {
        if (main.interactionType == InteractionType.VirtualHands) {
            main.cubeGrab.enabled = false;
            main.cubeGrabEvent.enabled = false;
            main.cubeGrabInteractable.enabled = false;
        }
        
        // graph：高亮点显示
        DataSetting.Instance.graphMgr.ShowHighlightPoints();

        // couple：显示箭头数据
        DataSetting.Instance.couple.ShowArrows();
    }

    // 检查切换状态的方法
    public override void SwitchState() { }
}