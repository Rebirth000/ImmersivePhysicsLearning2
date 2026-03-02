using ImmersivePhysics.App;
using QFramework;

namespace ImmersivePhysics.App
{
    /// <summary>
    /// 全局物理实验状态机的数据模型。
    /// 存储所有的物理参数（如速度、时间）和系统当前状态。
    /// 可以使用 BindableProperty 支持 UI 的响应式更新。
    /// </summary>
    public class PhysicsDataModel : AbstractModel
    {
        // ================= 数据定义 =================
        
        /// <summary>
        /// 用户的物理交互方式（虚拟手抓取 / 实体物体）
        /// </summary>
        public BindableProperty<InteractionType> InteractionType { get; } = new BindableProperty<InteractionType>(global::InteractionType.VirtualHands);

        /// <summary>
        /// 全局实验过程的当前状态 (Prepared, Running, 等等)
        /// </summary>
        public BindableProperty<StateType> StatusType { get; } = new BindableProperty<StateType>(StateType.Prepared);

        /// <summary>
        /// 给物体施加的最大初速度
        /// </summary>
        public BindableProperty<float> MaxSpeed { get; } = new BindableProperty<float>(1f);

        /// <summary>
        /// 当前施加初速度的物块 (A, B)
        /// </summary>
        public BindableProperty<BlockSpringCouple.EBlock> TargetBlock { get; } = new BindableProperty<BlockSpringCouple.EBlock>(BlockSpringCouple.EBlock.A);

        /// <summary>
        /// 物理引擎已经运行演进的运动时间
        /// </summary>
        public BindableProperty<float> MoveTime { get; } = new BindableProperty<float>(0f);


        // ================= 初始化 =================
        protected override void OnInit()
        {
            // 这里可以处理依赖或初始状态，目前保持默认值。
            // 例如：从 PlayerPrefs 加载上次设置的最大速度。
        }
    }
}
