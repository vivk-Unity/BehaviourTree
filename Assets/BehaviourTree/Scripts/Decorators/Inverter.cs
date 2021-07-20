using BehaviourTree.Scripts.Tree;

namespace BehaviourTree.Scripts.Decorators
{
    public class Inverter : DecoratorNode
    {
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate(float deltaTime)
        {
            switch (Child.UpdateCustom(deltaTime))
            {
                case State.Running:
                    return State.Running;
                case State.Failure:
                    return State.Success;
                case State.Success:
                    return State.Failure;
                default:
                    break;
            }

            return State.Failure;
        }
    }
}