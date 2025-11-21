using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Level
{
    [CreateAssetMenu(fileName = "Level_", menuName = "Game/Level Data")]
    public class LevelData : ScriptableObject
    {
        public Vector2Int gridSize = new Vector2Int(16, 6);

        // Позиции на сетке
        public Vector2Int chickenPosition;
        public List<Vector2Int> wallPositions;
        public List<Vector2Int> basketPositions;
        public List<Vector2Int> nestPositions;
    }
}