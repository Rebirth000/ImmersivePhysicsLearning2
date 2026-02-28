using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// csv 文件存储和读取器
/// </summary>
public partial class CsvSheet
{
    /// <summary>
    /// 存储路径
    /// </summary>
    public static string SAVE_PATH = Application.streamingAssetsPath + "/CSV/";

    private int _rowCount = 0; // 最大行数
    private int _colCount = 0; // 最大列数

    public int RowCount { get => _rowCount; }
    public int ColCount { get => _colCount; }

    private Dictionary<Index, string> _sheetDic = new Dictionary<Index, string>(); // 缓存当前数据的字典

    public string this[int row, int col] {
        get {
            // 越界检查
            if (row >= _rowCount || row < 0)
                Debug.LogError($"CsvSheet: Row {row} out of range!");
            if (col >= _colCount || col < 0)
                Debug.LogError($"CsvSheet: Column {col} out of range!");

            // 不存在结果，则返回空字符串
            return _sheetDic.GetValueOrDefault(new Index(row, col), "");
        }
        set {
            _sheetDic[new Index(row, col)] = value;

            // 记录最大行数和列数
            if (row >= _rowCount) _rowCount = row + 1;
            if (col >= _colCount) _colCount = col + 1;
        }
    }

    /// <summary>
    /// 存储 csv 文件
    /// </summary>
    /// <param name="filePath">文件路径，需要在末尾写 .csv</param>
    public void Save(string filePath) {
        string fullPath      = SAVE_PATH + filePath;                  // 文件完整路径
        string directoryPath = StringUtil.GetDirectoryPath(fullPath); // 文件目录路径

        if (!Directory.Exists(directoryPath)) // 不存在文件目录，则创建
            Directory.CreateDirectory(directoryPath);

        using FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write);

        Index idx = new Index(0, 0);
        for (int i = 0; i < _rowCount; i++) {
            idx.Row = i;
            idx.Col = 0;

            // 写入第一个 value
            var value = _sheetDic.GetValueOrDefault(idx, "");
            fs.Write(Encoding.UTF8.GetBytes(value));

            // 写入后续 value，需要添加 ","
            for (int j = 1; j < _colCount; j++) {
                idx.Col = j;
                value = "," + _sheetDic.GetValueOrDefault(idx, "");
                fs.Write(Encoding.UTF8.GetBytes(value));
            }

            // 写入 "\n"
            fs.Write(Encoding.UTF8.GetBytes("\n"));
        }

        Debug.Log($"CsvSheet: Save \"{filePath}\" successfully");
    }

    /// <summary>
    /// 读取 csv 文件
    /// </summary>
    /// <param name="filePath">文件路径，需要在末尾写 .csv</param>
    public void Load(string filePath) {
        // 清空当前数据
        _sheetDic.Clear();
        _rowCount = _colCount = 0;

        string fullPath = SAVE_PATH + filePath; // 文件完整路径

        if (!File.Exists(fullPath)) // 不存在文件，则报错
            Debug.LogError($"CsvSheet: Can't find path \"{fullPath}\"");

        // 读取文件
        string[] lines = File.ReadAllLines(fullPath); // 读取所有行
        for (int i = 0; i < lines.Length; i++) {
            string[] line = lines[i].Split(','); // 读取一行，逗号分割
            for (int j = 0; j < line.Length; j++) {
                if (line[j] != "") // 有数据才记录
                    _sheetDic.Add(new Index(i, j), line[j]);
            }

            // 更新最大行数和列数
            _colCount = Mathf.Max(_colCount, line.Length);
            _rowCount = i + 1;
        }

        Debug.Log($"CsvSheet: Load \"{filePath}\" successfully");
    }
}

public partial class CsvSheet
{
    public struct Index
    {
        public int Row;
        public int Col;

        public Index(int row, int col) {
            Row = row;
            Col = col;
        }
    }
}