using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Test_ResourceMgr : MonoBehaviour
{
    public int x = 0;

    private void Update() { }

    private void OnGUI() {
        GUILayout.BeginVertical();

        if (GUILayout.Button("Load Cube", GUILayout.Width(150), GUILayout.Height(60))) {
            GameObject res = ResourceMgr.Instance.Load<GameObject>("Test/Prefab/Cube");
            GameObject obj = Instantiate(res);
            obj.transform.Translate(x++, 0, 0);
        }

        if (GUILayout.Button("Load Sphere", GUILayout.Width(150), GUILayout.Height(60))) {
            GameObject res = ResourceMgr.Instance.Load<GameObject>("Test/Prefab/Sphere");
            GameObject obj = Instantiate(res);
            obj.transform.Translate(x++, 0, 0);
        }

        if (GUILayout.Button("Load Texture", GUILayout.Width(150), GUILayout.Height(60))) {
            Texture res = ResourceMgr.Instance.Load<Texture>("Test/Texture/New Render Texture");
        }

        if (GUILayout.Button("Unload Cube", GUILayout.Width(150), GUILayout.Height(60))) {
            ResourceMgr.Instance.UnloadAsset<GameObject>("Test/Prefab/Cube", false);
        }

        if (GUILayout.Button("Unload Texture", GUILayout.Width(150), GUILayout.Height(60))) {
            ResourceMgr.Instance.UnloadAsset<Texture>("Test/Texture/New Render Texture", true);
        }

        if (GUILayout.Button("Unload All", GUILayout.Width(150), GUILayout.Height(60))) {
            ResourceMgr.Instance.UnloadUnusedAssets(null);
        }

        GUILayout.EndVertical();
    }
}