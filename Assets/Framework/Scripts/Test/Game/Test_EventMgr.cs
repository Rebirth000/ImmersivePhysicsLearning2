using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_EventMgr : MonoBehaviour
{
    public float gapTime = 1f;

    public Coroutine c;

    // Start is called before the first frame update
    void Start() {
        EventMgr.Instance.AddListener(nameof(StartTimer), StartTimer, this, GetType());
        EventMgr.Instance.AddListener(nameof(StopTimer), StopTimer, this, GetType());
        EventMgr.Instance.AddListener<KeyCode>(nameof(PrintKeyCode), PrintKeyCode, this, GetType());
        EventMgr.Instance.AddListener(nameof(PrintHello), PrintHello, this, GetType());
    }

    // Update is called once per frame
    void Update() { }

    private void OnGUI() {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("StartTimer", GUILayout.Width(150), GUILayout.Height(60))) {
            EventMgr.Instance.EventTrigger(nameof(StartTimer));
        }

        if (GUILayout.Button("StopTimer", GUILayout.Width(150), GUILayout.Height(60))) {
            EventMgr.Instance.EventTrigger(nameof(StopTimer));
        }

        if (GUILayout.Button("PrintKeyCode", GUILayout.Width(150), GUILayout.Height(60))) {
            EventMgr.Instance.EventTrigger<KeyCode>(nameof(PrintKeyCode), KeyCode.C);
            EventMgr.Instance.EventTrigger<object>(nameof(PrintKeyCode), KeyCode.C);
        }

        if (GUILayout.Button("PrintHello", GUILayout.Width(150), GUILayout.Height(60))) {
            EventMgr.Instance.EventTrigger(nameof(PrintHello));
        }

        if (GUILayout.Button("Remove PrintHello", GUILayout.Width(150), GUILayout.Height(60))) {
            EventMgr.Instance.RemoveListener(nameof(PrintHello), PrintHello);
        }

        if (GUILayout.Button("Add PrintHello", GUILayout.Width(150), GUILayout.Height(60))) {
            EventMgr.Instance.AddListener(nameof(PrintHello), PrintHello, this, GetType());
        }

        if (GUILayout.Button("Add PrintHello2", GUILayout.Width(150), GUILayout.Height(60))) {
            EventMgr.Instance.AddListener(nameof(PrintHello), PrintHello2, this, GetType());
        }

        GUILayout.EndHorizontal();
    }

    public IEnumerator Timer() {
        while (true) {
            Debug.Log($"Time: {Time.time}");
            yield return new WaitForSeconds(gapTime);
        }
    }

    public void StartTimer() {
        c ??= StartCoroutine(Timer());
    }

    public void StopTimer() {
        if (c != null) StopCoroutine(c);
        c = null;
    }

    public void PrintKeyCode(KeyCode key) {
        Debug.Log($"{key} pressed!");
    }

    public void PrintHello() {
        Debug.Log($"Hello!");
    }

    public void PrintHello2() {
        Debug.Log($"Hello2!");
    }
}