using System.Collections.Generic;
using Maze2D.Domain;
using UnityEngine;

namespace Maze2D.Configs
{
    [CreateAssetMenu(fileName = "difficulty_config", menuName = "Configs/Difficulty", order = 0)]
    public class DifficultyConfigContainer : ScriptableObject
    {
        [SerializeField] private List<DifficultyConfig> _configs;

        public IReadOnlyList<DifficultyConfig> Configs => _configs;
        
        public DifficultyConfig GetConfigByLevel(Difficulty level)
        {
            return _configs.Find(config => config.Level == level);
        }
    }
}