using System;
using UnityEngine;
using View.Button;

namespace View.UI.Menu
{
    public class MainMenuScreen : UIScreen
    {
        [SerializeField] private UIScreen _optionsScreen;
        [SerializeField] private UIScreen _privacyScreen;
        [SerializeField] private UIScreen _levelsScreen;

        [SerializeField] private Misc.SceneManagment.SceneLoader _sceneLoader;

        [SerializeField] private CustomButton _startGame;
        [SerializeField] private CustomButton _settings;
        [SerializeField] private CustomButton _privacy;
        [SerializeField] private CustomButton _levels;

        private void OnEnable()
        {
            _startGame.AddListener(OpenGame);
            _settings.AddListener(OpenOptions);
            _privacy.AddListener(OpenPrivacy);
            _levels.AddListener(OpenLevels);
        }

        private void OnDisable()
        {
            _startGame.RemoveListener(OpenGame);
            _settings.RemoveListener(OpenOptions);
            _privacy.RemoveListener(OpenPrivacy);
            _levels.RemoveListener(OpenLevels);
        }

        public override void StartScreen()
        {
            base.StartScreen();
        }

        private void OpenGame()
        {
            _sceneLoader.ChangeScene(Misc.Data.SceneConstants.GAME_SCENE);
            CloseScreen();
        }

        private void OpenOptions()
        {
            _optionsScreen.StartScreen();
        }

        private void OpenPrivacy()
        {
            _privacyScreen.StartScreen();
        }

        private void OpenLevels()
        {
            _levelsScreen.StartScreen();
        }
    }
}