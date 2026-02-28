using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_InputMgr : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        InputMgr.Instance.EnableInput(true);

        InputMgr.Instance.AddKeyBoardListener(null, KeyCode.A, KeyCode_A_Down, InputInfo.InputType.Down);
        InputMgr.Instance.AddKeyBoardListener(null, KeyCode.D, KeyCode_D_Up, InputInfo.InputType.Up);
        InputMgr.Instance.AddKeyBoardListener("Changed 1", KeyCode.S, KeyCode_S_Keep, InputInfo.InputType.Keep);

        InputMgr.Instance.AddMouseListener(null, 0, Mouse_0_Down, InputInfo.InputType.Down);
        InputMgr.Instance.AddMouseListener(null, 1, Mouse_1_Up, InputInfo.InputType.Up);
        InputMgr.Instance.AddMouseListener(null, 2, Mouse_2_Keep, InputInfo.InputType.Keep);

        InputMgr.Instance.AddAxisListener("Changed 2", "Horizontal", Axis_Horizontal, false);
        InputMgr.Instance.AddAxisListener(null, "Vertical", AxisRaw_Vertical, true);
    }

    private void Update() { }
    
    private void OnGUI() {
        GUILayout.BeginVertical();
        
        if (GUILayout.Button("Change input", GUILayout.Width(150), GUILayout.Height(60))) {
            InputMgr.Instance.AddKeyBoardListener("Changed 1", KeyCode.H, KeyCode_H_Keep, InputInfo.InputType.Keep, true);
            InputMgr.Instance.AddAxisListener("Changed 2", "Horizontal", Axis_Horizontal_Change, false, true);
        }
        
        if (GUILayout.Button("Delete input", GUILayout.Width(150), GUILayout.Height(60))) {
            Debug.Log("Delete pressed! Ready to delete Input info!");
            InputMgr.Instance.GetInputInfo((info) => {
                Debug.Log($"Get Input info: {(info.Source == InputInfo.InputSource.KeyBoard ? info.Key : info.MouseID)}");
                InputMgr.Instance.RemoveListener("Changed 1");
                Debug.Log("Changed 1 has been removed!");
            });}
        
        GUILayout.EndVertical();
    }

    private void KeyCode_A_Down() {
        Debug.Log("A 按下");
    }

    private void KeyCode_D_Up() {
        Debug.Log("D 抬起");
    }

    private void KeyCode_S_Keep() {
        Debug.Log("S 保持");
    }

    private void KeyCode_H_Keep() {
        Debug.Log("H 保持");
    }

    private void Mouse_0_Down() {
        Debug.Log("Mouse_0 按下");
    }

    private void Mouse_1_Up() {
        Debug.Log("Mouse_1 抬起");
    }

    private void Mouse_2_Keep() {
        Debug.Log("Mouse_2 保持");
    }

    private void Axis_Horizontal(float value) {
        Debug.Log($"Axis_Horizontal: {value}");
    }

    private void AxisRaw_Vertical(float value) {
        Debug.Log($"AxisRaw_Vertical: {value}");
    }

    private void Axis_Horizontal_Change(float value) {
        Debug.Log($"Axis_Horizontal_Change: {value}");
    }
}