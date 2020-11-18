using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace zigzag
{
    public struct LevelData
    {
        public List<Vector2> tiles;
        public List<int> gems;
        public int levelLength;
    }
    public class LevelBuilder
    {
        static bool gemRandom = false;
        static int salt = 42;
        
        /// <summary>
        /// if automatically generated level is bad, override seed here
        /// </summary>
        static Dictionary<int,int> levelOverrides = new Dictionary<int, int>
        {
            [3] = 324123
        }; 
        
        
        /// <summary>
        /// Generate unique level by ID
        /// </summary>
        /// <param name="levelID"></param>
        /// <param name="difficulty"></param>
        /// <returns></returns>
        public static LevelData BuildLevelData(int levelID,int difficulty)
        {
            var seed = levelOverrides.ContainsKey(levelID) ? levelOverrides[levelID] :  salt + levelID;
            Random.InitState(seed); // Consistency!

            var result = new LevelData
            {
                tiles = new List<Vector2>(),
                gems = new List<int>(),
                levelLength = GetLength(levelID)
            };
            
            
            var gemOffset = 0;
            for (int i = 0; i < result.levelLength/5; i++)
            {
                if (gemRandom) 
                    gemOffset = Random.Range(0, 5);
                else
                    gemOffset = (gemOffset + 1)%5;
                result.gems.Add(5*i+gemOffset);
                
            }

            var direction = 1;
            var x = 0f;
            result.tiles.Add(Vector2.zero);
            for (int i = 1; i < result.levelLength; i++)
            {
                if (Random.Range(0, 2) == 0 || Math.Abs(x) > 4.5f - difficulty) 
                    direction *= -1;
                x += direction;
                result.tiles.Add(new Vector3(x, i));
            }

            return result;
        }

        /// <summary>
        /// Calculate path length for current level
        /// </summary>
        /// <param name="levelID"></param>
        /// <returns></returns>
        private static int GetLength(int levelID) => 10 + levelID;
    }
}