using System;
using BehaviourTree.Scripts.Blackboard;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace BehaviourTree.Scripts.Editor
{
    public class VariableView : VisualElement
    {
        private IVariableCore _variable;
        private Object _source;
        private string _propertyName;
        
        private Component _lastValue;
        private UnityEditor.Editor _editor;

        public VariableView(IVariableCore variable, Object source, string propertyName)
        {
            _variable = variable;
            _source = source;
            _propertyName = propertyName;
            _variable.OnValidation = UpdateView;
            _lastValue = null;
            UpdateView();
        }

        private void UpdateView()
        {
            if (_variable == null) 
                return;
            
            var value = _variable.GetComponent();
            try
            {
                var nullRefException = value.name;
            }
            catch (NullReferenceException)
            {
                Clear();
                _lastValue = null;
                return;
            }

            if (value == _lastValue) 
                return;
            
            _lastValue = value;
            
            Clear();
            
            Label label = new Label {text = $"\t[{_variable.Key}] {value.GetType().Name}"};
            Add(label);
            
            Object.DestroyImmediate(_editor);
            _editor = UnityEditor.Editor.CreateEditor(value);
            IMGUIContainer imguiContainer = new IMGUIContainer(() => { _editor.OnInspectorGUI(); });
            Add(imguiContainer);
        }
    }
}