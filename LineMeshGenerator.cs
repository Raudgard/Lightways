using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityToolKit
{
    public class LineMeshGenerator
    {
        [Serializable]
        public struct MyVector3
        {
            public int index;
            public Vector3 pos;
            public Vector3 tangent;

            public Vector3 forward;

            public Vector3 normal;

            public Vector3 leftNormal;
            public Vector3 rightNormal;


            public bool isPosDirty;

            public float leftWidth;
            public float rightWidth;
            public float thick;

            public Vector3 leftPos;
            public Vector3 rightPos;
            public Vector3 centerThickPos;
            public Vector3 leftThickPos;
            public Vector3 rightThickPos;
            public bool isGenDirty;

            public float lengthInAll;


            public void UpdateNormalAndTangent(MyVector3 previous, MyVector3 next)
            {
                var pp = pos - previous.pos;
                var p1 = pp.normalized;
                var p2 = (next.pos - pos).normalized;
                forward = p2;
                tangent = (p1 + p2).normalized;
                if (tangent == Vector3.zero)
                {
                    if (p1 != Vector3.zero)
                    {
                        // back point use previous tangent
                        tangent = previous.tangent;
                    }
                    else
                        Debug.LogError("---> tangent is ZERO \n" + previous.pos + " -> " + this.pos + " -> " +
                                       next.pos);
                }


                normal = Vector3.Cross(tangent, new Vector3(0, 0, -1)).normalized;
                if (normal == Vector3.zero)
                {
                    Debug.LogError("---> normal is ZERO\n" + previous.pos + " -> " + this.pos + " -> " + next.pos);
                }

                // var last = 

                leftNormal = normal;
                rightNormal = -normal;
                // Debug.Log(tangent + " ---> " + normal+"\n"+ this.ToString());
                lengthInAll = previous.lengthInAll + pp.magnitude;
                isPosDirty = false;
            }

            public void GenerateOtherPositions(MyVector3 previous, List<MyVector3> points)
            {
                leftPos = pos + leftNormal * leftWidth;
                rightPos = pos + rightNormal * rightWidth;

                #region not good

                /*if (Vector3.Angle(forward, previous.forward) > 20)
                {
                    var p = Vector3.zero;
                    var pre = index > 2 ? 2 : index;
                    var isLeft = Vector3.Angle(forward, normal) < 90;
                    var c = 0;
                    for (int i = 1; i <= pre; i++)
                    {
                        var temp = points[index - i];
                        if (Vector3.Distance(pos, temp.pos) < leftWidth * 2)
                        {
                            p += isLeft ? temp.leftPos : temp.rightPos;
                            c++;
                        }
                    }
                    p = (p + pos) / (c + 1f);
                    Debug.Log("----> " + c + "    " + (isLeft ? "left" : "right"));
                    for (int i = -pre; i <= 0; i++)
                    {
                        var temp = points[index + i];
                        if (Vector3.Distance(pos, temp.pos) < leftWidth * 2)
                        {
                            if (isLeft)
                            {
                                Debug.LogError(temp.leftPos + "   --->   " + p);
                                temp.leftPos = p;
                            }
                            else
                            {
                                Debug.LogError(temp.rightPos + "   --->   " + p);
                                temp.rightPos = p;
                            }
                        }
                       
                        points[index + i] = temp;
                    }
                }*/

                #endregion

                leftThickPos = leftPos + Vector3.forward * thick;
                rightThickPos = rightPos + Vector3.forward * thick;

                isGenDirty = false;
            }

            public override string ToString()
            {
                return pos + "\n" + JsonUtility.ToJson(this);
                // return JsonUtility.ToJson(this);
            }
        }

        public LineMeshGenerator()
        {
            positions = new List<MyVector3>();
            tempPostions = new List<Vector3>();
            mesh = new Mesh();
        }

        private List<MyVector3> positions;
        private bool _isDirty;

        private float mLeftWidth, mRightWidth;

        private float mThick;

        private int smoothStep = 4;

        private List<Vector3> tempPostions;

        public float thick
        {
            get { return mThick; }
            set
            {
                mThick = value;
                isDirty = true;
            }
        }

        public float leftWdith
        {
            get { return mLeftWidth; }
            set { mLeftWidth = value; }
        }

        public float rightWidth
        {
            get { return mRightWidth; }
            set { mRightWidth = value; }
        }

        private bool isDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;
                if (value)
                    Generate();
            }
        }

        public Vector3 this[int index]
        {
            get
            {
                if (CheckIndex(index, true))
                    return positions[index].pos;
                return Vector3.zero;
            }
            set
            {
                if (CheckIndex(index, true))
                {
                    var p = positions[index];
                    p.pos = value;
                    p.isPosDirty = true;
                    positions[index] = p;
                    isDirty = true;
                }
            }
        }
#if UNITY_EDITOR
        public void DrawGizmos(Transform transform)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            for (int i = 0; i < Count; i++)
            {
                Gizmos.color = new Color(0, 0, 1, 1 - i * 1f / Count);
                Gizmos.DrawSphere(this[i], .025f);
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(this[i], positions[i].leftNormal * .5f);
                Gizmos.color = Color.cyan;
                Gizmos.DrawRay(this[i], positions[i].rightNormal * .5f);
            }
        }
#endif
        public Vector3 GetProgressPosition(float percent)
        {
            Vector3 p = Vector3.zero;

            if (Count > 2)
            {
                var last = positions.Last();
                var length = last.lengthInAll * percent;
                for (int i = Count - 1; i >= 0; i--)
                {
                    if (length > positions[i].lengthInAll)
                    {
                        p = positions[i].pos + (length - positions[i].lengthInAll) * positions[i].forward;

                        break;
                    }
                }
            }

            return p;
        }


        public void Add(Vector3 pos)
        {
            if (tempPostions.Count > 0)
            {
                if (Vector3.Distance(tempPostions.Last(), pos) <= 1f * (leftWdith + rightWidth) / 2)
                {
                    //Debug.Log("1. Same pos as previous one");
                    return;
                }
            }

            tempPostions.Add(pos);

            if (tempPostions.Count < 2)
            {
                return;
            }

            var c = tempPostions.Count >= 5 ? 5 : tempPostions.Count;
            // var curved = MakeSmoothCurve(tempPostions, 4);


            var curved = MakeSmoothCurve(tempPostions.GetRange(tempPostions.Count - c, c), smoothStep);
            // Debug.Log((tempPostions.Count - c) + "    " + c + "--->" + curved.Count);

            for (int i = 0; i < curved.Count; i++)
            {
                var inx = c == 5 ? Count - (curved.Count - smoothStep) + i + 1 : i;
                if (((c == 5 && i < curved.Count - smoothStep) || c < 5) && CheckIndex(inx))
                {
                    var temp = positions[inx];
                    var ratio = i * 1f / curved.Count;
                    temp.pos = (1 - ratio) * temp.pos + ratio * curved[i];
                    temp.isPosDirty = true;
                    positions[inx] = temp;
                }
                else
                {
                    if (Count > 0)
                    {
                        if (Vector3.Distance(positions.Last().pos, curved[i]) <= 0.05f)
                        {
                            //Debug.Log("2. Same pos as previous one");
                            continue;
                        }

                        var temp = positions.Last();
                        temp.isPosDirty = true;
                        positions[Count - 1] = temp;
                    }

                    var p = new MyVector3()
                    {
                        index = positions.Count,
                        pos = curved[i],
                        leftWidth = leftWdith,
                        rightWidth = rightWidth,
                        thick = thick,
                        isGenDirty = true,
                        isPosDirty = true,
                    };

                    positions.Add(p);
                }
            }


            isDirty = true;
        }

        public void AddWithWidth(Vector3 pos, float lw, float rw)
        {
            UpdateWidth(lw, rw, false);
            Add(pos);
        }

        public void RemoveAt(int index)
        {
            if (CheckIndex(index))
                positions.RemoveAt(index);
            if (CheckIndex(index - 1))
            {
                SetGenDirty(index - 1, true);
            }

            if (CheckIndex(index - 2))
            {
                SetGenDirty(index - 2, true);
            }

            if (CheckIndex(index))
            {
                SetGenDirty(index, true);
            }

            if (CheckIndex(index + 1))
            {
                SetGenDirty(index + 1, true);
            }
        }

        public void Clear()
        {
            positions.Clear();
            tempPostions.Clear();
            isDirty = true;
            mesh.Clear();
        }

        public void UpdateWidth(float lw, float rw, bool shouldGen = true)
        {
            this.leftWdith = lw;
            this.rightWidth = rw;
            if (shouldGen) Generate();
        }

        public int Count
        {
            get { return positions.Count; }
        }

        public int VertexCount
        {
            get { return positions.Count * 6; }
        }

        public int TriangleCount
        {
            get { return (Count - 1) * 12; }
        }

        public Mesh mesh;

        public LineMeshGenerator(float left, float right, float thick) : this()
        {
            mLeftWidth = left;
            mRightWidth = right;
            this.mThick = thick;
        }

        void SetGenDirty(int inx, bool isGenDirty)
        {
            var p = positions[inx];
            p.isGenDirty = isGenDirty;
            positions[inx] = p;
        }

        bool CheckIndex(int index, bool logError = false)
        {
            bool isValid = index >= 0 && index < Count;
            if (logError && !isValid)
                Debug.LogError("Index out range: " + index + "  from  " + Count);
            return isValid;
        }


        void SimplePositions()
        {
            if (Count < 2)
            {
                return;
            }

            var p = positions[0];
            for (int i = 1; i < Count; i++)
            {
                if (p.pos == positions[i].pos)
                {
                    RemoveAt(i);
                }
            }
        }

        void Generate()
        {
            SimplePositions();
            bool isMeshDirty = false;

            if (Count < 2)
            {
                return;
            }

            for (int i = 0; i < Count; i++)
            {
                var p = positions[i];
                if (p.isPosDirty)
                {
                    isMeshDirty = true;
                    if (i > 0 && i < Count - 1)
                    {
                        var previous = positions[i - 1];
                        var next = positions[i + 1];
                        p.UpdateNormalAndTangent(previous, next);

                        if (i > 1)
                        {
                            previous.UpdateNormalAndTangent(positions[i - 2], p);
                            previous.isGenDirty = true;
                            positions[i - 1] = previous;
                        }

                        if (i < Count - 2)
                        {
                            next.UpdateNormalAndTangent(p, positions[i + 2]);
                            next.isGenDirty = true;
                            positions[i + 1] = next;
                        }
                    }
                    else if (i == 0)
                    {
                        p.UpdateNormalAndTangent(p, i + 1 < Count ? positions[i + 1] : p);
                    }
                    else
                    {
                        p.UpdateNormalAndTangent(i - 1 >= 0 ? positions[i - 1] : p, p);
                    }

                    p.isGenDirty = true;
                    positions[i] = p;
                }
            }

            var vertices = mesh.vertices;
            if (vertices == null || vertices.Length != VertexCount)
                vertices = new Vector3[VertexCount];

            var triangles = mesh.triangles;
            if (Count > 0)
            {
                if (triangles == null || triangles.Length != TriangleCount)
                    triangles = new int[TriangleCount];
            }
            else
            {
                mesh.triangles = new int[0];
                triangles = new int[0];
            }

            var uv = mesh.uv;
            if (uv == null || uv.Length != VertexCount)
                uv = new Vector2[VertexCount];

            for (int i = 0; i < Count; i++)
            {
                var p = positions[i];
                if (p.isGenDirty || isDirty)
                {
                    isMeshDirty = true;
                    p.GenerateOtherPositions(i == 0 ? p : positions[i - 1], positions);
                    positions[i] = p;
                }
            }

            for (int i = 0; i < Count; i++)
            {
                var p = positions[i];
                vertices[6 * i] = p.leftPos;
                vertices[6 * i + 1] = p.rightPos;
                vertices[6 * i + 2] = p.pos;
                vertices[6 * i + 3] = p.leftThickPos;
                vertices[6 * i + 4] = p.rightThickPos;
                vertices[6 * i + 5] = p.centerThickPos;
                if (i < Count - 1)
                {
                    triangles[12 * i + 0] = 6 * i;
                    triangles[12 * i + 1] = 6 * (i + 1) + 2;
                    triangles[12 * i + 2] = 6 * i + 2;
                    triangles[12 * i + 3] = 6 * (i + 1) + 2;
                    triangles[12 * i + 4] = 6 * i;
                    triangles[12 * i + 5] = 6 * (i + 1);
                    triangles[12 * i + 6] = 6 * i + 2;
                    triangles[12 * i + 7] = 6 * (i + 1) + 1;
                    triangles[12 * i + 8] = 6 * i + 1;
                    triangles[12 * i + 9] = 6 * (i + 1) + 1;
                    triangles[12 * i + 10] = 6 * i + 2;
                    triangles[12 * i + 11] = 6 * (i + 1) + 2;
                }

                uv[6 * i + 0] = new Vector2(0, p.lengthInAll / 64);
                uv[6 * i + 1] = new Vector2(1, p.lengthInAll / 64);
                uv[6 * i + 2] = new Vector2(0.5f, p.lengthInAll / 64);
                uv[6 * i + 3] = new Vector2(0, 0);
                uv[6 * i + 4] = new Vector2(1, 1);
                uv[6 * i + 5] = new Vector2(0, 0);
            }

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            if (isMeshDirty)
                mesh.UploadMeshData(false);

            isDirty = false;
            //Debug.Log("Generated!");
        }

        public static List<Vector3> MakeSmoothCurve(List<Vector3> arrayToCurve, float smoothness)
        {
            List<Vector3> points;
            List<Vector3> curvedPoints;
            int pointsLength = 0;
            int curvedLength = 0;

            if (smoothness < 1.0f) smoothness = 1.0f;

            pointsLength = arrayToCurve.Count;

            curvedLength = (pointsLength * Mathf.RoundToInt(smoothness)) - 1;
            curvedPoints = new List<Vector3>();

            float t = 0.0f;
            for (int pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLength + 1; pointInTimeOnCurve++)
            {
                t = Mathf.InverseLerp(0, curvedLength, pointInTimeOnCurve);

                points = new List<Vector3>(arrayToCurve);

                for (int j = pointsLength - 1; j > 0; j--)
                {
                    for (int i = 0; i < j; i++)
                    {
                        points[i] = (1 - t) * points[i] + t * points[i + 1];
                    }
                }

                curvedPoints.Add(points.First());
            }


            return curvedPoints;
        }
    }
}