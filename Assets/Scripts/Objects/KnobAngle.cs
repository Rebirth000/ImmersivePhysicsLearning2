using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnobAngle : MonoBehaviour
{
    public Transform pointer;
    
    public float tolerance = 0.5f; // 灵敏度

    private float _nowAngle;
    private float _lastAngle;

    public float Delta {
        get {
            float delta = _nowAngle - _lastAngle;
            
            if (Mathf.Abs(delta) < tolerance) return 0;
            if (Mathf.Abs(delta) > 180) return delta - Mathf.Sign(delta) * 360; // 严密推理
            return delta;
        }
    }

    void Awake() {
        pointer = DataSetting.GetComponentFromChild<Transform>(transform, "Up");
    }

    private void Start() { }

    // Update is called once per frame
    void Update() {
        _lastAngle = _nowAngle;
        _nowAngle = pointer.eulerAngles.y;
    }
}