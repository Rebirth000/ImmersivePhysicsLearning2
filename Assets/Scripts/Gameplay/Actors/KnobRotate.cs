using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnobRotate : MonoBehaviour
{
    public Transform pointer;

    public float rotateSpeed = 10f; // 旋转速度

    public Vector3 eulerAngle;  // 当前角度
    public Vector3 targetAngle; // 目标角度

    void Awake() {
        pointer = DataSetting.GetComponentFromChild<Transform>(transform, "Up");
    }

    private void Start() { }

    // Update is called once per frame
    void Update() {
        eulerAngle = pointer.eulerAngles;
        targetAngle = new Vector3(270, 360 - AngleInput.Instance.Value, 0);
        pointer.rotation = Quaternion.Lerp(pointer.rotation, // 先快后慢跟随移动，目的是每帧都更新位置，达到丝滑的效果
                                           Quaternion.Euler(targetAngle),
                                           rotateSpeed * Time.deltaTime);
    }
}