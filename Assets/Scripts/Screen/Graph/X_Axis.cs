using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class X_Axis : Axis
{
    public override float WorldLength => rectangle.Width;

    public Line unitLine; // X 轴单位线

    public float unitValue = 0.2f; // X 轴单位长度

    public float valueRange = 4; // X 轴取值范围

    public GraphDrawer drawer; // 属于哪个 drawer

    protected override void Awake() {
        drawer = DataSetting.GetComponent<GraphDrawer>(transform.parent.parent);
    }

    private void Update() {
        triangle.transform.localPosition = Vector3.right * WorldLength / 2;

        minIndex = maxIndex - valueRange;
    }

    public override void ReceiveNewValue(float x) {
        int isOut = IsOutOfRange(x); // 判断是否出界
        if (isOut == 1) {            // 超出上界
            maxIndex = x;
            minIndex = maxIndex - valueRange; // X 轴的长度默认为 valueRange
        }
        else if (isOut == -1) {
            minIndex = x;
            maxIndex = minIndex + valueRange;
        }
    }

    public override void DrawShapes(Camera cam) {
        using (Draw.Command(cam)) {
            if (unitValue <= 0)
                unitValue = 0.01f;                     // 防止计算除 0
            float dashCount  = valueRange / unitValue; // 坐标轴单位个数
            float dashOffset = maxIndex / unitValue;   // 坐标轴单位间距
            unitLine.DashSize   = dashCount;
            unitLine.DashOffset = dashOffset;

            Vector3 pos      = Vector3.up * valueTextOffset;    // 纵坐标 y 为 valueTextOffset
            Vector3 gridPos  = Vector3.zero;                    // 网格起始位置
            float   maxValue = maxIndex - maxIndex % unitValue; // 比 maxIndex 小的最大整数

            // if (cam.)

            // 绘制刻度
            for (int i = 0; i < dashCount; i++) {
                gridPos.x = pos.x = WorldLength / 2 - (i + dashOffset - (int) dashOffset) * WorldLength / dashCount;
                if (isTextShown) // 绘制坐标值
                    DrawTool.DrawText(pos, $"{maxValue - i * unitValue:F1}",
                                      textAlign, textColor, transform, fontSize, DataSetting.Instance.englishFont);
                if (isGridShown) // 绘制网格
                    DrawTool.DrawDashedLine(gridPos.Add(y: drawer.yAxis.UpLineLength),
                                            gridPos.Add(y: -drawer.yAxis.DownLineLength),
                                            gridLineColor, transform, gridLineWidth, DashSpace.Meters,
                                            0.02f, 0.02f, DashSnapping.EndToEnd);
            }
        }
    }
}