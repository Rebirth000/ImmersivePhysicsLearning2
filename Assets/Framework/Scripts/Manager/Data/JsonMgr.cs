using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Json 序列化存储。注意：<br/>
/// 1. float 序列化时看起来会有一些误差 <br/>
/// 2. 自定义类需要加上序列化特性 [System.Serializable] <br/>
/// 3. 想要序列化私有变量，需要加上特性 [SerializeField] <br/>
/// 4. 不支持字典；序列化数组时需要包裹一层 <br/>
/// 5. 存储 null 对象不会是 null，而是默认值的数据 <br/>
/// </summary>
public class JsonMgr : Singleton<JsonMgr>
{
    private JsonMgr() { }

    /// <summary>
    /// 存储路径
    /// </summary>
    public static string SAVE_PATH = Application.streamingAssetsPath + "/JSON/";

    /// <summary>
    /// 存储数据为 json 文件
    /// </summary>
    /// <param name="filePath">文件路径，需要写 .json 后缀</param>
    /// <param name="data">存储数据，如果是自定义结构，则需要添加 [Serializable] 特性</param>
    public void Save(string filePath, object data) {
        var fullPath      = SAVE_PATH + filePath;                  // 文件完整路径
        var directoryPath = StringUtil.GetDirectoryPath(fullPath); // 文件目录路径

        if (!Directory.Exists(directoryPath)) // 不存在文件目录，则创建
            Directory.CreateDirectory(directoryPath);

        File.WriteAllText(fullPath, JsonUtility.ToJson(data));
    }

    /// <summary>
    /// 读取 json 文件中的数据
    /// </summary>
    /// <param name="filePath">文件路径，需要写 .json 后缀</param>
    public T Load<T>(string filePath) {
        string fullPath = SAVE_PATH + filePath; // 文件完整路径

        if (!File.Exists(fullPath)) { // 不存在文件，则警告，并返回默认值
            Debug.LogWarning($"JsonMgr: Can't find path \"{fullPath}\"");
            return default(T);
        }
        
        return JsonUtility.FromJson<T>(File.ReadAllText(fullPath));
    }
}