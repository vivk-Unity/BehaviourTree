using System;
using UnityEngine;
using Component = UnityEngine.Component;

namespace BehaviourTree.Scripts.Blackboard
{
    [Serializable]
    public class Variable<T> : IVariableWrapper<T>, ISerializationCallbackReceiver where T : Component
    {
        [SerializeField] private string _key = "Key";
        [SerializeField, HideInInspector] private T _value = null;
        
        private string _lastKey = string.Empty;

        #region IVariableCore implementation

        [SerializeField, HideInInspector] private IBlackboard _blackboard;

        public void Setup(IBlackboard blackboard)
        {
            _blackboard = blackboard;
        }

        public void Connect()
        {
            if (_blackboard == null)
                return;

            _value = _blackboard.Get(_key) as T;

            OnValidation?.Invoke();
        }

        public string Key => _key;

        public Component GetComponent()
        {
            return _value;
        }

        public Action OnValidation { get; set; } = null;

        public void SetForce(T value)
        {
            _value = value;
        }

        #endregion

        #region IVariableWrapper implementation

        public T Value => _value;

        #endregion
        
        private void OnValidate()
        {
            if (_lastKey.Equals(_key) == true)
                return;
            
            if (_key.Equals(string.Empty) == true)
                return;
            
            _lastKey = _key;
            
            Connect();
        }

        #region ISerializationCallbackReceiver implementation

        public void OnBeforeSerialize()
        {
            OnValidate();
        }

        public void OnAfterDeserialize()
        {
            
        }

        #endregion

    }
}
