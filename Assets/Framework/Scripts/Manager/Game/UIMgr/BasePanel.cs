using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 面板基类
/// </summary>
public abstract class BasePanel : MonoBehaviour
{
    protected Dictionary<string, UIBehaviour> _controlDic = new Dictionary<string, UIBehaviour>();

    // 默认控件名称，用于查找控件时对其进行过滤
    private static List<string> _defaultNames = new List<string>() {
        "Image",
        "Text (TMP)",
        "RawImage",
        "Background",
        "Checkmark",
        "Label",
        "Text (Legacy)",
        "Arrow",
        "Placeholder",
        "Fill",
        "Handle",
        "Viewport",
        "Scrollbar Horizontal",
        "Scrollbar Vertical"
    };

    protected virtual void Awake() {
        // 某个对象上可能有多个控件，查找时应该注意查找顺序，先查找重要的组件
        FindChildrenUIBehaviour<Button>();
        FindChildrenUIBehaviour<Toggle>();
        FindChildrenUIBehaviour<Slider>();
        FindChildrenUIBehaviour<InputField>();
        FindChildrenUIBehaviour<ScrollRect>();
        FindChildrenUIBehaviour<Dropdown>();

        // 相对不重要的组件
        FindChildrenUIBehaviour<Text>();
        FindChildrenUIBehaviour<TextMeshPro>();
        FindChildrenUIBehaviour<Image>();
    }

    /// <summary>
    /// 获取某个 UI 控件
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetUIBehaviour<T>(string name) where T : UIBehaviour {
        if (_controlDic.TryGetValue(name, out UIBehaviour value)) {
            if (value == null)
                Debug.LogError($"{typeof(T).Name} in {name} is null!");
            return value as T;
        }
        else {
            Debug.LogError($"{typeof(T).Name} in {name} doesn't exist!");
            return null;
        }
    }

    /// <summary>
    /// 获取所有子物体上的 UI 控件，并添加对应的监听事件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void FindChildrenUIBehaviour<T>() where T : UIBehaviour {
        T[] behaviours = GetComponentsInChildren<T>(true);
        foreach (T bh in behaviours) {
            string bName = bh.gameObject.name;
            if (_defaultNames.Contains(bName)) continue; // 过滤未改名的默认控件

            if (_controlDic.TryAdd(bName, bh)) {
                switch (bh) { // 为对应的 UIBehaviour 添加事件监听
                    case Button btn:
                        btn.onClick.AddListener(() => OnClickButton(bName));
                        break;
                    case Slider sld:
                        sld.onValueChanged.AddListener((value) => OnSliderValueChanged(bName, value));
                        break;
                    case Toggle tog:
                        tog.onValueChanged.AddListener((value) => OnToggleValueChanged(bName, value));
                        break;
                    case InputField ifd:
                        ifd.onValueChanged.AddListener((value) => OnInputFieldValueChanged(bName, value));
                        break;
                }
            }
            else { // 尝试添加，失败则警告
                Debug.LogWarning($"{bh.gameObject.name}'s {bh.name} already exists in _controlDic!", this);
            }
        }
    }

    #region 需由子类实现的方法

    protected virtual void OnClickButton(string btnName) { }

    protected virtual void OnSliderValueChanged(string sldName, float value) { }

    protected virtual void OnToggleValueChanged(string togName, bool value) { }

    protected virtual void OnInputFieldValueChanged(string togName, string value) { }

    public abstract void Show();

    public abstract void Hide();

    #endregion
}