using UnityEngine;

namespace BehaviourTree.Scripts.Tree
{
    public abstract class DecoratorNode : Node
    {
        [HideInInspector] public Node Child;

        public override Node Clone()
        {
            var node = Instantiate(this);
            node.Child = Child.Clone();
            return node;
        }
    }
}
