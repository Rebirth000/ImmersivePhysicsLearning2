using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

/// <summary>
/// 音乐管理器，默认使用 ResourceMgr 加载
/// </summary>
public partial class MusicMgr : Singleton<MusicMgr>
{
    private MusicMgr() {
        // FixedUpdate 来延长更新时间间隔
        MonoMgr.Instance.AddUpdateFunc(Update, null, null, MonoMgr.EUpdateType.EFixedUpdateEvent);
    }

    private AudioSource _bgMusic = null;
    public  AudioSource BgMusic => _bgMusic;

    public readonly string ResourceMgrLoaderBgMusicPath = "Music/"; // 通过 ResourceMgr 方式加载的 BgMusic 需放在该目录下
    public readonly string ResourceMgrLoaderSoundPath   = "Sound/"; // 通过 ResourceMgr 方式加载的 Sound 需放在该目录下

    public readonly string SoundObjPath = "Prefab/Sound/SoundObj"; // 音效预制体路径
    
    private float _bgMusicVolume = 0.3f;
    public  float BgMusicVolume => _bgMusicVolume;

    private float _soundVolume = 0.3f;
    public  float SoundVolume => _soundVolume;

    private bool _isSoundPaused = false; // 是否暂停音效中
    public  bool IsSoundPaused => _isSoundPaused;
    
    private List<AudioSource> _soundList = new List<AudioSource>(); // 正在播放的音效列表

    public List<AudioSource> SoundList => _soundList;

    private void Update() {
        if (_isSoundPaused) return; // 音效在暂停中，不更新

        // 不停检测，移除播放完成的音效
        for (var i = _soundList.Count - 1; i >= 0; i--) {
            if (!_soundList[i].isPlaying) {
                _soundList[i].clip = null;
                PoolMgr.Instance.Push(_soundList[i].gameObject); // 放入缓存池中，表示被移除
                _soundList.RemoveAt(i);                          // 移除记录
            }
        }
    }

    /// <summary>
    /// 播放背景音乐，默认异步加载音乐
    /// </summary>
    /// <param name="name"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void PlayBgMusic(string name) {
        if (_bgMusic == null) { // 动态创建
            GameObject obj = new GameObject($"BgMusic: {name}");
            _bgMusic = obj.AddComponent<AudioSource>();
            Object.DontDestroyOnLoad(obj); // 过场景不移除
        }

        ResourceMgr.Instance.LoadAsync<AudioClip>(ResourceMgrLoaderBgMusicPath + name, clip => {
            _bgMusic.clip = clip;
            _bgMusic.loop = true;
            _bgMusic.volume = _bgMusicVolume;
            _bgMusic.Play();
        });
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBgMusic() {
        if (_bgMusic == null) return;
        _bgMusic.Stop();
    }

    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseBgMusic() {
        if (_bgMusic == null) return;
        _bgMusic.Pause();
    }

    /// <summary>
    /// 设置背景音乐音量
    /// </summary>
    /// <param name="v"></param>
    public void SetBgVolume(float v) {
        _bgMusicVolume = v;
        if (_bgMusic == null) return;
        _bgMusic.volume = _bgMusicVolume; // 及时修改音量
    }

    /// <summary>
    /// 播放音效，默认异步加载音效
    /// </summary>
    /// <param name="name"></param>
    /// <param name="isLoop"></param>
    /// <param name="volume"></param>
    /// <param name="callback"></param>
    public void PlaySound(string                   name,
                          float                    volume   = -1,
                          bool                     isLoop   = false,
                          UnityAction<AudioSource> callback = null) {
        // 从缓存池中取出预制体
        AudioSource source = PoolMgr.Instance.Pop(SoundObjPath).GetComponent<AudioSource>();
        source.Stop(); // 停止原先可能在播放的音效

        ResourceMgr.Instance.LoadAsync<AudioClip>(ResourceMgrLoaderSoundPath + name, clip => {
            source.clip = clip;
            source.loop = isLoop;
            source.volume = Mathf.Approximately(volume, -1) ? _soundVolume : volume;
            source.Play();
            if (!_soundList.Contains(source)) _soundList.Add(source); // 添加到列表中，且避免重复添加
            callback?.Invoke(source);
        });
    }

    /// <summary>
    /// 停止播放某个音效
    /// </summary>
    /// <param name="source">音效组件</param>
    public void StopSound(AudioSource source) {
        if (_soundList.Contains(source)) {
            source.Stop();
            _soundList.Remove(source);
            source.clip = null;
            PoolMgr.Instance.Push(source.gameObject); // 放入缓存池中
        }
    }

    /// <summary>
    /// 设置音效音量
    /// </summary>
    /// <param name="v"></param>
    public void SetAllSoundVolume(float v) {
        _soundVolume = v;
        foreach (AudioSource source in _soundList) {
            source.volume = v;
        }
    }

    /// <summary>
    /// 暂停播放所有音效
    /// </summary>
    public void PauseAllSound() {
        _isSoundPaused = true; // 标记暂停
        foreach (AudioSource source in _soundList) {
            source.Pause();
        }
    }

    /// <summary>
    /// 继续播放所有音效
    /// </summary>
    public void ContinueAllSound() {
        _isSoundPaused = false; // 解除暂停
        foreach (AudioSource source in _soundList) {
            source.Play();
        }
    }

    /// <summary>
    /// 清除所有音效，过场景时需要在清空 PoolMgr 前调用
    /// </summary>
    public void ClearAllSound() {
        foreach (AudioSource source in _soundList) {
            source.Stop();
            source.clip = null;
            PoolMgr.Instance.Push(source.gameObject);
        }

        _soundList.Clear();
    }
}