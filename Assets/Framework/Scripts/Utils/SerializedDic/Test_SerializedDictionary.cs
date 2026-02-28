using System;
using System.Collections;
using System.Collections.Generic;
using SerializedDic;
using UnityEngine;
using UnityEngine.Serialization;

public class Test_SerializedDictionary : MonoBehaviour
{
    public SerializedDictionary<int, Vector3>      dic_int_vector3 = new SerializedDictionary<int, Vector3>();
    public SerializedDictionary<string, List<int>> dic_string_list = new SerializedDictionary<string, List<int>>();

    [SerializedDictionary("ID", "Person")]
    public SerializedDictionary<string, Person> dic_string_person = new SerializedDictionary<string, Person>();

    // Start is called before the first frame update
    void Start() {
        dic_int_vector3.Add(1, Vector3.back);
        dic_int_vector3.Add(3, Vector3.right);
    }

    // Update is called once per frame
    void Update() { }

    private void OnGUI() {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Print dic_int_vector3 Count", GUILayout.Width(150), GUILayout.Height(60))) {
            Debug.Log(dic_int_vector3.Count);
        }

        if (GUILayout.Button("Print dic_int_vector3 0", GUILayout.Width(150), GUILayout.Height(60))) {
            Debug.Log(dic_int_vector3[0]);
        }

        GUILayout.EndHorizontal();
    }

    [Serializable]
    public struct Person
    {
        public int    id;
        public string name;
        public bool   sex;
    }
}