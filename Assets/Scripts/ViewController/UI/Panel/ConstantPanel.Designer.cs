// Generate Id:237d5c48-fcc8-4e5b-9e8e-de2a7c0b6cfe
using UnityEngine;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace ImmersivePhysics.ViewController
{
	public partial class ConstantPanel : QFramework.IController
	{
		public TMPro.TextMeshPro textMassA;
		
		public TMPro.TextMeshPro textMassB;
		
		public TMPro.TextMeshPro textSpringK;
		
		public TMPro.TextMeshPro textLStart;
		
		public TMPro.TextMeshPro textL;
		
		public TMPro.TextMeshPro textSpeedStart;
		
		QFramework.IArchitecture QFramework.IBelongToArchitecture.GetArchitecture()=>ImmersivePhysics.App.ImmersivePhysicsApp.Interface;
	}
}
