using BehaviourTree.Scripts.Tree;

namespace BehaviourTree.Scripts.Actions
{
    public class Wait : ActionNode
    {
        public float Duration = 1.0f;
        
        private float _waitTime = 0;

        protected override void OnStart()
        {
            _waitTime = 0;
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate(float deltaTime)
        {
            if (_waitTime > Duration)
            {
                return State.Success;
            }
            
            _waitTime += deltaTime;
            return State.Running;
        }
    }
}
