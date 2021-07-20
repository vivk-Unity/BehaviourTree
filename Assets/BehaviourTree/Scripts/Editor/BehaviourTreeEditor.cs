using System;
using System.Collections.Generic;
using BehaviourTree.Scripts.Tree;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace BehaviourTree.Scripts.Editor
{
    public class BehaviourTreeEditor : EditorWindow
    {
        private BehaviourTreeView _treeView;
        private Tree.BehaviourTree _tree;
        private InspectorView _inspectorView;
        private BlackboardView _blackboardView;
        private ToolbarMenu _toolbarMenu;
        private TextField _treeNameField;
        private TextField _locationPathField;
        private Button _createNewTreeButton;
        private VisualElement _overlay;
        private BehaviourTreeSettings _settings;

        private SerializedObject _treeObject;

        [MenuItem("Utility/BehaviourTree/Editor ...")]
        public static void OpenWindow()
        {
            var wnd = GetWindow<BehaviourTreeEditor>();
            wnd.titleContent = new GUIContent("BehaviourTreeEditor");
            wnd.minSize = new Vector2(800, 600);
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is Tree.BehaviourTree)
            {
                OpenWindow();
                return true;
            }

            return false;
        }

        private List<T> LoadAssets<T>() where T : Object
        {
            var assetIds = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            var assets = new List<T>();
            for (var i = 0; i < assetIds.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(assetIds[i]);
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                assets.Add(asset);
            }

            return assets;
        }

        public void CreateGUI()
        {
            _settings = BehaviourTreeSettings.GetOrCreateSettings();

            var root = rootVisualElement;

            var visualTree = _settings.BehaviourTreeXml;
            visualTree.CloneTree(root);
            
            var styleSheet = _settings.BehaviourTreeStyle;
            root.styleSheets.Add(styleSheet);

            _treeView = root.Q<BehaviourTreeView>();
            _treeView.OnNodeSelected = OnNodeSelectionChanged;

            _inspectorView = root.Q<InspectorView>();
            _blackboardView = root.Q<BlackboardView>();

            _toolbarMenu = root.Q<ToolbarMenu>();
            var behaviourTrees = LoadAssets<Tree.BehaviourTree>();
            behaviourTrees.ForEach(tree =>
            {
                _toolbarMenu.menu.AppendAction($"{tree.name}", (a) => { Selection.activeObject = tree; });
            });
            _toolbarMenu.menu.AppendSeparator();
            _toolbarMenu.menu.AppendAction("New Tree...", (a) => CreateNewTree("NewBehaviourTree"));

            _treeNameField = root.Q<TextField>("TreeName");
            _locationPathField = root.Q<TextField>("LocationPath");
            _overlay = root.Q<VisualElement>("Overlay");
            _createNewTreeButton = root.Q<Button>("CreateButton");
            _createNewTreeButton.clicked += () => CreateNewTree(_treeNameField.value);

            if (_tree == null)
            {
                OnSelectionChange();
            }
            else
            {
                SelectTree(_tree);
            }
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            switch (obj)
            {
                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                default:
                    break;
            }
        }

        private void OnSelectionChange()
        {
            EditorApplication.delayCall += () =>
            {
                var tree = Selection.activeObject as Tree.BehaviourTree;
                if (tree == null)
                {
                    if (Selection.activeGameObject)
                    {
                        var runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
                        if (runner)
                        {
                            var blackboard = runner.Blackboard;
                            tree = runner.Tree;
                            tree.Blackboard = blackboard;
                        }
                    }
                }

                SelectTree(tree);
            };
        }

        private void SelectTree(Tree.BehaviourTree newTree)
        {
            if (_treeView == null)
            {
                return;
            }

            if (newTree == null)
            {
                return;
            }

            _tree = newTree;

            _overlay.style.visibility = Visibility.Hidden;

            if (Application.isPlaying)
            {
                _treeView.PopulateView(_tree);
            }
            else
            {
                _treeView.PopulateView(_tree);
            }

            EditorApplication.delayCall += () => { _treeView.FrameAll(); };
        }

        private void OnNodeSelectionChanged(NodeView node)
        {
            _tree.Blackboard?.Bind(node.Node);
            _inspectorView.UpdateSelection(node);
            _blackboardView.UpdateSelection(node.Node);
        }

        private void OnInspectorUpdate()
        {
            _treeView?.UpdateNodeStates();
        }

        private void CreateNewTree(string assetName)
        {
            var path = System.IO.Path.Combine(_locationPathField.value, $"{assetName}.asset");
            var tree = ScriptableObject.CreateInstance<Tree.BehaviourTree>();
            tree.name = _treeNameField.ToString();
            AssetDatabase.CreateAsset(tree, path);
            AssetDatabase.SaveAssets();
            Selection.activeObject = tree;
            EditorGUIUtility.PingObject(tree);
        }
    }
}