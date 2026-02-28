using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Test_BeginPanel : BasePanel
{
    // Start is called before the first frame update
    void Start() {
        Button btn = GetUIBehaviour<Button>("BtnSetting");
        
        Debug.Log($"GetUIBehaviour<Button>(\"BtnSetting\") -> {btn}");
        
        UIMgr.AddCustomEventListener(btn, EventTriggerType.PointerExit, data => {
            Debug.Log("BtnSetting PointerExit");
        });
    }

    protected override void OnClickButton(string btnName) {
        switch (btnName) {
            case "BtnBegin":
                Debug.Log("BtnBegin clicked");
                break;
            case "BtnSetting":
                Debug.Log("BtnSetting clicked");
                break;
            case "BtnQuit":
                Debug.Log("BtnQuit clicked");
                break;
        }
    }

    public override void Show() {
        Debug.Log("Test_BeginPanel Show");
    }

    public override void Hide() { }
}