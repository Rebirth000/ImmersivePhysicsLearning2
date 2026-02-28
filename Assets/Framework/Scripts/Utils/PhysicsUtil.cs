using System;
using UnityEngine;
using UnityEngine.Events;

public static class PhysicsUtil
{
    /// <summary>
    /// 射线检测第一个物体，执行回调
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">回调函数</param>
    /// <param name="maxDistance">射线最大距离</param>
    /// <param name="layerMask">检测层级</param>
    /// <typeparam name="T">回调参数类型</typeparam>
    public static void RayCast<T>(Ray ray, UnityAction<T> callBack, float maxDistance = float.PositiveInfinity, int layerMask = -5)
        where T : class {
        Type type = typeof(T);

        if (Physics.Raycast(ray, out RaycastHit info, maxDistance, layerMask)) {
            if (type == typeof(RaycastHit))
                callBack.Invoke(info as T);
            else if (type == typeof(GameObject))
                callBack.Invoke(info.collider.gameObject as T);
            else
                callBack.Invoke(info.collider.gameObject.GetComponent<T>());
        }
    }

    /// <summary>
    /// 射线检测所有物体，执行回调
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="callBack">回调函数</param>
    /// <param name="maxDistance">射线最大距离</param>
    /// <param name="layerMask">检测层级</param>
    /// <typeparam name="T">回调参数类型</typeparam>
    public static void RayCastAll<T>(Ray ray, UnityAction<T> callBack, float maxDistance = float.PositiveInfinity, int layerMask = -5)
        where T : class {
        Type type = typeof(T);

        var hitInfos = Physics.RaycastAll(ray, maxDistance, layerMask);

        foreach (var info in hitInfos) {
            if (type == typeof(RaycastHit))
                callBack.Invoke(info as T);
            else if (type == typeof(GameObject))
                callBack.Invoke(info.collider.gameObject as T);
            else
                callBack.Invoke(info.collider.gameObject.GetComponent<T>());
        }
    }

    /// <summary>
    /// 盒状范围检测
    /// </summary>
    /// <param name="center">盒中心点</param>
    /// <param name="rotation">角度</param>
    /// <param name="halfExtents">盒长宽高的一半</param>
    /// <param name="callBack">回调函数</param>
    /// <param name="layerMask">检测层级</param>
    /// <param name="queryTriggerInteraction">是否检测触发器</param>
    /// <typeparam name="T">回调参数类型</typeparam>
    public static void OverlapBox<T>(Vector3                 center,
                                     Quaternion              rotation,
                                     Vector3                 halfExtents,
                                     UnityAction<T>          callBack,
                                     int                     layerMask               = -1,
                                     QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Collide)
        where T : class {
        Type type = typeof(T);

        var colliders = Physics.OverlapBox(center, halfExtents, rotation, layerMask, queryTriggerInteraction);

        foreach (var collider in colliders) {
            if (type == typeof(Collider))
                callBack.Invoke(collider as T);
            else if (type == typeof(GameObject))
                callBack.Invoke(collider.gameObject as T);
            else
                callBack.Invoke(collider.gameObject.GetComponent<T>());
        }
    }

    /// <summary>
    /// 球形范围检测
    /// </summary>
    /// <param name="center">球心位置</param>
    /// <param name="radius">半径</param>
    /// <param name="callBack">回调函数</param>
    /// <param name="layerMask">检测层级</param>
    /// <param name="queryTriggerInteraction">是否检测触发器</param>
    /// <typeparam name="T">回调参数类型</typeparam>
    public static void OverlapSphere<T>(Vector3                 center,
                                        float                   radius,
                                        UnityAction<T>          callBack,
                                        int                     layerMask               = -1,
                                        QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Collide)
        where T : class {
        Type type = typeof(T);

        var colliders = Physics.OverlapSphere(center, radius, layerMask, queryTriggerInteraction);

        foreach (var collider in colliders) {
            if (type == typeof(Collider))
                callBack.Invoke(collider as T);
            else if (type == typeof(GameObject))
                callBack.Invoke(collider.gameObject as T);
            else
                callBack.Invoke(collider.gameObject.GetComponent<T>());
        }
    }
}