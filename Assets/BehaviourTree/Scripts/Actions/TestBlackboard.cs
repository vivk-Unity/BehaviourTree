using BehaviourTree.Scripts.Blackboard;
using BehaviourTree.Scripts.Tree;
using UnityEngine;

namespace BehaviourTree.Scripts.Actions
{
    public class TestBlackboard : ActionNode
    {
        public Variable<Transform> _transform;
    
        protected override void OnStart() 
        {
        
        }

        protected override void OnStop() 
        {
        
        }

        protected override State OnUpdate(float deltaTime)
        {
            Debug.Log(_transform.Value);
            return State.Success;
        }

    
    }
}
