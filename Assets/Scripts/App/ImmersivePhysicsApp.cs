using QFramework;

namespace ImmersivePhysics.App
{
    /// <summary>
    /// QFramework Main Architecture Entry Point for Immersive Physics Learning.
    /// This acts as the global container for all Models, Systems, and Commands.
    /// </summary>
    public class ImmersivePhysicsApp : Architecture<ImmersivePhysicsApp>
    {
        protected override void Init()
        {
            // Register Models
            this.RegisterModel(new PhysicsDataModel());
            
            // Register Systems (To be added)
            // this.RegisterSystem(new CorePhysicsSystem());
        }
    }
}
