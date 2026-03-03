using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using QFramework;
using ImmersivePhysics.App;

namespace ImmersivePhysics.ViewController
{
    public partial class ConstantPanel : QFramework.ViewController, IController
    {
        public IArchitecture GetArchitecture()
        {
            return ImmersivePhysicsApp.Interface;
        }

        // Update is called once per frame
        void Update()
        {
            var blockA = DataSetting.Instance.blockA;
            var blockB = DataSetting.Instance.blockB;
            var couple = DataSetting.Instance.couple;
            var spring = DataSetting.Instance.springMove;

            var l0 = spring.StartLength;
            var delta = (blockA.MovePos - blockB.MovePos) / couple.moveRatio;
            var l = l0 + delta;

            textMassA.text = MathUtil.FormatFloat(blockA.Mass);
            textMassB.text = MathUtil.FormatFloat(blockB.Mass);
            textSpringK.text = MathUtil.FormatFloat(couple.k);
            textLStart.text = MathUtil.FormatFloat(l0);
            textL.text = MathUtil.FormatFloat(l);
        }

        public void SetStartSpeed(float speed)
        {
            textSpeedStart.text = MathUtil.FormatFloat(speed);
        }
    }
}