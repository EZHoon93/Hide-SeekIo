/*
Copyright (c) 2020 Omar Duarte
Unauthorized copying of this file, via any medium is strictly prohibited.
Writen by Omar Duarte, 2020.

This class demonstrates the code discussed in this forum:
https://forum.unity.com/threads/is-drag-and-drop-from-custom-editor-window-into-scene-not-possible.658810/

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using UnityEditor;
using UnityEngine;

namespace PluginMaster
{
    public static class SceneDragAndDrop
    {
        private static readonly int _sceneDragHint = "SceneDragAndDrop".GetHashCode();
        private const string DRAG_ID = "SceneDragAndDrop";

        public static void StartDrag(ISceneDragReceiver receiver, string title)
        {
            StopDrag();
            if (receiver == null) return;
            GUIUtility.hotControl = 0;
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.objectReferences = new Object[0];
            DragAndDrop.paths = new string[0];
            DragAndDrop.SetGenericData(DRAG_ID, receiver);
            receiver.StartDrag();
            DragAndDrop.StartDrag(title);
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui += OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif
        }

        public static void StopDrag()
        {
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui -= OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
#endif
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            int controlId = GUIUtility.GetControlID(_sceneDragHint, FocusType.Passive);
            Event evt = Event.current;
            EventType eventType = evt.GetTypeForControl(controlId);
            ISceneDragReceiver receiver;
            if(eventType == EventType.DragPerform || eventType == EventType.DragUpdated)
            {
                receiver = DragAndDrop.GetGenericData(DRAG_ID) as ISceneDragReceiver;
                if (receiver == null) return;
                DragAndDrop.visualMode = receiver.UpdateDrag(evt, eventType);
                if (eventType == EventType.DragPerform && DragAndDrop.visualMode != DragAndDropVisualMode.None)
                {
                    receiver.PerformDrag(evt);
                    DragAndDrop.AcceptDrag();
                    DragAndDrop.SetGenericData(DRAG_ID, default(ISceneDragReceiver));
                    StopDrag();
                }
                evt.Use();
            }
            else if (eventType == EventType.DragExited)
            {
                receiver = DragAndDrop.GetGenericData(DRAG_ID) as ISceneDragReceiver;
                if (receiver == null) return;
                receiver.StopDrag();
                evt.Use();
            }
        }
    }

    public interface ISceneDragReceiver
    {
        void StartDrag();
        void StopDrag();
        DragAndDropVisualMode UpdateDrag(Event evt, EventType eventType);
        void PerformDrag(Event evt);
    }
}
