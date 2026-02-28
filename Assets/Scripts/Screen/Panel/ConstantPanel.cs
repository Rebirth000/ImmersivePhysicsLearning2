using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConstantPanel : MonoBehaviour
{
    public TextMeshPro textMassA;
    public TextMeshPro textMassB;
    public TextMeshPro textSpringK;
    public TextMeshPro textSpeedStart;
    public TextMeshPro textLStart;
    public TextMeshPro textL;


    public void Awake() {
        textMassA      = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "MassA/Expression/Value");
        textMassB      = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "MassB/Expression/Value");
        textSpringK    = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "SpringK/Expression/Value");
        textSpeedStart = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "SpeedStart/Expression/Value");
        textLStart     = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "SpringLengthStart/Expression/Value");
        textL          = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "SpringLength/Expression/Value");
    }

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() {
        var blockA = DataSetting.Instance.blockA;
        var blockB = DataSetting.Instance.blockB;
        var couple = DataSetting.Instance.couple;
        var spring = DataSetting.Instance.springMove;
        
        var l0     = spring.StartLength;
        var delta  = (blockA.MovePos - blockB.MovePos) / couple.moveRatio;
        var l      = l0 + delta;
        
        textMassA.text   = MathUtil.FormatFloat(blockA.Mass);
        textMassB.text   = MathUtil.FormatFloat(blockB.Mass);
        textSpringK.text = MathUtil.FormatFloat(couple.k);
        textLStart.text = MathUtil.FormatFloat(l0);
        textL.text       = MathUtil.FormatFloat(l);
    }

    public void SetStartSpeed(float speed) {
        textSpeedStart.text = MathUtil.FormatFloat(speed);
    }
}