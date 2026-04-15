using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;
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
        internal int MaxBallLevel;
        [SerializeField]
        private bool EnableDebug; 
        internal LevelData LevelData;
        internal List<Ball> ActiveBalls;
        private int BallCount; // represents all balls ever spawned in game, used for ballIds;
        private HashSet<int> BallsToDeleteIds;
        internal Queue<Ball> QueuedBalls;
        

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }

        internal void Initialise(ref LevelData levelData)
        {
            LevelData = levelData;
            QueuedBalls = new Queue<Ball>();
            ActiveBalls = new List<Ball>();
            BallsToDeleteIds = new HashSet<int>();
            //QueueBalls(BallAmount);
            foreach (var space in LevelData.Spaces)
            {
                space.BallManager = this;
            }
        }

        internal Ball GetBallById(int ballId)
        {
            return ActiveBalls.FirstOrDefault(b => b.BallId == ballId);
        }

        internal void LoadStage()
        {
            
        }

        internal void MoveBalls()
        {
            for (int ballIndex = 0; ballIndex < ActiveBalls.Count; ballIndex++)
            {
                var ball = ActiveBalls[ballIndex];

                MoveBall(ball);
            }

            if (BallsToDeleteIds.Count > 0)
                DeleteBalls();
        }

        internal void DeleteBalls()
        {
            foreach (var balltoDeleteId in BallsToDeleteIds)
            {
                var ball = GetBallById(balltoDeleteId);
                ball.Delete();
                Destroy(ball.BallGameObject);
                ActiveBalls.Remove(ball);
            }

            BallsToDeleteIds.Clear();
        }

        private void MoveBall(Ball ball)
        {
            var containingSpace = ball.ContainingSpace;
            var containingRow = containingSpace.ContainingRow;
            var childRow = containingRow.ChildRow;

            if (childRow == null)
            {
                Debug.Log("Game Over");
                return;
            }

            var availableSpaces = childRow.Spaces
            .Where(space => !space.ContainsBall)
            .ToArray();

            if (availableSpaces.Length == 0)
            {
                var eligibleBallsForUpgrade = childRow.Spaces
                .Where(space => space.ContainsBall)
                .Select(s => s.ContainedBall)
                // .Where(b => !BallsToDeleteIds
                // .Contains(b.BallId))
                .ToArray();
                
                HandleBallUpgrade(ball, eligibleBallsForUpgrade);
                return;
            }

            var randomSpaceIndex = UnityEngine.Random.Range(0, availableSpaces.Length);
            var randomSpace = availableSpaces[randomSpaceIndex];

            ball.Move(randomSpace);
        }

        private void HandleBallUpgrade(Ball ball, Ball[] balls)
        {
            var ballIndex = RandomExtensions.RNG.Next(0, balls.Length); // should change to only select balls that won't die when upgraded
            var ballToUpgrade = balls[ballIndex];
            
            ballToUpgrade.Upgrade(ball);

            BallsToDeleteIds.Add(ball.BallId);

            if (ballToUpgrade.Attributes.LevelNo > ballToUpgrade.Attributes.MaxLevel)
                BallsToDeleteIds.Add(ballToUpgrade.BallId);
        }

        internal void QueueBalls(BallSpawn[] ballSpawnData)
        {     
            foreach (var ballSpawn in ballSpawnData)
            {
                var ball = new Ball(this, ballSpawn);
                QueuedBalls.Enqueue(ball);
            }
        }

        internal void SpawnBalls()
         { 
            if (ActiveBalls.Count > 0)
                MoveBalls();

            var ballAmount = QueuedBalls.Count;

            if (ballAmount == 0)
                return;

            while ( ballAmount > 0 )
            {
                var ball = QueuedBalls.Dequeue();
                var spawnLocation = ball.BallSpawnLocation;
                Row row = LevelData.StartRows[spawnLocation.Item1];
                var availableSpaces = row.Spaces.Where(s => !s.ContainsBall).ToArray();
                Space space = availableSpaces[spawnLocation.Item2];
                SpawnBall(space, ball);
                ballAmount--;
            }
        }

        private void SpawnBall(Space emptySpace, Ball ball)
        {
            var ballGameObject = Instantiate(BallPrefab, emptySpace.Position, quaternion.identity);
            ballGameObject.transform.localScale = new Vector3(BallSize, BallSize, BallSize);
            ballGameObject.GetComponent<SpriteRenderer>().color = ball.Attributes.Colour;
            ball.BallGameObject = ballGameObject;

            ball.BallId = BallCount;
            ball.ContainingSpace = emptySpace;
            emptySpace.BallId = ball.BallId;
            ActiveBalls.Add(ball);
            BallCount++;
        }
    }

    internal class Ball
    {
        internal BallAttributes Attributes;
        internal Space ContainingSpace;
        internal GameObject BallGameObject;
        private BallManager BallManager;
        internal int BallId;
        internal (int, int) BallSpawnLocation;

        internal Ball(BallManager ballManager, BallSpawn ballSpawnData)
        {
            BallManager = ballManager;
            Attributes = new BallAttributes
            {
                MaxLevel = ballManager.MaxBallLevel, 
                LevelNo = ballSpawnData.BallLevel,
            };
            BallSpawnLocation = ballSpawnData.SpawnLoc;
        }

        internal void Move(Space newSpace)
        {
            ContainingSpace.BallId = -1; // contains no ball
            ContainingSpace = newSpace;
            ContainingSpace.BallId = BallId;
            BallGameObject.transform.position = ContainingSpace.Position;
        }

        internal void Upgrade(Ball ball)
        {
            Attributes.LevelNo += ball.Attributes.LevelNo;
            BallGameObject.GetComponent<SpriteRenderer>().color = Attributes.Colour;
        }

        internal void Delete()
        {
            ContainingSpace.BallId = -1;
        }
    }

    internal class BallAttributes
    {
        private int _levelNo;
        internal int LevelNo 
        {
            get 
            {
                return _levelNo; 
            }
            set
            {
                _levelNo = value;
                SetColour(value);
            }
        }
        internal Color Colour {get; private set;}
        internal int MaxLevel;

        private void SetColour(int value)
        {
            if (value > MaxLevel)
                return;

            Colour = value switch
            {
                1 => Color.red,
                2 => Color.blue,
                3 => Color.green,
                4 => Color.yellow,
                5 => Color.magenta,
                _ => Color.white,
            };
        }
    }

    static class RandomExtensions
    {
        internal static System.Random RNG = new System.Random();
        public static void Shuffle<T>(T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = RNG.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}


