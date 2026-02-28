using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class Test_SceneMgr : MonoBehaviour
{
    void Start() {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update() { }

    public void OnLoading(float f) {
        Debug.Log($"Progress: {f}");
    }

    public void OnButtonClick1() {
        SceneMgr.Instance.LoadSceneAsync("Test", () => {
            Debug.Log("Change Scene to Test!");
        }, OnLoading);
    }
    
    public void OnButtonClick2() {
        SceneMgr.Instance.LoadSceneAsync("SceneMgrTest", () => {
            Debug.Log("Comeback to SceneMgrTest!");
            GameObject test = GameObject.Find("Test");
            Destroy(test);
        }, OnLoading);
    }
}