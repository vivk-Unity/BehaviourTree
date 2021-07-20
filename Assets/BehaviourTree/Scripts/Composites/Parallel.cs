using System.Collections.Generic;
using System.Linq;
using BehaviourTree.Scripts.Tree;

namespace BehaviourTree.Scripts.Composites
{
    public class Parallel : CompositeNode
    {
        private readonly List<State> _childrenExecute = new List<State>();

        protected override void OnStart()
        {
            _childrenExecute.Clear();
            for (var i = 0; i < Children.Count; i++)
            {
                _childrenExecute.Add(State.Running);
            }
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate(float deltaTime)
        {
            var stillRunning = false;
            for (var i = 0; i < _childrenExecute.Count; ++i)
            {
                if (_childrenExecute[i] == State.Running)
                {
                    var status = Children[i].UpdateCustom(deltaTime);
                    if (status == State.Failure)
                    {
                        AbortRunningChildren();
                        return State.Failure;
                    }

                    if (status == State.Running)
                    {
                        stillRunning = true;
                    }

                    _childrenExecute[i] = status;
                }
            }

            return stillRunning 
                ? State.Running 
                : State.Success;
        }

        private void AbortRunningChildren()
        {
            for (var i = 0; i < _childrenExecute.Count; ++i)
            {
                if (_childrenExecute[i] == State.Running)
                {
                    Children[i].Abort();
                }
            }
        }
    }
}