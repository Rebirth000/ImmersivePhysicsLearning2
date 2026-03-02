using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

[ExecuteAlways]
public class Y_Axis : Axis
{
    public const float Tolerance = 0.1f; // 坐标轴绘制的误差容忍度

    public override float WorldLength => rectangle.Height; // Y 轴长度

    public Line unitLineUp;   // Y 轴单位线（上方）
    public Line unitLineDown; // Y 轴单位线（下方）

    [Range(-1, 1)]
    public float centerPos = 0f;

    public float ValueRange => maxIndex - minIndex; // Y 轴数值范围

    public GraphDrawer drawer; // 属于哪个 drawer

    public float UpLineLength   => WorldLength * (1 - centerPos) / 2;
    public float DownLineLength => WorldLength * (1 + centerPos) / 2;

    protected override void Awake() {
        drawer = DataSetting.GetComponent<GraphDrawer>(transform.parent.parent);
    }

    private void Update() {
        float halfWorldLength = WorldLength / 2;

        triangle.transform.localPosition     = Vector3.up * halfWorldLength;
        drawer.xAxis.transform.localPosition = Vector3.up * (halfWorldLength * centerPos);

        if (!Application.isPlaying) {
            maxIndex = 1 - centerPos;
            minIndex = -(centerPos + 1);
        }

        unitLineUp.Start   = unitLineUp.Start.Set(y: halfWorldLength * centerPos);
        unitLineUp.End     = unitLineUp.End.Set(y: halfWorldLength);
        unitLineDown.Start = unitLineDown.Start.Set(y: halfWorldLength * centerPos);
        unitLineDown.End   = unitLineDown.End.Set(y: -halfWorldLength);
    }

    public override void ReceiveNewValue(float y) {
        int isOut = IsOutOfRange(y); // 判断是否出界

        if (isOut == 1) {
            maxIndex = y;
            minIndex = (centerPos + 1) / (1 - centerPos) * -y;
        }
        else if (isOut == -1) {
            minIndex = y;
            maxIndex = (1 - centerPos) / (1 + centerPos) * -y;
        }
    }

    public override void DrawShapes(Camera cam) {
        // if (!cam.CompareTag("MainCamera")) return;
        using (Draw.Command(cam)) {
            float absMaxIndex = Mathf.Max(maxIndex, -minIndex);
            float absMinIndex = Mathf.Min(maxIndex, -minIndex);
            float unitValue   = GetUnitValue(absMaxIndex); // 依据 Y 轴上界获取单位长度
            float unitCount   = absMaxIndex / unitValue;   // 计算单位数量

            float unitWorldLength = Mathf.Max(UpLineLength, DownLineLength) / unitCount; // 每单位对应的 WorldLength
            unitLineUp.DashSpacing   = unitWorldLength - unitLineUp.DashSize;
            unitLineDown.DashSpacing = unitWorldLength - unitLineDown.DashSize;

            // 为了让 maxIndex = 9.9 时，坐标轴也显示出 1,2,3,4,5,6,7,8,9,10 的工作，对 unitCount 进行微调
            if (unitCount + Tolerance > unitCount % 1) unitCount += Tolerance;

            float   halfWorldLength = WorldLength / 2;
            Vector3 startPos        = Vector3.up * halfWorldLength * centerPos;

            if (isTextShown)                                             // 先画 0
                DrawTool.DrawText(startPos.Add(x: valueTextOffset), "0", // 横坐标 x 为 valueTextOffset
                                  textAlign, textColor, transform, fontSize, DataSetting.Instance.englishFont);

            // 绘制刻度
            for (int i = 1; i <= maxIndex / unitValue; i++) {
                Vector3 textPos = startPos.Add(x: valueTextOffset, y: i * unitWorldLength);
                Vector3 gridPos = startPos.Add(y: i * unitWorldLength);
                if (isTextShown)
                    DrawTool.DrawText(textPos, $"{unitValue * i:F1}",
                                      textAlign, textColor, transform, fontSize, DataSetting.Instance.englishFont);
                if (isGridShown) // 绘制网格
                    DrawTool.DrawDashedLine(gridPos,
                                            gridPos + Vector3.right * drawer.xAxis.WorldLength,
                                            gridLineColor, transform, gridLineWidth, DashSpace.Meters,
                                            0.02f, 0.02f, DashSnapping.EndToEnd);
            }

            // 绘制刻度
            for (int i = 1; i <= -minIndex / unitValue; i++) {
                Vector3 textPos = startPos.Add(x: valueTextOffset, y: -i * unitWorldLength);
                Vector3 gridPos = startPos.Add(y: -i * unitWorldLength);
                if (isTextShown) // 对称绘制坐标值
                    DrawTool.DrawText(textPos, $"{-unitValue * i:F1}",
                                      textAlign, textColor, transform, fontSize, DataSetting.Instance.englishFont);
                if (isGridShown) // 对称绘制网格
                    DrawTool.DrawDashedLine(gridPos,
                                            gridPos + Vector3.right * drawer.xAxis.WorldLength,
                                            gridLineColor, transform, gridLineWidth, DashSpace.Meters,
                                            0.02f, 0.02f, DashSnapping.EndToEnd);
            }
        }
    }

    /// <summary>
    /// 依据 max 找到对应的单位长度
    /// </summary>
    /// <param name="max"></param>
    /// <returns></returns>
    public float GetUnitValue(float max) {
        float unitValue = 0.01f;       // 从这个值开始找
        while (unitValue * 10 < max) { // 每次更新 unitValue，使得其为不超过 max 的最大单位
            unitValue *= 10;
        }

        return (max / unitValue) switch { // 依据 max 有多少个单位来反向决定单位长度
            > 5 => unitValue * 2,         // 例如，max = 9，unitValue = 1，则显示 2,4,6,8
            > 2 => unitValue,             // 例如，max = 30，unitValue = 10，则显示 10,20,30
            > 1 => unitValue / 2,         // 例如，max = 110，unitValue = 100，则显示 50,100
            _   => unitValue              // 这种情况不存在，因为 max > unitValue
        };
    }
}