using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Test_TxtMgr : MonoBehaviour
{
    public string content;

    public string loadContent;

    // Start is called before the first frame update
    void Start() {
        Debug.Log($"TxtMgr SAVE_PATH: \"{TxtMgr.SAVE_PATH}\"");
    }

    // Update is called once per frame
    void Update() { }

    private void OnGUI() {
        GUILayout.BeginVertical();

        if (GUILayout.Button("Save content", GUILayout.Width(150), GUILayout.Height(60))) {
            TxtMgr.Instance.Save("test.txt", content);
        }

        content = GUILayout.TextField(content);

        if (GUILayout.Button("Load content", GUILayout.Width(150), GUILayout.Height(60))) {
            loadContent = TxtMgr.Instance.Load("test.txt");
        }

        GUILayout.Label($"content: {loadContent}");

        GUILayout.EndVertical();
    }
}