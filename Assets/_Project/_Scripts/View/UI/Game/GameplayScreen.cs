using UnityEngine;
using System;

namespace View.UI.Game
{
    public class GameplayScreen : UIScreen
    {
        [SerializeField] private Button.CustomButton _pause;
        [SerializeField] private Button.CustomButton _back;
        [SerializeField] private Button.CustomButton _restart;

        [SerializeField] private UIScreen _pauseScreen;

        [SerializeField] private Misc.SceneManagment.SceneLoader _sceneLoader;

        private void OnEnable()
        {
            _pause.AddListener(PauseGame);
            _back.AddListener(BackToMenu);
            _restart.AddListener(Restart);
        }

        private void OnDisable()
        {
            _pause.RemoveListener(PauseGame);
            _back.RemoveListener(BackToMenu);
            _restart.RemoveListener(Restart);
        }

        private void PauseGame()
        {
            _pauseScreen.StartScreen();
        }

        private void BackToMenu()
        {
            _sceneLoader.ChangeScene(Misc.Data.SceneConstants.MENU_SCENE);
            CloseScreen();
        }

        private void Restart()
        {
            GameCore.GameManager.Instance.RestartLevel();
            // CloseScreen();
        }
    }
}