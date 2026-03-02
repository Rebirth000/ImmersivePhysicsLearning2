using System.Collections;
using System.Collections.Generic;
using Shapes;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public static class DrawTool
{
    public static void DrawDashedLine(Vector3      start,
                                      Vector3      end,
                                      Color        color,
                                      Transform    transform,
                                      float        gridLineWidth   = 0.005f,
                                      DashSpace    gridDashSpace   = DashSpace.Meters,
                                      float        gridDashSize    = 0.05f,
                                      float        gridDashSpacing = 0.05f,
                                      DashSnapping gridSnapping    = DashSnapping.Off) {
        Draw.LineGeometry = LineGeometry.Flat2D;
        Draw.Matrix = transform.localToWorldMatrix;
        Draw.UseDashes = true;
        Draw.DashSpace = gridDashSpace;
        Draw.DashSize = gridDashSize;
        Draw.DashSpacing = gridDashSpacing;
        Draw.Thickness = gridLineWidth;
        Draw.DashSnap = gridSnapping;

        Draw.Line(start, end, color);
    }

    public static void DrawText(Vector3       pos,
                                string        content,
                                TextAlign     textAlign,
                                Color         color,
                                Transform     transform,
                                float         fontSize,
                                TMP_FontAsset fontAsset) {
        Draw.FontSize = fontSize;
        Draw.Matrix = transform.localToWorldMatrix;
        Draw.Font = fontAsset;

        Draw.Text(pos, content, textAlign, color);
    }

    public static void DrawCircle(Vector3 pos, float radius, Color color, Transform transform) {
        Draw.LineGeometry = LineGeometry.Flat2D;
        Draw.Matrix = transform.localToWorldMatrix;

        Draw.Disc(pos, radius, color);
    }
}