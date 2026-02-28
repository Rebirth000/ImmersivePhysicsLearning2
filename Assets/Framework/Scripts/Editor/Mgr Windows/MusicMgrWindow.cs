using System;
using System.Collections;
using System.Collections.Generic;
using SerializedDic;
using UnityEditor;
using UnityEngine;

public class MusicMgrWindow : MgrWindowBase<MusicMgrWindow>
{
    [SerializeField]
    private List<AudioSource> _soundList;

    private SerializedProperty _soundListProperty;

    private const float LabelWidth = 100;  // 成员名称显示宽度
    private const float Height     = 20f;  // 每行的高度
    private const float SpaceWidth = 4f;   // GUILayout 自动布局的间隔宽度
    
    [MenuItem("Framework/Windows/" + nameof(MusicMgrWindow))]
    private static void ShowWindow() {
        MusicMgrWindow win = GetWindow<MusicMgrWindow>();
        win.Show();
    }

    protected override void Init() {
        base.Init();
        _soundList = MusicMgr.Instance.SoundList;
        _soundListProperty = _serializedObject.FindProperty(nameof(_soundList));
    }

    protected override void OnGUIWhenOnPlay() {
        EditorTool.GUITextHorizontal("ResourceMgrLoaderBgMusicPath", MusicMgr.Instance.ResourceMgrLoaderBgMusicPath,
                                     LabelWidth, position.width - LabelWidth - SpaceWidth * 3, Height, MouseCursor.Arrow);

        EditorTool.GUITextHorizontal("ResourceMgrLoaderSoundPath", MusicMgr.Instance.ResourceMgrLoaderSoundPath,
                                     LabelWidth, position.width - LabelWidth - SpaceWidth * 3, Height, MouseCursor.Arrow);
        EditorTool.GUITextHorizontal("SoundObjPath", MusicMgr.Instance.SoundObjPath,
                                     LabelWidth, position.width - LabelWidth - SpaceWidth * 3, Height, MouseCursor.Arrow);

        EditorTool.GUIObjectHorizontal("BgMusic", MusicMgr.Instance.BgMusic, typeof(AudioSource),
                                       LabelWidth, position.width - LabelWidth - SpaceWidth * 3, Height, MouseCursor.Arrow);

        float bgMusicVolume = EditorTool.GUISliderHorizontal("BgMusicVolume", MusicMgr.Instance.BgMusicVolume, 0, 1,
                                                             LabelWidth, position.width - LabelWidth - SpaceWidth * 3, Height);
        MusicMgr.Instance.SetBgVolume(bgMusicVolume);

        float soundVolume = EditorTool.GUISliderHorizontal("SoundVolume", MusicMgr.Instance.SoundVolume, 0, 1,
                                                           LabelWidth, position.width - LabelWidth - SpaceWidth * 3, Height);
        MusicMgr.Instance.SetAllSoundVolume(soundVolume);

        bool lastPaused = MusicMgr.Instance.IsSoundPaused;
        bool paused = EditorTool.GUIToggleHorizontal("IsSoundPaused", MusicMgr.Instance.IsSoundPaused,
                                       LabelWidth, position.width - LabelWidth - SpaceWidth * 3, Height);
        
        if (paused && !lastPaused) 
            MusicMgr.Instance.PauseAllSound();
        if (!paused && lastPaused) 
            MusicMgr.Instance.ContinueAllSound();

        EditorGUILayout.PropertyField(_soundListProperty, new GUIContent("Sound List"));
    }
}