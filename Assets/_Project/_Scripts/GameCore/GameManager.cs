using System.Collections;
using TMPro;
using UnityEngine;

namespace GameCore
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public static Camera MainCamera { get; private set; }

        public int CurrentScore { get; private set; }
        public int CurrentLevelBestScore { get; private set; }
        public int CurrentLevelIndex { get; private set; }
        public int BasketsInNests { get; private set; }
        public int BasketsTotal { get; private set; }

        [SerializeField] private Level.LevelLoader _levelLoader;
        [SerializeField] private View.UIScreen _gameScreen;
        [SerializeField] private View.UIScreen _winScreen;
        [SerializeField] private View.UIScreen _loseScreen;
        [SerializeField] private View.UI.Game.StoryScreen _storyScreen;
        [SerializeField] private Misc.SceneManagment.SceneLoader _sceneLoader;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _timerText;

        private const float MAX_TIME = 60.0f;
        private Coroutine _timerCoroutine;
        private float _timeLeft;
        private bool _levelActive = false;

        private void Awake()
        {
            if (MainCamera == null)
            {
                MainCamera = Camera.main;
            }

            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        private void Start()
        {
            Time.timeScale = 1.0f;

            int selected = PlayerPrefs.GetInt(GameConstants.LAST_SELECTED_LEVEL_KEY, 0);
            selected = Mathf.Clamp(selected, 0, _levelLoader.LevelsCount - 1);
            _levelLoader.LoadLevel(selected);
        }

        public void OnLevelLoaded(int levelIndex)
        {
            CurrentLevelIndex = levelIndex;
            CurrentScore = 0;
            BasketsInNests = 0;

            LoadBestScore();
            UpdateScoreUI();
            _levelActive = true;
            StartLevelTimer();
        }

        public void StartLevelTimer()
        {
            if (_timerCoroutine != null)
                StopCoroutine(_timerCoroutine);

            _timeLeft = MAX_TIME;
            _timerCoroutine = StartCoroutine(TimerRoutine());
            UpdateTimerUI();
        }

        public void AddScore(int amount)
        {
            CurrentScore += amount;
            if (CurrentScore < 0) CurrentScore = 0;
            UpdateScoreUI();
        }

        public void BasketPlacedInNest()
        {
            BasketsInNests++;
            AddScore(10);
            CheckWinCondition();
        }

        public void BasketRemovedFromNest()
        {
            BasketsInNests--;
            if (BasketsInNests < 0) BasketsInNests = 0;
            AddScore(-10);
        }

        public void SetBasketsTotal(int total)
        {
            BasketsTotal = total;
        }

        public void FinishGame()
        {
            _levelActive = false;
            StopTimer();
            Debug.Log("All levels complete!");
        }

        public void RestartLevel()
        {
            StopTimer();
            _levelLoader.ReloadCurrentLevel();
        }

        public void LoadNextLevel()
        {
            StopTimer();
            _levelLoader.LoadNextLevel();
        }

        public bool IsLastLevel()
        {
            return CurrentLevelIndex >= _levelLoader.LevelsCount - 1;
        }

        private void CheckWinCondition()
        {
            if (BasketsTotal > 0 && BasketsInNests >= BasketsTotal)
            {
                FinishLevel(true);
            }
        }

        private void FinishLevel(bool win)
        {
            _levelActive = false;
            StopTimer();

            if (win)
            {
                int timeBonus = Mathf.CeilToInt(_timeLeft);
                AddScore(timeBonus);

                if (CurrentScore > CurrentLevelBestScore)
                {
                    CurrentLevelBestScore = CurrentScore;
                    SaveBestScore();
                }

                UnlockNextLevel(CurrentLevelIndex);

                _storyScreen.ShowStory(() =>
                {
                    _winScreen.StartScreen();
                });
            }
            else
            {
                _loseScreen.StartScreen();
            }
        }

        private IEnumerator TimerRoutine()
        {
            while (_timeLeft > 0 && _levelActive)
            {
                _timeLeft -= Time.deltaTime;
                UpdateTimerUI();
                yield return null;
            }

            if (_levelActive && _timeLeft <= 0)
            {
                _timeLeft = 0;
                UpdateTimerUI();
                FinishLevel(false);
            }
        }

        private void UpdateTimerUI()
        {
            int seconds = Mathf.CeilToInt(_timeLeft);
            if (_timerText != null)
                _timerText.text = $"{seconds}";
        }

        private void UpdateScoreUI()
        {
            if (_scoreText != null)
                _scoreText.text = $"{CurrentScore}";
        }

        private void StopTimer()
        {
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }
        }

        private void SaveBestScore()
        {
            PlayerPrefs.SetInt(GetBestScoreKey(CurrentLevelIndex), CurrentLevelBestScore);
            PlayerPrefs.Save();
        }

        private void LoadBestScore()
        {
            CurrentLevelBestScore = PlayerPrefs.GetInt(GetBestScoreKey(CurrentLevelIndex), 0);
        }

        private string GetBestScoreKey(int level)
        {
            return $"BestScore_Level_{level}";
        }

        private void UnlockNextLevel(int currentLevel)
        {
            int lastUnlocked = PlayerPrefs.GetInt(GameConstants.LAST_UNLOCKED_LEVEL_KEY, 0);

            if (currentLevel >= lastUnlocked)
            {
                int next = currentLevel + 1;
                if (next < _levelLoader.LevelsCount)
                {
                    PlayerPrefs.SetInt(GameConstants.LAST_UNLOCKED_LEVEL_KEY, next);
                    PlayerPrefs.Save();
                }
            }
        }
    }
}