using System;
using System.Collections;
using System.Collections.Generic;
using SerializedDic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

/// <summary>
/// Resources 资源加载模块管理器
/// </summary>
public partial class ResourceMgr : Singleton<ResourceMgr>
{
    private ResourceMgr() { }

    // 存储已加载或加载中的资源
    // 资源名称规定为：路径_类型
    [SerializedDictionary("ResName", "Resources")]
    private SerializedDictionary<string, BResInfo> _resDic = new SerializedDictionary<string, BResInfo>();

    public SerializedDictionary<string, BResInfo> ResDic => _resDic;

    /// <summary>
    /// 定义字典中的资源名称
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="type">资源类型</param>
    /// <returns>对应名称</returns>
    private static string GetName(string path, Type type) => $"{path}_{type.Name}";
    
    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="path">资源路径（Resources 下）</param>
    /// <param name="callBack">资源加载完成后的回调函数</param>
    /// <typeparam name="T">资源类型</typeparam>
    public void LoadAsync<T>(string path, UnityAction<T> callBack) where T : Object {
        string resName = GetName(path, typeof(T));              // 资源名称
        if (_resDic.TryGetValue(resName, out BResInfo bInfo)) { // 字典中有该资源的记录
            ResInfo<T> info = (ResInfo<T>) bInfo;
            info.AddRefCount();       // 引用计数增加
            if (info.Asset != null) { // 已经存在资源，则直接使用
                callBack?.Invoke(info.Asset as T);
            }
            else { // 正在进行异步加载但未完成，则添加该回调函数
                info.CallBack += callBack;
            }
        }
        else { // 字典中没有该资源的记录
            ResInfo<T> info = new ResInfo<T>(callBack: callBack);
            info.AddRefCount();                                            // 引用计数增加
            _resDic.Add(resName, info);                                    // 添加记录
            info.Ct = MonoMgr.Instance.StartCoroutine(LoadAsyncEmt(path)); // 通过 MonoMgr 开启异步加载协程
        }


        // Resources 异步加载协程函数
        IEnumerator LoadAsyncEmt(string path) {
            ResourceRequest rq = Resources.LoadAsync<T>(path); // 异步加载资源
            yield return rq;
            if (_resDic.TryGetValue(resName, out BResInfo bInfo)) { // 如果字典中记录了资源，则设置资源，并执行回调
                ResInfo<T> info = (ResInfo<T>) bInfo;
                if (info.RefCount > 0) {                    // 引用计数 > 0，则继续使用资源
                    info.Asset = rq.asset;                  // 设置资源
                    info.CallBack?.Invoke(info.Asset as T); // 执行回调

                    // 使用完毕，置空
                    info.CallBack = null;
                    info.Ct = null;
                }
                else { // 引用计数 <= 0，资源不可用，则卸载
                    UnloadAsset<T>(path, info.IsDel);
                }
            }
        }
    }

    /// <summary>
    /// 同步加载资源
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <typeparam name="T">资源类型</typeparam>
    /// <returns>加载后的资源</returns>
    public T Load<T>(string path) where T : Object {
        string resName = GetName(path, typeof(T));

        if (_resDic.TryGetValue(resName, out BResInfo bInfo)) { // 字典中存在资源
            ResInfo<T> info = (ResInfo<T>) bInfo;
            info.AddRefCount();       // 引用计数增加
            if (info.Asset != null) { // 已经存在资源，则直接返回
                return info.Asset as T;
            }
            else {                                                            // 正在进行异步加载但未完成
                if (info.Ct != null) MonoMgr.Instance.StopCoroutine(info.Ct); // 停止异步加载
                T res = Resources.Load<T>(path);                              // 同步加载该资源
                info.Asset = res;                                             // 设置资源
                info.CallBack?.Invoke(res);                                   // 执行回调函数

                // 使用完毕，置空
                info.CallBack = null;
                info.Ct = null;
                return res;
            }
        }
        else {                                            // 字典中不存在资源
            T          res  = Resources.Load<T>(path);    // 创建资源
            ResInfo<T> info = new ResInfo<T>(asset: res); // 创建资源信息
            info.AddRefCount();                           // 引用计数增加
            _resDic.Add(resName, info);                   // 添加记录
            return res;                                   // 返回
        }
    }

    /// <summary>
    /// 卸载指定资源，只能卸载 mesh / texture / material / shader 
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="isDel">引用计数为 0 时，是否立即卸载资源</param>
    /// <param name="callBack">需要取消执行的回调函数，不能是 Lambda 表达式</param>
    /// <typeparam name="T">资源类型</typeparam>
    public void UnloadAsset<T>(string path, bool isDel = false, UnityAction<T> callBack = null) where T : Object {
        string resName = GetName(path, typeof(T));

        if (_resDic.TryGetValue(resName, out BResInfo bInfo)) {
            ResInfo<T> info = (ResInfo<T>) bInfo;
            info.IsDel = isDel;
            info.SubRefCount();                                           // 引用计数递减
            if (info.Asset != null && info.RefCount <= 0 && info.IsDel) { // 资源已经加载结束，且引用计数 <= 0，则直接删除
                _resDic.Remove(resName);                                  // 移除记录
                Resources.UnloadAsset(info.Asset);                        // 卸载
            }
            else if (info.Asset == null) { // 资源正在异步加载中
                info.IsDel = false;        // 设置资源不可用，等待异步加载完成后删除
                if (callBack != null)      // 当异步加载不想使用时，应移除回调函数，而不是直接卸载
                    info.CallBack -= callBack;
            }
        }
    }

    /// <summary>
    /// 卸载未使用的资源
    /// </summary>
    /// <param name="callBack">回调函数</param>
    public void UnloadUnusedAssets(UnityAction callBack = null) {
        MonoMgr.Instance.StartCoroutine(UnloadUnusedAssetsEmt());
        return;

        IEnumerator UnloadUnusedAssetsEmt() {
            List<string> list = new List<string>(); // 需要卸载的资源名列表
            foreach (KeyValuePair<string, BResInfo> pair in _resDic) {
                if (pair.Value.RefCount <= 0)
                    list.Add(pair.Key);
            }

            foreach (string s in list) { _resDic.Remove(s); } // 移除资源
            
            AsyncOperation ao = Resources.UnloadUnusedAssets(); // 异步卸载资源
            yield return ao;

            // 资源卸载结束
            callBack?.Invoke(); // 执行回调
        }
    }
}

// 内部类的实现
public partial class ResourceMgr
{
    [Serializable]
    public class BResInfo
    {
        public Object    Asset;
        public Type      AssetType;
        public Coroutine Ct;       // 加载该资源的协同程序
        public bool      IsDel;    // 引用计数为 0 时，是否马上移除
        public int       RefCount; // 引用计数
    }

    /// <summary>
    /// 资源信息类
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    public class ResInfo<T> : BResInfo where T : Object
    {
        public UnityAction<T> CallBack; // 资源加载完成后的回调函数

        public ResInfo(T asset = null, UnityAction<T> callBack = null, Coroutine ct = null) {
            Asset = asset;
            AssetType = typeof(T);
            CallBack = callBack;
            Ct = ct;
            IsDel = false;
            RefCount = 0;
        }

        public void AddRefCount() {
            ++RefCount;
        }

        public void SubRefCount() {
            --RefCount;
            if (RefCount < 0)
                Debug.LogError("ResourceMgr.SubRefCount() Error! RefCount < 0!");
        }
    }
}