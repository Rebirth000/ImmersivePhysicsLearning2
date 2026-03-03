using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using QFramework;
using ImmersivePhysics.App;

namespace ImmersivePhysics.ViewController
{
    public partial class MomentumPanel : QFramework.ViewController, IController
    {
        public IArchitecture GetArchitecture()
        {
            return ImmersivePhysicsApp.Interface;
        }

        // Update is called once per frame
        void Update()
        {
            var momentumA = DataSetting.Instance.blockA.MoveMomentum;
            var momentumB = DataSetting.Instance.blockB.MoveMomentum;
            textPa.text = MathUtil.FormatFloat(momentumA);
            textPb.text = MathUtil.FormatFloat(momentumB);
            textPSum.text = MathUtil.FormatFloat(momentumA + momentumB);
        }

        public void SetStartMomentum(float speed)
        {
            textPStart.text = MathUtil.FormatFloat(DataSetting.Instance.blockB.Mass * speed);
        }
    }
}