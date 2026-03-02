using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

[ExecuteAlways]
public class GraphDrawer : MonoBehaviour
{
    public X_Axis          xAxis;     // X 轴
    public Y_Axis          yAxis;     // Y 轴
    public Rectangle       panelRect; // 绘图背景区域
    public List<GraphLine> lines;     // 包含的线条
    public bool            frozen = false;

    public Transform textTitle;
    public Transform lineLabel;

    public bool isShown = false;

    private void Awake() {
        xAxis     = DataSetting.GetComponentFromChild<X_Axis>(transform, "Coordinate/X_Axis");
        yAxis     = DataSetting.GetComponentFromChild<Y_Axis>(transform, "Coordinate/Y_Axis");
        panelRect = DataSetting.GetComponentFromChild<Rectangle>(transform, "DataPanel");
        textTitle = DataSetting.GetComponentFromChild<Transform>(transform, "TextTitle");
        lineLabel = DataSetting.GetComponentFromChild<Transform>(transform, "DataPanel/LineLabel");

        if (lines.Count == 0) {
            lines.Add(DataSetting.GetComponentFromChild<GraphLine>(transform, "DataPanel/LineA"));
            lines.Add(DataSetting.GetComponentFromChild<GraphLine>(transform, "DataPanel/LineB"));
        }
    }

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    /// <summary>
    /// 向第 lineIndex 条线添加点 (x,y)
    /// </summary>
    /// <param name="x">x 值</param>
    /// <param name="y">y 值</param>
    /// <param name="lineIndex">lines 中的哪条线</param>
    public void AddValue(float x, float y, int lineIndex = 0) {
        if (frozen) return; // 如果冻结，则不执行

        if (lineIndex >= lines.Count || lineIndex < 0) { // lineIndex 不合法，报错
            Debug.LogError("GraphDrawer: Line index out of range");
            return;
        }

        GraphLine line = lines[lineIndex]; // 向哪条线添加点
        xAxis.ReceiveNewValue(x);          // 更新 x 轴
        yAxis.ReceiveNewValue(y);          // 更新 y 轴

        int isOut = line.CheckXValueOut(x);

        if (isOut == 1) { // 添加的点在 X 轴右侧
            // 该循环用于删除超出 X 下界的点
            for (int i = 0; i < line.PointCount; i++) {
                GraphPoint point = line.GetPoint(i);
                if (point.xValue >= xAxis.minIndex) {            // 从左向右找到第一个在 X 轴范围内的点
                    if (i == 0) break;                           // 如果是第一个点，则结束操作
                    GraphPoint lastPoint = line.GetPoint(i - 1); // 最后一个超出 X 轴范围的点
                    float newY = lastPoint.yValue +
                                 (point.yValue - lastPoint.yValue) * (xAxis.minIndex - lastPoint.xValue) / (point.xValue - lastPoint.xValue);
                    line.SetPointValue(i - 1, xAxis.minIndex, newY);      // 将其位置设置到 X 轴下界处，Y 值设置为线性插值
                    for (int j = i - 1; j > 0; j--) { line.RemoveAt(0); } // 移除左边多余的点

                    break; // 后面的点都不再可能超出 X 轴下界，因此结束操作
                }
            }
        }
        else if (isOut == -1) { // 添加的点在 X 轴左侧
            // 该循环用于删除超出 X 上界的点
            for (int i = line.PointCount - 1; i >= 0; i--) {
                GraphPoint point = line.GetPoint(i);
                if (point.xValue <= xAxis.maxIndex) {            // 从右向左找到第一个在 X 轴范围内的点
                    if (i == line.PointCount - 1) break;         // 如果是最后一个点，则结束操作
                    GraphPoint lastPoint = line.GetPoint(i + 1); // 第一个超出 X 轴范围的点
                    float newY = lastPoint.yValue +
                                 (point.yValue - lastPoint.yValue) * (xAxis.maxIndex - lastPoint.xValue) / (point.xValue - lastPoint.xValue);
                    line.SetPointValue(i + 1, xAxis.maxIndex, newY);                                          // 将其位置设置到 X 轴下界处，Y 值设置为线性插值
                    for (int j = line.PointCount - i - 2; j > 0; j--) { line.RemoveAt(line.PointCount - 1); } // 移除右边多余的点

                    break; // 前面的点都不再可能超出 X 轴上界，因此结束操作
                }
            }
        }

        line.AddValue(x, y, xAxis.GetPosFromValue(x), yAxis.GetPosFromValue(y)); // 添加点 (x,y)

        // 更新所有点的 WorldPos
        line.UpdatePos(xAxis, yAxis);
    }

    public void SetHighlightPoint(float x, float y, int lineIndex = 0) {
        if (frozen) return; // 如果冻结，则不执行

        if (lineIndex >= lines.Count || lineIndex < 0) { // lineIndex 不合法，报错
            Debug.LogError("GraphDrawer: Line index out of range");
            return;
        }

        GraphLine line = lines[lineIndex]; // 向哪条线添加点
        xAxis.ReceiveNewValue(x);          // 更新 x 轴
        yAxis.ReceiveNewValue(y);          // 更新 y 轴
        line.SetHighlightPoint(x, y, xAxis.GetPosFromValue(x), yAxis.GetPosFromValue(y));
    }

    public void DrawGraph() {
        foreach (GraphLine line in lines) {
            line.DrawLine();
        }
    }

    public void ShowAxis() {
        xAxis.Show();
        yAxis.Show();
    }

    public void HideAxis() {
        xAxis.Hide();
        yAxis.Hide();
    }

    public void ShowGrid() {
        xAxis.ShowGrid();
        yAxis.ShowGrid();
    }

    public void HideGrid() {
        xAxis.HideGrid();
        yAxis.HideGrid();
    }

    public void ShowTitle() {
        textTitle.gameObject.SetActive(true);
    }

    public void HideTitle() {
        textTitle.gameObject.SetActive(false);
    }

    public void ShowLineLabel() {
        lineLabel.gameObject.SetActive(true);
    }
    
    public void HideLineLabel() {
        lineLabel.gameObject.SetActive(false);
    }

    public void ShowHighlightPoints() {
        foreach (GraphLine line in lines) {
            line.ShowHighlightPoint();
        }
    }

    public void HideHighlightPoints() {
        foreach (GraphLine line in lines) {
            line.HideHighlightPoint();
        }
    }

    public void Show() {
        isShown = true;
        foreach (GraphLine line in lines) {
            line.ShowLine();
        }

        ShowAxis();
        ShowPanel();
        ShowTitle();
        ShowLineLabel();
    }

    public void Hide() {
        isShown = false;
        foreach (GraphLine line in lines) {
            line.HideLine();
        }

        HideAxis();
        HidePanel();
        HideTitle();
        HideLineLabel();
    }

    public void ShowPanel() {
        panelRect.enabled = true;
    }

    public void HidePanel() {
        panelRect.enabled = false;
    }

    public void Reset() {
        xAxis.Reset();
        yAxis.Reset();
        for (int i = 0; i < lines.Count; i++) {
            lines[i].Clear();
        }

        if (frozen) Unfreeze();
    }

    public void Freeze() {
        frozen = true;
    }

    public void Unfreeze() {
        frozen = false;
    }
}