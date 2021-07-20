using BehaviourTree.Scripts.Blackboard;
using BehaviourTree.Scripts.Tree;
using UnityEditor;
using UnityEngine;

namespace BehaviourTree.Scripts.Editor
{
    public static class BlackboardEditor
    {
        [MenuItem("Utility/Blackboard/Bake current tree")]
        public static void BakeSelectedTreeBehaviour()
        {
            if (!Selection.activeGameObject)
                return;
            
            var runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
            if (runner == null)
                return;
            
            BakeTree(runner);
        }

        private static void BakeTree(BehaviourTreeRunner runner)
        {
            var blackboard = runner.Blackboard;
            if (blackboard == null) return;

            var root = runner.Tree.RootNode;
            Tree.BehaviourTree.Traverse(root, node => { BlackboardUtils.ConnectTo(node, blackboard); });

            Debug.Log("BAKE SUCCESS", runner); 
        }
    }
}