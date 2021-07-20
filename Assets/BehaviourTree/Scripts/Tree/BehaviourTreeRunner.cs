using BehaviourTree.Scripts.Blackboard;
using UnityEngine;

namespace BehaviourTree.Scripts.Tree
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        public BehaviourTree Tree;
        [SerializeField] private MonoBehaviour _blackboard;

        public IBlackboard Blackboard
        {
            get
            {
                if (_blackboard == null)
                    return null;
                return _blackboard as IBlackboard;
            }
        }


        public void Bind()
        {
            Tree.Bind(Blackboard);
        }

        private void Start()
        {
            Tree = Tree.Clone();
        }

        private void Update()
        {
            if (Tree)
            {
                Tree.UpdateCustom(Time.deltaTime);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!Tree)
            {
                return;
            }

            BehaviourTree.Traverse(Tree.RootNode, (n) =>
            {
                // TODO: How to ... ? n.OnDrawGizmos(); 
            });
        }
    }
}