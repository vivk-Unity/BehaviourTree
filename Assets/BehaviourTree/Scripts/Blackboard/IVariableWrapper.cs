using UnityEngine;

namespace BehaviourTree.Scripts.Blackboard
{
    public interface IVariableWrapper<out T> : IVariableCore where T : Component
    {
        T Value { get; }
    }
}