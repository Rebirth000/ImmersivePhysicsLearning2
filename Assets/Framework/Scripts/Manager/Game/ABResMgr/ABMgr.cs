using System.Collections;
using System.Collections.Generic;
using SerializedDic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// AB 包管理器
/// </summary>
public class ABMgr : Singleton<ABMgr>
{
    private AssetBundle         _mainAB   = null; // 主包
    private AssetBundleManifest _manifest = null; // 依赖包获取用的配置文件

    private ABMgr() { }

    public AssetBundle MainAB {
        get => _mainAB;
    }

    public AssetBundleManifest Manifest {
        get => _manifest;
    }

    public string ABPath { // AB 包存放路径，方便修改
        get => Application.streamingAssetsPath + "/";
    }

    public string MainABName { // AB 包存放路径，方便修改
        get =>
#if UNITY_IOS
            "IOS";
#elif UNITY_ANDROID
            "Android";
#else
            "StandaloneWindows";
#endif
    }

    // 记录所有 AB 包
    private SerializedDictionary<string, AssetBundle> _abDic = new SerializedDictionary<string, AssetBundle>();

    public SerializedDictionary<string, AssetBundle> ABDic => _abDic;

    /// <summary>
    /// 加载主包（同步加载，因为主包比较重要）
    /// </summary>
    public void LoadMainAB() {
        // 获取 AB 包
        if (_mainAB == null) {
            _mainAB = AssetBundle.LoadFromFile(ABPath + MainABName);
            _manifest = _mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
    }

    /// <summary>
    /// 加载 AB 包中的资源，同步异步可选
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="callback"></param>
    /// <param name="isAsync">是否异步加载</param>
    /// <typeparam name="T"></typeparam>
    public void LoadRes<T>(string abName, string resName, UnityAction<T> callback, bool isAsync = true) where T : Object {
        MonoMgr.Instance.StartCoroutine(ReallyLoadAsync(abName, resName, callback));

        IEnumerator ReallyLoadAsync(string abName, string resName, UnityAction<T> callback) {
            // 同步加载主包
            LoadMainAB();

            // 获取依赖包的相关信息
            string[] strs = _manifest.GetAllDependencies(abName);
            foreach (string t in strs) {
                // 判断包是否加载过
                if (!_abDic.ContainsKey(t)) {
                    if (!isAsync) { // 同步加载
                        AssetBundle ab = AssetBundle.LoadFromFile(ABPath + t);
                        _abDic.Add(t, ab);
                    }
                    else {                   // 异步加载
                        _abDic.Add(t, null); // 先使用 null 进行占位，表示在异步加载 t 中。。。
                        AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(ABPath + t);
                        yield return req;
                        _abDic[t] = req.assetBundle; // 加载完成后，进行赋值
                    }
                }
                else {
                    while (_abDic[t] == null) { // 发现正在加载中，则不停等待，直到加载完成
                        yield return null;
                    }
                }
            }

            // 加载资源包
            if (!_abDic.ContainsKey(abName)) { // 没有加载过，才添加
                if (!isAsync) {                // 同步加载
                    AssetBundle ab = AssetBundle.LoadFromFile(ABPath + abName);
                    _abDic.Add(abName, ab);
                }
                else {                        // 异步加载
                    _abDic.Add(abName, null); // 先使用 null 进行占位，表示在异步加载 t 中。。。
                    AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(ABPath + abName);
                    yield return req;
                    _abDic[abName] = req.assetBundle;
                }
            }
            else {
                while (_abDic[abName] == null) { // 发现正在加载中，则不停等待，直到加载完成
                    yield return null;
                }
            }

            if (!isAsync) { // 同步加载
                T res = _abDic[abName].LoadAsset<T>(resName);
                callback(res);
            }
            else { // 异步加载
                AssetBundleRequest abr = _abDic[abName].LoadAssetAsync(resName);
                yield return abr;

                callback(abr.asset as T);
            }
        }
    }

    /// <summary>
    /// 卸载 AB 包
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="callBackResult"></param>
    public void UnLoad(string abName, UnityAction<bool> callBackResult) {
        if (_abDic.TryGetValue(abName, out AssetBundle ab)) {
            if (ab == null) {
                Debug.LogWarning($"ABMgr: {abName} 正在异步加载中，无法卸载！");
                callBackResult(false);
                return;
            }

            ab.Unload(false);
            _abDic.Remove(abName);

            callBackResult(true);
        }
    }

    /// <summary>
    /// 卸载所有 AB 包
    /// </summary>
    public void ClearAB() {
        MonoMgr.Instance.StopAllCoroutines(); // AB 包异步加载，需要停止协程
        AssetBundle.UnloadAllAssetBundles(false);
        _abDic.Clear();
        _mainAB = null;
        _manifest = null;
    }
}