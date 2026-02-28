using System;
using System.Collections;
using System.Collections.Generic;
using HighlightPlus;
using Oculus.Interaction;
using TMPro;
using UnityEngine;

public class ButtonMenu : MonoBehaviour
{
    public RoundedBoxProperties box;

    public TextMeshPro textMassA;
    public TextMeshPro textMassB;
    public TextMeshPro textSpringK;

    private void Awake() {
        box         = DataSetting.GetComponentFromChild<RoundedBoxProperties>(transform, "Panel/Mesh");
        textMassA   = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "MassA/Value");
        textMassB   = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "MassB/Value");
        textSpringK = DataSetting.GetComponentFromChild<TextMeshPro>(transform, "SpringK/KValue");
    }

    private void Update() {
        textMassA.text   = $"{DataSetting.Instance.blockA.Mass:F0}";
        textMassB.text   = $"{DataSetting.Instance.blockB.Mass:F0}";
        textSpringK.text = $"{DataSetting.Instance.couple.k:F0}";
    }
}