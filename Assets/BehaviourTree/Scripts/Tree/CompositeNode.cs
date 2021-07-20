using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree.Scripts.Tree
{
    public abstract class CompositeNode : Node
    {
        [HideInInspector] public List<Node> Children = new List<Node>();

        public override Node Clone()
        {
            var node = Instantiate(this);
            node.Children = Children.ConvertAll(c => c.Clone());
            return node;
        }
    }
}