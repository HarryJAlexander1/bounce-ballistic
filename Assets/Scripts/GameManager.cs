using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Level;
using Unity.Mathematics;
using Unity.VisualScripting;
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

        BeginStage();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BeginStage();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            BeginTurn();
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            InitGameStageGenerator();
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
                            BallAmount = 17,
                            BallLevelRatios = new int[3] {8, 1, 1}
                        },
                        new TurnConfig
                        {
                            BallAmount = 17,
                            BallLevelRatios = new int[3] {6, 3, 1}
                        },
                        new TurnConfig
                        {
                            BallAmount = 25,
                            BallLevelRatios = new int[3] {6, 3, 1}
                        },
                        new TurnConfig
                        {
                            BallAmount = 25,
                            BallLevelRatios = new int[3] {4, 4, 2}
                        }
                    }
                },
                new GameStageConfig
                {
                    Turns = new TurnConfig[]
                    {
                        new TurnConfig
                        {
                            BallAmount = 27,
                            BallLevelRatios = new int[3] {5, 3, 2}
                        },
                        new TurnConfig
                        {
                            BallAmount = 33,
                            BallLevelRatios = new int[3] {4, 4, 2}
                        },
                        new TurnConfig
                        {
                            BallAmount = 47,
                            BallLevelRatios = new int[3] {3, 5, 2}
                        },
                        new TurnConfig
                        {
                            BallAmount = 53,
                            BallLevelRatios = new int[3] {2, 6, 2}
                        }
                    }
                },
                new GameStageConfig
                {
                    Turns = new TurnConfig[]
                    {
                        new TurnConfig
                        {
                            BallAmount = 33,
                            BallLevelRatios = new int[3] {4, 4, 2}
                        },
                        new TurnConfig
                        {
                            BallAmount = 40,
                            BallLevelRatios = new int[3] {3, 5, 2}
                        },
                        new TurnConfig
                        {
                            BallAmount = 50,
                            BallLevelRatios = new int[3] {2, 6, 2}
                        },
                        new TurnConfig
                        {
                            BallAmount = 60,
                            BallLevelRatios = new int[3] {2, 5, 3}
                        },
                        new TurnConfig
                        {
                            BallAmount = 73,
                            BallLevelRatios = new int[3] {1, 6, 3}
                        },
                        new TurnConfig
                        {
                            BallAmount = 87,
                            BallLevelRatios = new int[3] {1, 5, 4}
                        }
                    }
                },
                new GameStageConfig
                {
                    Turns = new TurnConfig[]
                    {
                        new TurnConfig
                        {
                            BallAmount = 43,
                            BallLevelRatios = new int[3] {4, 3, 3}
                        },
                        new TurnConfig
                        {
                            BallAmount = 60,
                            BallLevelRatios = new int[3] {1, 6, 3}
                        },
                        new TurnConfig
                        {
                            BallAmount = 87,
                            BallLevelRatios = new int[3] {1, 5, 4}
                        },
                        new TurnConfig
                        {
                            BallAmount = 113,
                            BallLevelRatios = new int[3] {0, 6, 4}
                        }
                    }
                },
                new GameStageConfig
                {
                    Turns = new TurnConfig[]
                    {
                        new TurnConfig
                        {
                            BallAmount = 50,
                            BallLevelRatios = new int[3] {3, 5, 2}
                        },
                        new TurnConfig
                        {
                            BallAmount = 67,
                            BallLevelRatios = new int[3] {2, 5, 3}
                        },
                        new TurnConfig
                        {
                            BallAmount = 93,
                            BallLevelRatios = new int[3] {1, 5, 4}
                        },
                        new TurnConfig
                        {
                            BallAmount = 120,
                            BallLevelRatios = new int[3] {0, 4, 6}
                        }
                    }
                },
                new GameStageConfig
                {
                    Turns = new TurnConfig[]
                    {
                        new TurnConfig
                        {
                            BallAmount = 60,
                            BallLevelRatios = new int[3] {2, 5, 3}
                        },
                        new TurnConfig
                        {
                            BallAmount = 73,
                            BallLevelRatios = new int[3] {1, 5, 4}
                        },
                        new TurnConfig
                        {
                            BallAmount = 83,
                            BallLevelRatios = new int[3] {1, 4, 5}
                        },
                        new TurnConfig
                        {
                            BallAmount = 107,
                            BallLevelRatios = new int[3] {0, 4, 6}
                        },
                        new TurnConfig
                        {
                            BallAmount = 133,
                            BallLevelRatios = new int[3] {0, 3, 7}
                        },
                        new TurnConfig
                        {
                            BallAmount = 160,
                            BallLevelRatios = new int[3] {0, 2, 8}
                        }
                    }
                },
                new GameStageConfig
                {
                    Turns = new TurnConfig[]
                    {
                        new TurnConfig
                        {
                            BallAmount = 67,
                            BallLevelRatios = new int[3] {2, 4, 4}
                        },
                        new TurnConfig
                        {
                            BallAmount = 83,
                            BallLevelRatios = new int[3] {1, 4, 5}
                        },
                        new TurnConfig
                        {
                            BallAmount = 117,
                            BallLevelRatios = new int[3] {0, 4, 6}
                        },
                        new TurnConfig
                        {
                            BallAmount = 150,
                            BallLevelRatios = new int[3] {0, 3, 7}
                        }
                    }
                },
                new GameStageConfig
                {
                    Turns = new TurnConfig[]
                    {
                        new TurnConfig
                        {
                            BallAmount = 73,
                            BallLevelRatios = new int[3] {1, 5, 4}
                        },
                        new TurnConfig
                        {
                            BallAmount = 93,
                            BallLevelRatios = new int[3] {1, 3, 6}
                        },
                        new TurnConfig
                        {
                            BallAmount = 100,
                            BallLevelRatios = new int[3] {0, 3, 7}
                        },
                        new TurnConfig
                        {
                            BallAmount = 133,
                            BallLevelRatios = new int[3] {0, 3, 7}
                        },
                        new TurnConfig
                        {
                            BallAmount = 167,
                            BallLevelRatios = new int[3] {0, 2, 8}
                        },
                        new TurnConfig
                        {
                            BallAmount = 200,
                            BallLevelRatios = new int[3] {0, 1, 9}
                        }
                    }
                },
                new GameStageConfig
                {
                    Turns = new TurnConfig[]
                    {
                        new TurnConfig
                        {
                            BallAmount = 83,
                            BallLevelRatios = new int[3] {1, 4, 5}
                        },
                        new TurnConfig
                        {
                            BallAmount = 100,
                            BallLevelRatios = new int[3] {0, 3, 7}
                        },
                        new TurnConfig
                        {
                            BallAmount = 140,
                            BallLevelRatios = new int[3] {0, 2, 8}
                        },
                        new TurnConfig
                        {
                            BallAmount = 180,
                            BallLevelRatios = new int[3] {0, 1, 9}
                        }
                    }
                },
                new GameStageConfig
                {
                    Turns = new TurnConfig[]
                    {
                        new TurnConfig
                        {
                            BallAmount = 93,
                            BallLevelRatios = new int[3] {0, 3, 7}
                        },
                        new TurnConfig
                        {
                            BallAmount = 117,
                            BallLevelRatios = new int[3] {0, 2, 8}
                        },
                        new TurnConfig
                        {
                            BallAmount = 133,
                            BallLevelRatios = new int[3] {0, 1, 9}
                        },
                        new TurnConfig
                        {
                            BallAmount = 167,
                            BallLevelRatios = new int[3] {0, 1, 9}
                        },
                        new TurnConfig
                        {
                            BallAmount = 217,
                            BallLevelRatios = new int[3] {0, 0, 10}
                        },
                        new TurnConfig
                        {
                            BallAmount = 267,
                            BallLevelRatios = new int[3] {0, 0, 10}
                        }
                    }
                }
            };
    }

    private void InitGameStageGenerator()
    {
        Debug.Log("Getting game stage data...");
        BuildGameStageConfigs();
        GameStageGenerator = Level.GetComponent<GameStageGenerator>();
        GameStageGenerator.Initialise(LevelData, GameStageConfigurations);
        GameStages = new Queue<GameStage>(GameStageGenerator.GameStages);
    }

    private void BeginStage()
    {
        GameStages.TryDequeue(out var gameStage); 

        if (gameStage is null || gameStage.Turns == null)
        {
            Debug.Log("ERROR: No valid game stage to start.");
            return;
        }

        Debug.Log("New stage started :)");

        ActiveGameStage = gameStage;

        if (BallManager.ActiveBalls.Count > 0)
            BallManager.ClearBalls();

        BeginTurn();     
    }

    private void BeginTurn()
    {
        ActiveGameStage.Turns.TryDequeue(out var turn);

        BallSpawn[] spawnData;

        if (turn is null || turn.BallSpawnData == null)
        {
            spawnData = null;
            Debug.Log("No valid wave to spawn, shifting balls - Note: player can start new stage by pressing SPACE");
        }
        else
        {
            spawnData = turn.BallSpawnData;
            Debug.Log("Sucessfully loaded new wave :)");
        }

        BallManager.SpawnBalls(spawnData);
    }
    
}
