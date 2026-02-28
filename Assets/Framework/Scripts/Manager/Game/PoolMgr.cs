using System;
using System.Collections;
using System.Collections.Generic;
using SerializedDic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 缓存（对象）池管理器，可管理 Mono 和 Data 两种类型的对象 <br/>
/// Mono：场景中的对象 <br/>
/// Data：自定义数据类
/// </summary>
public sealed partial class PoolMgr : Singleton<PoolMgr>
{
    private PoolMgr() { }

    private SerializedDictionary<string, PoolMonoContainer> _monoDic = new SerializedDictionary<string, PoolMonoContainer>(); // 容器存储字典

    private SerializedDictionary<string, PoolDataContainerBase> _dataDic = new SerializedDictionary<string, PoolDataContainerBase>();

    public SerializedDictionary<string, PoolMonoContainer> MonoDic => _monoDic;

    public SerializedDictionary<string, PoolDataContainerBase> DataDic => _dataDic;

    public GameObject Pool { get; set; }

    /// <summary>
    /// 依据 Mono 对象名称放入对应的容器
    /// </summary>
    /// <param name="obj">待放入的对象</param>
    public void Push(GameObject obj) {
        if (Pool == null) Pool = new GameObject("Pool"); // 创建缓存池根对象

        if (!_monoDic.TryGetValue(obj.name, out PoolMonoContainer ct)) { // 如果不存在对应名称的容器，则创建
            ct = new PoolMonoContainer(Pool, obj.name);
            _monoDic.Add(obj.name, ct);
        }

        ct.Push(obj); // 放入对象
    }

    /// <summary>
    /// 从名称为 name 的容器中取出 Mono 对象
    /// </summary>
    /// <param name="name">容器名称</param>
    /// <returns></returns>
    public GameObject Pop(string name) {
        // 存在对应的容器，且容器中含有对象
        if (_monoDic.TryGetValue(name, out PoolMonoContainer ct) && ct.Count > 0) return ct.Pop();

        // 否则，创建对象返回
        GameObject obj = Object.Instantiate(ResourceMgr.Instance.Load<GameObject>(name));
        obj.name = name; // 重置名称，以免 Instantiate 方法默认在名称后面添加 "(Clone)"
        return obj;
    }

    /// <summary>
    /// 压入自定义数据 T，依据 T 的名称进行存储
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void PushData<T>(T data, string spaceName = null) where T : class, IPoolData, new() {
        if (data == null) return; // 数据为空，则直接返回
        
        string poolName = spaceName != null ? $"{spaceName}_{typeof(T).Name}" : typeof(T).Name;

        PoolDataContainer<T> container = null;
        // 存在容器
        if (_dataDic.TryGetValue(poolName, out PoolDataContainerBase baseContainer)) {
            container = (PoolDataContainer<T>) baseContainer; // 转换容器
        }
        else {                                      // 不存在容器
            container = new PoolDataContainer<T>(); // 创建新容器并添加到字典
            _dataDic.Add(poolName, container);
        }

        data.ResetData();                // 放入对象前重置数据
        container.Objects.Enqueue(data); // 压入对象
    }

    /// <summary>
    /// 获取自定义数据 T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T PopData<T>(string spaceName = null) where T : class, IPoolData, new() {
        // 对应容器名称为 "命名空间_数据类型名"
        string poolName = spaceName != null ? $"{spaceName}_{typeof(T).Name}" : typeof(T).Name;

        // 存在容器
        if (_dataDic.TryGetValue(poolName, out PoolDataContainerBase baseContainer)) {
            PoolDataContainer<T> container = (PoolDataContainer<T>) baseContainer;

            if (container.Count > 0) { return container.Objects.Dequeue(); } // 容器中有对象，则弹出
            else return new T();                                             // 没有对象，则新创建
        }

        // 不存在容器，创建对象返回
        return new T();
    }

    /// <summary>
    /// 清除 Mono 池
    /// </summary>
    public void ClearMonoPool() {
        _monoDic.Clear();
        Pool = null;
    }
    
    /// <summary>
    /// 清除 Data 池
    /// </summary>
    public void ClearDataPool() {
        _dataDic.Clear();
    }

    /// <summary>
    /// 清除所有缓冲池
    /// </summary>
    public void ClearAllPool() {
        ClearMonoPool();
        ClearDataPool();
    }
}

// 内部类的实现
public sealed partial class PoolMgr
{
    /// <summary>
    /// 装载 Mono 的容器类
    /// </summary>
    [Serializable]
    public class PoolMonoContainer
    {
        public GameObject Root;

        public List<GameObject> Objects = new List<GameObject>();

        public PoolMonoContainer(GameObject pool, string name) {
            Root = new GameObject($"MonoContainer: {name}"); // 创建容器对象
            Root.transform.SetParent(pool.transform);        // 将容器置于 Pool 下方
        }

        public int Count => Objects.Count; // 容器中对象的个数

        public void Push(GameObject obj) {
            obj.SetActive(false);                    // 失活
            obj.transform.SetParent(Root.transform); // 创建父子关系
            Objects.Add(obj);                        // 压入对象
        }

        public GameObject Pop() {
            GameObject obj = Objects[0];
            Objects.RemoveAt(0);           // 取出对象
            obj.SetActive(true);           // 激活
            obj.transform.SetParent(null); // 断开父子关系
            return obj;
        }
    }

    /// <summary>
    /// 装载 Data 的容器类
    /// </summary>
    public class PoolDataContainer<T> : PoolDataContainerBase where T : class, IPoolData, new()
    {
        public Queue<T> Objects = new Queue<T>(); // 容器中的对象

        public override int Count => Objects.Count;
    }

    [Serializable]
    public class PoolDataContainerBase
    {
        public virtual int Count { get; }
    }
}

/// <summary>
/// 需要被缓存的 Data 对象需要实现该接口
/// </summary>
public interface IPoolData
{
    /// <summary>
    /// 重置数据的方法
    /// </summary>
    void ResetData();
}