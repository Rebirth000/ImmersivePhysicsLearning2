using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using QFramework;
using ImmersivePhysics.App;

namespace ImmersivePhysics.ViewController
{
    public partial class KineticEnergyPanel : QFramework.ViewController, IController
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
            var k = couple.k;
            var ea = blockA.MoveKineticEnergy;
            var eb = blockB.MoveKineticEnergy;
            var ep = k * delta * delta / 2;

            textDeltaL.text = MathUtil.FormatFloat(delta);
            textEa.text = MathUtil.FormatFloat(ea);
            textEb.text = MathUtil.FormatFloat(eb);
            textEp.text = MathUtil.FormatFloat(ep);
            textESum.text = MathUtil.FormatFloat(ea + eb + ep);
        }
    }
}