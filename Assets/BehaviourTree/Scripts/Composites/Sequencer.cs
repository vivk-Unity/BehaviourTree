using System;
using BehaviourTree.Scripts.Tree;

namespace BehaviourTree.Scripts.Composites
{
    public class Sequencer : CompositeNode
    {
        private int _currentIndex;

        protected override void OnStart()
        {
            _currentIndex = 0;
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate(float deltaTime)
        {
            for (var i = _currentIndex; i < Children.Count; i++)
            {
                _currentIndex = i;
                var child = Children[_currentIndex];

                switch (child.UpdateCustom(deltaTime))
                {
                    case State.Running:
                        return State.Running;
                    case State.Failure:
                        return State.Failure;
                    case State.Success:
                        continue;
                    default:
                        break;
                }
            }

            return State.Success;
        }
    }
}