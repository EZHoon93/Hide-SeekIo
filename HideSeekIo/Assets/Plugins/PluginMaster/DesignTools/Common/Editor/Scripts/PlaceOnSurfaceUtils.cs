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
using UnityEngine;

namespace PluginMaster
{
    public static class PlaceOnSurfaceUtils
    {
        public class PlaceOnSurfaceData
        {
            private Space _projectionDirectionSpace = Space.Self;
            private Vector3 _projectionDirection = Vector3.down;
            private bool _rotateToSurface = true;
            private Vector3 _objectOrientation = Vector3.down;
            private float _surfaceDistance = 0f;
            private LayerMask _mask = ~0;
            private bool _placeOnColliders = true;
            public bool rotateToSurface { get => _rotateToSurface; set => _rotateToSurface = value; }
            public Vector3 objectOrientation { get => _objectOrientation; set => _objectOrientation = value; }
            public float surfaceDistance { get => _surfaceDistance; set => _surfaceDistance = value; }
            public Vector3 projectionDirection { get => _projectionDirection; set => _projectionDirection = value; }
            public Space projectionDirectionSpace { get => _projectionDirectionSpace; set => _projectionDirectionSpace = value; }
            public LayerMask mask { get => _mask; set => _mask = value; }
            public bool placeOnColliders { get => _placeOnColliders; set => _placeOnColliders = value; }
        }
    }
}
