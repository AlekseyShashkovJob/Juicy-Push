using UnityEngine;
using TMPro;
using System;

namespace View.UI.Game
{
    public class PausedScreen : UIScreen
    {
        [SerializeField] private Button.CustomButton _continue;
        [SerializeField] private Button.CustomButton _restart;

        private void OnEnable()
        {
            _continue.AddListener(ContinueGame);
            _restart.AddListener(Restart);
        }

        private void OnDisable()
        {
            _continue.RemoveListener(ContinueGame);
            _restart.RemoveListener(Restart);
        }

        public override void StartScreen()
        {
            base.StartScreen();

            Time.timeScale = 0.0f;
        }

        private void ContinueGame()
        {
            Time.timeScale = 1.0f;
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