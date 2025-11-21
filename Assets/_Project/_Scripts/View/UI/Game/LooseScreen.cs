using UnityEngine;
using TMPro;

namespace View.UI.Game
{
    public class LooseScreen : UIScreen
    {
        [SerializeField] private Button.CustomButton _back;
        [SerializeField] private Button.CustomButton _restart;

        [SerializeField] private Misc.SceneManagment.SceneLoader _sceneLoader;

        [SerializeField] private TMP_Text _currentScoreText;
        [SerializeField] private TMP_Text _totalScoreText;

        private void OnEnable()
        {
            _back.AddListener(BackToMenu);
            _restart.AddListener(Restart);
        }

        private void OnDisable()
        {
            _back.RemoveListener(BackToMenu);
            _restart.RemoveListener(Restart);
        }

        public override void StartScreen()
        {
            base.StartScreen();

            Time.timeScale = 0.0f;

            _currentScoreText.text = $"{GameCore.GameManager.Instance.CurrentScore}";
            _totalScoreText.text = $"{GameCore.GameManager.Instance.CurrentLevelBestScore}";
        }

        private void BackToMenu()
        {
            Time.timeScale = 1.0f;
            _sceneLoader.ChangeScene(Misc.Data.SceneConstants.MENU_SCENE);
            CloseScreen();
        }

        private void Restart()
        {
            Time.timeScale = 1.0f;
            GameCore.GameManager.Instance.RestartLevel();
            CloseScreen();
        }
    }
}