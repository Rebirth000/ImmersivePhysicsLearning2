using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Test_CsvSheet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        Debug.Log($"CsvSheet SAVE_PATH: \"{CsvSheet.SAVE_PATH}\"");
    }

    // Update is called once per frame
    void Update() { }

    private void OnGUI() {
        GUILayout.BeginVertical();
        
        if (GUILayout.Button("Save 1", GUILayout.Width(150), GUILayout.Height(60))) {
            var sheet = new CsvSheet();
            sheet[1, 2] = "123";
            sheet.Save("11/test.csv");
        }

        if (GUILayout.Button("Load 1", GUILayout.Width(150), GUILayout.Height(60))) {
            var sheet = new CsvSheet();
            sheet.Load("test.csv");
            var str = new StringBuilder();

            for (int i = 0; i < sheet.RowCount; i++) {
                for (int j = 0; j < sheet.ColCount; j++) {
                    str.Append(sheet[i, j] + ",");
                }

                str.Append('\n');
            }

            Debug.Log($"sheet: \n{str}");
        }
        
        GUILayout.EndVertical();
    }
}