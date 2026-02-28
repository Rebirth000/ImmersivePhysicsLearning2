using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class Test_UIMgr : MonoBehaviour
{
    void Start() {
        // Debug.LogError("hello");
        
        UIMgr.Instance.ShowPanel<Test_BeginPanel>(callBack: (panel) => {
            Debug.Log("First call!");
        }, isAsync: false);
        UIMgr.Instance.HidePanel<Test_BeginPanel>();
        UIMgr.Instance.ShowPanel<Test_BeginPanel>(callBack: (panel) => {
            Debug.Log("Second call!");
        });
        
        UIMgr.Instance.GetPanel<Test_BeginPanel>((panel) => {
            Debug.Log("I get the Test_BeginPanel!");
        });
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.S)) {
            UIMgr.Instance.ShowPanel<Test_BeginPanel>();
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            UIMgr.Instance.HidePanel<Test_BeginPanel>();
        }
    }
}