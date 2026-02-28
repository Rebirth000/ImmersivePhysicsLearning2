using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// 二进制序列化存储。注意：<br/>
/// 1. 存储时要保证对象不为空，否则读取时会报错
/// </summary>
public class BinaryMgr : Singleton<BinaryMgr>
{
    private BinaryMgr() { }

    /// <summary>
    /// 存储路径
    /// </summary>
    public static string SAVE_PATH = Application.streamingAssetsPath + "/Binary/";

    /// <summary>
    /// 存储二进制数据
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="data">存储数据，如果是自定义结构，则需要添加 [Serializable] 特性</param>
    public void Save(string filePath, object data) {
        string fullPath      = SAVE_PATH + filePath;                  // 文件完整路径
        string directoryPath = StringUtil.GetDirectoryPath(fullPath); // 文件目录路径

        // 如果文件夹不存在，则创建
        if (!Directory.Exists(directoryPath)) {
            Directory.CreateDirectory(directoryPath);
        }

        using var fs = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write);
        var       bf = new BinaryFormatter();
        bf.Serialize(fs, data);
        fs.Close();
    }

    /// <summary>
    /// 读取二进制数据转换为对象
    /// </summary>
    /// <param name="filePath"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Load<T>(string filePath) {
        string fullPath = SAVE_PATH + filePath; // 文件完整路径

        if (!File.Exists(fullPath)) { // 不存在文件，则警告，并返回默认值
            Debug.LogWarning($"JsonMgr: Can't find path \"{fullPath}\"");
            return default(T);
        }

        using var fs   = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        var       bf   = new BinaryFormatter();
        var       data = (T) bf.Deserialize(fs);
        // fs.Close();
        return data;
    }
}