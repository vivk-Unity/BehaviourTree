using System;
using BehaviourTree.Scripts.Tree;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Node = BehaviourTree.Scripts.Tree.Node;

namespace BehaviourTree.Scripts.Editor
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Action<NodeView> OnNodeSelected;
        public Node Node;
        public Port Input;
        public Port Output;

        public NodeView(Node node) : base(
            AssetDatabase.GetAssetPath(BehaviourTreeSettings.GetOrCreateSettings().NodeXml))
        {
            Node = node;
            Node.name = node.GetType().Name;
            title = node.name.Replace("(Clone)", "").Replace("Node", "");
            viewDataKey = node.GUID;

            style.left = node.Position.x;
            style.top = node.Position.y;

            CreateInputPorts();
            CreateOutputPorts();
            SetupClasses();
            SetupDataBinding();
        }

        private void SetupDataBinding()
        {
            Label descriptionLabel = this.Q<Label>("description");
            descriptionLabel.bindingPath = "Description";
            descriptionLabel.Bind(new SerializedObject(Node));
        }

        private void SetupClasses()
        {
            if (Node is ActionNode)
            {
                AddToClassList("action");
            }
            else if (Node is CompositeNode)
            {
                AddToClassList("composite");
            }
            else if (Node is DecoratorNode)
            {
                AddToClassList("decorator");
            }
            else if (Node is RootNode)
            {
                AddToClassList("root");
            }
        }

        private void CreateInputPorts()
        {
            if (Node is ActionNode)
            {
                Input = new NodePort(Direction.Input, Port.Capacity.Single);
            }
            else if (Node is CompositeNode)
            {
                Input = new NodePort(Direction.Input, Port.Capacity.Single);
            }
            else if (Node is DecoratorNode)
            {
                Input = new NodePort(Direction.Input, Port.Capacity.Single);
            }
            else if (Node is RootNode)
            {
            }

            if (Input != null)
            {
                Input.portName = string.Empty;
                Input.style.flexDirection = FlexDirection.Column;
                inputContainer.Add(Input);
            }
        }

        private void CreateOutputPorts()
        {
            if (Node is ActionNode)
            {
            }
            else if (Node is CompositeNode)
            {
                Output = new NodePort(Direction.Output, Port.Capacity.Multi);
            }
            else if (Node is DecoratorNode)
            {
                Output = new NodePort(Direction.Output, Port.Capacity.Single);
            }
            else if (Node is RootNode)
            {
                Output = new NodePort(Direction.Output, Port.Capacity.Single);
            }

            if (Output != null)
            {
                Output.portName = string.Empty;
                Output.style.flexDirection = FlexDirection.ColumnReverse;
                outputContainer.Add(Output);
            }
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(Node, "Behaviour Tree (Set Position");
            Node.Position.x = newPos.xMin;
            Node.Position.y = newPos.yMin;
            EditorUtility.SetDirty(Node);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            if (OnNodeSelected != null)
            {
                OnNodeSelected.Invoke(this);
            }
        }

        public void SortChildren()
        {
            if (Node is CompositeNode composite)
            {
                composite.Children.Sort(SortByHorizontalPosition);
            }
        }

        private static int SortByHorizontalPosition(Node left, Node right)
        {
            return left.Position.x < right.Position.x ? -1 : 1;
        }

        public void UpdateState()
        {
            RemoveFromClassList("running");
            RemoveFromClassList("failure");
            RemoveFromClassList("success");

            if (Application.isPlaying)
            {
                switch (Node.CurrentState)
                {
                    case Node.State.Running:
                        if (Node.Started)
                        {
                            AddToClassList("running");
                        }
                        break;
                    case Node.State.Failure:
                        AddToClassList("failure");
                        break;
                    case Node.State.Success:
                        AddToClassList("success");
                        break;
                    default:
                        break;
                }
            }
        }
    }
}