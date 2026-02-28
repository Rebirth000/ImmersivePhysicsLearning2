using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_EditorResMgr : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    private void OnGUI() {
        GUILayout.BeginVertical();

        if (GUILayout.Button("Load Cube", GUILayout.Width(150), GUILayout.Height(60))) {
            GameObject res = EditorResMgr.Instance.LoadRes<GameObject>("Framework/Editor/ArtRes/Prefab/Cube");
            Instantiate(res);
        }

        GUILayout.EndVertical();
    }
}