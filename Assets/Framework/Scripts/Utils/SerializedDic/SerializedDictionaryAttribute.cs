using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace SerializedDic
{
    /// <summary>
    /// SerializedDictionary 特性，用于指定序列化字典的键和值的名称
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    public class SerializedDictionaryAttribute : Attribute
    {
        public readonly string KeyName;
        public readonly string ValueName;

        public SerializedDictionaryAttribute(string keyName = null, string valueName = null) {
            KeyName = keyName;
            ValueName = valueName;
        }
    }
}