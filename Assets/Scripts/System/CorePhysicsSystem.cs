using QFramework;

namespace ImmersivePhysics.App
{
    /// <summary>
    /// 全局物理系统核心逻辑管理器。
    /// 负责在特定生命周期或指令触发时，协调 Model 与一些无法抽取为 ViewController 的第三方系统，
    /// 或者用来维护游戏核心的状态机机制。
    /// </summary>
    public class CorePhysicsSystem : AbstractSystem
    {
        protected override void OnInit()
        {
            var model = this.GetModel<PhysicsDataModel>();

            // 在此注册任何全局级别的事件监听。
            // 比如，当应用进入 Prepared 状态时，执行清理逻辑。
            model.StatusType.Register(OnStatusChanged);
        }

        private void OnStatusChanged(StateType state)
        {
            // 例如：状态切换为 Finished 时在此判定分数或打点上报等脱离具体物体的纯逻辑。
        }
    }
}
