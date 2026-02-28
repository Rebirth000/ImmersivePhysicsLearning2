using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_ABMgr : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        // // TODO 在进行异步加载时，同步加载会报错
        // StartCoroutine(LoadAsync());
        // AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/model");

        ABResMgr.Instance.IsDebug = false;
    }

    public IEnumerator LoadAsync() {
        AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/model");
        yield return req;
        Debug.Log(req.assetBundle.name);
    }

    private void OnGUI() {
        GUILayout.BeginVertical();
        
        if (GUILayout.Button("Load Cube Async", GUILayout.Width(150), GUILayout.Height(60))) {
            ABResMgr.Instance.LoadResAsync<GameObject>("model", "Cube", (obj) => {
                GameObject gameObj = Instantiate(obj);
                gameObj.name = "ABMgrCreate";
            }, true);
        }
        
        if (GUILayout.Button("Load Cube Sync", GUILayout.Width(150), GUILayout.Height(60))) {
            ABResMgr.Instance.LoadResAsync<GameObject>("model", "Cube", (obj) => {
                GameObject gameObj = Instantiate(obj);
                gameObj.name = "ABMgrCreate";
            }, false);
        }
        
        GUILayout.EndVertical();
    }

    // Update is called once per frame
    void Update() { }
}