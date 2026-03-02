using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KineticEnergyPanel : MonoBehaviour
{
    public TextMeshPro textDeltaL;
    public TextMeshPro textEa;
    public TextMeshPro textEb;
    public TextMeshPro textEp;
    public TextMeshPro textESum;

    public void Awake() {
        textDeltaL = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "DeltaLength/Expression2/Value");
        textEa     = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "KineticEnergyA/Expression2/Value");
        textEb     = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "KineticEnergyB/Expression2/Value");
        textEp     = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "SpringEnergy/Expression2/Value");
        textESum   = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "EnergySum/Expression2/Value");
    }

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() {
        var blockA = DataSetting.Instance.blockA;
        var blockB = DataSetting.Instance.blockB;
        var couple = DataSetting.Instance.couple;
        var spring = DataSetting.Instance.springMove;

        var l0    = spring.StartLength;
        var delta = (blockA.MovePos - blockB.MovePos) / couple.moveRatio;
        var l     = l0 + delta;
        var k     = couple.k;
        var ea    = blockA.MoveKineticEnergy;
        var eb    = blockB.MoveKineticEnergy;
        var ep    = k * delta * delta / 2;

        textDeltaL.text = MathUtil.FormatFloat(delta);
        textEa.text     = MathUtil.FormatFloat(ea);
        textEb.text     = MathUtil.FormatFloat(eb);
        textEp.text     = MathUtil.FormatFloat(ep);
        textESum.text   = MathUtil.FormatFloat(ea + eb + ep);
    }
}