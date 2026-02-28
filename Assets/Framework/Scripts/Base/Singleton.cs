using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 单例模式基类（懒汉模式）<br/>
/// 规定：子类必须实现私有无参构造函数
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> where T : class
{
    // volatile 关键字将关闭编译器优化代码，防止重新排列对存储器位置的读取和写入
    private static volatile T _instance;

    private static readonly Object LockHelper = new Object(); // 用于线程加锁的对象

    public static T Instance {
        get {
            if (_instance == null) {
                lock (LockHelper) {            // 线程加锁
                    if (_instance == null) {   // 判空是有必要的，避免两个线程同时访问到此而重复创建 _instance
                        Type type = typeof(T); // 获取参数 T 的类型信息

                        ConstructorInfo info = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
                                                                   null,
                                                                   Type.EmptyTypes,
                                                                   null); // 得到类型 T 的私有无参构造函数

                        if (info != null)
                            _instance = info.Invoke(null) as T;
                        else
                            Debug.LogError($"{type.Name} 没有私有无参构造函数！！！"); // 子类必须实现私有无参构造函数
                    }
                }
            }

            // Debug.Log(typeof(T).Name + ": " + _instance);

            return _instance;
        }
    }
}