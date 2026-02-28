using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Test_JsonMgr : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        Debug.Log($"JsonMgr SAVE_PATH: \"{JsonMgr.SAVE_PATH}\"");
    }

    // Update is called once per frame
    void Update() { }

    private void OnGUI() {
        GUILayout.BeginVertical();

        if (GUILayout.Button("Save 1", GUILayout.Width(150), GUILayout.Height(60))) {
            var obj = new TestData() {
                name = "test",
                age = 18,
            };
            JsonMgr.Instance.Save("test.json", obj);
        }

        if (GUILayout.Button("Save 2", GUILayout.Width(150), GUILayout.Height(60))) {
            var obj = new TestData2() {
                name = "test2",
                age = 15,
                sex = true,
            };
            JsonMgr.Instance.Save("11/test.json", obj);
        }

        if (GUILayout.Button("Save 3", GUILayout.Width(150), GUILayout.Height(60))) {
            var obj = new TestData3 {
                data = new List<TestData>(new TestData[5]),
                data2 = new TestData[3],
            };
            JsonMgr.Instance.Save("testList.json", obj);
        }

        if (GUILayout.Button("Load 1", GUILayout.Width(150), GUILayout.Height(60))) {
            var obj = JsonMgr.Instance.Load<TestData>("test.json");
            Debug.Log($"\'TestData\' name: {obj.name}, age: {obj.age}");
        }

        if (GUILayout.Button("Load 2", GUILayout.Width(150), GUILayout.Height(60))) {
            var obj = JsonMgr.Instance.Load<TestData2>("11/test.json");
            Debug.Log($"\'TestData2\' name: {obj.name}, age: {obj.age}, sex: {obj.sex}, data: ({obj.data.name}, {obj.data.age})");
        }

        GUILayout.EndVertical();
    }

    [Serializable]
    public class TestData
    {
        public string name;
        public int    age;
    }

    public class TestData2
    {
        public string   name;
        public int      age;
        public bool     sex;
        public TestData data;
    }

    public class TestData3
    {
        public List<TestData> data;
        public TestData[]     data2;
    }
}