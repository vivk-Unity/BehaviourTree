using UnityEngine;

namespace BehaviourTree.Scripts.Blackboard
{
    public interface IBlackboard
    {
        void Bind(object source);
        Component Get(string key);
    }
}