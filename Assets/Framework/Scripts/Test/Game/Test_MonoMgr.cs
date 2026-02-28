using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_MonoMgr : MonoBehaviour
{
    public bool counting = true;


    // public SerializableDictionary<int, string> dic = new SerializableDictionary<int, string>() { };

    // Start is called before the first frame update
    void Start() {
        if (counting) MonoMgr.Instance.AddUpdateFunc(Timer, this, GetType());
        MonoMgr.Instance.AddUpdateFunc(PressALog, this, GetType(), MonoMgr.EUpdateType.EFixedUpdateEvent);
        MonoMgr.Instance.AddUpdateFunc(PressBLog, this, GetType(), MonoMgr.EUpdateType.ELateUpdateEvent);
        MonoMgr.Instance.AddUpdateFunc(PressCLog, this, GetType(), MonoMgr.EUpdateType.EUpdateEvent);
    }
    
    private void OnGUI() {
        GUILayout.BeginVertical();
        
        if (GUILayout.Button("添加计时器", GUILayout.Width(150), GUILayout.Height(60))) {
            MonoMgr.Instance.AddUpdateFunc(Timer, this, GetType());
        }
        
        if (GUILayout.Button("移除计时器", GUILayout.Width(150), GUILayout.Height(60))) {
            MonoMgr.Instance.RemoveUpdateFunc(Timer);
        }
        
        GUILayout.EndVertical();
    }

    public void Timer() {
        Debug.Log($"Time: {Time.time}");
    }

    public void PressALog() {
        if (Input.GetKeyDown(KeyCode.A))
            Debug.Log("Test_MonoMgr.cs: PressALog()");
    }
    
    public void PressBLog() {
        if (Input.GetKeyDown(KeyCode.A))
            Debug.Log("Test_MonoMgr.cs: PressALog()");
    }
    
    public void PressCLog() {
        if (Input.GetKeyDown(KeyCode.A))
            Debug.Log("Test_MonoMgr.cs: PressALog()");
    }
}