using UnityEngine;

namespace BehaviourTree.Scripts.Tree
{
    public class RootNode : Node
    { 
        [HideInInspector] public Node Child;

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate(float deltaTime)
        {
            return Child.UpdateCustom(deltaTime);
        }

        public override Node Clone()
        {
            var node = Instantiate(this);
            node.Child = Child.Clone();
            return node;
        }
    }
}