using BehaviourTree.Scripts.Tree;

namespace BehaviourTree.Scripts.Decorators
{
    public class Failure : DecoratorNode
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
            return state == State.Success 
                ? State.Failure 
                : state;
        }
    }
}