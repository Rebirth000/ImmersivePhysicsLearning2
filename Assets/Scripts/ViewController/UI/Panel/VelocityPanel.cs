using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VelocityPanel : MonoBehaviour
{
    public TextMeshPro textW;
    public TextMeshPro textVt;
    public TextMeshPro textVa;
    public TextMeshPro textVb;

    public void Awake() {
        textW  = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "w/Expression2/Value");
        textVt = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "VelocityTogether/Expression2/Value");
        textVa = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "VelocityA/Expression3/Value");
        textVb = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "VelocityB/Expression3/Value");
    }

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() {
        textW.text  = MathUtil.FormatFloat(DataSetting.Instance.couple.w);
        
        textVt.text = MathUtil.FormatFloat(DataSetting.Instance.blockA.B);
        textVa.text = MathUtil.FormatFloat(DataSetting.Instance.blockA.MoveVelocity);
        textVb.text = MathUtil.FormatFloat(DataSetting.Instance.blockB.MoveVelocity);
    }
}