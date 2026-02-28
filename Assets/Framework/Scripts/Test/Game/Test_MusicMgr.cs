using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_MusicMgr : MonoBehaviour
{
    public float bgVolume     = 0.3f;
    public float lastBgVolume = 0.3f;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    private void OnGUI() {
        GUILayout.BeginVertical();
        
        if (GUILayout.Button("播放背景音乐", GUILayout.Width(150), GUILayout.Height(60))) {
            MusicMgr.Instance.PlayBgMusic("Pillow talk");
        }

        if (GUILayout.Button("停止背景音乐", GUILayout.Width(150), GUILayout.Height(60))) {
            MusicMgr.Instance.StopBgMusic();
        }

        if (GUILayout.Button("暂停背景音乐", GUILayout.Width(150), GUILayout.Height(60))) {
            MusicMgr.Instance.PauseBgMusic();
        }

        bgVolume = GUILayout.HorizontalSlider(bgVolume, 0, 1, GUILayout.Width(400));

        if (!Mathf.Approximately(lastBgVolume, bgVolume)) {
            lastBgVolume = bgVolume;
            MusicMgr.Instance.SetBgVolume(bgVolume);
            MusicMgr.Instance.SetAllSoundVolume(bgVolume);
        }

        if (GUILayout.Button("播放音效", GUILayout.Width(150), GUILayout.Height(60))) {
            MusicMgr.Instance.PlaySound("BallHitPlastic");
        }

        if (GUILayout.Button("播放循环音效", GUILayout.Width(150), GUILayout.Height(60))) {
            MusicMgr.Instance.PlaySound("BallHitPlastic", isLoop: true);
        }

        if (GUILayout.Button("清除循环音效", GUILayout.Width(150), GUILayout.Height(60))) {
            MusicMgr.Instance.ClearAllSound();
        }

        if (GUILayout.Button("暂停循环音效", GUILayout.Width(150), GUILayout.Height(60))) {
            MusicMgr.Instance.PauseAllSound();
        }

        if (GUILayout.Button("继续播放循环音效", GUILayout.Width(150), GUILayout.Height(60))) {
            MusicMgr.Instance.ContinueAllSound();
        }
        
        GUILayout.EndVertical();
    }
}