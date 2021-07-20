using BehaviourTree.Scripts.Tree;
using UnityEngine;

namespace BehaviourTree.Scripts.Actions
{
    public class Breakpoint : ActionNode
    {
        protected override void OnStart()
        {
            Debug.Log("Breakpoint");
            Debug.Break();
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate(float deltaTime)
        {
            return State.Success;
        }
    }
}
