using System;
using UnityEngine;

namespace BehaviourTree.Scripts.Blackboard
{
    public interface IVariableCore
    {
        void Setup(IBlackboard blackboard);
        void Connect();
        
        string Key { get; }
        Component GetComponent();
        
        Action OnValidation { get; set; }
    }
}