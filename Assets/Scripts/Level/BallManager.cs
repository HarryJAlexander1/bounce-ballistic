using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

namespace Assets.Scripts.Level
{
    [RequireComponent(typeof(LevelGenerator))]
    public class BallManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject BallPrefab;
        [SerializeField]
        private float BallSize;
        [SerializeField]
        private int BallAmount;
        [SerializeField]
        private bool EnableDebug;
        private LevelData LevelData;
        private List<Ball> ActiveBalls;
        private Queue<Ball> QueuedBalls;
        private System.Random RNG;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!EnableDebug)
                return;

            HandleDebugFunctions();
        }

        internal void Initialise()
        {
            RNG = new System.Random();
            var level = GetComponent<LevelGenerator>();
            LevelData = new LevelData(level);
            ActiveBalls = new List<Ball>();
            QueueBalls(BallAmount);
        }

        private void MoveBalls()
        {
            var activeBalls = ActiveBalls.ToArray();
            RNG.Shuffle(activeBalls);
            foreach (var ball in activeBalls)
            {
                MoveBall(ball);
            }
        }

        private void MoveBall(Ball ball)
        {
            var containingSpace = ball.ContainingSpace;
            var containingRow = containingSpace.ContainingRow;
            var childRow = containingRow.ChildRow;
            var availableSpaces = childRow.Spaces.Where(space => !space.ContainsBall).ToArray();

            if (availableSpaces.Length == 0)
            {
                Debug.Log("No available spaces to move ball");
                return;
            }

            var randomSpaceIndex = UnityEngine.Random.Range(0, availableSpaces.Length);
            var randomSpace = availableSpaces[randomSpaceIndex];

            ball.Move(randomSpace);
        }

        internal void QueueBalls(int amount)
        {
            QueuedBalls = new Queue<Ball>();

            for (int ballNumber = 0; ballNumber < amount; ballNumber++)
            {
                var ball = new Ball();
                QueuedBalls.Enqueue(ball);
            }
        }

        internal void SpawnBalls(int rowIndex = -1, int ballAmount = 0)
        {
            Row row;
            var lastRowIndex = LevelData.StartRows.Length;

            if (!(rowIndex > -1 && rowIndex < lastRowIndex))
                rowIndex = UnityEngine.Random.Range(0, lastRowIndex);
            
            row = LevelData.StartRows[rowIndex];

            var maxBallAmount = row.Spaces.Length;

            if (!(ballAmount > 0 && ballAmount <= maxBallAmount))
                ballAmount = UnityEngine.Random.Range(1, maxBallAmount);

            if (ActiveBalls.Count > 0)
                MoveBalls();

            var spaces = row.Spaces;
            
            while (ballAmount > 0)
            {
                var availableSpaces = spaces.Where(s => !s.ContainsBall).ToArray();
                var spaceIndex = UnityEngine.Random.Range(0, availableSpaces.Length);
                var space = availableSpaces[spaceIndex];
                SpawnBall(space);
                ballAmount--;
            }
        }

        private void SpawnBall(Space emptySpace)
        {
            var ball = QueuedBalls.Dequeue();
            ball.ContainingSpace = emptySpace;
            var ballGameObject = Instantiate(BallPrefab, emptySpace.Position, quaternion.identity);
            ballGameObject.transform.localScale = new Vector3(BallSize, BallSize, BallSize);
            var sr = ballGameObject.GetComponent<SpriteRenderer>();
            sr.color = Color.red;
            ball.BallGameObject = ballGameObject;
            emptySpace.ContainsBall = true;
            ActiveBalls.Add(ball);
        }

        // Debug Zone

        private void HandleDebugFunctions()
        {
            DebugNewTurn();
        }

        private void DebugMoveBalls()
        {
            if (!Input.GetKeyDown(KeyCode.F))
                return;
            MoveBalls();
        }

        private void DebugNewTurn()
        {
            if (!Input.GetKeyDown(KeyCode.G))
                return;
            
            if (QueuedBalls.Count > 0)
                SpawnBalls();
            else
                MoveBalls();
        }
    }

    internal class Ball
    {
        internal BallAttributes Attributes;
        internal Space ContainingSpace;
        internal GameObject BallGameObject;

        internal void Move(Space newSpace)
        {
            ContainingSpace.ContainsBall = false;
            ContainingSpace = newSpace;
            ContainingSpace.ContainsBall = true;
            BallGameObject.transform.position = ContainingSpace.Position;
        }
    }

    internal class BallAttributes
    {
        internal int LevelNo;
        internal int Health;
        internal int MaximumHealth;
    }

    internal class LevelData
    {
        internal Row[] StartRows;
        internal TriangleSegment[] TriangleSegments;
        internal LevelData(LevelGenerator level)
        {
            TriangleSegments = level.TriangleSegments;
            StartRows = GetStartRows(TriangleSegments);
        }

        private Row[] GetStartRows(TriangleSegment[] triangles)
        {
            Row[] startRows = new Row[triangles.Length];
            var count = 0;
            foreach (var triangle in triangles)
            {
                startRows[count] = triangle.Rows[triangle.Rows.Length - 1];
                count++; 
            }

            return startRows;
        }
    }

    static class RandomExtensions
    {
        public static void Shuffle<T>(this System.Random rng, T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}


