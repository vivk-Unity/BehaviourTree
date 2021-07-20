using System;
using System.Collections.Generic;
using System.Linq;
using BehaviourTree.Scripts.Tree;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Node = BehaviourTree.Scripts.Tree.Node;

namespace BehaviourTree.Scripts.Editor
{
    public class BehaviourTreeView : GraphView
    {
        public Action<NodeView> OnNodeSelected;

        public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits>
        {
        }

        private Tree.BehaviourTree _tree;
        private readonly BehaviourTreeSettings _settings;

        public struct ScriptTemplate
        {
            public TextAsset templateFile;
            public string defaultFileName;
            public string subFolder;
        }

        public ScriptTemplate[] ScriptFileAssets =
        {
            new ScriptTemplate
            {
                templateFile = BehaviourTreeSettings.GetOrCreateSettings().ScriptTemplateActionNode,
                defaultFileName = "NewActionNode.cs",
                subFolder = "Actions"
            },
            new ScriptTemplate
            {
                templateFile = BehaviourTreeSettings.GetOrCreateSettings().ScriptTemplateCompositeNode,
                defaultFileName = "NewCompositeNode.cs",
                subFolder = "Composites"
            },
            new ScriptTemplate
            {
                templateFile = BehaviourTreeSettings.GetOrCreateSettings().ScriptTemplateDecoratorNode,
                defaultFileName = "NewDecoratorNode.cs",
                subFolder = "Decorators"
            },
        };

        public BehaviourTreeView()
        {
            _settings = BehaviourTreeSettings.GetOrCreateSettings();

            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new DoubleClickSelection());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = _settings.BehaviourTreeStyle;
            styleSheets.Add(styleSheet);

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnUndoRedo()
        {
            PopulateView(_tree);
            AssetDatabase.SaveAssets();
        }

        public NodeView FindNodeView(Node node)
        {
            return GetNodeByGuid(node.GUID) as NodeView;
        }

        internal void PopulateView(Tree.BehaviourTree tree)
        {
            _tree = tree;
            
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;

            if (tree.RootNode == null)
            {
                tree.RootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
                EditorUtility.SetDirty(tree);
                AssetDatabase.SaveAssets();
            }

            foreach (var node in tree.Nodes)
            {
                CreateNodeView(node);
            }

            foreach (var node in tree.Nodes)
            {
                var children = Tree.BehaviourTree.GetChildren(node);
                foreach (var child in children)
                {
                    var parentView = FindNodeView(node);
                    var childView = FindNodeView(child);

                    var edge = parentView.Output.ConnectTo(childView.Input);
                    AddElement(edge);
                }
            }
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var list = new List<Port>();
            foreach (var endPort in ports.ToList())
            {
                if (endPort.direction != startPort.direction &&
                    endPort.node != startPort.node)
                {
                    list.Add(endPort);
                }
            }

            return list;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                foreach (var elem in graphViewChange.elementsToRemove)
                {
                    if (elem is NodeView nodeView)
                    {
                        _tree.DeleteNode(nodeView.Node);
                    }

                    if (elem is Edge edge)
                    {
                        var parentView = edge.output.node as NodeView;
                        var childView = edge.input.node as NodeView;
                        Tree.BehaviourTree.RemoveChild(parentView.Node, childView.Node);
                    }
                }
            }

            if (graphViewChange.edgesToCreate != null)
            {
                foreach (var edge in graphViewChange.edgesToCreate)
                {
                    var parentView = edge.output.node as NodeView;
                    var childView = edge.input.node as NodeView;
                    Tree.BehaviourTree.AddChild(parentView.Node, childView.Node);
                }
            }

            nodes.ForEach((n) =>
            {
                var view = n as NodeView;
                view.SortChildren();
            });

            return graphViewChange;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction($"Create Script.../New Action Node", (a) => CreateNewScript(ScriptFileAssets[0]));
            evt.menu.AppendAction($"Create Script.../New Composite Node", (a) => CreateNewScript(ScriptFileAssets[1]));
            evt.menu.AppendAction($"Create Script.../New Decorator Node", (a) => CreateNewScript(ScriptFileAssets[2]));
            evt.menu.AppendSeparator();
            
            var nodePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
            {
                var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
                foreach (var type in types)
                {
                    evt.menu.AppendAction($"[Action]/{type.Name}", (a) => CreateNode(type, nodePosition));
                }
            }
            
            {
                var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
                foreach (var type in types)
                {
                    evt.menu.AppendAction($"[Composite]/{type.Name}", (a) => CreateNode(type, nodePosition));
                }
            }
            
            {
                var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
                foreach (var type in types)
                {
                    evt.menu.AppendAction($"[Decorator]/{type.Name}", (a) => CreateNode(type, nodePosition));
                }
            }
        }

        private static void SelectFolder(string path)
        {
            // https://forum.unity.com/threads/selecting-a-folder-in-the-project-via-button-in-editor-window.355357/

            if (path[path.Length - 1] == '/')
            {
                path = path.Substring(0, path.Length - 1);
            }

            var obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));

            Selection.activeObject = obj;

            EditorGUIUtility.PingObject(obj);
        }

        private void CreateNewScript(ScriptTemplate template)
        {
            SelectFolder($"{_settings.NewNodeBasePath}/{template.subFolder}");
            var templatePath = AssetDatabase.GetAssetPath(template.templateFile);
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, template.defaultFileName);
        }

        private void CreateNode(System.Type type, Vector2 position)
        {
            var node = _tree.CreateNode(type);
            node.Position = position;
            CreateNodeView(node);
        }

        private void CreateNodeView(Node node)
        {
            var nodeView = new NodeView(node);
            nodeView.OnNodeSelected = OnNodeSelected;
            AddElement(nodeView);
        }

        public void UpdateNodeStates()
        {
            nodes.ForEach(n =>
            {
                var view = n as NodeView;
                view.UpdateState();
            });
        }
    }
}