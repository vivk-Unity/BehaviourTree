using System.Collections.Generic;
using BehaviourTree.Scripts.Blackboard;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

#endif

namespace BehaviourTree.Scripts.Tree
{
    [CreateAssetMenu()]
    public class BehaviourTree : ScriptableObject
    {
        public Node RootNode;
        public Node.State TreeState = Node.State.Running;
        public List<Node> Nodes = new List<Node>();
        public IBlackboard Blackboard;

        public Node.State UpdateCustom(float deltaTime)
        {
            if (RootNode.CurrentState == Node.State.Running)
            {
                TreeState = RootNode.UpdateCustom(deltaTime);
            }

            return TreeState;
        }

        public static List<Node> GetChildren(Node parent)
        {
            var children = new List<Node>();

            if (parent is DecoratorNode decorator && decorator.Child != null)
            {
                children.Add(decorator.Child);
            }

            if (parent is RootNode rootNode && rootNode.Child != null)
            {
                children.Add(rootNode.Child);
            }

            if (parent is CompositeNode composite)
            {
                return composite.Children;
            }

            return children;
        }

        public static void Traverse(Node node, System.Action<Node> callback)
        {
            if (node)
            {
                callback.Invoke(node);
                var children = GetChildren(node);
                foreach (var n in children)
                {
                    Traverse(n, callback);
                }
            }
        }

        public BehaviourTree Clone()
        {
            var tree = Instantiate(this);
            tree.RootNode = tree.RootNode.Clone();
            tree.Nodes = new List<Node>();
            Traverse(tree.RootNode, (n) => { tree.Nodes.Add(n); });

            return tree;
        }

        public void Bind(IBlackboard blackboard)
        {
            Traverse(RootNode, node =>
            {
                BlackboardUtils.ConnectTo(node, blackboard);
            });
        }

        #region Editor Compatibility

        #if UNITY_EDITOR

        public Node CreateNode(System.Type type)
        {
            var node = ScriptableObject.CreateInstance(type) as Node;
            node.name = type.Name;
            node.GUID = GUID.Generate().ToString();

            Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
            Nodes.Add(node);

            if (Application.isPlaying == false)
            {
                AssetDatabase.AddObjectToAsset(node, this);
            }

            Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");

            AssetDatabase.SaveAssets();
            return node;
        }

        public void DeleteNode(Node node)
        {
            Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");
            Nodes.Remove(node);

            Undo.DestroyObjectImmediate(node);

            AssetDatabase.SaveAssets();
        }

        public static void AddChild(Node parent, Node child)
        {
            if (parent is DecoratorNode decorator)
            {
                Undo.RecordObject(decorator, "Behaviour Tree (AddChild)");
                decorator.Child = child;
                EditorUtility.SetDirty(decorator);
            }

            if (parent is RootNode rootNode)
            {
                Undo.RecordObject(rootNode, "Behaviour Tree (AddChild)");
                rootNode.Child = child;
                EditorUtility.SetDirty(rootNode);
            }

            if (parent is CompositeNode composite)
            {
                Undo.RecordObject(composite, "Behaviour Tree (AddChild)");
                composite.Children.Add(child);
                EditorUtility.SetDirty(composite);
            }
        }

        public static void RemoveChild(Node parent, Node child)
        {
            if (parent is DecoratorNode decorator)
            {
                Undo.RecordObject(decorator, "Behaviour Tree (RemoveChild)");
                decorator.Child = null;
                EditorUtility.SetDirty(decorator);
            }

            if (parent is RootNode rootNode)
            {
                Undo.RecordObject(rootNode, "Behaviour Tree (RemoveChild)");
                rootNode.Child = null;
                EditorUtility.SetDirty(rootNode);
            }

            if (parent is CompositeNode composite)
            {
                Undo.RecordObject(composite, "Behaviour Tree (RemoveChild)");
                composite.Children.Remove(child);
                EditorUtility.SetDirty(composite);
            }
        }
        #endif

        #endregion Editor Compatibility
    }
}