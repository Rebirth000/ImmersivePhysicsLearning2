using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteAlways]
public class SpringMove : MonoBehaviour
{
    public BlockSpringCouple couple;

    public BlockMove blockA;
    public BlockMove blockB;

    [SerializeField]
    private float   _startLength;
    public  Vector3 startLocalScale;
    public  Vector3 startPos;

    public float StartLength { // 在 couple 中提供 DeltaLength，是因为无法同步，计算会有一帧的延迟
        get => _startLength / couple.moveRatio;
    }

    void Awake() {
        couple = DataSetting.GetComponent<BlockSpringCouple>(transform.parent);

        blockA = DataSetting.GetComponentFromChild<BlockMove>(transform.parent, "BlockA");
        blockB = DataSetting.GetComponentFromChild<BlockMove>(transform.parent, "BlockB");
    }

    private void Start() {
        _startLength    = CalculateLength();
        startLocalScale = transform.localScale;
        startPos        = transform.localPosition;
    }

    // Update is called once per frame
    void Update() {
        float length = CalculateLength();

        float springPosZ = (blockA.transform.localPosition.z + blockB.transform.localPosition.z) / 2;
        float scale      = length / _startLength * startLocalScale.x;
        transform.localPosition = startPos.Set(x: 0, z: springPosZ);
        transform.localScale    = startLocalScale.Set(x: scale);
    }

    public float CalculateLength() {
        return Mathf.Abs(blockB.transform.localPosition.z - blockA.transform.localPosition.z)
             - blockA.BoxSize.z - 0.03f; // 0.3f 为精密计算的结果
    }
}