using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 整合 ABMgr 和 EditorResMgr 的管理器
/// </summary>
public class ABResMgr : Singleton<ABResMgr>
{
    private ABResMgr() { }

    public bool IsDebug = true; // true：通过 EditorResMgr 加载资源；false：通过 ABMgr 加载资源

    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callback, bool isAsync = true) where T : Object {
#if UNITY_EDITOR
        if (IsDebug) {
            T res = EditorResMgr.Instance.LoadRes<T>($"{abName}/{resName}/");
            callback?.Invoke(res);
        }
        else {
            ABMgr.Instance.LoadRes<T>(abName, resName, callback, isAsync);
        }
#else
            ABMgr.Instance.LoadRes<T>(abName, resName, callback, isAsync);
#endif
    }
}