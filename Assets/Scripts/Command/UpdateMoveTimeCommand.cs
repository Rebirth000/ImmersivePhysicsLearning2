using QFramework;

namespace ImmersivePhysics.App
{
    /// <summary>
    /// 更新物理引擎已经运行时间的指令。
    /// 相当于模拟时间的推进或者倒退（拖动滑条）。
    /// </summary>
    public class UpdateMoveTimeCommand : AbstractCommand
    {
        private readonly float _newTime;

        public UpdateMoveTimeCommand(float newTime)
        {
            _newTime = newTime;
        }

        protected override void OnExecute()
        {
            var model = this.GetModel<PhysicsDataModel>();
            
            // 确保时间不为负数
            float validTime = _newTime < 0 ? 0 : _newTime;
            
            // 数据层只负责修改数据，具体的画面更新(Graph 和 Couple) 将在 System 或 ViewController 里响应此数据变化
            model.MoveTime.Value = validTime;
        }
    }
}
