using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_TimerMgr : MonoBehaviour
{
    public int timerId1 = 1;
    public int timerId2 = 1;
    
    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    private void OnGUI() {
        GUILayout.BeginVertical();

        if (GUILayout.Button("Add timer1", GUILayout.Width(150), GUILayout.Height(60))) {
            timerId1 = TimerMgr.Instance.CreateTimer(5, () => Debug.Log("5s 计时结束！"),
                                                    0.1f, () => Debug.Log("0.1s 间隔计时"), true);
        }
        
        if (GUILayout.Button("Add timer2", GUILayout.Width(150), GUILayout.Height(60))) {
            timerId2 = TimerMgr.Instance.CreateTimer(3, () => Debug.Log("3s 计时结束！"), isRealTime: false);
        }
        
        if (GUILayout.Button("Stop timer1", GUILayout.Width(150), GUILayout.Height(60))) {
            TimerMgr.Instance.RemoveTimer(timerId1);
        }
        
        if (GUILayout.Button("Stop timer2", GUILayout.Width(150), GUILayout.Height(60))) {
            TimerMgr.Instance.RemoveTimer(timerId2);
        }
        
        if (GUILayout.Button("TimeScale = 0", GUILayout.Width(150), GUILayout.Height(60))) {
            Time.timeScale = 0;
        }
        
        if (GUILayout.Button("TimeScale = 1", GUILayout.Width(150), GUILayout.Height(60))) {
            Time.timeScale = 1;
        }

        GUILayout.EndVertical();
    }
}