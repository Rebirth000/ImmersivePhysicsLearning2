using System;
using UnityEngine;

public static class MathUtil
{
    /// <summary>
    /// 角度转弧度
    /// </summary>
    /// <param name="deg">角度</param>
    /// <returns>弧度</returns>
    public static float Deg2Rad(float deg) {
        return deg * Mathf.Deg2Rad;
    }

    /// <summary>
    /// 弧度转角度
    /// </summary>
    /// <param name="rad">弧度</param>
    /// <returns>角度</returns>
    public static float Rad2Deg(float rad) {
        return rad * Mathf.Rad2Deg;
    }

    /// <summary>
    /// 判断 float 是否为整数
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsInteger(float value) {
        return Math.Abs(value - (int) value) < 1e-6f;
    }

    /// <summary>
    /// 格式化浮点数
    /// </summary>
    /// <param name="value">浮点值</param>
    /// <param name="maxDecimalPlaces">小数点后的最大位数</param>
    /// <returns></returns>
    public static string FormatFloat(float value, int maxDecimalPlaces = 2) {
        int   decimalPlaces = 0; // 小数点后的位数
        float tmp           = value;

        while (decimalPlaces < maxDecimalPlaces) { // 当小数点后的位数小于最大位数时，循环
            // 如果tmp为整数，则返回格式化后的字符串
            if (IsInteger(tmp))
                return Math.Round(value, decimalPlaces).ToString($"F{decimalPlaces}");
            ++decimalPlaces;
            tmp *= 10;
        }

        return value.ToString($"F{maxDecimalPlaces}");
    }
}