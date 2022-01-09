/*
Copyright (c) 2020 Omar Duarte
Unauthorized copying of this file, via any medium is strictly prohibited.
Writen by Omar Duarte, 2020.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using System;
using UnityEngine;

namespace PluginMaster
{
    public static class RandomUtils
    {
        [Serializable]
        public class Range: ISerializationCallbackReceiver
        {
            [SerializeField] private float _v1 = -1f;
            [SerializeField] private float _v2 = 1f;
            [SerializeField] private float _min = -1f;
            [SerializeField] private float _max = 1f;

            public Range() { }
            public Range(Range other) => (_v1, _v2) = (other._v1, other._v2);
            public Range(float v1, float v2)
            {
                _v1 = v1;
                _v2 = v2;
            }

            public float v1 { get => _v1; set => _v1 = value; }
            public float v2 { get => _v2; set => _v2 = value; }

            public float min => Mathf.Min(_v1, _v2);
            public float max => Mathf.Max(_v1, _v2);

            public override int GetHashCode()
            {
                int hashCode = -1605643878;
                hashCode = hashCode * -1521134295 + _v1.GetHashCode();
                hashCode = hashCode * -1521134295 + _v2.GetHashCode();
                return hashCode;
            }
            public override bool Equals(object obj) => obj is Range range && _v1 == range._v1 & _v2 == range._v2;

            public void OnBeforeSerialize()
            {
                _min = min;
                _max = max;
            }

            public void OnAfterDeserialize()
            {
                _v1 = _min;
                _v2 = _max;
            }

            public static bool operator ==(Range value1, Range value2) => Equals(value1, value2);
            public static bool operator !=(Range value1, Range value2) => !Equals(value1, value2);
            
            public float randomValue => UnityEngine.Random.Range(min, max);

        }

        [Serializable]
        public class Range3
        {
            public Range x = new Range(0,0);
            public Range y = new Range(0, 0);
            public Range z = new Range(0, 0);

            public Range3(Vector3 v1, Vector3 v2)
            {
                x = new Range(v1.x, v2.x);
                y = new Range(v1.y, v2.y);
                z = new Range(v1.z, v2.z);
            }

            public Range3(Range3 other)
            {
                x = new Range(other.x);
                y = new Range(other.y);
                z = new Range(other.z);
            }
            public Vector3 v1
            {
                get => new Vector3(x.v1, y.v1, z.v1); 
                set
                {
                    x.v1 = value.x;
                    y.v1 = value.y;
                    z.v1 = value.z;
                }
            }

            public Vector3 v2
            {
                get => new Vector3(x.v2, y.v2, z.v2);
                set
                {
                    x.v2 = value.x;
                    y.v2 = value.y;
                    z.v2 = value.z;
                }
            }

            public Vector3 min => new Vector3(x.min, y.min, z.min);
            public Vector3 max => new Vector3(x.max, y.max, z.max);

            public override int GetHashCode()
            {
                int hashCode = 373119288;
                hashCode = hashCode * -1521134295 + x.GetHashCode();
                hashCode = hashCode * -1521134295 + y.GetHashCode();
                hashCode = hashCode * -1521134295 + z.GetHashCode();
                return hashCode;
            }
            public override bool Equals(object obj)
                => obj is Range3 range3 && x == range3.x && y == range3.y && z == range3.z;
            public static bool operator ==(Range3 value1, Range3 value2) => Equals(value1, value2);
            public static bool operator !=(Range3 value1, Range3 value2) => !Equals(value1, value2);

            public Vector3 randomVector => new Vector3(x.randomValue, y.randomValue, z.randomValue);

            
        }
    }
}
