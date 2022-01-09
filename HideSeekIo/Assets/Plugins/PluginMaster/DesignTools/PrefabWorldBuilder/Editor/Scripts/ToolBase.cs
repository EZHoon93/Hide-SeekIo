/*
Copyright (c) 2021 Omar Duarte
Unauthorized copying of this file, via any medium is strictly prohibited.
Writen by Omar Duarte, 2021.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PluginMaster
{
    #region MANAGER
    public static class ToolProfile
    {
        public const string DEFAULT = "Default";
    }

    public interface IToolManager
    {
        string selectedProfileName { get; set; }
        string[] profileNames { get; }
        void SaveProfile();
        void SaveProfileAs(string name);
        void DeleteProfile();
        void Revert();
        void FactoryReset();
    }

    [Serializable]
    public class ToolManagerBase<T> : IToolManager, ISerializationCallbackReceiver where T : IToolSettings, new()
    {
        private static ToolManagerBase<T> _instance = null;
        private static Dictionary<string, T> _staticProfiles = new Dictionary<string, T>
        { { ToolProfile.DEFAULT, new T() } };
        [SerializeField] private string[] _profileKeys = { ToolProfile.DEFAULT };
        [SerializeField] private T[] _profileValues = { new T() };
        private static string _staticSelectedProfileName = ToolProfile.DEFAULT;
        [SerializeField] private string _selectedProfileName = _staticSelectedProfileName;
        private static T _staticUnsavedProfile = new T();
        [SerializeField] private T _unsavedProfile = _staticUnsavedProfile;

        public ToolManagerBase()
        {
            _instance = this;
        }
        public static ToolManagerBase<T> instance
        {
            get
            {
                if (_instance == null) _instance = new ToolManagerBase<T>();
                return _instance;
            }
        }

        public static T settings => _staticUnsavedProfile;

        private void UpdateUnsaved()
        {
            if (!_staticProfiles.ContainsKey(_staticSelectedProfileName))
                _staticSelectedProfileName = ToolProfile.DEFAULT;
            _staticUnsavedProfile.Copy(_staticProfiles[_staticSelectedProfileName]);
        }

        public string selectedProfileName
        {
            get => _staticSelectedProfileName;
            set
            {
                if (_staticSelectedProfileName == value) return;
                _staticSelectedProfileName = value;
                UpdateUnsaved();
                _staticUnsavedProfile.DataChanged();
            }
        }

        public string[] profileNames => _staticProfiles.Keys.ToArray();
        public void SaveProfile()
        {
            _staticProfiles[_staticSelectedProfileName].Copy(_staticUnsavedProfile);
            PWBCore.staticData.Save();
        }
        public void SaveProfileAs(string name)
        {
            if (!_staticProfiles.ContainsKey(name))
            {
                var newProfile = new T();
                newProfile.Copy(_unsavedProfile);
                _staticProfiles.Add(name, newProfile);
            }
            else _staticProfiles[name].Copy(_staticUnsavedProfile);
            _staticSelectedProfileName = name;
            UpdateUnsaved();
            _staticUnsavedProfile.DataChanged();
            PWBCore.staticData.Save();
        }
        public void DeleteProfile()
        {
            if (_staticSelectedProfileName == ToolProfile.DEFAULT) return;
            _staticProfiles.Remove(_staticSelectedProfileName);
            _staticSelectedProfileName = ToolProfile.DEFAULT;
            _staticUnsavedProfile.Copy(_staticProfiles[ToolProfile.DEFAULT]);
            _staticUnsavedProfile.DataChanged();
            PWBCore.staticData.Save();
        }
        public void Revert()
        {
            UpdateUnsaved();
            _staticUnsavedProfile.DataChanged();
            PWBCore.staticData.Save();
        }

        public void FactoryReset()
        {
            _staticUnsavedProfile = new T();
            _staticUnsavedProfile.DataChanged();
            PWBCore.staticData.Save();
        }

        public virtual void OnBeforeSerialize()
        {
            _selectedProfileName = _staticSelectedProfileName;
            _profileKeys = _staticProfiles.Keys.ToArray();
            _profileValues = _staticProfiles.Values.ToArray();
        }

        public virtual void OnAfterDeserialize()
        {
            _staticSelectedProfileName = _selectedProfileName;
            if (_profileKeys.Length > 1)
            {
                _staticProfiles.Clear();
                for (int i = 0; i < _profileKeys.Length; ++i) _staticProfiles.Add(_profileKeys[i], _profileValues[i]);
            }
            UpdateUnsaved();
        }
    }
    #endregion

    #region SETTINGS
    public interface IToolSettings
    {
        void DataChanged();
        void Copy(IToolSettings other);
    }

    [Serializable]
    public class CircleToolBase : IToolSettings
    {
        [SerializeField] private float _radius = 1f;
        [SerializeField] private bool _randomizePositions = true;
        public float radius
        {
            get => _radius;
            set
            {
                value = Mathf.Max(value, 0.05f);
                if (_radius == value) return;
                _radius = value;
                DataChanged();
            }
        }
        public bool randomizePositions
        {
            get => _randomizePositions;
            set
            {
                if (_randomizePositions == value) return;
                _randomizePositions = value;
                DataChanged();
            }
        }
        public virtual void Copy(IToolSettings other)
        {
            var otherCircleToolBase = other as CircleToolBase;
            if (otherCircleToolBase == null) return;
            _radius = otherCircleToolBase._radius;
            _randomizePositions = otherCircleToolBase._randomizePositions;
        }
        public virtual void DataChanged() => PWBCore.SetSavePending();
    }

    [Serializable]
    public class BrushToolBase : CircleToolBase, IPaintToolSettings
    {
        [SerializeField] private PaintToolSettings _paintTool = new PaintToolSettings();
        public enum BrushShape { POINT, CIRCLE, SQUARE }
        [SerializeField] protected BrushShape _brushShape = BrushShape.CIRCLE;
        [SerializeField] private int _density = 50;
        [SerializeField] private bool _orientAlongBrushstroke = false;
        [SerializeField] private Vector3 _additionalOrientationAngle = Vector3.zero;
        public enum SpacingType { AUTO, CUSTOM }
        [SerializeField] private SpacingType _spacingType = SpacingType.AUTO;
        [SerializeField] protected float _minSpacing = 1f;

        public BrushToolBase() : base() => _paintTool.OnDataChanged += DataChanged;

        public BrushShape brushShape
        {
            get => _brushShape;
            set
            {
                if (_brushShape == value) return;
                _brushShape = value;
                DataChanged();
            }
        }
        public int density
        {
            get => _density;
            set
            {
                value = Mathf.Clamp(value, 0, 100);
                if (_density == value) return;
                _density = value;
                DataChanged();
            }
        }
        public bool orientAlongBrushstroke
        {
            get => _orientAlongBrushstroke;
            set
            {
                if (_orientAlongBrushstroke == value) return;
                _orientAlongBrushstroke = value;
                DataChanged();
            }
        }
        public Vector3 additionalOrientationAngle
        {
            get => _additionalOrientationAngle;
            set
            {
                if (_additionalOrientationAngle == value) return;
                _additionalOrientationAngle = value;
                DataChanged();
            }
        }
        public SpacingType spacingType
        {
            get => _spacingType;
            set
            {
                if (_spacingType == value) return;
                _spacingType = value;
                DataChanged();
            }
        }
        public float minSpacing
        {
            get => _minSpacing;
            set
            {
                if (_minSpacing == value) return;
                _minSpacing = value;
                DataChanged();
            }
        }

        public Transform parent { get => _paintTool.parent; set => _paintTool.parent = value; }
        public bool overwritePrefabLayer { get => _paintTool.overwritePrefabLayer;
            set => _paintTool.overwritePrefabLayer = value; }
        public int layer { get => _paintTool.layer; set => _paintTool.layer = value; }
        public bool autoCreateParent { get => _paintTool.autoCreateParent; set => _paintTool.autoCreateParent = value; }
        public bool createSubparent { get => _paintTool.createSubparent; set => _paintTool.createSubparent = value; }
        public bool overwriteBrushProperties { get => _paintTool.overwriteBrushProperties;
                set => _paintTool.overwriteBrushProperties = value; }

        public BrushSettings brushSettings => _paintTool.brushSettings;

        public override void DataChanged()
        {
            base.DataChanged();
            BrushstrokeManager.UpdateBrushstroke();
        }

        public override void Copy(IToolSettings other)
        {
            var otherBrushToolBase = other as BrushToolBase;
            if (otherBrushToolBase == null) return;
            base.Copy(other);
            _paintTool.Copy(otherBrushToolBase._paintTool);
            _brushShape = otherBrushToolBase._brushShape;
            _density = otherBrushToolBase.density;
            _orientAlongBrushstroke = otherBrushToolBase._orientAlongBrushstroke;
            _additionalOrientationAngle = otherBrushToolBase._additionalOrientationAngle;
            _spacingType = otherBrushToolBase._spacingType;
            _minSpacing = otherBrushToolBase._minSpacing;
        }
    }

    public interface IPaintToolSettings
    {
        bool autoCreateParent { get; set; }
        bool createSubparent { get; set; }
        Transform parent { get; set; }
        bool overwritePrefabLayer { get; set; }
        int layer { get; set; }
        bool overwriteBrushProperties { get; set; }
        BrushSettings brushSettings { get; }
    }

    [Serializable]
    public class PaintToolSettings : IPaintToolSettings, ISerializationCallbackReceiver, IToolSettings
    {
        private Transform _parent = null;
        [SerializeField] private string _parentGlobalId = null;
        [SerializeField] private bool _autoCreateParent = true;
        [SerializeField] private bool _createSubparent = true;
        [SerializeField] private bool _overwritePrefabLayer = false;
        [SerializeField] private int _layer = 0;
        [SerializeField] private bool _overwriteBrushProperties = false;
        [SerializeField] private BrushSettings _brushSettings = new BrushSettings();

        public Action OnDataChanged;

        public PaintToolSettings() => OnDataChanged += DataChanged;

        public Transform parent
        {
            get
            {
                if (_parent == null && _parentGlobalId != null)
                {
                    if (GlobalObjectId.TryParse(_parentGlobalId, out GlobalObjectId id))
                    {
                        var obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(id) as GameObject;
                        if (obj == null) _parentGlobalId = null;
                        else _parent = obj.transform;
                    }
                }
                return _parent;
            }
            set
            {
                if (_parent == value) return;
                _parent = value;
                _parentGlobalId = _parent == null ? null
                    : GlobalObjectId.GetGlobalObjectIdSlow(_parent.gameObject).ToString();
                OnDataChanged();
            }
        }

        public bool autoCreateParent
        {
            get => _autoCreateParent;
            set
            {
                if (_autoCreateParent == value) return;
                _autoCreateParent = value;
                OnDataChanged();
            }
        }
        public bool createSubparent
        {
            get => _createSubparent;
            set
            {
                if (_createSubparent == value) return;
                _createSubparent = value;
                OnDataChanged();
            }
        }
        public bool overwritePrefabLayer
        {
            get => _overwritePrefabLayer;
            set
            {
                if (_overwritePrefabLayer == value) return;
                _overwritePrefabLayer = value;
                OnDataChanged();
            }
        }

        public int layer
        {
            get => _layer;
            set
            {
                if (_layer == value) return;
                _layer = value;
                OnDataChanged();
            }
        }

        public bool overwriteBrushProperties
        {
            get => _overwriteBrushProperties;
            set
            {
                if (_overwriteBrushProperties == value) return;
                _overwriteBrushProperties = value;
                OnDataChanged();
            }
        }

        public BrushSettings brushSettings => _brushSettings;

        public virtual void DataChanged() => PWBCore.SetSavePending();

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize() => _parent = null;

        public virtual void Copy(IToolSettings other)
        {
            var otherPaintToolSettings = other as PaintToolSettings;
            if (otherPaintToolSettings == null) return;
            _parent = otherPaintToolSettings._parent;
            _parentGlobalId = otherPaintToolSettings._parentGlobalId;
            _overwritePrefabLayer = otherPaintToolSettings._overwritePrefabLayer;
            _layer = otherPaintToolSettings._layer;
            _autoCreateParent = otherPaintToolSettings._autoCreateParent;
            _createSubparent = otherPaintToolSettings._createSubparent;
            _overwriteBrushProperties = otherPaintToolSettings._overwriteBrushProperties;
            _brushSettings.Copy(otherPaintToolSettings._brushSettings);
        }
    }

    public interface IPaintOnSurfaceToolSettings
    {
        bool paintOnMeshesWithoutCollider { get; set; }
        bool paintOnSelectedOnly { get; set; }
        bool paintOnPalettePrefabs { get; set; }
    }

    [Serializable]
    public class PaintOnSurfaceToolSettings : IPaintOnSurfaceToolSettings,
        ISerializationCallbackReceiver, IToolSettings
    {
        public enum PaintMode
        {
            AUTO,
            ON_SURFACE,
            ON_SHAPE
        }

        [SerializeField] private bool _paintOnMeshesWithoutCollider = false;
        [SerializeField] private bool _paintOnSelectedOnly = false;
        [SerializeField] private bool _paintOnPalettePrefabs = false;
        
        private bool _updateMeshColliders = false;
        public Action OnDataChanged;

        public PaintOnSurfaceToolSettings() => OnDataChanged += DataChanged;

        public bool paintOnMeshesWithoutCollider
        {
            get
            {
                if (_updateMeshColliders)
                {
                    _updateMeshColliders = false;
                    PWBCore.UpdateTempColliders();
                }
                return _paintOnMeshesWithoutCollider;
            }
            set
            {
                if (_paintOnMeshesWithoutCollider == value) return;
                _paintOnMeshesWithoutCollider = value;
                OnDataChanged();
                if (_paintOnMeshesWithoutCollider) PWBCore.UpdateTempColliders();
            }
        }
        public bool paintOnSelectedOnly
        {
            get => _paintOnSelectedOnly;
            set
            {
                if (_paintOnSelectedOnly == value) return;
                _paintOnSelectedOnly = value;
                OnDataChanged();
            }
        }

        public bool paintOnPalettePrefabs
        {
            get => _paintOnPalettePrefabs;
            set
            {
                if (_paintOnPalettePrefabs == value) return;
                _paintOnPalettePrefabs = value;
                OnDataChanged();
            }
        }

        public virtual void Copy(IToolSettings other)
        {
            var otherPaintOnSurfaceToolSettings = other as PaintOnSurfaceToolSettings;
            if (otherPaintOnSurfaceToolSettings == null) return;
            _paintOnMeshesWithoutCollider = otherPaintOnSurfaceToolSettings._paintOnMeshesWithoutCollider;
            _paintOnSelectedOnly = otherPaintOnSurfaceToolSettings._paintOnSelectedOnly;
            _paintOnPalettePrefabs = otherPaintOnSurfaceToolSettings._paintOnPalettePrefabs;
        }
        public virtual void DataChanged() => PWBCore.SetSavePending();
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() => _updateMeshColliders = _paintOnMeshesWithoutCollider;

    }

    [Serializable]
    public class SelectionToolBaseBasic : IToolSettings
    {
        [SerializeField] private bool _embedInSurface = false;
        [SerializeField] private bool _embedAtPivotHeight = false;
        [SerializeField] private float _surfaceDistance = 0f;

        public bool embedInSurface
        {
            get => _embedInSurface;
            set
            {
                if (_embedInSurface == value) return;
                _embedInSurface = value;
                DataChanged();
            }
        }

        public bool embedAtPivotHeight
        {
            get => _embedAtPivotHeight;
            set
            {
                if (_embedAtPivotHeight == value) return;
                _embedAtPivotHeight = value;
                DataChanged();
            }
        }

        public float surfaceDistance
        {
            get => _surfaceDistance;
            set
            {
                if (_surfaceDistance == value) return;
                _surfaceDistance = value;
                DataChanged();
            }
        }

        
        public virtual void Copy(IToolSettings other)
        {
            var otherSelectionTool = other as SelectionToolBaseBasic;
            if (otherSelectionTool == null) return;
            _embedInSurface = otherSelectionTool._embedInSurface;
            _embedAtPivotHeight = otherSelectionTool._embedAtPivotHeight;
            _surfaceDistance = otherSelectionTool._surfaceDistance;
        }

        public virtual void DataChanged() => PWBCore.SetSavePending();
    }

    [Serializable]
    public class SelectionToolBase : SelectionToolBaseBasic
    {
        [SerializeField] private bool _rotateToTheSurface = false;

        public bool rotateToTheSurface
        {
            get => _rotateToTheSurface;
            set
            {
                if (_rotateToTheSurface == value) return;
                _rotateToTheSurface = value;
                DataChanged();
            }
        }

        public override void Copy(IToolSettings other)
        {
            var otherSelectionTool = other as SelectionToolBase;
            if (otherSelectionTool == null) return;
            base.Copy(other);
            _rotateToTheSurface = otherSelectionTool._rotateToTheSurface;
        }
    }

    public interface IModifierTool
    {
        ModifierToolSettings.Command command { get; set; }
        bool modifyAllButSelected { get; set; }
    }
    [Serializable]
    public class ModifierToolSettings : IModifierTool, IToolSettings
    {
        public enum Command
        {
            MODIFY_ALL,
            MODIFY_PALETTE_PREFABS,
            MODIFY_BRUSH_PREFABS
        }

        [SerializeField] private Command _command = Command.MODIFY_ALL;
        [SerializeField] private bool _allButSelected = false;
        public Action OnDataChanged;

        public ModifierToolSettings() => OnDataChanged += DataChanged;
        public Command command
        {
            get => _command;
            set
            {
                if (_command == value) return;
                _command = value;
                DataChanged();
            }
        }

        public bool modifyAllButSelected
        {
            get => _allButSelected;
            set
            {
                if (_allButSelected == value) return;
                _allButSelected = value;
                DataChanged();
            }
        }

        public void DataChanged() => PWBCore.SetSavePending();

        public virtual void Copy(IToolSettings other)
        {
            var otherModifier = other as IModifierTool;
            if (otherModifier == null) return;
            _command = otherModifier.command;
            _allButSelected = otherModifier.modifyAllButSelected;
        }
    }
    #endregion
}
