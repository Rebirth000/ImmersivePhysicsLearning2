using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

[ExecuteAlways]
public class Arrow : MonoBehaviour
{
    public Line     line;
    public Triangle triangle;

    public bool flip = false; // 是否反向

    private float _value;

    public float maxLength = 1;

    public float lineLength;

    [Range(0, 0.02f)]
    public float lineWidth;

    [Range(0.02f, 0.1f)]
    public float arrowLength;

    [Range(0f, 0.02f)]
    public float arrowWidth;

    public TextMeshPro text;

    public string  preFix;
    public string  sufFix;
    public Vector3 offset;

    private void Awake() {
        line     = DataSetting.GetComponentFromChild<Line>(transform, "Line");
        triangle = DataSetting.GetComponentFromChild<Triangle>(transform, "Triangle");
        text     = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "TextValue");
    }

    // Update is called once per frame
    void Update() {
        int   flipFlag = flip ? -1 : 1;
        float length   = lineLength * flipFlag;

        line.Thickness = lineWidth;
        line.End       = length * Vector3.right;

        float ratio     = Mathf.Clamp(length / arrowLength, -1, 1);
        float addWidth  = arrowWidth * Mathf.Sign(length) * ratio;
        float addLength = arrowLength * ratio;

        triangle.A = new Vector3(length, lineWidth * 0.5f + addWidth, 0);
        triangle.B = new Vector3(length + addLength, 0, 0);
        triangle.C = new Vector3(length, -lineWidth * 0.5f - addWidth, 0);

        text.text = _value == 0 ? "" : $"{preFix}{Mathf.Abs(_value):F2}{sufFix}";

        text.transform.localPosition = new Vector3(length, 0, 0) + offset;
    }

    public void SetValue(float value, float maxValue) {
        _value = value;
        if (maxValue == 0)
            return;
        lineLength = _value / maxValue * maxLength;
    }
}