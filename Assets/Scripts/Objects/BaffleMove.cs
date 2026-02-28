using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制挡板移动到某处
/// </summary>
public partial class BaffleMove : MonoBehaviour
{
    public float     moveSpeed = 10;
    public Vector3   targetPos;
    public EMoveType moveType;

    private float   t = 0;    // Lerp 函数中的 t 参数
    private Vector3 _startPos; // 初始位置

    // Start is called before the first frame update
    void Start() {
        _startPos = targetPos = transform.localPosition; // 初始位置不变
    }

    // Update is called once per frame
    void Update() {
        if (moveType == EMoveType.Lerp) { // 先快后慢移动
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, moveSpeed * Time.deltaTime);
        }
        else if (moveType == EMoveType.Uniform) { // 匀速移动
            if (transform.localPosition != targetPos && t >= 1) {
                t = 0;
                _startPos = targetPos;
            }
            if (t < 1) t += Time.deltaTime * moveSpeed;
            transform.localPosition = Vector3.Lerp(_startPos, targetPos, t);
        }
    }

    public void MoveTo(Vector3 targetPos, float moveSpeed, EMoveType moveType = EMoveType.Uniform) {
        this.targetPos = targetPos;
        this.moveSpeed = moveSpeed;
        this.moveType = moveType;
    }
}

public partial class BaffleMove
{
    public enum EMoveType
    {
        Lerp,   // 先快后慢 
        Uniform // 匀速移动
    }
}