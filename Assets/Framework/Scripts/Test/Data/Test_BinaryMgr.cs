using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Test_BinaryMgr : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        Debug.Log($"BinaryMgr SAVE_PATH: \"{JsonMgr.SAVE_PATH}\"");
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
            BinaryMgr.Instance.Save("test", obj);
        }

        if (GUILayout.Button("Save 2", GUILayout.Width(150), GUILayout.Height(60))) {
            var obj = new TestData2() {
                name = "test2",
                age = 15,
                sex = true,
                data = new TestData(),
            };
            BinaryMgr.Instance.Save("11/test", obj);
        }

        if (GUILayout.Button("Save 3", GUILayout.Width(150), GUILayout.Height(60))) {
            var obj = new TestData3 {
                data = new List<TestData>(new TestData[5]),
                data2 = new TestData[3],
            };
            BinaryMgr.Instance.Save("testList", obj);
        }

        if (GUILayout.Button("Load 1", GUILayout.Width(150), GUILayout.Height(60))) {
            var obj = BinaryMgr.Instance.Load<TestData>("test");
            Debug.Log($"\'TestData\' name: {obj.name}, age: {obj.age}");
        }

        if (GUILayout.Button("Load 2", GUILayout.Width(150), GUILayout.Height(60))) {
            var obj = BinaryMgr.Instance.Load<TestData2>("11/test");
            Debug.Log($"\'TestData2\' name: {obj.name}, age: {obj.age}, sex: {obj.sex}, data: ({obj.data.name}, {obj.data.age})");
        }
        
        if (GUILayout.Button("Load 3", GUILayout.Width(150), GUILayout.Height(60))) {
            var obj = BinaryMgr.Instance.Load<TestData3>("testList");
            Debug.Log($"\'TestData3\' data: {obj.data}, data2: {obj.data2}");
        }

        GUILayout.EndVertical();
    }

    [Serializable]
    public class TestData
    {
        public string name;
        public int    age;
    }

    [Serializable]
    public class TestData2
    {
        public string   name;
        public int      age;
        public bool     sex;
        public TestData data;
    }

    [Serializable]
    public class TestData3
    {
        public List<TestData> data;
        public TestData[]     data2;
    }
}