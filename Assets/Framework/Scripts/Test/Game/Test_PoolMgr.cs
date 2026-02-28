using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public class Test_PoolMgr : MonoBehaviour
{
    public float existTime = 1f;

    private void Update() { }

    private void OnGUI() {
        GUILayout.BeginVertical();

        if (GUILayout.Button("取出 Cube", GUILayout.Width(150), GUILayout.Height(60))) {
            GameObject cube = PoolMgr.Instance.Pop("Test/Prefab/Cube");
            StartCoroutine(Destroy(cube));
        }
        
        if (GUILayout.Button("取出 Sphere", GUILayout.Width(150), GUILayout.Height(60))) {
            GameObject sphere = PoolMgr.Instance.Pop("Test/Prefab/Sphere");
            StartCoroutine(Destroy(sphere));
        }
        
        if (GUILayout.Button("取出 TestData", GUILayout.Width(150), GUILayout.Height(60))) {
            TestData data = PoolMgr.Instance.PopData<TestData>();
            data.i = 1;
            Debug.Log(data.Info());
            StartCoroutine(PushData(data));
        }

        GUILayout.EndVertical();
    }

    private IEnumerator Destroy(GameObject obj) {
        yield return new WaitForSeconds(existTime);
        PoolMgr.Instance.Push(obj);
    }

    private IEnumerator PushData(TestData data) {
        yield return new WaitForSeconds(existTime);
        PoolMgr.Instance.PushData<TestData>(data);
    }
}

[Serializable]
public class TestData : IPoolData
{
    public int    i;
    public string str;
    public object o;

    public void ResetData() {
        i = 0;
        str = "";
        o = null;
    }

    public string Info() {
        return $"i: {i}, str: {str}, o: {o}";
    }
}