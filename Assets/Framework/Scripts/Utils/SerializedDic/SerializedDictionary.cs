using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SerializedDic
{
    /// <summary>
    /// 可序列化字典，通过序列化列表实现
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        internal List<SerializedKeyValuePair<TKey, TValue>> _serializedList = new List<SerializedKeyValuePair<TKey, TValue>>(); // 序列化列表

        #region Constructors 重载构造函数

        public SerializedDictionary() : base() { }

        public SerializedDictionary(SerializedDictionary<TKey, TValue> serializedDictionary) : base(serializedDictionary) {
#if UNITY_EDITOR
            foreach (var kvp in serializedDictionary._serializedList)
                _serializedList.Add(new SerializedKeyValuePair<TKey, TValue>(kvp.Key, kvp.Value));
#endif
        }

        public SerializedDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) {
            SyncDictionaryToBackingField_Editor();
        }

        public SerializedDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(
             dictionary, comparer) {
            SyncDictionaryToBackingField_Editor();
        }

        public SerializedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : base(collection) {
            SyncDictionaryToBackingField_Editor();
        }

        public SerializedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection,
                                    IEqualityComparer<TKey>                 comparer) : base(collection, comparer) {
            SyncDictionaryToBackingField_Editor();
        }

        public SerializedDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }

        public SerializedDictionary(int capacity) : base(capacity) { }

        public SerializedDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) { }

        #endregion

        #region Override Functions 部分方法需要重载

#if UNITY_EDITOR
        public new TValue this[TKey key] {
            get => base[key];
            set {
                base[key] = value;

                bool hasKey = false; // 标记是否找到键
                for (var i = 0; i < _serializedList.Count; i++) { // 只能用 for 而不是 foreach
                    var pair = _serializedList[i];
                    if (pair.Key.Equals(key) || (object) pair.Key == (object) key) { // 如果键相等或者键的对象相等
                        hasKey = true;
                        // 更新键对应的值
                        pair.Value = value;
                        _serializedList[i] = pair;
                        break;
                    }
                }
                
                if (!hasKey) { // 如果找不到键，则添加一个新的键值对
                    _serializedList.Add(new SerializedKeyValuePair<TKey, TValue>(key, value));
                }
            }
        }

        public new void Add(TKey key, TValue value) {
            base.Add(key, value);
            _serializedList.Add(new SerializedKeyValuePair<TKey, TValue>(key, value)); // 添加序列化键值对
        }

        public new void Clear() {
            base.Clear();
            _serializedList.Clear(); // 清空序列化列表
        }

        public new bool Remove(TKey key) {
            if (TryGetValue(key, out var value)) {
                base.Remove(key);
                _serializedList.Remove(new SerializedKeyValuePair<TKey, TValue>(key, value)); // 移除序列化键值对
                return true;
            }

            return false;
        }

        public new bool TryAdd(TKey key, TValue value) {
            if (base.TryAdd(key, value)) {
                _serializedList.Add(new SerializedKeyValuePair<TKey, TValue>(key, value));
                return true;
            }

            return false;
        }
#endif

        #endregion
        
        public void OnBeforeSerialize() {
#if UNITY_EDITOR
            if (_serializedList.Count == 0 && Count > 0)
                SyncDictionaryToBackingField_Editor(); // 直接使用 SyncDictionaryToBackingField_Editor 方法添加序列化记录
#else
            _serializedList.Clear();
            foreach (var pair in this) // 没有 SyncDictionaryToBackingField_Editor 方法，只能手写一遍
                _serializedList.Add(new SerializedKeyValuePair<TKey, TValue>(pair.Key, pair.Value)); 
#endif
        }

        public void OnAfterDeserialize() {
            base.Clear();
            foreach (var pair in _serializedList) {
#if UNITY_EDITOR
                if (!ContainsKey(pair.Key)) { // 需要去重
                    base.Add(pair.Key, pair.Value);
                }
                else { // 重复的键报个警告
                    Debug.LogWarning($"{nameof(SerializedDictionary<TKey, TValue>)}: Duplicate key detected: {pair.Key}");
                }
#else
                Add(pair.Key, pair.Value); // 不在 UNITY_EDITOR 下，直接添加即可
#endif
            }

#if UNITY_EDITOR
#else
            _serializedList.Clear();
#endif
        }
        
        /// <summary>
        /// 在 UNITY_EDITOR 下记录键值对，仅在 UNITY_EDITOR 下有效
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        private void SyncDictionaryToBackingField_Editor() {
            foreach (var pair in this)
                _serializedList.Add(new SerializedKeyValuePair<TKey, TValue>(pair.Key, pair.Value));
        }
    }

    /// <summary>
    /// 可序列化键值对，需要声明为 Struct
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public struct SerializedKeyValuePair<TKey, TValue>
    {
        public TKey   Key;
        public TValue Value;

        public SerializedKeyValuePair(TKey key, TValue value) {
            Key = key;
            Value = value;
        }
    }
}