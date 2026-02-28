using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换管理器
/// </summary>
public class SceneMgr : Singleton<SceneMgr>
{
    private SceneMgr() { }

    /// <summary>
    /// 同步加载场景
    /// </summary>
    /// <param name="name">场景名称</param>
    /// <param name="callback">加载完成后需要做的委托</param>
    public void LoadScene(string name, UnityAction callback = null) {
        SceneManager.LoadScene(name); // 同步加载场景
        callback?.Invoke();           // 执行委托
    }

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="name">场景名称</param>
    /// <param name="callback">加载完成后需要做的委托</param>
    /// <param name="onLoading">异步加载时需要进行的工作，默认输入参数为加载场景的进度</param>
    public void LoadSceneAsync(string name, UnityAction callback, UnityAction<float> onLoading = null)  {
        MonoMgr.Instance.StartCoroutine(LoadSceneCoroutine(name, callback)); // 开启异步加载协程

        IEnumerator LoadSceneCoroutine(string name, UnityAction callback) {
            AsyncOperation ao = SceneManager.LoadSceneAsync(name); // 异步加载场景
            while (!ao.isDone) { // 加载未完成时
                onLoading?.Invoke(ao.progress);
                yield return null;
            }
            onLoading?.Invoke(1); // 加载完成时，进度为 1

            callback.Invoke(); // 执行委托
        }
    }
}