using System;
using System.Collections.Generic;

public static class ReflectionUtil
{
    /// <summary>
    /// 判断是否为可操作的列表类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsList(this Type type) {
        if (typeof(System.Collections.IList).IsAssignableFrom(type)) { // 是否继承 IList 接口
            return true;
        }

        foreach (var it in type.GetInterfaces()) { // 是否为泛型并实现了 IList<T> 接口
            if (it.IsGenericType && typeof(IList<>) == it.GetGenericTypeDefinition())
                return true;
        }

        return false;
    }

    /// <summary>
    /// 判断是否为列表类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsEnumerable(this Type type) {
        if (type.IsArray) { // 是否为数组
            return true;
        }

        if (typeof(System.Collections.IList).IsAssignableFrom(type)) { // 是否继承 IList 接口
            return true;
        }

        foreach (var it in type.GetInterfaces()) { // 是否为泛型并实现了 IList<T> 接口
            if (it.IsGenericType && typeof(IList<>) == it.GetGenericTypeDefinition())
                return true;
        }

        return false;
    }

    /// <summary>
    /// 判断是否为 C# 内置类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsBulitinType(this Type type) {
        return (type == typeof(object) || Type.GetTypeCode(type) != TypeCode.Object);
    }
}