using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Level
{

    internal class TurnConfig
    {
        internal int BallAmount; // number of balls to spawn in turn.
        internal int [] BallLevelRatios; // The proportion of each ball level
    }

    internal class GameStageConfig
    {
        internal TurnConfig[] Turns;
    }

    //---------------------------------------------------------------------
    internal class GameStage
    {
        internal Queue<TurnData> Turns { get; set; }
    }

    internal class TurnData
    {
        internal BallSpawn[] BallSpawnData { get; set; }
    }

    internal class BallSpawn
    {
        internal (int, int) SpawnLoc {get; set;}
        internal int BallLevel { get; set; }
    }

    internal class GameStageGenerator : MonoBehaviour
    {
        private LevelData LevelData;
        private GameStageConfig[] GameStageConfigurations;
        internal GameStage[] GameStages;

        internal void Initialise(LevelData levelData, GameStageConfig[] gameStageConfigs)
        {
            LevelData = levelData;
            GameStageConfigurations = gameStageConfigs;
            GameStages = GenerateGameStages();
        }

        internal GameStage[] GenerateGameStages()
        {
            GameStage[] gameStages = new GameStage[GameStageConfigurations.Length];

            for (int configIndex = 0; configIndex < GameStageConfigurations.Length; configIndex++)
            {
                var gameStage = GetGameStage(GameStageConfigurations[configIndex]);
                gameStages[configIndex] = gameStage;
            }

            return gameStages;
        }

        internal GameStage GetGameStage(GameStageConfig gameStageConfig)
        {
            TurnData[] turns = new TurnData[gameStageConfig.Turns.Length];

            var turnCount = 0;

            foreach (var turnconfig in gameStageConfig.Turns)
            {
                var turn = GetTurn(turnconfig);
                turns[turnCount] = turn;
                turnCount++;
            }

            return new GameStage{ Turns = new Queue<TurnData>(turns) };
        }

        private TurnData GetTurn(TurnConfig turnConfig)
        {
            var ballAmount = turnConfig.BallAmount;
            var ballLevelRatios = turnConfig.BallLevelRatios;

            var segmentCount = LevelData.SegmentCount; // represents number of available segments a ball to spawn in
            var spaceCount = LevelData.StartRowSpaceCount; // represents the number of spaces per segment a ball can spawn in
            var maxSpawnAmount = spaceCount * segmentCount;

            if (ballAmount > maxSpawnAmount)
                ballAmount = maxSpawnAmount;
            
            var ballAmountsPerLevel = new int[ballLevelRatios.Length];

            for (int levelIndex = 0; levelIndex < ballLevelRatios.Length; levelIndex++)
            {
                float percentage = (float)ballLevelRatios[levelIndex] / (float)10;
                ballAmountsPerLevel[levelIndex] = (int)Math.Round(ballAmount * percentage, 0, MidpointRounding.AwayFromZero);
            }

            BallSpawn[] ballSpawns = new BallSpawn[ballAmount];

            var maxSegmentIndex = segmentCount - 1;
            var maxSpaceIndex = spaceCount - 1;

            (int, int) spawnLocations = (maxSegmentIndex, maxSpaceIndex); // minus 1 for zero indexing

            int ballSpawnCounter = 0;

            while (ballSpawnCounter < ballAmount)
            {
                ballSpawns[ballSpawnCounter] = new BallSpawn{ SpawnLoc = spawnLocations };
                
                spawnLocations.Item2--;
                ballSpawnCounter++;

                if (spawnLocations.Item2 == -1)
                {
                    spawnLocations.Item1--;
                    spawnLocations.Item2 = maxSpaceIndex;
                }

            }

            ballSpawnCounter = 0;
            
            var ballLevelIndex = 0;
            var amountSpawnedPerLevel = 0;
            var amountToSpawnPerLevel = ballAmountsPerLevel[0];

            while (ballSpawnCounter < ballAmount)
            {
                if (amountSpawnedPerLevel == amountToSpawnPerLevel)
                {
                    ballLevelIndex++;
                    amountSpawnedPerLevel = 0;
                    amountToSpawnPerLevel = ballAmountsPerLevel[ballLevelIndex];
                }

                ballSpawns[ballSpawnCounter].BallLevel = ballLevelIndex + 1;
                amountSpawnedPerLevel++;
                ballSpawnCounter++;

            }

            return new TurnData{ BallSpawnData = ballSpawns };
        }
    }
}