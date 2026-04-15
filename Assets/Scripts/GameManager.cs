using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Level;
using Unity.Mathematics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    Vector3 LevelPosition;

    [SerializeField]
    GameObject LevelPrefab;

    [SerializeField]
    LevelData LevelData;

    [SerializeField]
    LevelGenerator LevelGenerator;

    [SerializeField]
    BallManager BallManager;

    [SerializeField]
    GameStageGenerator GameStageGenerator;

    [SerializeField]
    GameStageConfig[] GameStageConfigurations;

    [SerializeField]
    Queue<GameStage> GameStages;

    GameStage ActiveGameStage;

    GameObject Level;

    // Start is called before the first frame update
    void Start()
    {
        InitLevelGameObject(); // level in-game object
        InitLevelGenerator(); // level generator
        InitLevelData(); // level data object - used amongst other objects
        InitGameStageGenerator(); // 
        InitBallManager();

        StartGameStage();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartTurn();
        }
    }
 
    private void InitLevelGameObject()
    {
        Level = Instantiate(LevelPrefab, LevelPosition, quaternion.identity);
    }

    private void InitLevelGenerator()
    {
        LevelGenerator = Level.GetComponent<LevelGenerator>();
        LevelGenerator.Initialise();
    }

    private void InitLevelData()
    {
        LevelData = LevelGenerator.GetLevelData();
    }

    private void InitBallManager()
    {
        BallManager = Level.GetComponent<BallManager>();
        BallManager.Initialise(ref LevelData);
    }

    private void BuildGameStageConfigs()
    {
        GameStageConfigurations = new GameStageConfig[]
            {
                new GameStageConfig
                {
                    Turns = new TurnConfig[]
                    {
                        new TurnConfig
                        {
                            BallAmount = 5,
                            BallLevelRatios = new int[3] {5, 4, 1}
                        },
                        new TurnConfig
                        {
                            BallAmount = 5,
                            BallLevelRatios = new int[3] {5, 4, 1}
                        }
                        //  new TurnConfig
                        // {
                        //     BallAmount = 56,
                        //     BallLevelRatios = new int[3] {5, 4, 1}
                        // },
                        //  new TurnConfig
                        // {
                        //     BallAmount = 69,
                        //     BallLevelRatios = new int[3] {5, 4, 1}
                        // },
                        //  new TurnConfig
                        // {
                        //     BallAmount = 190,
                        //     BallLevelRatios = new int[3] {5, 4, 1}
                        // },
                        //   new TurnConfig
                        // {
                        //     BallAmount = 190,
                        //     BallLevelRatios = new int[3] {5, 4, 1}
                        // },
                        //   new TurnConfig
                        // {
                        //     BallAmount = 190,
                        //     BallLevelRatios = new int[3] {5, 4, 1}
                        // },
                        //   new TurnConfig
                        // {
                        //     BallAmount = 190,
                        //     BallLevelRatios = new int[3] {5, 4, 1}
                        // },
                        //   new TurnConfig
                        // {
                        //     BallAmount = 190,
                        //     BallLevelRatios = new int[3] {5, 4, 1}
                        // }
                    }
                }
            };
    }

    private void InitGameStageGenerator()
    {
        BuildGameStageConfigs();
        GameStageGenerator = Level.GetComponent<GameStageGenerator>();
        GameStageGenerator.Initialise(LevelData, GameStageConfigurations);
        GameStages = new Queue<GameStage>(GameStageGenerator.GameStages);
    }

    private void StartGameStage()
    {
        ActiveGameStage = GameStages.Dequeue(); 
        StartTurn();     
    }

    private void StartTurn()
    {
        ActiveGameStage.Turns.TryDequeue(out var turn);

        if (turn != null)
            BallManager.QueueBalls(turn?.BallSpawnData);

        BallManager.SpawnBalls();
    }
    
}
