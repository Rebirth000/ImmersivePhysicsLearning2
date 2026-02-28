using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class MgrWindowBase<T> : EditorWindow where T : MgrWindowBase<T>
{
    protected SerializedObject _serializedObject;

    protected Vector2 _scrollViewPos; // 滚动视图框的范围

    protected bool _isNowPlaying  = false; // 当前帧是否是运行状态
    protected bool _isLastPlaying = false; // 上一帧是否是运行状态

    protected UnityAction _StartPlayingDo = null; // Unity 开始运行时执行的委托
    protected UnityAction _StopPlayingDo  = null; // Unity 结束运行时执行的委托

    /// <summary>
    /// 初始化成员引用
    /// </summary>
    protected virtual void Init() {
        _serializedObject = new SerializedObject(this);
    }

    /// <summary>
    /// 更新运行状态，并执行委托
    /// </summary>
    protected void CheckRunning() {
        _isLastPlaying = _isNowPlaying;
        _isNowPlaying = Application.isPlaying;

        if (_isNowPlaying && !_isLastPlaying) { // 开始运行
            _StartPlayingDo?.Invoke();
        }

        if (!_isNowPlaying && _isLastPlaying) { // 停止运行
            _StopPlayingDo?.Invoke();
        }
    }

    protected virtual void OnEnable() {
        _StartPlayingDo = Init;

        // Waiting for Subclass to do
    }

    protected virtual void OnDisable() {
        _StartPlayingDo = null;
        _StopPlayingDo = null;
    }

    protected void OnGUI() {
        CheckRunning();

        if (!_isNowPlaying) { // 编辑状态下
            OnGUIWhenOnEditor();
        }
        else { // 运行状态下
            _serializedObject.Update();
            _scrollViewPos = EditorGUILayout.BeginScrollView(_scrollViewPos); // 开启滚动视图

            OnGUIWhenOnPlay();

            EditorGUILayout.EndScrollView(); // 结束滚动视图
            _serializedObject.ApplyModifiedProperties();
        }
    }

    /// <summary>
    /// 运行状态下显示的内容
    /// </summary>
    protected virtual void OnGUIWhenOnPlay() { }

    /// <summary>
    /// 编辑状态下显示的内容
    /// </summary>
    protected virtual void OnGUIWhenOnEditor() {
        EditorGUILayout.LabelField("Please Play First!");
    }

    protected virtual void Update() {
        Repaint();
    }
}