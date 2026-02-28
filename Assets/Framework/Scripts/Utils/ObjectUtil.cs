using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectUtil
{
    /// <summary>
    /// 依据绝对路径获取组件
    /// </summary>
    /// <param name="path">子物体路径</param>
    /// <typeparam name="T">组件类型</typeparam>
    public static T GetComponent<T>(string path) where T : Object {
        T comp = GameObject.Find(path).GetComponent<T>();

        if (comp == null)
            Debug.LogError($"Script DataSetting.cs: \"{path}\" is null");

        return comp;
    }

    /// <summary>
    /// 获取 trans 上的组件
    /// </summary>
    /// <param name="trans">物体 transform</param>
    /// <typeparam name="T">组件类型</typeparam>
    public static T GetComponent<T>(Transform trans) where T : Object {
        T comp = trans.GetComponent<T>();

        if (comp == null)
            Debug.LogError($"Script DataSetting.cs: \"{trans.name}\" has no component \"{typeof(T)}\"");

        return comp;
    }

    /// <summary>
    /// 从子物体中获取组件
    /// </summary>
    /// <param name="path">子物体路径</param>
    /// <param name="parent">父物体</param>
    /// <typeparam name="T">组件类型</typeparam>
    public static T GetComponentFromChild<T>(Transform parent, string path) where T : Object {
        Transform transPath = parent.Find(path);
        
        if (transPath == null)
            Debug.LogError($"Script DataSetting.cs: \"{parent.name}: {path}\" is null");

        T comp = transPath.GetComponent<T>();

        if (comp == null)
            Debug.LogError($"Script DataSetting.cs: \"{transPath.name}\" has no component \"{typeof(T)}\"");
        
        return comp;
    }
}
