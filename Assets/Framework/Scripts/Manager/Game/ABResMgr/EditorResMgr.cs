using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

/// <summary>
/// 编辑器资源管理器 <br/>
/// 注意：只有开发时能使用该管理器加载资源，且不会有卸载资源的方法
/// </summary>
public class EditorResMgr : Singleton<EditorResMgr>
{
    public string RootPath = "Assets/"; // 用于放置需要打进 AB 包中的资源路径

    private EditorResMgr() { }

    /// <summary>
    /// 加载单个资源
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <typeparam name="T">资源类型</typeparam>
    /// <returns></returns>
    public T LoadRes<T>(string path) where T : Object {
        string suffix = GetSuffix(typeof(T));
#if UNITY_EDITOR
        T res = AssetDatabase.LoadAssetAtPath<T>(RootPath + path + suffix); // 该方法传入的名称需要带有后缀
        return res;
#else
        return null;
#endif
    }

    /// <summary>
    /// 加载图集
    /// </summary>
    /// <param name="path">图集路径</param>
    /// <param name="spriteName">图集名称</param>
    /// <returns></returns>
    public Sprite LoaSprite(string path, string spriteName) {
#if UNITY_EDITOR
        Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(RootPath + path);
        return sprites.FirstOrDefault(sp => sp.name == spriteName) as Sprite;
#else
        return null;
#endif
    }

    /// <summary>
    /// 加载所有图集
    /// </summary>
    /// <param name="path">图集路径</param>
    /// <returns></returns>
    public Dictionary<string, Sprite> LoadSprites(string path) {
        Dictionary<string, Sprite> spriteDic = new Dictionary<string, Sprite>();
#if UNITY_EDITOR
        Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(RootPath + path);
        foreach (Object item in sprites) {
            spriteDic.Add(item.name, item as Sprite);
        }

        return spriteDic;
#else
        return null;
#endif
    }

    /// <summary>
    /// 依据不同类型返回后缀
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>后缀</returns>
    private string GetSuffix(Type type) {
        return type.Name switch {
            nameof(GameObject)    => ".prefab",        // 预制体
            nameof(Material)      => ".mat",           // 材质球
            nameof(RenderTexture) => ".renderTexture", // 渲染纹理
            nameof(AudioClip)     => ".mp3",           // 音频文件
            _                     => ""
        };
    }
}