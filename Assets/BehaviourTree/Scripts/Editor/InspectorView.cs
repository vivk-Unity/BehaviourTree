using UnityEngine.UIElements;

namespace BehaviourTree.Scripts.Editor
{
    public class InspectorView : ScrollView
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, ScrollView.UxmlTraits>
        {
        }

        private UnityEditor.Editor _editor;

        public InspectorView()
        {
        }

        internal void UpdateSelection(NodeView nodeView)
        {
            Clear();

            UnityEngine.Object.DestroyImmediate(_editor);

            _editor = UnityEditor.Editor.CreateEditor(nodeView.Node);
            var imguiContainer = new IMGUIContainer(() =>
            {
                if (_editor && _editor.target)
                {
                    _editor.OnInspectorGUI();
                }
            });
            Add(imguiContainer);
        }
    }
}