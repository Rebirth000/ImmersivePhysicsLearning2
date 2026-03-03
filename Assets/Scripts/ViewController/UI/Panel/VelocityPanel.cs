using ImmersivePhysics.App;
using QFramework;
using UnityEngine;

namespace ImmersivePhysics.ViewController
{
    /// <summary>
    /// 速度面板 — 显示 ω、V共、Va、Vb
    /// 绑定变量 TextW, TextVt, TextVa, TextVb 由 VelocityPanel.Designer.cs 自动生成
    /// </summary>
    public partial class VelocityPanel : QFramework.ViewController, IController
    {
        public IArchitecture GetArchitecture()
        {
            return ImmersivePhysicsApp.Interface;
        }

        // Update is called once per frame
        void Update()
        {
            TextW.text = MathUtil.FormatFloat(DataSetting.Instance.couple.w);
            TextVt.text = MathUtil.FormatFloat(DataSetting.Instance.blockA.B);
            TextVa.text = MathUtil.FormatFloat(DataSetting.Instance.blockA.MoveVelocity);
            TextVb.text = MathUtil.FormatFloat(DataSetting.Instance.blockB.MoveVelocity);
        }
    }
}