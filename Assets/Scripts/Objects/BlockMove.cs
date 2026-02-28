using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 使物体速度呈三角函数运动
/// </summary>
public partial class BlockMove : MonoBehaviour
{
    [Header("Move Function: V = Asin(wt+f)+B or cos(wt+f)+B")]
    public bool isMoving = false; // 是否正在通过代码控制移动

    public BlockSpringCouple couple;

    public float maxMass = 5;
    public float minMass = 1;
    // public float moveRatio = 0.1f; // 运动的比例系数

    public Vector3   moveDir  = Vector3.forward; // 移动方向
    public EMoveType moveType = EMoveType.Cos;   // 移动类型

    public new BoxCollider collider;

    [SerializeField]
    private float _moveX = 0; // 移动位移 x

    [Range(0, 10)] [SerializeField]
    private float t;

    [SerializeField]
    public float A, w, f, B; // 运动参数

    [SerializeField]
    private Rigidbody _body;

    [SerializeField]
    private Vector3 _startPos; // 初始位置

    public float Mass { // 关联 body 质量
        get => _body.mass;
        set => _body.mass = Mathf.Clamp(value, minMass, maxMass);
    }

    public float MoveTime => t;

    public float MovePos {
        get => CalculatePos(t);
    }

    public float MoveVelocity { // 根据公式获取速度
        get => isMoving ? CalculateVelocity(this.t) : 0;
    }

    public float MoveForce { // 根据公式获取受力
        get => isMoving ? CalculateForce(this.t) : 0;
    }

    public float MoveMomentum { // 动量
        get => Mass * MoveVelocity;
    }

    public float MoveKineticEnergy { // 动能
        get => isMoving ? CalculateKineticEnergy(this.t) : 0;
    }

    public float MaxVelocity { // 最大速度
        get => A + B;
    }

    public float MaxForce {
        get => Mass * A * w;
    }

    public Vector3 BoxSize {
        get => collider.bounds.size;
    }

    private void Awake() {
        _body    = DataSetting.GetComponent<Rigidbody>(transform);
        collider = DataSetting.GetComponentFromChild<BoxCollider>(transform, "Model/Block");
        couple   = DataSetting.GetComponent<BlockSpringCouple>(transform.parent);
    }

    // Start is called before the first frame update
    void Start() {
        _startPos = transform.position; // 设置初始位置
    }

    // Update is called once per frame
    void Update() {
        if (isMoving) {
            _moveX = CalculatePos(t);
            Vector3 targetPos = _startPos + _moveX * moveDir;
            _body.MovePosition(targetPos);
        }
    }

    /// <summary>
    /// 设置当前移动进度
    /// </summary>
    /// <param name="t">已运动的时间</param>
    public void SetMoveTime(float t) {
        this.t = t;
    }

    /// <summary>
    /// 设置参数
    /// </summary>
    /// <param name="A">参数 A</param>
    /// <param name="w">参数 w</param>
    /// <param name="f">参数 f</param>
    /// <param name="B">参数 B</param>
    /// <param name="type">移动类型</param>
    public void SetParameter(float A, float w, float B = 0, float f = 0, EMoveType type = EMoveType.Cos) {
        this.A   = A;
        this.w   = w;
        this.f   = f;
        this.B   = B;
        moveType = type;
        isMoving = true;
    }

    /// <summary>
    /// 停止移动
    /// </summary>
    /// <param name="setVelocityToZero">是否使物体停止</param>
    public void StopMove(bool setVelocityToZero = true) {
        isMoving = false;
        if (setVelocityToZero) _body.velocity = Vector3.zero;
        else _body.velocity                   = CalculateVelocity(this.t) * couple.moveRatio * moveDir; // 如果不停止，则设置 body 速度，继续运动下去

        t = 0;
    }

    /// <summary>
    /// 依据运动时间计算当前位置
    /// </summary>
    /// <param name="t">运动时间</param>
    /// <returns></returns>
    public float CalculatePos(float t) {
        switch (moveType) {
            case EMoveType.Sin:
                return (A / w * -Mathf.Cos(w * t + f) + B * t + A / w * Mathf.Cos(f)) * couple.moveRatio;
            case EMoveType.Cos:
                return (A / w * Mathf.Sin(w * t + f) + B * t) * couple.moveRatio;
            default:
                return 0;
        }
    }

    /// <summary>
    /// 依据运动时间计算当前速度
    /// </summary>
    /// <param name="t">运动时间</param>
    /// <returns></returns>
    public float CalculateVelocity(float t) {
        switch (moveType) {
            case EMoveType.Sin:
                return A * Mathf.Sin(w * t + f) + B;
            case EMoveType.Cos:
                return A * Mathf.Cos(w * t + f) + B;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// 依据运动时间计算当前受力
    /// </summary>
    /// <param name="t">运动时间</param>
    /// <returns></returns>
    public float CalculateForce(float t) {
        switch (moveType) {
            case EMoveType.Sin:
                return Mass * A * w * Mathf.Cos(w * t + f);
            case EMoveType.Cos:
                return -Mass * A * w * Mathf.Sin(w * t + f);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// 依据运动时间计算当前动量
    /// </summary>
    /// <param name="t">运动时间</param>
    /// <returns></returns>
    public float CalculateMomentum(float t) {
        return CalculateVelocity(t) * Mass;
    }

    /// <summary>
    /// 依据运动时间计算当前动能 Ek
    /// </summary>
    /// <param name="t">运动时间</param>
    /// <returns></returns>
    public float CalculateKineticEnergy(float t) {
        float v = CalculateVelocity(t);
        return 0.5f * Mass * v * v;
    }
}


public partial class BlockMove
{
    public enum EMoveType
    {
        Sin,
        Cos
    }
}