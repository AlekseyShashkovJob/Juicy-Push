using UnityEngine;

namespace GameCore.Level
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private LevelData[] _levelDataAssets;
        [SerializeField] private LevelGenerator generator;

        public int CurrentLevel { get; private set; }

        public int LevelsCount => _levelDataAssets != null ? _levelDataAssets.Length : 0;

        public void LoadLevel(int index)
        {
            if (_levelDataAssets == null || index < 0 || index >= _levelDataAssets.Length)
            {
                Debug.LogError($"No level at index {index}");
                return;
            }
            CurrentLevel = index;
            generator.BuildLevel(_levelDataAssets[index]);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetBasketsTotal(_levelDataAssets[index].basketPositions.Count);
                GameManager.Instance.OnLevelLoaded(index);
            }
        }

        public void ReloadCurrentLevel()
        {
            LoadLevel(CurrentLevel);
        }

        public void LoadNextLevel()
        {
            int next = CurrentLevel + 1;
            if (next >= LevelsCount)
            {
                GameManager.Instance.FinishGame();
                Debug.Log("All levels complete!");
            }
            else
            {
                LoadLevel(next);
            }
        }
    }
}