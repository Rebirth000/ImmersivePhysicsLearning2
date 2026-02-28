using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectUtil
{
    /// <summary>
    /// 移动矩形
    /// </summary>
    /// <param name="rect">待移动的矩形</param>
    /// <param name="x">x 距离</param>
    /// <param name="y">y 距离</param>
    /// <param name="ratio">是否将距离变成比例？即 rect.x += rect.width * x，而不是 rect.x += x</param>
    /// <returns></returns>
    public static Rect Move(this Rect rect, float x = 0, float y = 0, bool ratio = false) {
        rect.x += ratio ? rect.width * x : x;
        rect.y += ratio ? rect.height * y : y;
        return rect;
    }

    /// <summary>
    /// 缩放矩形
    /// </summary>
    /// <param name="rect">待缩放的矩形</param>
    /// <param name="x">x 缩放</param>
    /// <param name="y">y 缩放</param>
    /// <param name="ratio">是否将缩放变成比例？即 rect.width = rect.width * x，而不是 rect.width = x</param>
    /// <returns></returns>
    public static Rect Resize(this Rect rect, float x = 0, float y = 0, bool ratio = false) {
        if (x > 0) rect.width = ratio ? rect.width * x : x;
        if (y > 0) rect.height = ratio ? rect.height * y : y;
        return rect;
    }

    /// <summary>
    /// 切除左部分矩形
    /// </summary>
    /// <param name="rect">待切除的矩形</param>
    /// <param name="x">左部分切除的宽度</param>
    /// <param name="ratio">是否比例切除？即切除宽度为 rect.width * x 的矩形，而不是宽度为 x</param>
    /// <returns></returns>
    public static Rect CutLeft(this Rect rect, float x, bool ratio = false) {
        return rect.Move(x, 0, ratio).Resize(ratio ? rect.width * (1 - x) : rect.width - x, 0);
    }
    
    /// <summary>
    /// 切除矩形，保留左部分
    /// </summary>
    /// <param name="rect">待切除的矩形</param>
    /// <param name="x">切除后左部分保留的宽度</param>
    /// <param name="ratio">是否比例切除？即切除宽度为 rect.width * x 的矩形，而不是宽度为 x</param>
    /// <returns></returns>
    public static Rect CutToLeft(this Rect rect, float x, bool ratio = false) {
        return rect.Resize(x, 0, ratio);
    }

    /// <summary>
    /// 切除右部分矩形
    /// </summary>
    /// <param name="rect">待切除的矩形</param>
    /// <param name="x">右部分切除的宽度</param>
    /// <param name="ratio">是否比例切除？即切除宽度为 rect.width * x 的矩形，而不是宽度为 x</param>
    /// <returns></returns>
    public static Rect CutRight(this Rect rect, float x, bool ratio = false) {
        return rect.Resize(ratio ? rect.width * (1 - x) : rect.width - x, 0);
    }
    
    /// <summary>
    /// 切除矩形，保留右部分
    /// </summary>
    /// <param name="rect">待切除的矩形</param>
    /// <param name="x">切除后右部分保留的宽度</param>
    /// <param name="ratio">是否比例切除？即切除宽度为 rect.width * x 的矩形，而不是宽度为 x</param>
    /// <returns></returns>
    public static Rect CutToRight(this Rect rect, float x, bool ratio = false) {
        return rect.Resize(x, 0, ratio).Move(ratio ? rect.width * (1 - x) : rect.width - x, 0);
    }

    /// <summary>
    /// 切除上部分矩形
    /// </summary>
    /// <param name="rect">待切除的矩形</param>
    /// <param name="y">上部分切除的高度</param>
    /// <param name="ratio">是否比例切除？即切除高度为 rect.height * y 的矩形，而不是宽度为 y</param>
    /// <returns></returns>
    public static Rect CutUp(this Rect rect, float y, bool ratio = false) {
        return rect.Move(0, y, ratio).Resize(0, ratio ? rect.height * (1 - y) : rect.height - y);
    }
    
    /// <summary>
    /// 切除矩形，保留上部分
    /// </summary>
    /// <param name="rect">待切除的矩形</param>
    /// <param name="y">切除后上部分保留的高度</param>
    /// <param name="ratio">是否比例切除？即切除高度为 rect.height * y 的矩形，而不是宽度为 y</param>
    /// <returns></returns>
    public static Rect CutToUp(this Rect rect, float y, bool ratio = false) {
        return rect.Resize(0, y, ratio);
    }

    /// <summary>
    /// 切除下部分矩形
    /// </summary>
    /// <param name="rect">待切除的矩形</param>
    /// <param name="y">下部分切除的高度</param>
    /// <param name="ratio">是否比例切除？即切除高度为 rect.height * y 的矩形，而不是宽度为 y</param>
    /// <returns></returns>
    public static Rect CutDown(this Rect rect, float y, bool ratio = false) {
        return rect.Resize(0, ratio ? rect.height * (1 - y) : rect.height - y);
    }
    
    /// <summary>
    /// 切除矩形，保留下部分
    /// </summary>
    /// <param name="rect">待切除的矩形</param>
    /// <param name="y">切除后下部分保留的高度</param>
    /// <param name="ratio">是否比例切除？即切除高度为 rect.height * y 的矩形，而不是宽度为 y</param>
    /// <returns></returns>
    public static Rect CutToDown(this Rect rect, float y, bool ratio = false) {
        return rect.Resize(0, y, ratio).Move(0, ratio ? rect.height * (1 - y) : rect.height - y);
    }
}