using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class BarMgr : MonoBehaviour
{
    public List<LineBar> lineBars;

    public List<GraphLine> lines;

    public float startPosY;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() {
        var   graph = DataSetting.Instance.graphMgr;
        float posY  = startPosY, valueY = 0;
        for (int i = 0; i < lines.Count; i++) {
            LineBar   lineBar = lineBars[i];
            GraphLine line    = lines[i];
            X_Axis    xAxis   = line.drawer.xAxis;
            
            if (!line.isHighlightPointShown) continue;

            var point = line.highlightPoint;
            lineBar.barHeight =  point.pos.y - xAxis.transform.localPosition.y;
            lineBar.barPosY   =  posY;
            posY              += lineBar.barHeight;

            lineBar.targetPos1 = graph.transform.TransformPoint(point.pos.Set(y: startPosY));
            lineBar.targetPos2 = graph.transform.TransformPoint(point.pos);

            valueY                  += point.yValue;
            lineBar.textCenter.text =  MathUtil.FormatFloat(point.yValue);
            lineBar.textRight.text  =  MathUtil.FormatFloat(valueY);
        }
    }

    public void Show() {
        foreach (var bar in lineBars) {
            bar.Show();
        }
    }

    public void Hide() {
        foreach (var bar in lineBars) {
            bar.Hide();
        }
    }
}