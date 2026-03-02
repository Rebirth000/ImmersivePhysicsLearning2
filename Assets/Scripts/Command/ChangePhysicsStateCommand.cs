using QFramework;

namespace ImmersivePhysics.App
{
    /// <summary>
    /// 修改物理系统当前运作状态的指令
    /// 比如从 Prepared 切换到 Running，或者从 Running 切回 Prepared。
    /// 一旦状态改变，绑定的物理系统或者 UI 都可以收到通知做出响应。
    /// </summary>
    public class ChangePhysicsStateCommand : AbstractCommand
    {
        private readonly StateType _targetState;

        public ChangePhysicsStateCommand(StateType targetState)
        {
            _targetState = targetState;
        }

        protected override void OnExecute()
        {
            var model = this.GetModel<PhysicsDataModel>();
            
            // 只有当状态发生真正改变时才设置
            if (model.StatusType.Value != _targetState)
            {
                model.StatusType.Value = _targetState;
                
                // 如果是切回 Prepared，我们同时把移动时间重置为 0
                if (_targetState == StateType.Prepared)
                {
                    model.MoveTime.Value = 0f;
                    model.MaxSpeed.Value = 0f;
                }
            }
        }
    }
}
