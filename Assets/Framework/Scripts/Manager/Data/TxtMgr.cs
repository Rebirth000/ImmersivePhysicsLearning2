using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TxtMgr : Singleton<TxtMgr>
{
    private TxtMgr() { }

    /// <summary>
    /// 存储路径
    /// </summary>
    public static string SAVE_PATH = Application.streamingAssetsPath + "/TXT/";

    /// <summary>
    /// 存储数据为 txt 文件
    /// </summary>
    /// <param name="filePath">文件路径，需要写 .txt 后缀</param>
    /// <param name="content">内容</param>
    public void Save(string filePath, string content) {
        var fullPath      = SAVE_PATH + filePath;                  // 文件完整路径
        var directoryPath = StringUtil.GetDirectoryPath(fullPath); // 文件目录路径

        if (!Directory.Exists(directoryPath)) // 不存在文件目录，则创建
            Directory.CreateDirectory(directoryPath);

        File.WriteAllText(fullPath, content);
    }

    /// <summary>
    /// 读取 txt 文件中的数据
    /// </summary>
    /// <param name="filePath">文件路径，需要写 .txt 后缀</param>
    public string Load(string filePath) {
        string fullPath = SAVE_PATH + filePath; // 文件完整路径

        if (!File.Exists(fullPath)) { // 不存在文件，则警告，并返回默认值
            Debug.LogWarning($"TxtMgr: Can't find path \"{fullPath}\"");
            return default(string);
        }
        
        return File.ReadAllText(fullPath);
    }
}
