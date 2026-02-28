using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Shapes;
using UnityEngine;

/// <summary>
/// 物块弹簧对
/// </summary>
public partial class BlockSpringCouple : MonoBehaviour
{
    public BlockMove blockA; // 物块 A
    public BlockMove blockB; // 物块 B

    public SpringJoint joint;

    public Vector3 direction;   // 移动方向
    public float   originalLen; // 弹簧原长

    public float maxK = 100;
    public float minK = 20;

    public float k = 1; // 劲度系数

    [Range(0, 1)]
    public float moveRatio = 0.1f; // 运动的比例系数

    [Range(0, 20)]
    public float moveTime;

    public Arrow forceArrowA;
    public Arrow velocityArrowA;
    public Arrow forceArrowB;
    public Arrow velocityArrowB;

    [SerializeField]
    private bool _showArrows = false;

    private Vector3 _startPosA; // 物块 A 初始位置
    private Vector3 _startPosB; // 物块 B 初始位置

    public bool IsShowArrows {
        get => _showArrows;
    }

    public float w {
        get => Mathf.Sqrt(k * (blockA.Mass + blockB.Mass) / (blockA.Mass * blockB.Mass));
    }

    private void Awake() {
        blockA         = DataSetting.GetComponentFromChild<BlockMove>(transform, "BlockA");
        blockB         = DataSetting.GetComponentFromChild<BlockMove>(transform, "BlockB");
        joint          = DataSetting.GetComponentFromChild<SpringJoint>(transform, "BlockA");
        forceArrowA    = DataSetting.GetComponentFromChild<Arrow>(transform, "BlockA/ForceArrow");
        velocityArrowA = DataSetting.GetComponentFromChild<Arrow>(transform, "BlockA/VelocityArrow");
        forceArrowB    = DataSetting.GetComponentFromChild<Arrow>(transform, "BlockB/ForceArrow");
        velocityArrowB = DataSetting.GetComponentFromChild<Arrow>(transform, "BlockB/VelocityArrow");

        // 记录初始位置
        _startPosA = blockA.transform.position;
        _startPosB = blockB.transform.position;

        // 计算并设置相关参数
        direction         = (_startPosA - _startPosB).normalized; // 移动方向
        blockA.moveDir    = blockB.moveDir = direction;
        originalLen       = (_startPosA - _startPosB).magnitude; // 弹簧原长
        joint.maxDistance = joint.minDistance = originalLen - blockA.BoxSize.z;
    }

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() {
        if (_showArrows) {
            forceArrowA.SetValue(blockA.MoveForce, blockA.MaxForce);
            velocityArrowA.SetValue(blockA.MoveVelocity, blockA.MaxVelocity);
            forceArrowB.SetValue(blockB.MoveForce, blockA.MaxForce);
            velocityArrowB.SetValue(blockB.MoveVelocity, blockB.MaxVelocity);
        }

        blockA.SetMoveTime(moveTime);
        blockB.SetMoveTime(moveTime);
    }

    /// <summary>
    /// 给物块 which 赋予初速度 speed
    /// </summary>
    /// <param name="which">哪个物块</param>
    /// <param name="speed">初速度</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void SetPreSpeed(EBlock which, float speed) {
        BlockMove chosenBlock = which switch { // 赋予初速度的物块
            EBlock.A => blockA,
            EBlock.B => blockB,
            _        => throw new ArgumentOutOfRangeException(nameof(which), which, null)
        };
        BlockMove anotherBlock = which switch { // 另一个物块
            EBlock.A => blockB,
            EBlock.B => blockA,
            _        => throw new ArgumentOutOfRangeException(nameof(which), which, null)
        };
        float w  = Mathf.Sqrt(k * (blockA.Mass + blockB.Mass) / (blockA.Mass * blockB.Mass));
        float vg = chosenBlock.Mass * speed / (blockA.Mass + blockB.Mass); // 共速时的速度 V共

        // 分别设置速度
        chosenBlock.SetParameter(speed - vg, w, vg, 0, BlockMove.EMoveType.Cos);
        anotherBlock.SetParameter(vg, w, vg, -Mathf.PI / 2, BlockMove.EMoveType.Sin);
    }

    /// <summary>
    /// 设置当前运动时间
    /// </summary>
    /// <param name="time">运动时间</param>
    public void SetMoveTime(float time) {
        moveTime = time;
    }

    /// <summary>
    /// 停止移动
    /// </summary>
    /// <param name="setVelocityToZero">是否使物体停止</param>
    public void StopMove(bool setVelocityToZero = true) {
        blockA.StopMove(setVelocityToZero);
        blockB.StopMove(setVelocityToZero);
    }

    /// <summary>
    /// 重置位置
    /// </summary>
    public void ResetPos() {
        blockA.transform.position = _startPosA;
        blockB.transform.position = _startPosB;
    }

    /// <summary>
    /// 获取物块 which 的速度
    /// </summary>
    /// <param name="which">选择获取哪个物块</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public float GetSpeed(EBlock which) {
        return which switch {
            EBlock.A => blockA.MoveVelocity,
            EBlock.B => blockB.MoveVelocity,
            _        => throw new ArgumentOutOfRangeException(nameof(which), which, null)
        };
    }

    /// <summary>
    /// 获取当前运动时间
    /// </summary>
    /// <returns></returns>
    public float GetMoveTime(EBlock which) {
        return moveTime;
    }

    public void EnableJoint() {
        joint.spring = k;
    }

    public void DisableJoint() {
        joint.spring = 0;
    }

    public void ShowArrows() {
        _showArrows = true;
        forceArrowA.gameObject.SetActive(true);
        forceArrowB.gameObject.SetActive(true);
        velocityArrowA.gameObject.SetActive(true);
        velocityArrowB.gameObject.SetActive(true);
    }

    public void HideArrows() {
        _showArrows = false;
        forceArrowA.gameObject.SetActive(false);
        forceArrowB.gameObject.SetActive(false);
        velocityArrowA.gameObject.SetActive(false);
        velocityArrowB.gameObject.SetActive(false);
    }

    public void AddBlockAMass(float f) {
        if (DataSetting.Instance.main.menuEffect.highlighted)
            blockA.Mass += f;
    }

    public void AddBlockBMass(float f) {
        if (DataSetting.Instance.main.menuEffect.highlighted)
            blockB.Mass += f;
    }

    public void AddSpringK(float f) {
        if (DataSetting.Instance.main.menuEffect.highlighted)
            k = Mathf.Clamp(k + f, minK, maxK);
    }
}

public partial class BlockSpringCouple
{
    public enum EBlock
    {
        A,
        B
    }
}