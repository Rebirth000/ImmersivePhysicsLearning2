using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class LineBar : MonoBehaviour
{
    public Rectangle   bar;
    public Line        line1;
    public Line        line2;
    public TextMeshPro textCenter;
    public TextMeshPro textRight;

    public float barHeight;
    public float barPosY;

    public Vector3 targetPos1;
    public Vector3 targetPos2;

    public  Color darkColor = Color.gray;
    [SerializeField]
    private Color _barColor;

    private void Awake() {
        bar        = DataSetting.GetComponentFromChild<Rectangle>(transform, "Bar");
        line1      = DataSetting.GetComponentFromChild<Line>(transform, "Line1");
        line2      = DataSetting.GetComponentFromChild<Line>(transform, "Line2");
        textCenter = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "TextCenter");
        textRight  = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "TextRight");
        _barColor  = bar.Color;
    }

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() {
        var localPos   = transform.localPosition;
        var offsetPosY = Mathf.Min(0, barHeight);

        transform.localPosition            = localPos.Set(y: barPosY + offsetPosY);
        textCenter.transform.localPosition = localPos.Set(x: bar.Width / 2, y: barHeight / 2 - offsetPosY);
        textRight.transform.localPosition  = localPos.Set(x: bar.Width + 0.02f, y: barHeight - offsetPosY);

        bar.Height = Mathf.Abs(barHeight);

        line1.Start = Vector3.zero.Set(y: -offsetPosY);
        line1.End   = transform.InverseTransformPoint(targetPos1);
        line2.Start = Vector3.up * (barHeight - offsetPosY);
        line2.End   = transform.InverseTransformPoint(targetPos2);

        if (barHeight < 0)
            bar.Color = darkColor;
        else
            bar.Color = _barColor;
    }

    public void Show() {
        bar.gameObject.SetActive(true);
        line1.gameObject.SetActive(true);
        line2.gameObject.SetActive(true);
        textCenter.gameObject.SetActive(true);
        textRight.gameObject.SetActive(true);
    }

    public void Hide() {
        bar.gameObject.SetActive(false);
        line1.gameObject.SetActive(false);
        line2.gameObject.SetActive(false);
        textCenter.gameObject.SetActive(false);
        textRight.gameObject.SetActive(false);
    }
}