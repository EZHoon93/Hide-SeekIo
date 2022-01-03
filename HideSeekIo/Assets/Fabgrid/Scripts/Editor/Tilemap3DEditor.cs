﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Fabgrid
{
    [CustomEditor(typeof(Tilemap3D))]
    [ExecuteInEditMode]
    public class Tilemap3DEditor : Editor
    {
        private Tilemap3D tilemap;
        private VisualElement root;
        private VisualTreeAsset visualTree;
        private VisualElement selectedPanel;
        private Dictionary<string, VisualTreeAsset> panelAssets;
        private FSM fsm;

        private StyleSheet darkTheme;
        private StyleSheet lightTheme;

        private string fabgridFolder;

        public State CurrentState => fsm.CurrentState;

        public VisualElement Root => root;

        private void OnEnable()
        {
            fabgridFolder = PathUtility.GetFabgridFolder();

            fsm = new FSM();
            SetupPanelAssets();

            tilemap = target as Tilemap3D;
            tilemap.tileRotation = Quaternion.identity;

            root = new VisualElement();

            visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{fabgridFolder}/Scripts/Editor/Tilemap3DEditor.uxml");

            AddStyleSheets();

            LoadThemes();
            ApplyCurrentTheme();

            SceneView.duringSceneGui += OnDuringSceneGUI;
        }

        private void AddStyleSheets()
        {
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>($"{fabgridFolder}/Scripts/Editor/Tilemap3DEditor.uss"));
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>($"{PathUtility.PanelsPath}/PaintPanel.uss"));
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>($"{PathUtility.PanelsPath}/AddTilePanel.uss"));
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>($"{PathUtility.PanelsPath}/ConfigurationPanel.uss"));
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>($"{PathUtility.PanelsPath}/HelpPanel.uss"));
        }

        private void LoadThemes()
        {
            darkTheme = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{fabgridFolder}/Scripts/Editor/Themes/Dark.uss");
            lightTheme = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{fabgridFolder}/Scripts/Editor/Themes/Light.uss");
        }

        private void ApplyCurrentTheme()
        {
            RemoveAppliedThemes();

            if (EditorGUIUtility.isProSkin)
            {
                root.styleSheets.Add(darkTheme);
            }
            else 
            {
                root.styleSheets.Add(lightTheme);
            }
        }

        private void RemoveAppliedThemes()
        {
            if (root.styleSheets.Contains(darkTheme))
            {
                root.styleSheets.Remove(darkTheme);
            }

            if (root.styleSheets.Contains(lightTheme))
            {
                root.styleSheets.Remove(lightTheme);
            }
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnDuringSceneGUI;
            fsm.OnDestroy();
        }

        private void OnDuringSceneGUI(SceneView sceneView)
        {
            if (tilemap == null) return;

            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            fsm.OnSceneGUI();
        }

        private void SetupPanelAssets()
        {
            panelAssets = new Dictionary<string, VisualTreeAsset>
            {
                { "paint-panel", AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{PathUtility.PanelsPath}/PaintPanel.uxml") },
                { "add-tile-panel", AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{PathUtility.PanelsPath}/AddTilePanel.uxml") },
                { "configuration-panel", AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{PathUtility.PanelsPath}/ConfigurationPanel.uxml") },
                { "help-panel", AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{PathUtility.PanelsPath}/HelpPanel.uxml") }
            };
        }

        public override VisualElement CreateInspectorGUI()
        {
            root.Clear();
            root.Add(visualTree.CloneTree());
            SetupControls();
            SelectPanel("paint-panel", root.Q<Button>("paint-panel-button"), new Paint(root, tilemap));
            return root;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            fsm.OnInspectorGUI();
        }

        private void SetupControls()
        {
            var paintPanelButton = root.Q<Button>("paint-panel-button");
            paintPanelButton.clickable.clicked += () => OnClickNavigationBarButton("paint-panel", paintPanelButton, new Paint(root, tilemap));

            var addTilePanelButton = root.Q<Button>("add-tile-panel-button");
            addTilePanelButton.clickable.clicked += () => OnClickNavigationBarButton("add-tile-panel", addTilePanelButton, new AddTile(root, tilemap));

            var configurationPanelButton = root.Q<Button>("configuration-panel-button");
            configurationPanelButton.clickable.clicked += () => OnClickNavigationBarButton("configuration-panel", configurationPanelButton, new Configurate(root, tilemap));

            var helpPanelButton = root.Q<Button>("help-panel-button");
            helpPanelButton.clickable.clicked += () => OnClickNavigationBarButton("help-panel", helpPanelButton, new Help(root));

            AddNavigationButtonIcon(paintPanelButton, $"{fabgridFolder}/Textures/PaintButtonIcon.png");
            AddNavigationButtonIcon(addTilePanelButton, $"{fabgridFolder}/Textures/AddTileButtonIcon.png");
            AddNavigationButtonIcon(configurationPanelButton, $"{fabgridFolder}/Textures/ConfigurateButtonIcon.png");
            AddNavigationButtonIcon(helpPanelButton, $"{fabgridFolder}/Textures/HelpButtonIcon.png");
        }

        private void SelectPanel(string panelName, Button navigationBarButton, State state)
        {
            root.Query<Button>(null, "navigation-bar-button").ForEach(button =>
            {
                if (button.ClassListContains("selected-navigation-bar-button"))
                {
                    button.RemoveFromClassList("selected-navigation-bar-button");
                }
            });

            var panelContainer = root.Q<VisualElement>("main-panel-viewport");

            if (selectedPanel != null)
            {
                selectedPanel.RemoveFromHierarchy();
            }

            var instantiatedTemplate = panelAssets[panelName].CloneTree();
            panelContainer.Add(instantiatedTemplate);
            selectedPanel = instantiatedTemplate;

            navigationBarButton.AddToClassList("selected-navigation-bar-button");
            fsm.Transition(state);
        }

        private void AddNavigationButtonIcon(Button button, string iconPath)
        {
            var icon = new Image
            {
                image = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath)
            };
            icon.AddToClassList("navigation-bar-button-icon");
            button.Add(icon);
        }

        private void OnClickNavigationBarButton(string panelName, Button button, State state)
        {
            SelectPanel(panelName, button, state);
        }

        private void OnDestroy()
        {
            fsm.OnDestroy();
        }
    }
}