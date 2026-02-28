using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 继承 Mono 的单例模式基类（自动挂载式）
/// </summary>
/// <typeparam name="T"></typeparam>
[DisallowMultipleComponent] [Serializable]
public abstract class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance {
        get {
            if (_instance == null) {
                Type type = typeof(T); // 获取参数 T 的类型信息

                GameObject obj = GameObject.Find(type.Name);
                if (obj == null) {
                    obj = new GameObject(type.Name);   // 创建名称相同的 obj
                    _instance = obj.AddComponent<T>(); // 添加 _instance
                }
                else
                    _instance = obj.GetComponent<T>(); // 获取 _instance

                DontDestroyOnLoad(obj); // 过场景不移除
            }

            return _instance;
        }
    }
}