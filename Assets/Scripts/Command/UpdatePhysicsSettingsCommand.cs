using QFramework;

namespace ImmersivePhysics.App
{
    /// <summary>
    /// 更新初始物理设定（比如最大速度和目标控制块）
    /// </summary>
    public class UpdatePhysicsSettingsCommand : AbstractCommand
    {
        private readonly float _maxSpeed;
        private readonly BlockSpringCouple.EBlock _targetBlock;

        public UpdatePhysicsSettingsCommand(float maxSpeed, BlockSpringCouple.EBlock targetBlock)
        {
            _maxSpeed = maxSpeed;
            _targetBlock = targetBlock;
        }

        protected override void OnExecute()
        {
            var model = this.GetModel<PhysicsDataModel>();
            model.MaxSpeed.Value = _maxSpeed;
            model.TargetBlock.Value = _targetBlock;
        }
    }
}
