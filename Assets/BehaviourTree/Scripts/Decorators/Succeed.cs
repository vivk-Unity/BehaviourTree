using BehaviourTree.Scripts.Tree;

namespace BehaviourTree.Scripts.Decorators
{
    public class Succeed : DecoratorNode
    {
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate(float deltaTime)
        {
            var state = Child.UpdateCustom(deltaTime);
            if (state == State.Failure)
            {
                return State.Success;
            }

            return state;
        }
    }
}