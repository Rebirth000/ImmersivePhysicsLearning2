using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MomentumPanel : MonoBehaviour
{
    public TextMeshPro textPStart;
    public TextMeshPro textPa;
    public TextMeshPro textPb;
    public TextMeshPro textPSum;

    public void Awake() {
        textPStart = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "MomentumStart/Expression2/Value");
        textPa     = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "MomentumA/Expression3/Value");
        textPb     = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "MomentumB/Expression3/Value");
        textPSum   = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "MomentumSum/Expression2/Value");
    }

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() {
        var momentumA = DataSetting.Instance.blockA.MoveMomentum;
        var momentumB = DataSetting.Instance.blockB.MoveMomentum;
        textPa.text   = MathUtil.FormatFloat(momentumA);
        textPb.text   = MathUtil.FormatFloat(momentumB);
        textPSum.text = MathUtil.FormatFloat(momentumA + momentumB);
    }

    public void SetStartMomentum(float speed) {
        textPStart.text = MathUtil.FormatFloat(DataSetting.Instance.blockB.Mass * speed);
    }
}