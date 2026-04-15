using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Level
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(BallManager))]
    [RequireComponent(typeof(GameStageGenerator))]
    public class LevelGenerator : MonoBehaviour
    {
        internal TriangleSegment[] TriangleSegments;

        [SerializeField]
        Vector2 LevelCenter;
        [SerializeField]
        int Radius;
        [SerializeField]
        int NoSides;
        [SerializeField]
        GameObject DebugMarkerPrefab;
        [SerializeField]
        float BallSize;
        [SerializeField]
        bool ShowWalkableVertices;

        void Awake()
        {
            //GenerateLevel();
            //InitialiseBallManager();
        }
        // Update is called once per frame
        void Update()
        {

        }

        internal void Initialise()
        {
            GenerateLevel();
        }

        internal LevelData GetLevelData()
        {
            return new LevelData(this);
        }

        private void GenerateLevel()
        {
            GeneratePolygon(LevelCenter, Radius, NoSides);
        }

        private void GeneratePolygon(Vector2 center, int radius, int noSides)
        {
            var circle = new Circle(center, radius);
            var maxAngle = 360f;
            var angleBetweenVertices = maxAngle / noSides;
            var noVertices = noSides + 1; // plus one for center vertex
            Vector2[] vertices = new Vector2[noVertices];
            var triangleSegments = new TriangleSegment[noSides];

            var vertexIndex = 0;
            for (float angle = 0f; angle <= maxAngle; angle += angleBetweenVertices)
            {
                var vertex = GetVertexOnCircumference(angle, circle);
                vertices[vertexIndex] = vertex;

                if (vertexIndex > 0) // create TriangleSegment object
                {
                    var v1 = circle.Center;
                    var v2 = vertex;
                    var v3 = vertices[vertexIndex - 1];

                    var triangleSegment = new TriangleSegment(v1, v2, v3, BallSize);

                    var triangleSegIndex = vertexIndex - 1;

                    triangleSegments[triangleSegIndex] = triangleSegment;

                    if (ShowWalkableVertices)    
                        DebugShowWalkableVertices(triangleSegment.GetAllSpacePositions(), 6);
                }
        
                vertexIndex++;
            }

            vertices[noVertices - 1] = circle.Center;

            GetComponent<MeshFilter>().mesh = BuildMesh(vertices); // assign mesh component
            TriangleSegments = triangleSegments;
        }

        private Mesh BuildMesh(Vector2[] vertices)
        {
            Vector3[] vertices3d = new Vector3[vertices.Length];

            for (int vi = vertices.Length - 1; vi >= 0; vi--) // convert vector2s to vector3s for mesh
            {
                var v2 = vertices[vi];
                vertices3d[vi] = new Vector3(v2.x, v2.y, 0);
                var triangleVertexGO = Instantiate(DebugMarkerPrefab, vertices3d[vi], Quaternion.identity);
                triangleVertexGO.transform.localScale = new Vector3(BallSize, BallSize, BallSize);
            }

            int centerVertex = NoSides;

            var triangleIndices = new int[NoSides * 3];

            int c1 = 1;
            int c2 = 2;
            for (int i = 0; i < triangleIndices.Length; i += 3)
            {
                triangleIndices[i] = centerVertex;
                triangleIndices[i + 1] = c1;
                triangleIndices[i + 2] = c2;
                c1++;
                c2++;
                if (c1 == centerVertex)
                    c1 = 0;
                if (c2 == centerVertex)
                    c2 = 0;
            }

            Mesh mesh = new Mesh
            {
                vertices = vertices3d,
                triangles = triangleIndices
            };

            mesh.RecalculateNormals();
            return mesh;
        }

        private Vector2 GetVertexOnCircumference(float angleDegrees, Circle circle)
        {
            var center = circle.Center;
            var radius = circle.Radius;
            var degreeRadians = angleDegrees * (Mathf.PI / 180);
            var x = center.x + (radius * Mathf.Cos(degreeRadians));
            var y = center.y + (radius * Mathf.Sin(degreeRadians));

            return new Vector2(x, y);
        }

        private void DebugShowWalkableVertices(Vector2[] walkableSquares, int colourIndex)
        {
             var colours = new Color[]
            {
                Color.black,
                Color.red,
                Color.cyan,
                Color.blue,
                Color.gray,
                Color.yellow,
                Color.green,
                Color.magenta,
                Color.black,
                Color.red
            };

            foreach (var square in walkableSquares)
            {
                var squareGO = Instantiate(DebugMarkerPrefab, square, Quaternion.identity);
                squareGO.transform.localScale = new Vector3(BallSize, BallSize, BallSize);
                squareGO.GetComponent<SpriteRenderer>().color = colours[colourIndex];
            }
        }
    }

    internal class Circle
    {
        public Vector2 Center;
        public int Radius;
        public int Circumference;

        public Circle(Vector2 center, int radius)
        {
            Center = center;
            Radius = radius;
            SetCircumference();
        }

        private void SetCircumference()
        {
            Circumference = (int)(2 * Mathf.PI * Radius);
        }
    }

    internal class TriangleSegment
    {
        internal Vector2[] Vertices;
        internal Row[] Rows;
        public TriangleSegment(Vector2 centerVertex, Vector2 v2, Vector2 v3, float ballSize)
        {
            Initialise(centerVertex, v2, v3, ballSize);
        }

        private void Initialise(Vector2 centerVertex, Vector2 v2, Vector2 v3, float ballSize)
        {
            Vertices = new Vector2[]
            {
                centerVertex,
                v2,
                v3
            };

            var distanceBetweenPoints = ballSize;
            var increment = 0.05f;
            var fullDistance = 1.0f;

            var rows = new List<Row>();

            var rowCount = 0;

            for (float posA = increment; posA < fullDistance; posA += increment)
            {
                var newPoint1 = InterpolatePoint(centerVertex, v2, posA);
                var newPoint2 = InterpolatePoint(centerVertex, v3, posA);

                var distance = Vector2.Distance(newPoint1, newPoint2);
                var percentage = distanceBetweenPoints / distance;

                var row = new Row();
                var spaces = new List<Space>();

                for (float posB = percentage; posB < fullDistance; posB += percentage)
                {
                    var spacePosition = InterpolatePoint(newPoint1, newPoint2, posB);
                    var space = new Space(spacePosition, row);
                    spaces.Add(space);
                }

                row.Spaces = spaces.ToArray();
                rows.Add(row);
                rowCount++;
            }

            Rows = rows.ToArray();

            AssignChildren(Rows);
        }

        private void AssignChildren(Row[] rows)
        {
            for (int rowIndex = rows.Length - 1; rowIndex > 0; rowIndex--)
            {
                var row = Rows[rowIndex];
                var childRow = Rows[rowIndex - 1];
                row.ChildRow = childRow;
            }
        }

        private Vector2 InterpolatePoint(Vector2 v1, Vector2 v2, float percentage) // move point a to point b
        {
            return (1 - percentage) * v1 + percentage * v2;
        }

        internal Vector2[] GetAllSpacePositions()
        {
            return Rows.SelectMany(r => r.Spaces).Select(sp => sp.Position).ToArray();
        }
    }

    internal class Row
    {
        internal Space[] Spaces;
        internal Row ChildRow;
    }

    internal class Space
    {
        internal Row ContainingRow;
        internal Vector2 Position;
        internal Ball ContainedBall { get => BallManager.GetBallById(BallId); }
        internal bool ContainsBall { get => BallId > -1; }
        internal int BallId;
        internal BallManager BallManager;

        internal Space(Vector2 position, Row containingRow)
        {
            ContainingRow = containingRow;
            Position = position;
            BallId = -1;
        }
    }

    internal class LevelData
    {
        internal Row[] StartRows;
        internal TriangleSegment[] TriangleSegments;
        internal Space[] Spaces;
        internal int SegmentCount;
        internal int StartRowSpaceCount;
        internal LevelData(LevelGenerator level)
        {
            TriangleSegments = level.TriangleSegments;
            StartRows = GetStartRows(TriangleSegments);
            Spaces = InitSpaces(TriangleSegments);

            SegmentCount = TriangleSegments.Length;
            StartRowSpaceCount = StartRows[0].Spaces.Length; // assumes all start row lengths are the same, should be 👀
        }

        private Space[] InitSpaces(TriangleSegment[] triangles)
        {
            var spaces = new List<Space>();

            foreach (var triangle in triangles)
            {
                foreach (var row in triangle.Rows)
                {
                   foreach (var space in row.Spaces)
                    {
                        spaces.Add(space);
                    }
                }                
            }

            return spaces.ToArray();
        }

        private Row[] GetStartRows(TriangleSegment[] triangles)
        {
            Row[] startRows = new Row[triangles.Length];
            var count = 0;
            foreach (var triangle in triangles)
            {
                var row = triangle.Rows[^1];
                startRows[count] = row;
                count++; 
            }

            return startRows;
        }
    }
}