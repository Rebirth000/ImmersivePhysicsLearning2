using System;
using System.Reflection;
using System.Text;

public static class StringUtil
{
    /// <summary>
    /// 拆分字符串，避免出现中文分隔符
    /// </summary>
    /// <param name="str"></param>
    /// <param name="delimiter"></param>
    /// <returns></returns>
    public static string[] Split(string str, char delimiter = ',') {
        if (str == null) return Array.Empty<string>(); // 判空

        switch (delimiter) { // 替换中文分隔符
            case ':':
                str = str.Replace('：', delimiter);
                break;
            case ',':
                str = str.Replace('，', delimiter);
                break;
            case ';':
                str = str.Replace('；', delimiter);
                break;
        }

        return str.Split(delimiter);
    }

    /// <summary>
    /// 依据 delimiter 拆分字符串，并转换为 int 数组
    /// </summary>
    /// <param name="str"></param>
    /// <param name="delimiter"></param>
    /// <returns></returns>
    public static int[] SplitToInt(string str, char delimiter = ',') {
        string[] strs = Split(str, delimiter);

        if (strs.Length == 0) return Array.Empty<int>(); // 判空

        return Array.ConvertAll<string, int>(strs, int.Parse); // 转换 int
    }

    /// <summary>
    /// int 转 string，int 长度不够则会在前面补 0
    /// </summary>
    /// <param name="value"></param>
    /// <param name="len"></param>
    /// <returns></returns>
    public static string IntToString(int value, int len) {
        return value.ToString($"D{len}");
    }

    /// <summary>
    /// float 转 string，保留小数点后 len 位
    /// </summary>
    /// <param name="value"></param>
    /// <param name="len"></param>
    /// <returns></returns>
    public static string FloatToString(float value, int len) {
        return value.ToString($"F{len}");
    }

    /// <summary>
    /// 秒转 1h 1m 1s 格式
    /// </summary>
    /// <param name="s">秒数</param>
    /// <param name="isKeepLen">是否保留 2 位长度，即 01h 01m 01s</param>
    /// <param name="ignoreZero">是否忽略 0</param>
    /// <param name="hourStr">小时的拼接字符</param>
    /// <param name="minuteStr">分钟的拼接字符</param>
    /// <param name="secondStr">秒的拼接字符</param>
    /// <returns></returns>
    public static string SecondToHourMinuteSecond(int    s,
                                                  bool   isKeepLen  = true,
                                                  bool   ignoreZero = false,
                                                  string hourStr    = "h",
                                                  string minuteStr  = "m",
                                                  string secondStr  = "s") {
        if (s < 0) s = 0; // 避免出现负数

        int hour   = s / 3600;
        int minute = (s % 3600) / 60;
        int second = s % 60;

        StringBuilder builder = new StringBuilder();

        // 输出 小时
        if (!ignoreZero || hour != 0) {
            builder.Append(isKeepLen ? IntToString(hour, 2) : hour);
            builder.Append(hourStr);
        }

        // 输出 分钟
        if (!ignoreZero || hour != 0 || minute != 0) {
            builder.Append(isKeepLen ? IntToString(minute, 2) : minute);
            builder.Append(minuteStr);
        }

        // 输出 秒
        if (!ignoreZero || hour != 0 || minute != 0 || second != 0) {
            builder.Append(isKeepLen ? IntToString(second, 2) : second);
            builder.Append(secondStr);
        }

        // 如果 builder 为空，则返回 0s
        if (builder.Length == 0) {
            builder.Append(0);
            builder.Append(secondStr);
        }

        return builder.ToString();
    }

    /// <summary>
    /// 根据委托函数的参数信息，获取对应的完整名称
    /// </summary>
    /// <param name="info">委托函数信息</param>
    /// <returns></returns>
    public static string GetVoidMethodFullName(MethodInfo info) {
        if (info == null) return "Null";

        ParameterInfo[] parameters = info.GetParameters(); // 获取方法的参数
        string          param      = "";

        // 拼接参数类型和参数名
        for (int i = 0; i < parameters.Length; i++) {
            param += $"{parameters[i].ParameterType.Name} {parameters[i].Name}";
            if (i < parameters.Length - 1)
                param += ", ";
        }

        return $"void {info.Name}({param});"; // 返回完整名称
    }

    /// <summary>
    /// 获取文件的目录路径
    /// </summary>
    /// <param name="fullPath"></param>
    /// <returns></returns>
    public static string GetDirectoryPath(string fullPath) {
        int index = fullPath.LastIndexOf('/');
        if (index == -1) return "";
        return fullPath[..(index + 1)];
    }
}