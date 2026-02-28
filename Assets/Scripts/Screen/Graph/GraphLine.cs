using System;
using System.Collections.Generic;
using Shapes;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public partial class GraphLine : ImmediateModeShapeDrawer
{
    public LineRenderer lineRenderer; // 画线的 LineRenderer

    public List<GraphPoint> points; // 缓存点，存储点的信息

    public bool isHighlightPointShown = false; // 是否画线

    public GraphPoint highlightPoint;             // 高亮点
    public float      highlightPointSize = 0.02f; // 高亮点显示的大小
    public Color      highlightPointColor;        // 高亮点的颜色

    public HighlightPointValueTextInfo xAxisTextInfo; // 高亮点到 x 轴的文本信息
    public HighlightPointValueTextInfo yAxisTextInfo; // 高亮点到 y 轴的文本信息

    public GraphDrawer drawer; // 属于哪个 drawer

    [Range(0, 1f)]
    public float lineWidth = 0.05f; // 线宽

    public int PointCount => points.Count; // 点数量

    private void Awake() {
        lineRenderer = DataSetting.GetComponentFromChild<LineRenderer>(transform, "LineRenderer");

        drawer = DataSetting.GetComponent<GraphDrawer>(transform.parent.parent);
    }

    private void Start() {
        useCullingMasks       = true;
        isHighlightPointShown = false;

        highlightPoint = new GraphPoint();
    }

    // Update is called once per frame
    void Update() {
        lineRenderer.startWidth = lineRenderer.endWidth = lineWidth; // 实时更新线宽
    }

    /// <summary>
    /// 将缓存的点绘制到 LineRenderer 上
    /// </summary>
    public void DrawLine() {
        lineRenderer.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++) {
            lineRenderer.SetPosition(i, points[i].pos);
        }
    }

    public void Show() {
        isHighlightPointShown = true;
        lineRenderer.enabled  = true;
    }

    public void Hide() {
        isHighlightPointShown = false;
        lineRenderer.enabled  = false;
    }
    
    public void ShowLine() {
        lineRenderer.enabled  = true;
    }

    public void HideLine() {
        lineRenderer.enabled  = false;
    }
    
    public void ShowHighlightPoint() {
        isHighlightPointShown = true;
    }

    public void HideHighlightPoint() {
        isHighlightPointShown = false;
    }

    /// <summary>
    /// 添加点，并实时更新
    /// </summary>
    /// <param name="y">y 值</param>
    public void AddValue(float x, float y, float posX, float posY) {
        // highlightPoint.SetValue(x, y);
        int isOut = CheckXValueOut(x);
        if (isOut == 1)
            points.Add(new GraphPoint { xValue = x, yValue = y, pos = new Vector3(posX, posY, 0) });
        else if (isOut == -1)
            points.Insert(0, new GraphPoint { xValue = x, yValue = y, pos = new Vector3(posX, posY, 0) });
        else
            Debug.LogWarning("GraphLine: x of the New Value is within XAxis range!");
    }

    public void SetHighlightPoint(float x, float y, float posX = 0, float posY = 0) {
        highlightPoint.SetValue(x, y, posX, posY);
    }

    /// <summary>
    /// 检查 x 是否超出目前存储点的范围
    /// 返回值：1 表示超出右侧，-1 表示超出左侧，0 表示在范围内
    /// </summary>
    /// <param name="x">待检查的横坐标 x</param>
    /// <returns></returns>
    public int CheckXValueOut(float x) {
        if (PointCount == 0 || x > points[^1].xValue) return 1;
        if (x < points[0].xValue) return -1;
        return 0;
    }

    public void SetPointPos(int index, float posX, float posY) {
        points[index].pos.x = posX;
        points[index].pos.y = posY;
    }

    public void SetPointValue(int index, float x, float y) {
        points[index].xValue = x;
        points[index].yValue = y;
    }

    public void UpdatePos(X_Axis xAxis, Y_Axis yAxis) {
        // 更新所有点的 WorldPos
        for (int i = 0; i < PointCount; i++) {
            GraphPoint point = GetPoint(i);
            float      px    = point.xValue, py = point.yValue;
            SetPointPos(i, xAxis.GetPosFromValue(px), yAxis.GetPosFromValue(py));
        }
    }

    public GraphPoint GetPoint(int index) {
        return points[index];
    }

    public void RemoveAt(int index) {
        points.RemoveAt(index);
    }

    public void Clear() {
        points.Clear();
        lineRenderer.positionCount = 0;
    }

    public override void DrawShapes(Camera cam) {
        if (!isHighlightPointShown || !drawer.isShown) return;

        X_Axis xAxis = drawer.xAxis;
        Y_Axis yAxis = drawer.yAxis;
        
        using (Draw.Command(cam)) {
            // float   posX        = xAxis.GetPosFromValue(highlightPoint.xValue);
            // float   posY        = yAxis.GetPosFromValue(highlightPoint.yValue);
            float   posX        = highlightPoint.pos.x;
            float   posY        = highlightPoint.pos.y;
            float   xAxisLength = xAxis.WorldLength;
            Vector3 pos         = new Vector3(posX, posY, 0);

            // 绘制高亮点
            DrawTool.DrawCircle(pos, highlightPointSize, highlightPointColor, transform);

            Vector3 xAxisTextPos = new Vector3(posX, yAxis.WorldLength * yAxis.centerPos / 2, 0);
            // 绘制指向 x 轴的虚线与坐标值文本
            DrawTool.DrawDashedLine(xAxisTextPos, pos,
                                    highlightPointColor, transform, 0.002f);

            DrawTool.DrawText((Vector3)xAxisTextInfo.offset + xAxisTextPos,
                              $"{highlightPoint.xValue:F2}",
                              xAxisTextInfo.align,
                              xAxisTextInfo.color,
                              transform,
                              xAxisTextInfo.fontSize,
                              DataSetting.Instance.englishFont);

            // 绘制指向 y 轴的虚线与坐标值文本
            DrawTool.DrawDashedLine(new Vector3(-xAxisLength / 2, posY, 0), pos,
                                    highlightPointColor, transform, 0.002f);
            DrawTool.DrawText(yAxisTextInfo.offset + new Vector2(-xAxisLength / 2, posY),
                              $"{highlightPoint.yValue:F2}",
                              yAxisTextInfo.align,
                              yAxisTextInfo.color,
                              transform,
                              yAxisTextInfo.fontSize,
                              DataSetting.Instance.englishFont);
        }
    }
}

public partial class GraphLine
{
    /// <summary>
    /// 高亮点对应的坐标值文本信息
    /// </summary>
    [Serializable]
    public class HighlightPointValueTextInfo
    {
        public Vector2   offset;   // 坐标值文本偏移量
        public Color     color;    // 字体颜色
        public TextAlign align;    // 文本对齐方式
        public float     fontSize; // 字体大小
    }
}

[Serializable]
public class GraphPoint
{
    public float   xValue = 0;            // 对应的 X 值
    public float   yValue = 0;            // 对应的 Y 值
    public Vector3 pos    = Vector3.zero; // WorldPos

    public void SetValue(float x = 0, float y = 0, float posX = 0, float posY = 0, float posZ = 0) {
        xValue = x;
        yValue = y;
        pos.x  = posX;
        pos.y  = posY;
        pos.z  = posZ;
    }
}