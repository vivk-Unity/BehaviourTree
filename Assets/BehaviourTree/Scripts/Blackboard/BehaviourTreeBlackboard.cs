using System;
using UnityEngine;

namespace BehaviourTree.Scripts.Blackboard
{
    public class BehaviourTreeBlackboard : MonoBehaviour, IBlackboard
    {
        [SerializeField] private VariableContainer[] _variables;
        
        [Serializable]
        public class VariableContainer
        {
            public string Key = "Value";
            public Component Value = null;
        }

        public void Bind(object source)
        {
            BlackboardUtils.ConnectTo(source, this);
        }

        public Component Get(string key)
        {
            foreach (var variableContainer in _variables)
            {
                if (variableContainer.Key.Equals(key) == true)
                {
                    return variableContainer.Value;
                }
            }

            return null;
        }
    }
}
