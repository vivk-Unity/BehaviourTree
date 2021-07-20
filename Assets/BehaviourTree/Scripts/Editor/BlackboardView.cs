using System.Collections.Generic;
using System.Linq;
using BehaviourTree.Scripts.Blackboard;
using BehaviourTree.Scripts.Tree;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree.Scripts.Editor
{
    public class BlackboardView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<BlackboardView, VisualElement.UxmlTraits> { }

        private List<VariableView> _container = new List<VariableView>();

        private ScrollView _scrollView;
        
        public BlackboardView()
        {
            var styleSheet = BehaviourTreeSettings.GetOrCreateSettings().BlackboardStyle;
            styleSheets.Add(styleSheet);

            _scrollView = new ScrollView();
            Add(_scrollView);
        }

        public void UpdateSelection(Node node)
        {
            _scrollView.Clear();
            _container.Clear();
            
            var nodeType = node.GetType();
            var fieldInfos = nodeType.GetFields();
            var variableType = typeof(IVariableCore);
            foreach (var fInfo in fieldInfos)
            {
                var fType = fInfo.FieldType;
                var fInterfaces = fType.GetInterfaces();
                var isVariable = fInterfaces.Contains(variableType);
                if (isVariable)
                {
                    if (fInfo.GetValue(node) is IVariableCore variable)
                    {
                        
                        var variableView = new VariableView(variable: variable,
                            source: node,
                            propertyName: fInfo.Name);
                        _container.Add(variableView);
                        
                        _scrollView.Add(variableView);
                    }
                }
            }
        }
    }
}